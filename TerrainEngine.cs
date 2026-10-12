using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Writes terrain edits straight into the game's TerrainComp delta arrays and
    /// persists them to the owning ZDO so the server stores &amp; replicates them.
    ///
    /// This mirrors what the vanilla TerrainOp / TerrainModifier system does, but
    /// with no radius / delta / cost limits, and without needing a shared prefab.
    /// All game members used here are private, so access is done via reflection.
    /// </summary>
    internal static class TerrainEngine
    {
        private static bool s_cached;

        private static FieldInfo f_width;
        private static FieldInfo f_size;
        private static FieldInfo f_levelDelta;
        private static FieldInfo f_smoothDelta;
        private static FieldInfo f_modifiedHeight;
        private static FieldInfo f_paintMask;
        private static FieldInfo f_modifiedPaint;
        private static FieldInfo f_nview;
        private static MethodInfo m_save;

        private static readonly List<Heightmap> s_hmaps = new List<Heightmap>();

        public static void Cache()
        {
            if (s_cached)
                return;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type t = typeof(TerrainComp);

            f_width = t.GetField("m_width", flags);
            f_size = t.GetField("m_size", flags);
            f_levelDelta = t.GetField("m_levelDelta", flags);
            f_smoothDelta = t.GetField("m_smoothDelta", flags);
            f_modifiedHeight = t.GetField("m_modifiedHeight", flags);
            f_paintMask = t.GetField("m_paintMask", flags);
            f_modifiedPaint = t.GetField("m_modifiedPaint", flags);
            f_nview = t.GetField("m_nview", flags);
            m_save = t.GetMethod("Save", flags, null, new[] { typeof(bool) }, null);

            s_cached = true;

            if (f_levelDelta == null || f_modifiedPaint == null || m_save == null)
                TerrainForgePlugin.Log?.LogError("TerrainEngine: failed to bind one or more TerrainComp members.");
        }

        // ---- accessors used by the clamp patch -----------------------------------

        internal static float[] LevelDelta(TerrainComp c) => (float[])f_levelDelta.GetValue(c);
        internal static float[] SmoothDelta(TerrainComp c) => (float[])f_smoothDelta.GetValue(c);
        internal static Color[] PaintMask(TerrainComp c) => (Color[])f_paintMask.GetValue(c);
        internal static bool[] ModifiedPaint(TerrainComp c) => (bool[])f_modifiedPaint.GetValue(c);
        internal static int Width(TerrainComp c) => (int)f_width.GetValue(c);

        // ---- public entry point --------------------------------------------------

        public static bool Apply(Vector3 center, BrushSpec spec)
        {
            Cache();
            if (spec.Radius <= 0.01f)
                return false;

            s_hmaps.Clear();
            float searchRadius = spec.Radius + 4f;
            if (spec.Operation == TerrainOperation.Ramp && spec.HasRampLine)
                searchRadius = Mathf.Max(spec.Radius, Vector3.Distance(spec.RampA, spec.RampB) * 0.5f + spec.Radius) + 4f;

            Heightmap.FindHeightmap(center, searchRadius, s_hmaps);
            if (s_hmaps.Count == 0)
                return false;

            bool changed = false;

            for (int i = 0; i < s_hmaps.Count; i++)
            {
                Heightmap hm = s_hmaps[i];
                if (hm == null || hm.m_scale <= 0f)
                    continue;

                TerrainComp comp = hm.GetAndCreateTerrainCompiler();
                if (comp == null)
                    continue;

                if (!EnsureOwnership(comp))
                    continue;

                if (ApplyToComp(comp, hm, center, spec))
                {
                    Save(comp);
                    hm.Poke(0);
                    changed = true;
                }
            }

            if (changed && ClutterSystem.instance != null)
                ClutterSystem.instance.ResetGrass(center, searchRadius + 1f);

            return changed;
        }

        private static bool EnsureOwnership(TerrainComp comp)
        {
            try
            {
                if (comp.IsOwner())
                    return true;

                var nview = f_nview.GetValue(comp) as ZNetView;
                if (nview != null && nview.IsValid() && !nview.IsOwner())
                    nview.ClaimOwnership();

                return comp.IsOwner();
            }
            catch (Exception e)
            {
                TerrainForgePlugin.Log?.LogWarning("TerrainForge ownership claim failed: " + e.Message);
                return false;
            }
        }

        private static void Save(TerrainComp comp)
        {
            try
            {
                m_save.Invoke(comp, new object[] { false });
            }
            catch (Exception e)
            {
                TerrainForgePlugin.Log?.LogWarning("TerrainForge save failed: " + e.Message);
            }
        }

        private static bool ApplyToComp(TerrainComp comp, Heightmap hm, Vector3 center, BrushSpec spec)
        {
            if (spec.Operation == TerrainOperation.Ramp && spec.HasRampLine)
                return ApplyRampLine(comp, hm, spec);

            int width = hm.m_width;
            int pitch = width + 1;
            float scale = hm.m_scale;

            hm.WorldToVertex(center, out int cx, out int cy);

            float radiusIdx = spec.Radius / scale;
            if (radiusIdx <= 0.0001f)
                return false;

            int span = Mathf.CeilToInt(radiusIdx);
            float blendIdx = radiusIdx * 0.5f * Mathf.Clamp01(spec.Softness / 100f);

            float[] level = LevelDelta(comp);
            float[] smooth = SmoothDelta(comp);
            bool[] modHeight = (bool[])f_modifiedHeight.GetValue(comp);
            Color[] paint = PaintMask(comp);
            bool[] modPaint = ModifiedPaint(comp);

            if (level == null || smooth == null || modHeight == null || paint == null || modPaint == null)
                return false;

            Color paintTarget = PaintColor(spec.Paint);
            bool any = false;

            int y0 = Mathf.Max(0, cy - span);
            int y1 = Mathf.Min(width, cy + span);
            int x0 = Mathf.Max(0, cx - span);
            int x1 = Mathf.Min(width, cx + span);

            // Smooth needs a frozen snapshot of the surrounding heights so every
            // vertex averages its ORIGINAL neighbours; without it the blur would
            // compound within a single pass and create artifacts.
            float[] src = null;
            int sx0 = 0, sy0 = 0, sw = 0, sh = 0;
            if (spec.Operation == TerrainOperation.Smooth)
            {
                sx0 = Mathf.Max(0, x0 - 1);
                sy0 = Mathf.Max(0, y0 - 1);
                int sx1 = Mathf.Min(width, x1 + 1);
                int sy1 = Mathf.Min(width, y1 + 1);
                sw = sx1 - sx0 + 1;
                sh = sy1 - sy0 + 1;
                src = new float[sw * sh];
                for (int i = sy0; i <= sy1; i++)
                    for (int j = sx0; j <= sx1; j++)
                        src[(i - sy0) * sw + (j - sx0)] = hm.GetHeight(j, i);
            }

            for (int i = y0; i <= y1; i++)
            {
                for (int j = x0; j <= x1; j++)
                {
                    float d;
                    if (spec.Shape == TerrainShape.Square)
                        d = Mathf.Max(Mathf.Abs(j - cx), Mathf.Abs(i - cy));
                    else
                        d = Mathf.Sqrt((j - cx) * (j - cx) + (i - cy) * (i - cy));

                    if (d > radiusIdx)
                        continue;

                    int idx = i * pitch + j;
                    float curWorld = hm.GetHeight(j, i);
                    float w = Falloff(d, radiusIdx, blendIdx, spec);

                    switch (spec.Operation)
                    {
                        case TerrainOperation.Flatten:
                        {
                            float newWorld = Mathf.Lerp(curWorld, spec.TargetWorld, w);
                            level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                            modHeight[idx] = true;
                            any = true;
                            break;
                        }
                        case TerrainOperation.Raise:
                        case TerrainOperation.Lower:
                        {
                            float sign = spec.Operation == TerrainOperation.Lower ? -1f : 1f;
                            float delta = spec.RaiseDelta * w * sign;
                            level[idx] = ClampDelta(level[idx] + delta, spec);
                            modHeight[idx] = true;
                            any = true;
                            break;
                        }
                        case TerrainOperation.Smooth:
                        {
                            float sum = 0f;
                            int n = 0;
                            int gi = i - sy0;
                            int gj = j - sx0;
                            for (int oy = -1; oy <= 1; oy++)
                            {
                                int ni = gi + oy;
                                if (ni < 0 || ni >= sh)
                                    continue;
                                for (int ox = -1; ox <= 1; ox++)
                                {
                                    int nj = gj + ox;
                                    if (nj < 0 || nj >= sw)
                                        continue;
                                    sum += src[ni * sw + nj];
                                    n++;
                                }
                            }

                            float avg = n > 0 ? sum / n : curWorld;
                            float newWorld = Mathf.Lerp(curWorld, avg, w);
                            level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                            modHeight[idx] = true;
                            any = true;
                            break;
                        }
                        case TerrainOperation.Paint:
                        {
                            if (spec.Paint == TerrainPaint.Reset)
                            {
                                if (modPaint[idx])
                                {
                                    modPaint[idx] = false;
                                    any = true;
                                }
                            }
                            else
                            {
                                Color c = paint[idx];
                                float a = c.a;
                                float k = Mathf.Clamp01(spec.PaintStrength) * w;
                                c = Color.Lerp(c, paintTarget, k);
                                if (spec.Paint != TerrainPaint.ClearVegetation)
                                    c.a = a;
                                paint[idx] = c;
                                modPaint[idx] = true;
                                any = true;
                            }
                            break;
                        }
                        case TerrainOperation.Restore:
                        {
                            if (level[idx] != 0f || smooth[idx] != 0f || modHeight[idx] || modPaint[idx])
                            {
                                level[idx] = 0f;
                                smooth[idx] = 0f;
                                modHeight[idx] = false;
                                modPaint[idx] = false;
                                any = true;
                            }
                            break;
                        }
                    }
                }
            }

            return any;
        }

        /// <summary>
        /// Builds a straight ramp between two marked points (A -> B). Every vertex
        /// inside the A-B corridor (width = 2 * Radius) is set so its height follows
        /// the linear slope between the terrain heights at A and B, fading out to the
        /// sides so it blends into the surrounding terrain.
        /// </summary>
        private static bool ApplyRampLine(TerrainComp comp, Heightmap hm, BrushSpec spec)
        {
            int width = hm.m_width;
            int pitch = width + 1;
            float scale = hm.m_scale;

            hm.WorldToVertex(spec.RampA, out int ax, out int ay);
            hm.WorldToVertex(spec.RampB, out int bx, out int by);

            // Endpoint heights are world-space so a ramp that crosses several
            // heightmap tiles still uses the same two heights on every tile.
            float hA = GroundHeight(spec.RampA);
            float hB = GroundHeight(spec.RampB);

            // Uniform scale, so working in "vertex * scale" space keeps distances in
            // meters; the constant world origin cancels out in every difference.
            float axw = ax * scale, ayw = ay * scale;
            float bxw = bx * scale, byw = by * scale;
            float dx = bxw - axw, dy = byw - ayw;
            float len2 = dx * dx + dy * dy;
            if (len2 < 0.0001f)
                return false;

            float halfWidth = Mathf.Max(0.5f, spec.Radius);
            // Flat core across the corridor, with a soft blend only near the sides.
            // The core matches the footprint's full-strength ring, so the ramp is
            // created exactly as wide as the preview shows.
            float sideFade = halfWidth * 0.5f * Mathf.Clamp01(spec.Softness / 100f);
            int halfIdx = Mathf.CeilToInt(halfWidth / scale);

            float[] level = LevelDelta(comp);
            bool[] modHeight = (bool[])f_modifiedHeight.GetValue(comp);
            if (level == null || modHeight == null)
                return false;

            int x0 = Mathf.Max(0, Mathf.Min(ax, bx) - halfIdx);
            int x1 = Mathf.Min(width, Mathf.Max(ax, bx) + halfIdx);
            int y0 = Mathf.Max(0, Mathf.Min(ay, by) - halfIdx);
            int y1 = Mathf.Min(width, Mathf.Max(ay, by) + halfIdx);

            bool any = false;
            for (int i = y0; i <= y1; i++)
            {
                for (int j = x0; j <= x1; j++)
                {
                    float px = j * scale;
                    float py = i * scale;

                    float t = ((px - axw) * dx + (py - ayw) * dy) / len2;
                    t = Mathf.Clamp01(t);

                    float nx = axw + t * dx;
                    float ny = ayw + t * dy;
                    float latDist = Mathf.Sqrt((px - nx) * (px - nx) + (py - ny) * (py - ny));
                    if (latDist > halfWidth)
                        continue;

                    float lat = 1f;
                    if (sideFade > 0.001f)
                        lat = Mathf.Clamp01((halfWidth - latDist) / sideFade);
                    float wl = lat * lat * (3f - 2f * lat);

                    float target = Mathf.Lerp(hA, hB, t);
                    float curWorld = hm.GetHeight(j, i);
                    float newWorld = Mathf.Lerp(curWorld, target, wl);

                    int idx = i * pitch + j;
                    level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                    modHeight[idx] = true;
                    any = true;
                }
            }

            return any;
        }

        private static float Falloff(float d, float radiusIdx, float blendIdx, BrushSpec spec)
        {
            switch (spec.Operation)
            {
                case TerrainOperation.Paint:
                {
                    float t = 1f - Mathf.Clamp01(d / radiusIdx);
                    return Mathf.Clamp01(Mathf.Pow(t, 0.1f));
                }
                case TerrainOperation.Raise:
                case TerrainOperation.Lower:
                {
                    float t = 1f - Mathf.Clamp01(d / radiusIdx);
                    if (spec.Power > 0f && spec.Power != 1f)
                        t = Mathf.Pow(t, spec.Power);
                    return Mathf.Clamp01(t);
                }
                default:
                {
                    if (blendIdx <= 0.0001f)
                        return 1f;
                    float w = Mathf.Clamp01((radiusIdx - d) / blendIdx);
                    return w * w * (3f - 2f * w); // smoothstep
                }
            }
        }

        private static float ClampDelta(float value, BrushSpec spec)
        {
            float maxUp = spec.MaxUp > 0f ? spec.MaxUp : 8f;
            float maxDown = spec.MaxDown > 0f ? spec.MaxDown : 8f;
            return Mathf.Clamp(value, -maxDown, maxUp);
        }

        private static float GroundHeight(Vector3 pos)
        {
            if (ZoneSystem.instance != null)
                return ZoneSystem.instance.GetGroundHeight(pos);
            return pos.y;
        }

        private static Color PaintColor(TerrainPaint paint)
        {
            switch (paint)
            {
                case TerrainPaint.Cultivated: return Heightmap.m_paintMaskCultivated;
                case TerrainPaint.Paved: return Heightmap.m_paintMaskPaved;
                case TerrainPaint.ClearVegetation: return Heightmap.m_paintMaskClearVegetation;
                case TerrainPaint.DeepSnow: return Heightmap.m_paintMaskDeepSnow;
                case TerrainPaint.Reset: return Heightmap.m_paintMaskNothing;
                default: return Heightmap.m_paintMaskDirt;
            }
        }
    }
}
