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

            // One undo frame per Apply call: every comp touched below records
            // the old values of exactly the vertices it overwrites.
            List<UndoEntry> frame = null;
            if (GetDepth() > 0)
                frame = new List<UndoEntry>();

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

                if (ApplyToComp(comp, hm, center, spec, frame))
                {
                    Save(comp);
                    hm.Poke(0);
                    changed = true;
                }
            }

            if (changed && ClutterSystem.instance != null)
                ClutterSystem.instance.ResetGrass(center, searchRadius + 1f);

            if (changed && frame != null && frame.Count > 0)
            {
                PushUndo(new UndoFrame
                {
                    Label = spec.Operation.ToString(),
                    Center = center,
                    Radius = searchRadius,
                    Entries = frame
                });
            }
            else if (GetDepth() <= 0)
            {
                ClearHistory();
            }

            return changed;
        }

        // ---- undo / redo -------------------------------------------------------

        private static readonly List<UndoFrame> s_undo = new List<UndoFrame>();
        private static readonly List<UndoFrame> s_redo = new List<UndoFrame>();

        public static int UndoCount => s_undo.Count;
        public static int RedoCount => s_redo.Count;

        public static void ClearHistory()
        {
            s_undo.Clear();
            s_redo.Clear();
        }

        private static int GetDepth()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.UndoDepth == null)
                return 50;
            return Mathf.Clamp(p.UndoDepth.Value, 0, 500);
        }

        private static void PushUndo(UndoFrame f)
        {
            s_undo.Add(f);
            TrimStack(s_undo);
            s_redo.Clear();
        }

        private static void TrimStack(List<UndoFrame> stack)
        {
            int depth = GetDepth();
            while (stack.Count > depth)
                stack.RemoveAt(0);
        }

        public static bool Undo(out string label) => Step(s_undo, s_redo, out label);
        public static bool Redo(out string label) => Step(s_redo, s_undo, out label);

        private static bool Step(List<UndoFrame> from, List<UndoFrame> to, out string label)
        {
            label = null;
            if (from.Count == 0)
                return false;

            UndoFrame f = from[from.Count - 1];
            from.RemoveAt(from.Count - 1);

            var back = new UndoFrame { Label = f.Label, Center = f.Center, Radius = f.Radius };
            bool ok = false;
            for (int i = 0; i < f.Entries.Count; i++)
            {
                UndoEntry restored = CaptureAndRestore(f.Entries[i]);
                if (restored != null)
                {
                    back.Entries.Add(restored);
                    ok = true;
                }
            }

            if (back.Entries.Count > 0)
            {
                to.Add(back);
                TrimStack(to);
            }

            if (ok && ClutterSystem.instance != null)
                ClutterSystem.instance.ResetGrass(f.Center, f.Radius + 1f);

            label = f.Label;
            return ok;
        }

        /// <summary>
        /// Writes an entry's old values back into its TerrainComp and returns a new
        /// entry holding the values that were there (for the opposite direction).
        /// Returns null when the comp is gone or ownership failed.
        /// </summary>
        private static UndoEntry CaptureAndRestore(UndoEntry e)
        {
            if (e == null || e.Comp == null || e.Before == null || e.Before.Count == 0)
                return null;
            if (!EnsureOwnership(e.Comp))
                return null;

            float[] level = LevelDelta(e.Comp);
            float[] smooth = e.WithSmooth ? SmoothDelta(e.Comp) : null;
            bool[] modHeight = null;
            Color[] paint = e.WithPaint ? PaintMask(e.Comp) : null;
            bool[] modPaint = e.WithPaint ? ModifiedPaint(e.Comp) : null;
            try { modHeight = (bool[])f_modifiedHeight.GetValue(e.Comp); }
            catch { modHeight = null; }

            if (level == null || modHeight == null)
                return null;
            if (e.WithSmooth && smooth == null)
                return null;
            if (e.WithPaint && (paint == null || modPaint == null))
                return null;

            var back = new UndoEntry
            {
                Comp = e.Comp,
                Hm = e.Hm,
                WithSmooth = e.WithSmooth,
                WithPaint = e.WithPaint
            };

            for (int k = 0; k < e.Before.Count; k++)
            {
                UndoCell old = e.Before[k];
                int idx = old.Index;
                if (idx < 0 || idx >= level.Length)
                    continue;

                back.Before.Add(new UndoCell
                {
                    Index = idx,
                    Level = level[idx],
                    Smooth = smooth != null ? smooth[idx] : 0f,
                    ModifiedHeight = modHeight[idx],
                    Paint = paint != null ? paint[idx] : default(Color),
                    ModifiedPaint = modPaint != null && modPaint[idx]
                });

                level[idx] = old.Level;
                if (smooth != null)
                    smooth[idx] = old.Smooth;
                modHeight[idx] = old.ModifiedHeight;
                if (paint != null)
                    paint[idx] = old.Paint;
                if (modPaint != null)
                    modPaint[idx] = old.ModifiedPaint;
            }

            Save(e.Comp);
            if (e.Hm != null)
            {
                try { e.Hm.Poke(0); }
                catch { /* heightmap gone */ }
            }

            return back;
        }

        /// <summary>
        /// Collects the pre-write values of touched vertices. Pass null arrays for
        /// channels an operation never writes (e.g. the ramp only writes level).
        /// </summary>
        private sealed class Recorder
        {
            public readonly List<UndoCell> Cells = new List<UndoCell>();
            private readonly float[] _level;
            private readonly float[] _smooth;
            private readonly bool[] _modH;
            private readonly Color[] _paint;
            private readonly bool[] _modP;

            public Recorder(float[] level, float[] smooth, bool[] modH, Color[] paint, bool[] modP)
            {
                _level = level;
                _smooth = smooth;
                _modH = modH;
                _paint = paint;
                _modP = modP;
            }

            public bool TrackSmooth => _smooth != null;
            public bool TrackPaint => _paint != null;

            public void Add(int idx)
            {
                Cells.Add(new UndoCell
                {
                    Index = idx,
                    Level = _level[idx],
                    Smooth = _smooth != null ? _smooth[idx] : 0f,
                    ModifiedHeight = _modH[idx],
                    Paint = _paint != null ? _paint[idx] : default(Color),
                    ModifiedPaint = _modP != null && _modP[idx]
                });
            }
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

        private static bool ApplyToComp(TerrainComp comp, Heightmap hm, Vector3 center, BrushSpec spec, List<UndoEntry> frame)
        {
            if (spec.Operation == TerrainOperation.Ramp && spec.HasRampLine)
                return ApplyRampLine(comp, hm, spec, frame);

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

            Recorder rec = frame != null
                ? new Recorder(level, smooth, modHeight, paint, modPaint)
                : null;

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
                            if (Mathf.Abs(newWorld - curWorld) > 0.0001f || !modHeight[idx])
                            {
                                if (rec != null)
                                    rec.Add(idx);
                                level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                                modHeight[idx] = true;
                                any = true;
                            }
                            break;
                        }
                        case TerrainOperation.Raise:
                        case TerrainOperation.Lower:
                        {
                            float sign = spec.Operation == TerrainOperation.Lower ? -1f : 1f;
                            float delta = spec.RaiseDelta * w * sign;
                            if (Mathf.Abs(delta) > 0.0001f || !modHeight[idx])
                            {
                                if (rec != null)
                                    rec.Add(idx);
                                level[idx] = ClampDelta(level[idx] + delta, spec);
                                modHeight[idx] = true;
                                any = true;
                            }
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
                            if (Mathf.Abs(newWorld - curWorld) > 0.0001f || !modHeight[idx])
                            {
                                if (rec != null)
                                    rec.Add(idx);
                                level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                                modHeight[idx] = true;
                                any = true;
                            }
                            break;
                        }
                        case TerrainOperation.Paint:
                        {
                            if (spec.Paint == TerrainPaint.Reset)
                            {
                                if (modPaint[idx])
                                {
                                    if (rec != null)
                                        rec.Add(idx);
                                    modPaint[idx] = false;
                                    any = true;
                                }
                            }
                            else
                            {
                                float k = Mathf.Clamp01(spec.PaintStrength) * w;
                                if (k > 0.0001f)
                                {
                                    Color c = paint[idx];
                                    float a = c.a;
                                    c = Color.Lerp(c, paintTarget, k);
                                    if (spec.Paint != TerrainPaint.ClearVegetation)
                                        c.a = a;
                                    if (rec != null)
                                        rec.Add(idx);
                                    paint[idx] = c;
                                    modPaint[idx] = true;
                                    any = true;
                                }
                            }
                            break;
                        }
                        case TerrainOperation.Restore:
                        {
                            if (level[idx] != 0f || smooth[idx] != 0f || modHeight[idx] || modPaint[idx])
                            {
                                if (rec != null)
                                    rec.Add(idx);
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

            if (any && frame != null && rec != null && rec.Cells.Count > 0)
            {
                frame.Add(new UndoEntry
                {
                    Comp = comp,
                    Hm = hm,
                    WithSmooth = true,
                    WithPaint = true,
                    Before = rec.Cells
                });
            }

            return any;
        }

        /// <summary>
        /// Builds a straight ramp between two marked points (A -> B). Every vertex
        /// inside the A-B corridor (width = 2 * Radius) is set so its height follows
        /// the linear slope between the terrain heights at A and B, fading out to the
        /// sides so it blends into the surrounding terrain.
        /// </summary>
        private static bool ApplyRampLine(TerrainComp comp, Heightmap hm, BrushSpec spec, List<UndoEntry> frame)
        {
            int width = hm.m_width;
            int pitch = width + 1;
            float scale = hm.m_scale;

            hm.WorldToVertex(spec.RampA, out int ax, out int ay);
            hm.WorldToVertex(spec.RampB, out int bx, out int by);

            // Endpoint heights are world-space so a ramp that crosses several
            // heightmap tiles still uses the same two heights on every tile.
            // Fixed-angle ramps pass literal heights instead of sampling.
            float hA = spec.UseFixedHeights ? spec.FixedHA : GroundHeight(spec.RampA);
            float hB = spec.UseFixedHeights ? spec.FixedHB : GroundHeight(spec.RampB);

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

            // The ramp only writes level deltas, so only those are recorded.
            Recorder rec = frame != null
                ? new Recorder(level, null, modHeight, null, null)
                : null;

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
                    if (Mathf.Abs(newWorld - curWorld) > 0.0001f || !modHeight[idx])
                    {
                        if (rec != null)
                            rec.Add(idx);
                        level[idx] = ClampDelta(level[idx] + (newWorld - curWorld), spec);
                        modHeight[idx] = true;
                        any = true;
                    }
                }
            }

            if (any && frame != null && rec != null && rec.Cells.Count > 0)
            {
                frame.Add(new UndoEntry
                {
                    Comp = comp,
                    Hm = hm,
                    WithSmooth = false,
                    WithPaint = false,
                    Before = rec.Cells
                });
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
