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
            while (!Pump())
            {
                // Drain any previous stroke first (FIFO order).
            }
            if (spec.Radius <= 0.01f)
                return false;

            s_hmaps.Clear();
            Vector3 searchCenter = center;
            float searchRadius = spec.Radius + 4f;
            if (spec.Operation == TerrainOperation.Ramp && spec.HasRampLine)
            {
                searchRadius = Mathf.Max(spec.Radius, Vector3.Distance(spec.RampA, spec.RampB) * 0.5f + spec.Radius) + 4f;
            }
            else if (spec.HasRampPath && spec.RampPath != null && spec.RampPath.Length >= 3)
            {
                Vector3 c = Vector3.zero;
                float maxD = 0f;
                for (int k = 0; k < spec.RampPath.Length; k++)
                    c += spec.RampPath[k];
                c /= spec.RampPath.Length;
                for (int k = 0; k < spec.RampPath.Length; k++)
                    maxD = Mathf.Max(maxD, Vector3.Distance(c, spec.RampPath[k]));
                searchCenter = c;
                searchRadius = maxD + spec.Radius + 4f;
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

            // Chunked application: tiles are processed a few per frame so huge
            // radii never hitch the game. One undo frame spans the whole stroke.
            s_pending = new PendingApply
            {
                Hmaps = new List<Heightmap>(s_hmaps),
                Index = 0,
                Center = center,
                SearchCenter = searchCenter,
                SearchRadius = searchRadius,
                Spec = spec,
                Frame = GetDepth() > 0 ? new List<UndoEntry>() : null,
                Changed = false,
                Label = spec.Operation.ToString()
            };
            Pump();
            return s_pending != null || s_pendingChanged;
        }

        private static bool s_pendingChanged;
        private static float s_dbgApplyAt;

        /// <summary>
        /// A brush stroke in progress: tiles left to process plus the undo frame
        /// being accumulated. Completed strokes push a single undo step.
        /// </summary>
        private sealed class PendingApply
        {
            public List<Heightmap> Hmaps;
            public int Index;
            public Vector3 Center;
            public Vector3 SearchCenter;
            public float SearchRadius;
            public BrushSpec Spec;
            public List<UndoEntry> Frame;
            public bool Changed;
            public string Label;
        }

        private static PendingApply s_pending;

        public static bool HasPending => s_pending != null;

        private static int ChunkBudget()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.ChunkCompsPerFrame == null)
                return 4;
            return Mathf.Clamp(p.ChunkCompsPerFrame.Value, 1, 32);
        }

        /// <summary>
        /// Processes up to ChunkCompsPerFrame tiles of the pending stroke.
        /// Returns true when nothing remains pending.
        /// </summary>
        public static bool Pump()
        {
            PendingApply p = s_pending;
            if (p == null)
                return true;

            int budget = ChunkBudget();
            int n = 0;
            while (p.Index < p.Hmaps.Count && n < budget)
            {
                Heightmap hm = p.Hmaps[p.Index++];
                n++;
                if (hm == null || hm.m_scale <= 0f)
                    continue;

                TerrainComp comp = hm.GetAndCreateTerrainCompiler();
                if (comp == null)
                    continue;

                if (!EnsureOwnership(comp))
                    continue;

                if (ApplyToComp(comp, hm, p.Center, p.Spec, p.Frame))
                {
                    Save(comp);
                    hm.Poke(0);
                    p.Changed = true;
                    if (Time.time >= s_dbgApplyAt)
                    {
                        s_dbgApplyAt = Time.time + 0.5f;
                        float[] lv = LevelDelta(comp);
                        int mh = 0;
                        float mx = 0f;
                        if (lv != null)
                            for (int q = 0; q < lv.Length; q++)
                            {
                                bool[] m = (bool[])f_modifiedHeight.GetValue(comp);
                                if (m != null && m[q]) mh++;
                                float aa = Mathf.Abs(lv[q]);
                                if (aa > mx) mx = aa;
                            }
                        TerrainForgePlugin.Log?.LogInfo("[TF] applied op=" + p.Spec.Operation +
                            " modH=" + mh + " maxAbsLevel=" + mx.ToString("0.00") +
                            " origin=" + hm.transform.position);
                    }
                }
            }

            if (p.Index < p.Hmaps.Count)
            {
                s_pendingChanged = p.Changed;
                return false;
            }

            if (p.Changed && ClutterSystem.instance != null)
                ClutterSystem.instance.ResetGrass(p.SearchCenter, p.SearchRadius + 1f);

            if (p.Changed && p.Frame != null && p.Frame.Count > 0)
            {
                PushUndo(new UndoFrame
                {
                    Label = p.Label,
                    Center = p.SearchCenter,
                    Radius = p.SearchRadius,
                    Entries = p.Frame
                });
            }
            else if (GetDepth() <= 0)
            {
                ClearHistory();
            }

            s_pendingChanged = p.Changed;
            s_pending = null;
            return true;
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
            if (spec.HasRampPath)
                return ApplyRampPath(comp, hm, spec, frame);
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
            if (x1 < x0 || y1 < y0)
                return false;

            // Smooth needs a frozen snapshot of the surrounding heights so every
            // vertex averages its ORIGINAL neighbours; without it the blur would
            // compound within a single pass and create artifacts. The 1-vertex
            // margin is intentionally UNCLAMPED: border vertices sample across
            // the tile edge (see ExtendedHeight) so smoothing never opens a
            // seam between neighbouring tiles.
            float[] src = null;
            int sx0 = 0, sy0 = 0, sw = 0, sh = 0;
            if (spec.Operation == TerrainOperation.Smooth)
            {
                sx0 = x0 - 1;
                sy0 = y0 - 1;
                int sx1 = x1 + 1;
                int sy1 = y1 + 1;
                sw = sx1 - sx0 + 1;
                sh = sy1 - sy0 + 1;
                src = new float[sw * sh];
                for (int i = sy0; i <= sy1; i++)
                    for (int j = sx0; j <= sx1; j++)
                        src[(i - sy0) * sw + (j - sx0)] = ExtendedHeight(hm, width, scale, j, i);
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
                        bool dirOk = spec.SmoothMode == SmoothMode.Full ||
                            (spec.SmoothMode == SmoothMode.Bumps && newWorld < curWorld) ||
                            (spec.SmoothMode == SmoothMode.Dips && newWorld > curWorld);
                        if (dirOk && (Mathf.Abs(newWorld - curWorld) > 0.0001f || !ModH[idx]))
                        {
                            if (Rec != null)
                                Rec.Add(idx);
                            Level[idx] = ClampDelta(Level[idx] + (newWorld - curWorld), spec);
                            ModH[idx] = true;
                            Any = true;
                        }
                        break;
                    }
                    case TerrainOperation.Shore:
                    {
                        float newWorld = Mathf.Lerp(curWorld, spec.ShoreTarget, w);
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
                        if (spec.Paint == TerrainPaint.DeepSnow && spec.SnowClear)
                        {
                            if (ModP[idx])
                            {
                                if (Rec != null) Rec.Add(idx);
                                ModP[idx] = false;
                                Any = true;
                            }
                        }
                        else if (spec.Paint == TerrainPaint.Reset)
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
                            PaintToward(PaintTarget, spec.Paint != TerrainPaint.ClearVegetation, idx, w);
                        }
                        break;
                    }
                    case TerrainOperation.Snow:
                    {
                        if (spec.SnowClear)
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
                            PaintToward(PaintColor(TerrainPaint.DeepSnow), true, idx, w);
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

            private void PaintToward(Color target, bool preserveAlpha, int idx, float w)
            {
                float k = Mathf.Clamp01(Spec.PaintStrength) * w;
                if (k <= 0.0001f)
                    return;
                Color c = Paint[idx];
                float a = c.a;
                c = Color.Lerp(c, target, k);
                if (preserveAlpha)
                    c.a = a;
                if (Rec != null)
                    Rec.Add(idx);
                Paint[idx] = c;
                ModP[idx] = true;
                Any = true;
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
        /// Builds a smooth ramp along a 3+ point path. Control points are densified
        /// with Catmull-Rom (≈0.5 vertex between samples); every vertex takes the
        /// height of its nearest sample, with the usual flat-core + side-fade
        /// profile across the corridor (width = 2 * Radius). One-shot call.
        /// </summary>
        private static bool ApplyRampPath(TerrainComp comp, Heightmap hm, BrushSpec spec, List<UndoEntry> frame)
        {
            int width = hm.m_width;
            int pitch = width + 1;
            float scale = hm.m_scale;
            if (scale <= 0f)
                return false;
            if (spec.RampPath == null || spec.RampPath.Length < 3 ||
                spec.RampPathH == null || spec.RampPathH.Length != spec.RampPath.Length)
                return false;

            int n = spec.RampPath.Length;
            float[] vx = new float[n];
            float[] vy = new float[n];
            for (int k = 0; k < n; k++)
            {
                hm.WorldToVertex(spec.RampPath[k], out int qx, out int qy);
                vx[k] = qx;
                vy[k] = qy;
            }

            var sPts = new List<Vector2>();
            var sH = new List<float>();
            for (int i = 0; i < n - 1; i++)
            {
                int i0 = Mathf.Max(0, i - 1);
                int i3 = Mathf.Min(n - 1, i + 2);
                float segLen = Vector2.Distance(new Vector2(vx[i], vy[i]), new Vector2(vx[i + 1], vy[i + 1]));
                int steps = Mathf.Max(2, Mathf.CeilToInt(segLen / 0.5f));
                for (int s = (i == 0 ? 0 : 1); s <= steps; s++)
                {
                    float t = (float)s / steps;
                    sPts.Add(new Vector2(
                        CR(vx[i0], vx[i], vx[i + 1], vx[i3], t),
                        CR(vy[i0], vy[i], vy[i + 1], vy[i3], t)));
                    sH.Add(Mathf.Lerp(spec.RampPathH[i], spec.RampPathH[i + 1], t));
                }
            }
            if (sPts.Count < 2)
                return false;

            float halfWidth = Mathf.Max(0.5f, spec.Radius);
            float sideFade = halfWidth * 0.5f * Mathf.Clamp01(spec.Softness / 100f);

            float minx = float.MaxValue, maxx = float.MinValue;
            float minz = float.MaxValue, maxz = float.MinValue;
            for (int k = 0; k < sPts.Count; k++)
            {
                if (sPts[k].x < minx) minx = sPts[k].x;
                if (sPts[k].x > maxx) maxx = sPts[k].x;
                if (sPts[k].y < minz) minz = sPts[k].y;
                if (sPts[k].y > maxz) maxz = sPts[k].y;
            }
            int pad = Mathf.CeilToInt(halfWidth / scale) + 1;
            int x0 = Mathf.Max(0, (int)minx - pad);
            int x1 = Mathf.Min(width, (int)maxx + pad);
            int y0 = Mathf.Max(0, (int)minz - pad);
            int y1 = Mathf.Min(width, (int)maxz + pad);

            float[] level = LevelDelta(comp);
            bool[] modHeight = (bool[])f_modifiedHeight.GetValue(comp);
            if (level == null || modHeight == null)
                return false;

            Recorder rec = frame != null
                ? new Recorder(level, null, modHeight, null, null)
                : null;

            bool any = false;
            for (int i = y0; i <= y1; i++)
            {
                for (int j = x0; j <= x1; j++)
                {
                    float best = float.MaxValue;
                    float bestH = 0f;
                    for (int k = 0; k < sPts.Count - 1; k++)
                    {
                        Vector2 A = sPts[k];
                        Vector2 B = sPts[k + 1];
                        float dx = B.x - A.x, dz = B.y - A.y;
                        float len2 = dx * dx + dz * dz;
                        float t = len2 < 0.000001f ? 0f : ((j - A.x) * dx + (i - A.y) * dz) / len2;
                        t = Mathf.Clamp01(t);
                        float nx = A.x + t * dx, nz = A.y + t * dz;
                        float ddx = j - nx, ddz = i - nz;
                        float d = Mathf.Sqrt(ddx * ddx + ddz * ddz);
                        if (d < best)
                        {
                            best = d;
                            bestH = Mathf.Lerp(sH[k], sH[k + 1], t);
                        }
                    }

                    float latDist = best * scale;
                    if (latDist > halfWidth)
                        continue;

                    float lat = 1f;
                    if (sideFade > 0.001f)
                        lat = Mathf.Clamp01((halfWidth - latDist) / sideFade);
                    float wl = lat * lat * (3f - 2f * lat);

                    float curWorld = hm.GetHeight(j, i);
                    float newWorld = Mathf.Lerp(curWorld, bestH, wl);

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

        private static float CR(float p0, float p1, float p2, float p3, float t)
        {
            float t2 = t * t, t3 = t2 * t;
            return 0.5f * ((2f * p1) + (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
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

            if (x1 < x0 || y1 < y0)
                return false;

            // Frozen snapshot for Smooth, over the bbox expanded by one vertex
            // (unclamped: border vertices sample across the tile edge, so the
            // blur stays seamless between neighbouring tiles).
            float[] src = null;
            int sx0 = 0, sy0 = 0, sw = 0, sh = 0;
            if (spec.Operation == TerrainOperation.Smooth)
            {
                sx0 = x0 - 1;
                sy0 = y0 - 1;
                int sx1 = x1 + 1;
                int sy1 = y1 + 1;
                sw = sx1 - sx0 + 1;
                sh = sy1 - sy0 + 1;
                src = new float[sw * sh];
                for (int i = sy0; i <= sy1; i++)
                    for (int j = sx0; j <= sx1; j++)
                        src[(i - sy0) * sw + (j - sx0)] = ExtendedHeight(hm, width, scale, j, i);
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
                case TerrainOperation.Snow:
                case TerrainOperation.CopyPaste:
                {
                    float t = 1f - Mathf.Clamp01(d / radiusIdx);
                    return Mathf.Clamp01(Mathf.Pow(t, 0.1f));
                }
                case TerrainOperation.Raise:
                case TerrainOperation.Lower:
                {
                    float t = 1f - Mathf.Clamp01(d / radiusIdx);
                    switch (spec.Falloff)
                    {
                        case FalloffMode.Flat:
                            // Uniform disk: constant weight inside radius
                            return 1f;
                        case FalloffMode.Smooth:
                            // Cosine falloff: smooth bell curve
                            return Mathf.Clamp01(Mathf.Cos(t * Mathf.PI * 0.5f));
                        case FalloffMode.Linear:
                            // Linear taper from center to edge
                            return Mathf.Clamp01(t);
                        case FalloffMode.Pointy:
                        default:
                            // Original: power falloff (sharp center peak)
                            if (spec.Power > 0f && spec.Power != 1f)
                                t = Mathf.Pow(t, spec.Power);
                            return Mathf.Clamp01(t);
                    }
                }
                default:
                {
                    if (spec.Operation == TerrainOperation.Shore)
                    {
                        // Shore-specific falloff using ShoreFalloffMode
                        float t = 1f - Mathf.Clamp01(d / radiusIdx);
                        switch (spec.ShoreFalloff)
                        {
                            case ShoreFalloffMode.Linear:
                                return Mathf.Clamp01(t);
                            case ShoreFalloffMode.Smooth:
                                return Mathf.Clamp01(Mathf.Cos(t * Mathf.PI * 0.5f));
                            case ShoreFalloffMode.Flat:
                                return 1f;
                            case ShoreFalloffMode.Pointy:
                            default:
                                if (spec.Power > 0f && spec.Power != 1f)
                                    t = Mathf.Pow(t, spec.Power);
                                return Mathf.Clamp01(t);
                        }
                    }
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

        private static readonly List<Heightmap> s_borderHmaps = new List<Heightmap>();

        /// <summary>
        /// Height of vertex (j,i), sampling the neighbouring tile when it falls
        /// outside this one. Vertex positions invert the game's own WorldToVertex
        /// mapping exactly, so the cross-border sample lands on the true neighbour
        /// vertex (with its own deltas). Falls back to the clamped local height.
        /// </summary>
        private static float ExtendedHeight(Heightmap hm, int width, float scale, int j, int i)
        {
            if (j >= 0 && j <= width && i >= 0 && i <= width)
                return hm.GetHeight(j, i);
            try
            {
                Vector3 origin = hm.transform.position;
                Vector3 wpos = new Vector3(
                    origin.x + (j - width * 0.5f) * scale, 0f,
                    origin.z + (i - width * 0.5f) * scale);
                s_borderHmaps.Clear();
                Heightmap.FindHeightmap(wpos, 2f, s_borderHmaps);
                for (int k = 0; k < s_borderHmaps.Count; k++)
                {
                    Heightmap nhm = s_borderHmaps[k];
                    if (nhm == null || nhm == hm || nhm.m_scale <= 0f)
                        continue;
                    nhm.WorldToVertex(wpos, out int nj, out int ni);
                    int nw = nhm.m_width;
                    if (nj < 0 || ni < 0 || nj > nw || ni > nw)
                        continue;
                    return nhm.GetHeight(nj, ni);
                }
            }
            catch { /* fall through to clamped local height */ }
            return hm.GetHeight(Mathf.Clamp(j, 0, width), Mathf.Clamp(i, 0, width));
        }

        internal static float WaterLevel()
        {
            if (ZoneSystem.instance != null)
                return ZoneSystem.instance.m_waterLevel;
            return ZoneSystem.c_WaterLevel;
        }

        // ---- terrain stamps (copy/paste in RAM) ------------------------------------

        /// <summary>
        /// One copied terrain area: a set of tiles with their full delta arrays.
        /// Can be stored in memory (clipboard) or serialized to disk.
        /// </summary>
        public sealed class TerrainStamp
        {
            public List<StampTile> Tiles = new List<StampTile>();
            public Vector3 Center;      // world center of the copied area
            public float Radius;        // approximate radius
            public string Label;        // e.g. "Copy: 5 tiles"
            public DateTime Time;       // when it was copied
            public string Name;         // user name for saved stamps (empty = clipboard)
            public bool RegionMask;     // true = per-vertex ModH is a selection mask (rect copy)
        }

        public sealed class StampTile
        {
            public Vector3 TilePos;     // tile world position (Heightmap transform)
            public int Width;           // vertex grid size (width+1 per side)
            public float Size;          // footprint side in metres (runtime only, for preview)
            public float[] Level;
            public float[] Smooth;
            public bool[] ModH;
            public Color[] Paint;
            public bool[] ModP;
        }

        private const int MaxClipboardStamps = 10; // RAM history
        private static readonly List<TerrainStamp> s_clipboard = new List<TerrainStamp>();

        /// <summary>Current clipboard index (0 = latest). -1 = none.</summary>
        private static int s_clipboardIndex = -1;

        /// <summary>
        /// Gets the current clipboard stamp (latest), or null if empty.
        /// </summary>
        public static TerrainStamp GetStamp() => (s_clipboardIndex >= 0 && s_clipboardIndex < s_clipboard.Count)
            ? s_clipboard[s_clipboardIndex] : null;

        /// <summary>
        /// Gets all clipboard stamps (newest first).
        /// </summary>
        public static IReadOnlyList<TerrainStamp> GetClipboard() => s_clipboard.AsReadOnly();

        /// <summary>
        /// Sets the active clipboard index (for cycling through history).
        /// </summary>
        public static bool SetClipboardIndex(int index)
        {
            if (index >= 0 && index < s_clipboard.Count)
            {
                s_clipboardIndex = index;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cycles to the next stamp in clipboard history.
        /// </summary>
        public static TerrainStamp CycleClipboard(int delta)
        {
            if (s_clipboard.Count == 0) return null;
            s_clipboardIndex = (s_clipboardIndex + delta) % s_clipboard.Count;
            if (s_clipboardIndex < 0) s_clipboardIndex += s_clipboard.Count;
            return s_clipboard[s_clipboardIndex];
        }

        /// <summary>
        /// Adds a stamp to the clipboard (newest at index 0).
        /// </summary>
        private static void AddToClipboard(TerrainStamp stamp)
        {
            s_clipboard.Insert(0, stamp);
            s_clipboardIndex = 0;
            while (s_clipboard.Count > MaxClipboardStamps)
                s_clipboard.RemoveAt(s_clipboard.Count - 1);
        }

        /// <summary>
        /// Copies the currently marked area (or the brush footprint at aim)
        /// into the RAM stamp buffer. Returns the stamp or null on failure.
        /// </summary>
        public static TerrainStamp CopyStamp(Vector3 center, float radius, TerrainShape shape,
                                             Vector3? rectA, Vector3? rectB,
                                             Vector3[] polyPoints, bool hasLine, Vector3? lineA, Vector3? lineB)
        {
            Cache();
            var stamp = new TerrainStamp { Center = center, Radius = radius, Time = DateTime.Now };

            // Determine search area based on shape
            Vector3 searchCenter = center;
            float searchRadius = radius + 4f;
            var hmaps = new List<Heightmap>();

            if (hasLine && lineA.HasValue && lineB.HasValue)
            {
                searchCenter = (lineA.Value + lineB.Value) * 0.5f;
                searchRadius = Vector3.Distance(lineA.Value, lineB.Value) * 0.5f + radius + 4f;
            }
            else if (rectA.HasValue && rectB.HasValue)
            {
                searchCenter = (rectA.Value + rectB.Value) * 0.5f;
                float dx = Mathf.Abs(rectB.Value.x - rectA.Value.x);
                float dz = Mathf.Abs(rectB.Value.z - rectA.Value.z);
                searchRadius = Mathf.Sqrt(dx * dx + dz * dz) * 0.5f + radius + 4f;
            }
            else if (polyPoints != null && polyPoints.Length >= 3)
            {
                Vector3 c = Vector3.zero;
                float maxD = 0f;
                for (int k = 0; k < polyPoints.Length; k++)
                    c += polyPoints[k];
                c /= polyPoints.Length;
                for (int k = 0; k < polyPoints.Length; k++)
                    maxD = Mathf.Max(maxD, Vector3.Distance(c, polyPoints[k]));
                searchCenter = c;
                searchRadius = maxD + radius + 4f;
            }

            // Rectangle selection: clip every tile to the A->B box (per-vertex mask).
            bool rectMode = shape == TerrainShape.Rect && rectA.HasValue && rectB.HasValue;
            float minX = 0f, maxX = 0f, minZ = 0f, maxZ = 0f;
            if (rectMode)
            {
                minX = Mathf.Min(rectA.Value.x, rectB.Value.x);
                maxX = Mathf.Max(rectA.Value.x, rectB.Value.x);
                minZ = Mathf.Min(rectA.Value.z, rectB.Value.z);
                maxZ = Mathf.Max(rectA.Value.z, rectB.Value.z);
            }

            Heightmap.FindHeightmap(searchCenter, searchRadius, hmaps);
            if (hmaps.Count == 0)
                return null;

            int copied = 0;
            foreach (Heightmap hm in hmaps)
            {
                if (hm == null || hm.m_scale <= 0f)
                    continue;
                TerrainComp comp = hm.GetAndCreateTerrainCompiler();
                if (comp == null)
                    continue;

                float[] level = LevelDelta(comp);
                float[] smooth = SmoothDelta(comp);
                bool[] modH = null;
                Color[] paint = PaintMask(comp);
                bool[] modP = ModifiedPaint(comp);
                try { modH = (bool[])f_modifiedHeight.GetValue(comp); }
                catch { modH = null; }
                if (level == null || smooth == null || modH == null || paint == null || modP == null)
                    continue;

                int w = Width(comp);
                int pitch = w + 1;
                if (level.Length != pitch * pitch)
                    continue;

                int modHCount = 0;
                float maxAbs = 0f;
                for (int q = 0; q < level.Length; q++)
                {
                    if (modH[q])
                        modHCount++;
                    float aa = Mathf.Abs(level[q]);
                    if (aa > maxAbs)
                        maxAbs = aa;
                }
                TerrainForgePlugin.Log?.LogInfo("[TF] CopyStamp tile w=" + w +
                    " modH=" + modHCount + " maxAbsLevel=" + maxAbs.ToString("0.00") +
                    " size=" + (w * hm.m_scale).ToString("0") + " pos=" + hm.transform.position);

                if (rectMode)
                {
                    int len = pitch * pitch;
                    var lv = new float[len];
                    var sm = new float[len];
                    var mh = new bool[len];
                    var pt = new Color[len];
                    var mp = new bool[len];
                    Vector3 o = hm.transform.position;
                    float scale = hm.m_scale;
                    float half = w * 0.5f;
                    int inside = 0;
                    for (int ii = 0; ii <= w; ii++)
                    {
                        for (int jj = 0; jj <= w; jj++)
                        {
                            int idx = ii * pitch + jj;
                            float wx = o.x + (jj - half) * scale;
                            float wz = o.z + (ii - half) * scale;
                            if (wx >= minX && wx <= maxX && wz >= minZ && wz <= maxZ)
                            {
                                lv[idx] = level[idx];
                                sm[idx] = smooth[idx];
                                mh[idx] = true;
                                pt[idx] = paint[idx];
                                mp[idx] = modP[idx];
                                inside++;
                            }
                        }
                    }
                    if (inside == 0)
                        continue;
                    stamp.Tiles.Add(new StampTile
                    {
                        TilePos = o,
                        Width = w,
                        Size = w * scale,
                        Level = lv,
                        Smooth = sm,
                        ModH = mh,
                        Paint = pt,
                        ModP = mp
                    });
                    copied++;
                    continue;
                }

                // Clone the whole tile (simple, fast enough for copy)
                var tile = new StampTile
                {
                    TilePos = hm.transform.position,
                    Width = w,
                    Size = w * hm.m_scale,
                    Level = (float[])level.Clone(),
                    Smooth = (float[])smooth.Clone(),
                    ModH = (bool[])modH.Clone(),
                    Paint = (Color[])paint.Clone(),
                    ModP = (bool[])modP.Clone()
                };
                stamp.Tiles.Add(tile);
                copied++;
            }

            if (copied == 0)
                return null;

            stamp.RegionMask = rectMode;
            stamp.Label = (rectMode ? "Rect: " : "Copy: ") + copied + " tile" + (copied > 1 ? "s" : "");
            AddToClipboard(stamp);
            return stamp;
        }

        /// <summary>
        /// Clears the clipboard.
        /// </summary>
        public static void ClearClipboard()
        {
            s_clipboard.Clear();
            s_clipboardIndex = -1;
        }

        /// <summary>
        /// Clears the current clipboard stamp (keeps history).
        /// </summary>
        public static void ClearCurrentStamp()
        {
            if (s_clipboardIndex >= 0 && s_clipboardIndex < s_clipboard.Count)
            {
                s_clipboard.RemoveAt(s_clipboardIndex);
                if (s_clipboardIndex >= s_clipboard.Count)
                    s_clipboardIndex = s_clipboard.Count - 1;
            }
        }

        /// <summary>
        /// Pastes the stamp at the given world position with optional rotation (degrees)
        /// and scale. Returns true if any tile was written.
        /// </summary>
        public static bool PasteStamp(Vector3 targetCenter, out string message,
                                      float rotationDeg = 0f, float scale = 1f)
        {
            message = null;
            TerrainStamp stamp = GetStamp();
            if (stamp == null || stamp.Tiles.Count == 0)
            {
                message = "Stamp vacío: copiá algo primero (Ctrl+Shift+C).";
                return false;
            }

            Cache();

            // We'll write each source tile to a target tile computed by transforming
            // the source tile position relative to the stamp center.
            float cosR = Mathf.Cos(rotationDeg * Mathf.Deg2Rad);
            float sinR = Mathf.Sin(rotationDeg * Mathf.Deg2Rad);
            Vector3 stampCenter = stamp.Center;

            var frame = new List<UndoEntry>();
            int written = 0;
            float minx = float.MaxValue, maxx = float.MinValue;
            float minz = float.MaxValue, maxz = float.MinValue;

            foreach (StampTile src in stamp.Tiles)
            {
                // Transform source tile position to target position
                Vector3 rel = src.TilePos - stampCenter;
                rel.x *= scale;
                rel.z *= scale;
                // Rotate around Y
                float rx = rel.x * cosR - rel.z * sinR;
                float rz = rel.x * sinR + rel.z * cosR;
                Vector3 targetTilePos = targetCenter + new Vector3(rx, 0f, rz);

                // Find or create the target heightmap tile
                var found = new List<Heightmap>();
                Heightmap.FindHeightmap(targetTilePos, 2f, found);
                Heightmap hm = null;
                TerrainComp comp = null;
                foreach (Heightmap h in found)
                {
                    if (h == null || h.m_scale <= 0f)
                        continue;
                    TerrainComp c = h.GetAndCreateTerrainCompiler();
                    if (c != null)
                    {
                        hm = h;
                        comp = c;
                        break;
                    }
                }
                if (comp == null)
                {
                    // Try to get the heightmap at that position and create compiler
                    foreach (Heightmap h in found)
                    {
                        if (h == null || h.m_scale <= 0f)
                            continue;
                        TerrainComp c = h.GetAndCreateTerrainCompiler();
                        if (c != null)
                        {
                            hm = h;
                            comp = c;
                            break;
                        }
                    }
                }
                if (comp == null)
                    continue;

                if (!EnsureOwnership(comp))
                    continue;

                float[] level = LevelDelta(comp);
                float[] smooth = SmoothDelta(comp);
                bool[] modH = null;
                Color[] paint = PaintMask(comp);
                bool[] modP = ModifiedPaint(comp);
                try { modH = (bool[])f_modifiedHeight.GetValue(comp); }
                catch { modH = null; }
                if (level == null || smooth == null || modH == null || paint == null || modP == null)
                    continue;

                int w = Width(comp);
                int pitch = w + 1;
                if (level.Length != pitch * pitch)
                    continue;

                // Source tile may have different resolution; we only paste if same size.
                // (In practice all tiles in a world share the same resolution.)
                if (src.Width != w)
                    continue;

                var rec = new Recorder(level, smooth, modH, paint, modP);
                bool any = false;

                if (stamp.RegionMask)
                {
                    // Selection stamp: only vertices inside the A->B rectangle are written.
                    for (int idx = 0; idx < src.Level.Length; idx++)
                    {
                        if (!src.ModH[idx])
                            continue;
                        bool h = level[idx] != src.Level[idx] || smooth[idx] != src.Smooth[idx] || !modH[idx];
                        bool pc = src.ModP[idx] && (paint[idx] != src.Paint[idx] || !modP[idx]);
                        if (!h && !pc)
                            continue;
                        rec.Add(idx);
                        if (h)
                        {
                            level[idx] = src.Level[idx];
                            smooth[idx] = src.Smooth[idx];
                            modH[idx] = true;
                        }
                        if (pc)
                        {
                            paint[idx] = src.Paint[idx];
                            modP[idx] = true;
                        }
                        any = true;
                    }
                }
                else
                {
                    for (int idx = 0; idx < src.Level.Length; idx++)
                    {
                        if (level[idx] != src.Level[idx] || smooth[idx] != src.Smooth[idx] ||
                            modH[idx] != src.ModH[idx] || paint[idx] != src.Paint[idx] || modP[idx] != src.ModP[idx])
                        {
                            rec.Add(idx);
                            level[idx] = src.Level[idx];
                            smooth[idx] = src.Smooth[idx];
                            modH[idx] = src.ModH[idx];
                            paint[idx] = src.Paint[idx];
                            modP[idx] = src.ModP[idx];
                            any = true;
                        }
                    }
                }

                if (any && rec.Cells.Count > 0)
                {
                    frame.Add(new UndoEntry
                    {
                        Comp = comp,
                        Hm = hm,
                        WithSmooth = true,
                        WithPaint = true,
                        Before = rec.Cells
                    });
                    Save(comp);
                    if (hm != null)
                    {
                        try { hm.Poke(0); }
                        catch { }
                    }
                    written++;
                    if (targetTilePos.x < minx) minx = targetTilePos.x;
                    if (targetTilePos.x > maxx) maxx = targetTilePos.x;
                    if (targetTilePos.z < minz) minz = targetTilePos.z;
                    if (targetTilePos.z > maxz) maxz = targetTilePos.z;
                }
            }

            if (written == 0)
            {
                message = "No se pudo pegar: tiles de destino no coinciden.";
                return false;
            }

            Vector3 center = new Vector3((minx + maxx) * 0.5f, 0f, (minz + maxz) * 0.5f);
            float radius = Mathf.Max(maxx - minx, maxz - minz) * 0.5f + 4f;
            if (frame.Count > 0)
            {
                PushUndo(new UndoFrame
                {
                    Label = "Paste: " + stamp.Label,
                    Center = center,
                    Radius = radius,
                    Entries = frame
                });
                if (ClutterSystem.instance != null)
                    ClutterSystem.instance.ResetGrass(center, radius + 1f);
            }

            message = "Pegado: " + written + " tile" + (written > 1 ? "s" : "") + ".";
            return true;
        }

        // ---- terrain stamps (disk persistence) -----------------------------------

        private const string StampMagic = "TFST";
        private const int StampVersion = 2;
        private const int MaxSavedStamps = 12;

        private static string StampDir()
        {
            string dir = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, "TerrainForge", "stamps");
            System.IO.Directory.CreateDirectory(dir);
            return dir;
        }

        private static string SanitizeStampName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new System.Text.StringBuilder(name.Trim());
            for (int i = 0; i < sb.Length; i++)
            {
                for (int k = 0; k < invalid.Length; k++)
                {
                    if (sb[i] == invalid[k])
                    {
                        sb[i] = '_';
                        break;
                    }
                }
            }
            string clean = sb.ToString().Trim();
            if (clean.Length == 0)
                return null;
            if (clean.Length > 60)
                clean = clean.Substring(0, 60);
            return clean;
        }

        public static List<string> StampNames()
        {
            var list = new List<string>();
            try
            {
                string[] files = System.IO.Directory.GetFiles(StampDir(), "*.stamp");
                System.Array.Sort(files);
                for (int i = 0; i < files.Length; i++)
                    list.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));
            }
            catch { }
            return list;
        }

        public static bool DeleteStamp(string name)
        {
            try
            {
                name = SanitizeStampName(name);
                if (string.IsNullOrEmpty(name))
                    return false;
                string path = System.IO.Path.Combine(StampDir(), name + ".stamp");
                if (!System.IO.File.Exists(path))
                    return false;
                System.IO.File.Delete(path);
                return true;
            }
            catch { return false; }
        }

        public static bool SaveStamp(string name, TerrainStamp stamp, out string message)
        {
            message = null;
            name = SanitizeStampName(name);
            if (string.IsNullOrEmpty(name) || stamp == null || stamp.Tiles.Count == 0)
            {
                message = "Nombre o stamp inválido.";
                return false;
            }

            try
            {
                string dir = StampDir();
                string path = System.IO.Path.Combine(dir, name + ".stamp");
                if (!System.IO.File.Exists(path) &&
                    System.IO.Directory.GetFiles(dir, "*.stamp").Length >= MaxSavedStamps)
                {
                    message = "Máximo de stamps guardados alcanzado, borra alguno.";
                    return false;
                }
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create,
                    System.IO.FileAccess.Write, System.IO.FileShare.None))
                using (var w = new System.IO.BinaryWriter(fs))
                {
                    w.Write(StampMagic);
                    w.Write(StampVersion);
                    w.Write(stamp.Name ?? "");
                    w.Write(stamp.Center.x);
                    w.Write(stamp.Center.y);
                    w.Write(stamp.Center.z);
                    w.Write(stamp.Radius);
                    w.Write(stamp.Time.ToBinary());
                    w.Write(stamp.RegionMask);
                    w.Write(stamp.Tiles.Count);
                    foreach (var tile in stamp.Tiles)
                    {
                        w.Write(tile.TilePos.x);
                        w.Write(tile.TilePos.y);
                        w.Write(tile.TilePos.z);
                        w.Write(tile.Width);
                        WriteFloats(w, tile.Level);
                        WriteFloats(w, tile.Smooth);
                        WriteBools(w, tile.ModH);
                        WriteColors(w, tile.Paint);
                        WriteBools(w, tile.ModP);
                    }
                }
                stamp.Name = name;
                message = "Stamp guardado: " + name + " (" + stamp.Tiles.Count + " tiles).";
                return true;
            }
            catch (System.Exception e)
            {
                message = "Error guardando stamp: " + e.Message;
                return false;
            }
        }

        public static bool LoadStamp(string name, out TerrainStamp stamp, out string message)
        {
            stamp = null;
            message = null;
            name = SanitizeStampName(name);
            if (string.IsNullOrEmpty(name))
            {
                message = "Nombre inválido.";
                return false;
            }

            try
            {
                string path = System.IO.Path.Combine(StampDir(), name + ".stamp");
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open,
                    System.IO.FileAccess.Read, System.IO.FileShare.Read))
                using (var r = new System.IO.BinaryReader(fs))
                {
                    if (r.ReadString() != StampMagic)
                    {
                        message = "Archivo de stamp inválido (magic).";
                        return false;
                    }
                    int version = r.ReadInt32();
                    if (version < 1 || version > StampVersion)
                    {
                        message = "Versión de stamp incompatible.";
                        return false;
                    }
                    string stampName = r.ReadString();
                    Vector3 center = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                    float radius = r.ReadSingle();
                    DateTime time = DateTime.FromBinary(r.ReadInt64());
                    bool regionMask = version >= 2 && r.ReadBoolean();
                    int tileCount = r.ReadInt32();
                    if (tileCount <= 0 || tileCount > 512)
                    {
                        message = "Tile count inválido.";
                        return false;
                    }

                    var tiles = new List<StampTile>();
                    for (int i = 0; i < tileCount; i++)
                    {
                        var tile = new StampTile
                        {
                            TilePos = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()),
                            Width = r.ReadInt32()
                        };
                        int len = (tile.Width + 1) * (tile.Width + 1);
                        tile.Level = ReadFloats(r, len);
                        tile.Smooth = ReadFloats(r, len);
                        tile.ModH = ReadBools(r, len);
                        tile.Paint = ReadColors(r, len);
                        tile.ModP = ReadBools(r, len);
                        if (tile.Level == null || tile.Smooth == null || tile.ModH == null ||
                            tile.Paint == null || tile.ModP == null)
                        {
                            message = "Datos de tile corruptos.";
                            return false;
                        }
                        tiles.Add(tile);
                    }

                    stamp = new TerrainStamp
                    {
                        Name = stampName,
                        Center = center,
                        Radius = radius,
                        Time = time,
                        RegionMask = regionMask,
                        Tiles = tiles
                    };
                    message = "Stamp cargado: " + name + " (" + tiles.Count + " tiles).";
                    return true;
                }
            }
            catch (System.Exception e)
            {
                message = "Error cargando stamp: " + e.Message;
                return false;
            }
        }

        public static bool LoadStampToClipboard(string name, out string message)
        {
            message = null;
            if (LoadStamp(name, out TerrainStamp stamp, out message))
            {
                AddToClipboard(stamp);
                message = "Stamp cargado al portapapeles: " + name;
                return true;
            }
            return false;
        }

        // ---- terrain snapshots -------------------------------------------------

        private const string SnapMagic = "TFSS";
        private const int SnapVersion = 1;
        private const int MaxSnapshots = 12;

        private sealed class SnapComp
        {
            public float TileX, TileZ;
            public int Width;
            public float[] Level, Smooth;
            public bool[] ModH, ModP;
            public Color[] Paint;
        }

        private sealed class ReadySnap
        {
            public TerrainComp Comp;
            public Heightmap Hm;
            public SnapComp Rec;
        }

        private static string SnapDir()
        {
            string dir = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, "TerrainForge", "snapshots");
            System.IO.Directory.CreateDirectory(dir);
            return dir;
        }

        private static string SanitizeSnapshotName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new System.Text.StringBuilder(name.Trim());
            for (int i = 0; i < sb.Length; i++)
            {
                for (int k = 0; k < invalid.Length; k++)
                {
                    if (sb[i] == invalid[k])
                    {
                        sb[i] = '_';
                        break;
                    }
                }
            }
            string clean = sb.ToString().Trim();
            if (clean.Length == 0)
                return null;
            if (clean.Length > 60)
                clean = clean.Substring(0, 60);
            return clean;
        }

        public static List<string> SnapshotNames()
        {
            var list = new List<string>();
            try
            {
                string[] files = System.IO.Directory.GetFiles(SnapDir(), "*.dat");
                System.Array.Sort(files);
                for (int i = 0; i < files.Length; i++)
                    list.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));
            }
            catch { }
            return list;
        }

        public static bool DeleteSnapshot(string name)
        {
            try
            {
                name = SanitizeSnapshotName(name);
                if (string.IsNullOrEmpty(name))
                    return false;
                string path = System.IO.Path.Combine(SnapDir(), name + ".dat");
                if (!System.IO.File.Exists(path))
                    return false;
                System.IO.File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ExportSnapshot(string name, out string message)
        {
            message = null;
            Cache();
            name = SanitizeSnapshotName(name);
            if (string.IsNullOrEmpty(name))
            {
                message = Loc.T("snap.badname");
                return false;
            }

            var comps = new List<TerrainComp>();
            var tiles = new List<Vector3>();
            var widths = new List<int>();
            CollectHistoryComps(s_undo, comps, tiles, widths);
            CollectHistoryComps(s_redo, comps, tiles, widths);

            var records = new List<SnapComp>();
            for (int i = 0; i < comps.Count; i++)
            {
                TerrainComp comp = comps[i];
                float[] level = LevelDelta(comp);
                float[] smooth = SmoothDelta(comp);
                bool[] modH = null;
                Color[] paint = PaintMask(comp);
                bool[] modP = ModifiedPaint(comp);
                try { modH = (bool[])f_modifiedHeight.GetValue(comp); }
                catch { modH = null; }
                if (level == null || smooth == null || modH == null || paint == null || modP == null)
                    continue;
                int pitch = widths[i] + 1;
                if (level.Length != pitch * pitch)
                    continue;
                records.Add(new SnapComp
                {
                    TileX = tiles[i].x,
                    TileZ = tiles[i].z,
                    Width = widths[i],
                    Level = (float[])level.Clone(),
                    Smooth = (float[])smooth.Clone(),
                    ModH = (bool[])modH.Clone(),
                    Paint = (Color[])paint.Clone(),
                    ModP = (bool[])modP.Clone()
                });
            }

            if (records.Count == 0)
            {
                message = Loc.T("snap.empty");
                return false;
            }

            try
            {
                string dir = SnapDir();
                string path = System.IO.Path.Combine(dir, name + ".dat");
                if (!System.IO.File.Exists(path) &&
                    System.IO.Directory.GetFiles(dir, "*.dat").Length >= MaxSnapshots)
                {
                    message = Loc.T("snap.max");
                    return false;
                }
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create,
                    System.IO.FileAccess.Write, System.IO.FileShare.None))
                using (var w = new System.IO.BinaryWriter(fs))
                {
                    w.Write(SnapMagic);
                    w.Write(SnapVersion);
                    w.Write(records.Count);
                    for (int i = 0; i < records.Count; i++)
                    {
                        SnapComp r = records[i];
                        w.Write(r.TileX);
                        w.Write(r.TileZ);
                        w.Write(r.Width);
                        WriteFloats(w, r.Level);
                        WriteFloats(w, r.Smooth);
                        WriteBools(w, r.ModH);
                        WriteColors(w, r.Paint);
                        WriteBools(w, r.ModP);
                    }
                }
                message = Loc.F("snap.saved", name, records.Count);
                return true;
            }
            catch (System.Exception e)
            {
                message = Loc.F("snap.ioerror", e.Message);
                return false;
            }
        }

        public static bool ImportSnapshot(string name, out string message)
        {
            message = null;
            Cache();
            name = SanitizeSnapshotName(name);
            if (string.IsNullOrEmpty(name))
            {
                message = Loc.T("snap.badname");
                return false;
            }

            List<SnapComp> records;
            try
            {
                records = ReadSnapshot(name);
            }
            catch (System.Exception e)
            {
                message = Loc.F("snap.ioerror", e.Message);
                return false;
            }
            if (records == null || records.Count == 0)
            {
                message = Loc.T("snap.badfile");
                return false;
            }

            // Pass 1: resolve + own everything first so a failure never half-applies.
            var ready = new List<ReadySnap>();
            int missing = 0;
            for (int i = 0; i < records.Count; i++)
            {
                SnapComp r = records[i];
                var found = new List<Heightmap>();
                Heightmap.FindHeightmap(new Vector3(r.TileX, 0f, r.TileZ), 2f, found);
                TerrainComp comp = null;
                Heightmap hm = null;
                for (int k = 0; k < found.Count; k++)
                {
                    TerrainComp c = found[k] != null ? found[k].GetAndCreateTerrainCompiler() : null;
                    if (c != null)
                    {
                        comp = c;
                        hm = found[k];
                        break;
                    }
                }
                if (comp == null)
                {
                    missing++;
                    continue;
                }
                int w = 0;
                try { w = Width(comp); }
                catch { w = 0; }
                if (w != r.Width || (w + 1) * (w + 1) != r.Level.Length || !EnsureOwnership(comp))
                {
                    missing++;
                    continue;
                }
                ready.Add(new ReadySnap { Comp = comp, Hm = hm, Rec = r });
            }

            if (ready.Count == 0)
            {
                message = missing > 0 ? Loc.F("snap.missing", missing) : Loc.T("snap.badfile");
                return false;
            }

            // Pass 2: diff-write through the recorder, so the import is undoable.
            var frame = new List<UndoEntry>();
            float minx = float.MaxValue, maxx = float.MinValue;
            float minz = float.MaxValue, maxz = float.MinValue;
            for (int i = 0; i < ready.Count; i++)
            {
                TerrainComp comp = ready[i].Comp;
                Heightmap hm = ready[i].Hm;
                SnapComp r = ready[i].Rec;
                float[] level = LevelDelta(comp);
                float[] smooth = SmoothDelta(comp);
                bool[] modH = null;
                Color[] paint = PaintMask(comp);
                bool[] modP = ModifiedPaint(comp);
                try { modH = (bool[])f_modifiedHeight.GetValue(comp); }
                catch { modH = null; }
                if (level == null || smooth == null || modH == null || paint == null || modP == null)
                    continue;
                if (level.Length != r.Level.Length)
                    continue;

                var rec = new Recorder(level, smooth, modH, paint, modP);
                bool any = false;
                for (int idx = 0; idx < r.Level.Length; idx++)
                {
                    if (level[idx] != r.Level[idx] || smooth[idx] != r.Smooth[idx] ||
                        modH[idx] != r.ModH[idx] || paint[idx] != r.Paint[idx] || modP[idx] != r.ModP[idx])
                    {
                        rec.Add(idx);
                        level[idx] = r.Level[idx];
                        smooth[idx] = r.Smooth[idx];
                        modH[idx] = r.ModH[idx];
                        paint[idx] = r.Paint[idx];
                        modP[idx] = r.ModP[idx];
                        any = true;
                    }
                }
                if (any && rec.Cells.Count > 0)
                {
                    frame.Add(new UndoEntry
                    {
                        Comp = comp,
                        Hm = hm,
                        WithSmooth = true,
                        WithPaint = true,
                        Before = rec.Cells
                    });
                    Save(comp);
                    if (hm != null)
                    {
                        try { hm.Poke(0); }
                        catch { }
                    }
                    if (r.TileX < minx) minx = r.TileX;
                    if (r.TileX > maxx) maxx = r.TileX;
                    if (r.TileZ < minz) minz = r.TileZ;
                    if (r.TileZ > maxz) maxz = r.TileZ;
                }
            }

            Vector3 center = new Vector3((minx + maxx) * 0.5f, 0f, (minz + maxz) * 0.5f);
            float radius = Mathf.Max(maxx - minx, maxz - minz) * 0.5f + 4f;
            if (frame.Count > 0)
            {
                PushUndo(new UndoFrame
                {
                    Label = "Snapshot:" + name,
                    Center = center,
                    Radius = radius,
                    Entries = frame
                });
                if (ClutterSystem.instance != null)
                    ClutterSystem.instance.ResetGrass(center, radius + 1f);
            }

            message = Loc.F("snap.loaded", name) +
                (missing > 0 ? " " + Loc.F("snap.missing", missing) : "");
            return true;
        }

        private static void CollectHistoryComps(List<UndoFrame> stack, List<TerrainComp> comps,
            List<Vector3> tiles, List<int> widths)
        {
            for (int f = 0; f < stack.Count; f++)
            {
                List<UndoEntry> entries = stack[f].Entries;
                if (entries == null)
                    continue;
                for (int i = 0; i < entries.Count; i++)
                {
                    UndoEntry e = entries[i];
                    if (e == null || e.Comp == null || comps.Contains(e.Comp))
                        continue;
                    Vector3 tp = e.Hm != null ? e.Hm.transform.position : e.Comp.transform.position;
                    int w = 0;
                    try { w = Width(e.Comp); }
                    catch { w = 0; }
                    if (w <= 0)
                        continue;
                    comps.Add(e.Comp);
                    tiles.Add(tp);
                    widths.Add(w);
                }
            }
        }

        private static List<SnapComp> ReadSnapshot(string name)
        {
            string path = System.IO.Path.Combine(SnapDir(), name + ".dat");
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open,
                System.IO.FileAccess.Read, System.IO.FileShare.Read))
            using (var r = new System.IO.BinaryReader(fs))
            {
                if (r.ReadString() != SnapMagic)
                    return null;
                if (r.ReadInt32() != SnapVersion)
                    return null;
                int count = r.ReadInt32();
                if (count < 0 || count > 512)
                    return null;
                var list = new List<SnapComp>();
                for (int i = 0; i < count; i++)
                {
                    var rec = new SnapComp
                    {
                        TileX = r.ReadSingle(),
                        TileZ = r.ReadSingle(),
                        Width = r.ReadInt32()
                    };
                    if (rec.Width <= 0 || rec.Width > 512)
                        return null;
                    int len = (rec.Width + 1) * (rec.Width + 1);
                    rec.Level = ReadFloats(r, len);
                    rec.Smooth = ReadFloats(r, len);
                    rec.ModH = ReadBools(r, len);
                    rec.Paint = ReadColors(r, len);
                    rec.ModP = ReadBools(r, len);
                    if (rec.Level == null || rec.Smooth == null || rec.ModH == null ||
                        rec.Paint == null || rec.ModP == null)
                        return null;
                    list.Add(rec);
                }
                return list;
            }
        }

        private static void WriteFloats(System.IO.BinaryWriter w, float[] a)
        {
            for (int i = 0; i < a.Length; i++)
                w.Write(a[i]);
        }

        private static float[] ReadFloats(System.IO.BinaryReader r, int len)
        {
            var a = new float[len];
            for (int i = 0; i < len; i++)
                a[i] = r.ReadSingle();
            return a;
        }

        private static void WriteBools(System.IO.BinaryWriter w, bool[] a)
        {
            for (int i = 0; i < a.Length; i++)
                w.Write(a[i] ? (byte)1 : (byte)0);
        }

        private static bool[] ReadBools(System.IO.BinaryReader r, int len)
        {
            var a = new bool[len];
            for (int i = 0; i < len; i++)
                a[i] = r.ReadByte() != 0;
            return a;
        }

        private static void WriteColors(System.IO.BinaryWriter w, Color[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                w.Write(a[i].r);
                w.Write(a[i].g);
                w.Write(a[i].b);
                w.Write(a[i].a);
            }
        }

        private static Color[] ReadColors(System.IO.BinaryReader r, int len)
        {
            var a = new Color[len];
            for (int i = 0; i < len; i++)
                a[i] = new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            return a;
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
