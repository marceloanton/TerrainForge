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
            Vector3 searchCenter = center;
            float searchRadius = spec.Radius + 4f;
            if (spec.Operation == TerrainOperation.Ramp && spec.HasRampLine)
            {
                searchRadius = Mathf.Max(spec.Radius, Vector3.Distance(spec.RampA, spec.RampB) * 0.5f + spec.Radius) + 4f;
            }
            else if (spec.HasLine)
            {
                searchCenter = (spec.LineA + spec.LineB) * 0.5f;
                searchRadius = Vector3.Distance(spec.LineA, spec.LineB) * 0.5f + spec.Radius + 4f;
            }
            else if (spec.HasRect)
            {
                searchCenter = (spec.RectA + spec.RectB) * 0.5f;
                float dx = Mathf.Abs(spec.RectB.x - spec.RectA.x);
                float dz = Mathf.Abs(spec.RectB.z - spec.RectA.z);
                searchRadius = Mathf.Sqrt(dx * dx + dz * dz) * 0.5f + EdgeBlendMeters(spec) + 4f;
            }
            else if (spec.HasPoly && spec.PolyPoints != null && spec.PolyPoints.Length >= 3)
            {
                Vector3 c = Vector3.zero;
                float maxD = 0f;
                for (int k = 0; k < spec.PolyPoints.Length; k++)
                    c += spec.PolyPoints[k];
                c /= spec.PolyPoints.Length;
                for (int k = 0; k < spec.PolyPoints.Length; k++)
                    maxD = Mathf.Max(maxD, Vector3.Distance(c, spec.PolyPoints[k]));
                searchCenter = c;
                searchRadius = maxD + EdgeBlendMeters(spec) + 4f;
            }

            Heightmap.FindHeightmap(searchCenter, searchRadius, s_hmaps);
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
                ClutterSystem.instance.ResetGrass(searchCenter, searchRadius + 1f);

            if (changed && frame != null && frame.Count > 0)
            {
                PushUndo(new UndoFrame
                {
                    Label = spec.Operation.ToString(),
                    Center = searchCenter,
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
            if (spec.HasLine || spec.HasRect || spec.HasPoly)
                return ApplyFootprint(comp, hm, spec, frame);

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

            var writer = new VertexWriter
            {
                Spec = spec,
                PaintTarget = paintTarget,
                Level = level,
                Smooth = smooth,
                ModH = modHeight,
                ModP = modPaint,
                Paint = paint,
                Snap = src,
                Sx0 = sx0,
                Sy0 = sy0,
                Sw = sw,
                Sh = sh,
                Rec = rec
            };

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
                    writer.Write(idx, j, i, curWorld, w);
                }
            }

            bool any = writer.Any;
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
        /// Writes one vertex for area ops (Flatten / Raise / Lower / Smooth /
        /// Paint / Restore). Shared by the circular brush and the Line / Rect /
        /// Poly footprints; only the caller-provided falloff weight differs.
        /// </summary>
        private sealed class VertexWriter
        {
            public BrushSpec Spec;
            public Color PaintTarget;
            public float[] Level;
            public float[] Smooth;
            public bool[] ModH;
            public bool[] ModP;
            public Color[] Paint;
            public float[] Snap;
            public int Sx0, Sy0, Sw, Sh;
            public Recorder Rec;
            public bool Any;

            public void Write(int idx, int j, int i, float curWorld, float w)
            {
                BrushSpec spec = Spec;
                switch (spec.Operation)
                {
                    case TerrainOperation.Flatten:
                    {
                        float newWorld = Mathf.Lerp(curWorld, spec.TargetWorld, w);
                        if (Mathf.Abs(newWorld - curWorld) > 0.0001f || !ModH[idx])
                        {
                            if (Rec != null)
                                Rec.Add(idx);
                            Level[idx] = ClampDelta(Level[idx] + (newWorld - curWorld), spec);
                            ModH[idx] = true;
                            Any = true;
                        }
                        break;
                    }
                    case TerrainOperation.Raise:
                    case TerrainOperation.Lower:
                    {
                        float sign = spec.Operation == TerrainOperation.Lower ? -1f : 1f;
                        float delta = spec.RaiseDelta * w * sign;
                        if (Mathf.Abs(delta) > 0.0001f || !ModH[idx])
                        {
                            if (Rec != null)
                                Rec.Add(idx);
                            Level[idx] = ClampDelta(Level[idx] + delta, spec);
                            ModH[idx] = true;
                            Any = true;
                        }
                        break;
                    }
                    case TerrainOperation.Smooth:
                    {
                        float sum = 0f;
                        int n = 0;
                        int gi = i - Sy0;
                        int gj = j - Sx0;
                        for (int oy = -1; oy <= 1; oy++)
                        {
                            int ni = gi + oy;
                            if (ni < 0 || ni >= Sh)
                                continue;
                            for (int ox = -1; ox <= 1; ox++)
                            {
                                int nj = gj + ox;
                                if (nj < 0 || nj >= Sw)
                                    continue;
                                sum += Snap[ni * Sw + nj];
                                n++;
                            }
                        }

                        float avg = n > 0 ? sum / n : curWorld;
                        float newWorld = Mathf.Lerp(curWorld, avg, w);
                        if (Mathf.Abs(newWorld - curWorld) > 0.0001f || !ModH[idx])
                        {
                            if (Rec != null)
                                Rec.Add(idx);
                            Level[idx] = ClampDelta(Level[idx] + (newWorld - curWorld), spec);
                            ModH[idx] = true;
                            Any = true;
                        }
                        break;
                    }
                    case TerrainOperation.Paint:
                    {
                        if (spec.Paint == TerrainPaint.Reset)
                        {
                            if (ModP[idx])
                            {
                                if (Rec != null)
                                    Rec.Add(idx);
                                ModP[idx] = false;
                                Any = true;
                            }
                        }
                        else
                        {
                            float k = Mathf.Clamp01(spec.PaintStrength) * w;
                            if (k > 0.0001f)
                            {
                                Color c = Paint[idx];
                                float a = c.a;
                                c = Color.Lerp(c, PaintTarget, k);
                                if (spec.Paint != TerrainPaint.ClearVegetation)
                                    c.a = a;
                                if (Rec != null)
                                    Rec.Add(idx);
                                Paint[idx] = c;
                                ModP[idx] = true;
                                Any = true;
                            }
                        }
                        break;
                    }
                    case TerrainOperation.Restore:
                    {
                        if (Level[idx] != 0f || Smooth[idx] != 0f || ModH[idx] || ModP[idx])
                        {
                            if (Rec != null)
                                Rec.Add(idx);
                            Level[idx] = 0f;
                            Smooth[idx] = 0f;
                            ModH[idx] = false;
                            ModP[idx] = false;
                            Any = true;
                        }
                        break;
                    }
                }
            }
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

        /// <summary>
        /// Applies an area op inside a Line (capsule), Rect or Poly footprint.
        /// Points are converted to vertex space (like the ramp) so every distance
        /// stays in meters; Rect/Poly use an edge blend, Line reuses the ramp's
        /// flat-core + side-fade profile with the brush radius as half-width.
        /// </summary>
        private static bool ApplyFootprint(TerrainComp comp, Heightmap hm, BrushSpec spec, List<UndoEntry> frame)
        {
            int width = hm.m_width;
            int pitch = width + 1;
            float scale = hm.m_scale;
            if (scale <= 0f)
                return false;

            float[] level = LevelDelta(comp);
            float[] smooth = SmoothDelta(comp);
            bool[] modHeight = (bool[])f_modifiedHeight.GetValue(comp);
            Color[] paint = PaintMask(comp);
            bool[] modPaint = ModifiedPaint(comp);
            if (level == null || smooth == null || modHeight == null || paint == null || modPaint == null)
                return false;

            int x0 = 0, x1 = 0, y0 = 0, y1 = 0;
            float blendMeters = EdgeBlendMeters(spec);

            // Per-shape data in vertex*scale (meter) space.
            float axw = 0f, ayw = 0f, bxw = 0f, byw = 0f; // line
            float halfWidth = Mathf.Max(0.5f, spec.Radius);
            float sideFade = halfWidth * 0.5f * Mathf.Clamp01(spec.Softness / 100f);
            float rx0 = 0f, rx1 = 0f, rz0 = 0f, rz1 = 0f; // rect (vertex units)
            float[] px = null, pz = null;                 // poly (vertex units)

            if (spec.HasLine)
            {
                hm.WorldToVertex(spec.LineA, out int ax, out int ay);
                hm.WorldToVertex(spec.LineB, out int bx, out int by);
                if (ax == bx && ay == by)
                    return false;
                axw = ax * scale; ayw = ay * scale;
                bxw = bx * scale; byw = by * scale;
                int pad = Mathf.CeilToInt(halfWidth / scale) + 1;
                x0 = Mathf.Max(0, Mathf.Min(ax, bx) - pad);
                x1 = Mathf.Min(width, Mathf.Max(ax, bx) + pad);
                y0 = Mathf.Max(0, Mathf.Min(ay, by) - pad);
                y1 = Mathf.Min(width, Mathf.Max(ay, by) + pad);
            }
            else if (spec.HasRect)
            {
                hm.WorldToVertex(spec.RectA, out int ax, out int ay);
                hm.WorldToVertex(spec.RectB, out int bx, out int by);
                if (ax == bx || ay == by)
                    return false;
                rx0 = Mathf.Min(ax, bx); rx1 = Mathf.Max(ax, bx);
                rz0 = Mathf.Min(ay, by); rz1 = Mathf.Max(ay, by);
                int pad = Mathf.CeilToInt(blendMeters / scale) + 1;
                x0 = Mathf.Max(0, (int)rx0 - pad);
                x1 = Mathf.Min(width, (int)rx1 + pad);
                y0 = Mathf.Max(0, (int)rz0 - pad);
                y1 = Mathf.Min(width, (int)rz1 + pad);
            }
            else if (spec.HasPoly && spec.PolyPoints != null && spec.PolyPoints.Length >= 3)
            {
                int n = spec.PolyPoints.Length;
                px = new float[n];
                pz = new float[n];
                float minx = float.MaxValue, maxx = float.MinValue;
                float minz = float.MaxValue, maxz = float.MinValue;
                for (int k = 0; k < n; k++)
                {
                    hm.WorldToVertex(spec.PolyPoints[k], out int vx, out int vy);
                    px[k] = vx; pz[k] = vy;
                    if (vx < minx) minx = vx;
                    if (vx > maxx) maxx = vx;
                    if (vy < minz) minz = vy;
                    if (vy > maxz) maxz = vy;
                }
                if (maxx - minx < 1f && maxz - minz < 1f)
                    return false;
                int pad = Mathf.CeilToInt(blendMeters / scale) + 1;
                x0 = Mathf.Max(0, (int)minx - pad);
                x1 = Mathf.Min(width, (int)maxx + pad);
                y0 = Mathf.Max(0, (int)minz - pad);
                y1 = Mathf.Min(width, (int)maxz + pad);
            }
            else
            {
                return false;
            }

            // Frozen snapshot for Smooth, over the bbox expanded by one vertex.
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

            Recorder rec = frame != null
                ? new Recorder(level, smooth, modHeight, paint, modPaint)
                : null;

            var writer = new VertexWriter
            {
                Spec = spec,
                PaintTarget = PaintColor(spec.Paint),
                Level = level,
                Smooth = smooth,
                ModH = modHeight,
                ModP = modPaint,
                Paint = paint,
                Snap = src,
                Sx0 = sx0,
                Sy0 = sy0,
                Sw = sw,
                Sh = sh,
                Rec = rec
            };

            for (int i = y0; i <= y1; i++)
            {
                for (int j = x0; j <= x1; j++)
                {
                    float w = -1f;
                    if (spec.HasLine)
                    {
                        float dSeg = DistToSegment(j * scale, i * scale, axw, ayw, bxw, byw);
                        if (dSeg > halfWidth)
                            continue;
                        float lat = 1f;
                        if (sideFade > 0.001f)
                            lat = Mathf.Clamp01((halfWidth - dSeg) / sideFade);
                        w = lat * lat * (3f - 2f * lat);
                    }
                    else if (spec.HasRect)
                    {
                        if (j < rx0 || j > rx1 || i < rz0 || i > rz1)
                            continue;
                        float dEdge = Mathf.Min(Mathf.Min(j - rx0, rx1 - j), Mathf.Min(i - rz0, rz1 - i)) * scale;
                        w = EdgeWeight(dEdge, blendMeters);
                    }
                    else
                    {
                        if (!PointInPoly(j, i, px, pz))
                            continue;
                        w = EdgeWeight(DistToPoly(j, i, px, pz) * scale, blendMeters);
                    }

                    int idx = i * pitch + j;
                    writer.Write(idx, j, i, hm.GetHeight(j, i), w);
                }
            }

            bool any = writer.Any;
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

        private static float EdgeBlendMeters(BrushSpec spec)
        {
            return Mathf.Max(0.05f, spec.Radius * 0.5f * Mathf.Clamp01(spec.Softness / 100f));
        }

        private static float EdgeWeight(float distMeters, float blendMeters)
        {
            if (blendMeters <= 0.001f)
                return 1f;
            float t = Mathf.Clamp01(distMeters / blendMeters);
            return t * t * (3f - 2f * t);
        }

        private static float DistToSegment(float px, float pz, float ax, float az, float bx, float bz)
        {
            float dx = bx - ax, dz = bz - az;
            float len2 = dx * dx + dz * dz;
            if (len2 < 0.000001f)
                return Mathf.Sqrt((px - ax) * (px - ax) + (pz - az) * (pz - az));
            float t = ((px - ax) * dx + (pz - az) * dz) / len2;
            t = Mathf.Clamp01(t);
            float nx = ax + t * dx, nz = az + t * dz;
            return Mathf.Sqrt((px - nx) * (px - nx) + (pz - nz) * (pz - nz));
        }

        private static bool PointInPoly(float x, float z, float[] px, float[] pz)
        {
            bool inside = false;
            int n = px.Length;
            for (int a = 0, b = n - 1; a < n; b = a++)
            {
                float ax = px[a], az = pz[a], bx = px[b], bz = pz[b];
                if (((az > z) != (bz > z)) && (x < (bx - ax) * (z - az) / (bz - az) + ax))
                    inside = !inside;
            }
            return inside;
        }

        private static float DistToPoly(float x, float z, float[] px, float[] pz)
        {
            float m = float.MaxValue;
            int n = px.Length;
            for (int a = 0, b = n - 1; a < n; b = a++)
            {
                float d = DistToSegment(x, z, px[a], pz[a], px[b], pz[b]);
                if (d < m)
                    m = d;
            }
            return m;
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
