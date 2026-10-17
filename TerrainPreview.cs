using System.Collections.Generic;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// State handed to <see cref="TerrainPreview.Update(in PreviewState)"/> each frame.
    /// </summary>
    internal struct PreviewState
    {
        public Vector3 Center;      // brush centre (aim point)
        public Vector3 PlayerPos;   // player feet, for the aim-reach ring
        public float Radius;        // footprint radius (where terrain is generated)
        public float FullRadius;    // full-strength zone radius
        public float ReachRadius;   // maximum brush radius (RadiusMax)
        public float AimReach;      // maximum aim distance (MaxAimDistance)
        public TerrainShape Shape;
        public Color Color;
        public bool IsRamp;
        public bool HasRampA;
        public Vector3 RampA;
        public bool SuppressCircles;
        public bool HasRect;
        public Vector3 RectA;
        public Vector3 RectB;
        public Vector3[] PolyPreview;
        public float SlopeDeg;
        public bool GuidesOn;
        public float LevelY; // NaN = hide the target-level marker
        public bool Blocked; // server/ward deny: draw guides greyed out
        public float WaterLevel; // for underwater preview: draw at water surface
        public bool ShowStamp;           // stamp paste preview active
        public TerrainEngine.TerrainStamp Stamp; // stamp to visualise (paste target or copy source)
        public Vector3 StampTarget;      // aim point where the stamp will land
        public float StampRotation;      // rotation in degrees
        public float StampScale;         // scale factor
        public float CopyHighlightStart; // Time.time of the last copy (0 = none)
        public bool ShowSlopeHeatmap;    // show slope heatmap overlay
        public bool ShowHeightHeatmap;   // show height heatmap overlay
    }

    /// <summary>
    /// Draws the live brush footprint plus auxiliary "reach" guides:
    ///  - solid ring: the footprint edge (where terrain is actually generated),
    ///  - faint ring: the full-strength zone,
    ///  - fainter ring: the maximum brush radius,
    ///  - faint ground ring: the maximum aim distance around the player,
    ///  - for ramps: the real A-B corridor edges and the A-&gt;cursor guide.
    /// All lines share the operation colour; guides are thinner and more faint.
    /// </summary>
    internal sealed class TerrainPreview
    {
        private const int Segments = 64;

        private GameObject _root;
        private Material _lineMat;

        private LineRenderer _edge;      // footprint edge (solid)
        private LineRenderer _full;      // full-strength zone
        private LineRenderer _reach;     // max brush radius
        private LineRenderer _aim;       // max aim distance (world, around player)
        private LineRenderer _rampLine;  // A -> cursor centre line (world)
        private LineRenderer _rampL;     // ramp corridor left edge (world)
        private LineRenderer _rampR;     // ramp corridor right edge (world)
        private LineRenderer _rampFill;  // ramp corridor filled polygon (world)
        private LineRenderer _rampMarkA; // height marker at A
        private LineRenderer _rampMarkB; // height marker at B
        private LineRenderer _rect;      // rectangle footprint outline (world)
        private LineRenderer _poly;      // polygon footprint outline (world)
        private LineRenderer _edgeShadow; // dark outline backing (contrast)
        private LineRenderer _rectShadow; // dark outline backing (contrast)
        private LineRenderer _polyShadow; // dark outline backing (contrast)
        private LineRenderer _level;     // target-level marker ring (world)
        private LineRenderer _slopeHeatmap;  // slope heatmap overlay (world)
        private LineRenderer _heightHeatmap; // height heatmap overlay (world)

        private GameObject _patchGo;     // translucent 3D surface patch
        private MeshFilter _patchMf;
        private MeshRenderer _patchMr;
        private Mesh _patchMesh;
        private Material _patchMat;
        private TerrainShape _patchShape = (TerrainShape)(-1);
        private float _patchRadius = -1f;
        private float _patchNext;

        private GameObject _stampGo;     // translucent footprint for stamps
        private MeshFilter _stampMf;
        private MeshRenderer _stampMr;
        private Mesh _stampMesh;
        private Material _stampMat;
        private float _stampNext;

        private Vector3[] _unitRing;

        private TerrainShape _builtShape = (TerrainShape)(-1);
        private float _builtEdge = -1f;
        private float _builtFull = -1f;
        private float _builtReach = -1f;

        private bool _disabled;

        public void Ensure()
        {
            if (_disabled || _root != null)
                return;

            Shader shader = FindShader("Sprites/Default", "Unlit/Color", "Hidden/Internal-Colored");
            if (shader == null)
            {
                _disabled = true;
                TerrainForgePlugin.Log?.LogWarning("TerrainPreview: no usable shader found, preview disabled.");
                return;
            }

            _root = new GameObject("TerrainForgePreview") { hideFlags = HideFlags.HideAndDontSave };
            _lineMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _lineMat.color = Color.white;
            TerrainForgePlugin.Log?.LogInfo("TerrainPreview: initialised with shader '" + shader.name + "'.");

            _edge = MakeLine("edge", 2, false);
            _full = MakeLine("full", 2, false);
            _reach = MakeLine("reach", 2, false);
            _aim = MakeLine("aim", 2, true);
            _rampLine = MakeLine("ramp", 0, true);
            _rampL = MakeLine("rampL", 0, true);
            _rampR = MakeLine("rampR", 0, true);
            _rampFill = MakeLine("rampFill", 0, true);
            _rampFill.loop = true;
            _rampFill.numCornerVertices = 2;
            _rampMarkA = MakeLine("rampMarkA", 0, true);
            _rampMarkA.loop = true;
            _rampMarkB = MakeLine("rampMarkB", 0, true);
            _rampMarkB.loop = true;
            _rect = MakeLine("rect", 0, true);
            _poly = MakeLine("poly", 0, true);

            // Dark, slightly wider backing lines drawn behind the main outlines so
            // they stay legible over bright or busy terrain (snow, water, foliage).
            _edgeShadow = MakeLine("edgeShadow", 2, false);
            _edgeShadow.sortingOrder = -1;
            _rectShadow = MakeLine("rectShadow", 0, true);
            _rectShadow.sortingOrder = -1;
            _polyShadow = MakeLine("polyShadow", 0, true);
            _polyShadow.sortingOrder = -1;

            // Translucent surface patch: samples the ground under the brush and
            // draws a soft cap so you can read the affected 3D volume.
            _patchMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _patchMat.color = new Color(1f, 1f, 1f, 0.22f);
            _patchGo = new GameObject("patch") { hideFlags = HideFlags.HideAndDontSave };
            _patchMf = _patchGo.AddComponent<MeshFilter>();
            _patchMr = _patchGo.AddComponent<MeshRenderer>();
            _patchMr.sharedMaterial = _patchMat;
            _patchMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _patchMr.receiveShadows = false;
            _patchMesh = new Mesh { hideFlags = HideFlags.HideAndDontSave };
            _patchMf.sharedMesh = _patchMesh;
            _patchGo.SetActive(false);
            _level = MakeLine("level", 2, true);

            _slopeHeatmap = MakeLine("slopeHeatmap", 0, true);
            _slopeHeatmap.loop = true;
            _slopeHeatmap.numCornerVertices = 2;

            _heightHeatmap = MakeLine("heightHeatmap", 0, true);
            _heightHeatmap.loop = true;
            _heightHeatmap.numCornerVertices = 2;

            // Translucent footprint for stamps: a filled quad per tile so the
            // copied region is actually visible where it will be placed.
            _stampMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _stampMat.color = new Color(1f, 1f, 1f, 0.3f);
            _stampGo = new GameObject("stampFoot") { hideFlags = HideFlags.HideAndDontSave };
            _stampMf = _stampGo.AddComponent<MeshFilter>();
            _stampMr = _stampGo.AddComponent<MeshRenderer>();
            _stampMr.sharedMaterial = _stampMat;
            _stampMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _stampMr.receiveShadows = false;
            _stampMesh = new Mesh { hideFlags = HideFlags.HideAndDontSave };
            _stampMf.sharedMesh = _stampMesh;
            _stampGo.SetActive(false);

            BuildRing();
            _root.SetActive(false);
        }

        private LineRenderer MakeLine(string name, int cornerVertices, bool worldSpace)
        {
            var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
            go.transform.SetParent(_root.transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = worldSpace;
            lr.loop = false;
            lr.numCornerVertices = cornerVertices;
            lr.textureMode = LineTextureMode.Stretch;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.alignment = LineAlignment.View;
            lr.material = _lineMat;
            return lr;
        }

        private void BuildRing()
        {
            _unitRing = new Vector3[Segments];
            for (int i = 0; i < Segments; i++)
            {
                float a = (float)i / Segments * Mathf.PI * 2f;
                _unitRing[i] = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
            }
        }

        public void Hide()
        {
            if (_root != null && _root.activeSelf)
                _root.SetActive(false);
            if (_patchGo != null && _patchGo.activeSelf)
                _patchGo.SetActive(false);
            if (_stampGo != null && _stampGo.activeSelf)
                _stampGo.SetActive(false);
        }

        public void Update(PreviewState s)
        {
            Ensure();
            if (_root == null)
                return;

            // If the aimed terrain is underwater, draw the preview at water surface
            // so it's visible. Use a small offset above water to avoid z-fighting.
            float waterY = s.WaterLevel > 0f ? s.WaterLevel + 0.1f : s.Center.y;
            float centerY = Mathf.Max(s.Center.y, waterY);

            _root.transform.position = new Vector3(s.Center.x, centerY + 0.05f, s.Center.z);
            _root.transform.rotation = Quaternion.identity;
            _root.transform.localScale = Vector3.one;

            if (_builtShape != s.Shape)
            {
                _builtShape = s.Shape;
                _builtEdge = -1f;
                _builtFull = -1f;
                _builtReach = -1f;
            }

            if (!Mathf.Approximately(_builtEdge, s.Radius))
            {
                _builtEdge = s.Radius;
                SetRing(_edge, s.Radius, s.Shape);
            }

            float fullR = Mathf.Min(s.FullRadius, s.Radius);
            if (fullR > 0.05f && !Mathf.Approximately(_builtFull, fullR))
            {
                _builtFull = fullR;
                SetRing(_full, fullR, s.Shape);
            }

            if (s.ReachRadius > 0.05f && !Mathf.Approximately(_builtReach, s.ReachRadius))
            {
                _builtReach = s.ReachRadius;
                SetRing(_reach, s.ReachRadius, s.Shape);
            }

            float w = Mathf.Clamp(s.Radius * 0.035f, 0.08f, 0.7f);
            SetWidth(_edge, w);
            SetWidth(_full, w * 0.45f);
            SetWidth(_reach, w * 0.35f);
            SetWidth(_aim, w * 0.30f);
            SetWidth(_rampLine, w * 0.50f);
            SetWidth(_rampL, w * 0.42f);
            SetWidth(_rampR, w * 0.42f);

            Color c = s.Blocked ? new Color(0.45f, 0.45f, 0.48f, s.Color.a) : s.Color;
            // Slope shading: tint the footprint edge toward red as the ground
            // under the aim gets steeper (guides only, op color is preserved
            // on flat ground).
            Color edgeC = c;
            if (s.GuidesOn && s.SlopeDeg > 15f)
            {
                edgeC = Color.Lerp(c,
                    new Color(1f, 0.3f, 0.25f, c.a),
                    Mathf.Clamp01((s.SlopeDeg - 15f) / 30f));
            }
            SetColor(_edge, edgeC);
            SetColor(_full, Col(c, 0.55f));
            SetColor(_reach, Col(c, 0.35f));
            SetColor(_aim, Col(c, 0.28f));

            // Once a corridor / footprint projection exists it replaces the round
            // footprint, so hide the circular guides to avoid clutter.
            bool suppress = s.SuppressCircles || (s.IsRamp && s.HasRampA);

            SetActive(_edge, !suppress);
            SyncShadow(_edge, _edgeShadow, w * 2.2f, 0.55f);
            SetActive(_full, !suppress && s.FullRadius < s.Radius - 0.05f && fullR > 0.05f);
            SetActive(_reach, !suppress && s.ReachRadius > s.Radius + 0.05f);

            if (!suppress && s.AimReach > 0.5f && s.PlayerPos.sqrMagnitude > 0.01f)
            {
                _aim.positionCount = Segments + 1;
                for (int i = 0; i < Segments; i++)
                {
                    float a = (float)i / Segments * Mathf.PI * 2f;
                    float wx = s.PlayerPos.x + Mathf.Cos(a) * s.AimReach;
                    float wz = s.PlayerPos.z + Mathf.Sin(a) * s.AimReach;
                    _aim.SetPosition(i, new Vector3(wx, DrawHeight(wx, wz, s) + 0.25f, wz));
                }
                _aim.SetPosition(Segments, _aim.GetPosition(0));
                SetActive(_aim, true);
            }
            else
            {
                SetActive(_aim, false);
            }

            Color cCorr = Col(c, 0.70f);
            if (s.HasRampA)
            {
                Vector3 a = s.RampA;
                Vector3 b = s.Center;

                Vector3 dir = new Vector3(b.x - a.x, 0f, b.z - a.z);
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Vector3 perp = Vector3.Cross(dir.normalized, Vector3.up) * s.Radius;

                    float aY = DrawHeight(a.x, a.z, s) + 0.15f;
                    float bY = DrawHeight(b.x, b.z, s) + 0.15f;
                    Vector3 a0 = new Vector3(a.x, aY, a.z);
                    Vector3 b0 = new Vector3(b.x, bY, b.z);

                    _rampLine.positionCount = 2;
                    _rampLine.SetPosition(0, a0);
                    _rampLine.SetPosition(1, b0);

                    _rampL.positionCount = 2;
                    _rampL.SetPosition(0, a0 + perp);
                    _rampL.SetPosition(1, b0 + perp);

                    _rampR.positionCount = 2;
                    _rampR.SetPosition(0, a0 - perp);
                    _rampR.SetPosition(1, b0 - perp);

                    SetColor(_rampLine, c);
                    SetColor(_rampL, cCorr);
                    SetColor(_rampR, cCorr);
                    SetActive(_rampLine, true);
                    SetActive(_rampL, true);
                    SetActive(_rampR, true);

                    // Filled corridor polygon (A-left, B-left, B-right, A-right)
                    _rampFill.positionCount = 4;
                    _rampFill.SetPosition(0, a0 + perp);
                    _rampFill.SetPosition(1, b0 + perp);
                    _rampFill.SetPosition(2, b0 - perp);
                    _rampFill.SetPosition(3, a0 - perp);
                    Color fillC = new Color(c.r, c.g, c.b, c.a * 0.18f);
                    SetColor(_rampFill, fillC);
                    SetWidth(_rampFill, w * 0.6f);
                    SetActive(_rampFill, true);

                    // Height markers at A and B (small rings at ground height)
                    UpdateHeightMarker(_rampMarkA, a0, c);
                    UpdateHeightMarker(_rampMarkB, b0, c);
                    SetActive(_rampMarkA, true);
                    SetActive(_rampMarkB, true);
                }
                else
                {
                    SetActive(_rampLine, false);
                    SetActive(_rampL, false);
                    SetActive(_rampR, false);
                    SetActive(_rampFill, false);
                    SetActive(_rampMarkA, false);
                    SetActive(_rampMarkB, false);
                }
            }
            else
            {
                SetActive(_rampLine, false);
                SetActive(_rampL, false);
                SetActive(_rampR, false);
                SetActive(_rampFill, false);
                SetActive(_rampMarkA, false);
                SetActive(_rampMarkB, false);
            }

            // Rectangle footprint outline (corner A -> cursor corner B).
            if (s.HasRect)
            {
                float x0 = Mathf.Min(s.RectA.x, s.RectB.x);
                float x1 = Mathf.Max(s.RectA.x, s.RectB.x);
                float z0 = Mathf.Min(s.RectA.z, s.RectB.z);
                float z1 = Mathf.Max(s.RectA.z, s.RectB.z);
                _rect.positionCount = 5;
                _rect.SetPosition(0, new Vector3(x0, DrawHeight(x0, z0, s) + 0.15f, z0));
                _rect.SetPosition(1, new Vector3(x0, DrawHeight(x0, z1, s) + 0.15f, z1));
                _rect.SetPosition(2, new Vector3(x1, DrawHeight(x1, z1, s) + 0.15f, z1));
                _rect.SetPosition(3, new Vector3(x1, DrawHeight(x1, z0, s) + 0.15f, z0));
                _rect.SetPosition(4, new Vector3(x0, DrawHeight(x0, z0, s) + 0.15f, z0));
                SetWidth(_rect, w * 0.6f);
                SetColor(_rect, c);
                SetActive(_rect, true);
            }
            else
            {
                SetActive(_rect, false);
            }
            SyncShadow(_rect, _rectShadow, w * 1.5f, 0.55f);

            // Polygon footprint outline (fixed points + cursor, closed loop).
            if (s.PolyPreview != null && s.PolyPreview.Length >= 2)
            {
                int n = s.PolyPreview.Length;
                _poly.positionCount = n + 1;
                for (int k = 0; k < n; k++)
                {
                    Vector3 pt = s.PolyPreview[k];
                    _poly.SetPosition(k, new Vector3(pt.x, DrawHeight(pt.x, pt.z, s) + 0.15f, pt.z));
                }
                _poly.SetPosition(n, _poly.GetPosition(0));
                SetWidth(_poly, w * 0.6f);
                SetColor(_poly, c);
                SetActive(_poly, true);
            }
            else
            {
                SetActive(_poly, false);
            }
            SyncShadow(_poly, _polyShadow, w * 1.5f, 0.55f);

            // Target-level marker: a horizontal ring at the height Flatten /
            // Shore (or a fixed eyedrop height) would produce. Terrain occludes
            // it naturally, so it reads as a contour line on slopes.
            if (!suppress && !float.IsNaN(s.LevelY))
            {
                float ly = s.LevelY + 0.05f;
                if (s.Shape == TerrainShape.Square)
                {
                    float r = s.Radius;
                    _level.positionCount = 5;
                    _level.SetPosition(0, new Vector3(s.Center.x - r, ly, s.Center.z - r));
                    _level.SetPosition(1, new Vector3(s.Center.x - r, ly, s.Center.z + r));
                    _level.SetPosition(2, new Vector3(s.Center.x + r, ly, s.Center.z + r));
                    _level.SetPosition(3, new Vector3(s.Center.x + r, ly, s.Center.z - r));
                    _level.SetPosition(4, new Vector3(s.Center.x - r, ly, s.Center.z - r));
                }
                else
                {
                    _level.positionCount = Segments + 1;
                    for (int k = 0; k < Segments; k++)
                    {
                        float a = (float)k / Segments * Mathf.PI * 2f;
                        _level.SetPosition(k, new Vector3(
                            s.Center.x + Mathf.Cos(a) * s.Radius, ly,
                            s.Center.z + Mathf.Sin(a) * s.Radius));
                    }
                    _level.SetPosition(Segments, _level.GetPosition(0));
                }
                SetWidth(_level, w * 0.45f);
                SetColor(_level, Col(c, 0.65f));
                SetActive(_level, true);
            }
            else
            {
                SetActive(_level, false);
            }

            DrawStampFootprint(s);

            // Draw heatmap overlays
            DrawSlopeHeatmap(s);
            DrawHeightHeatmap(s);

            UpdatePatch(s, c, suppress);

            if (!_root.activeSelf)
                _root.SetActive(true);
        }

        private void DrawSlopeHeatmap(PreviewState s)
        {
            if (!s.ShowSlopeHeatmap)
            {
                SetActive(_slopeHeatmap, false);
                return;
            }

            float w = Mathf.Clamp(s.Radius * 0.035f, 0.08f, 0.7f);
            Color c = s.Blocked ? new Color(0.45f, 0.45f, 0.48f, s.Color.a) : s.Color;

            // Draw a grid of slope indicators
            const int gridSize = 16;
            const int segments = 8;
            float spacing = s.Radius * 2f / gridSize;
            float startX = s.Center.x - s.Radius;
            float startZ = s.Center.z - s.Radius;

            int totalPoints = gridSize * gridSize * (segments + 1);
            _slopeHeatmap.positionCount = totalPoints;
            int idx = 0;

            for (int gx = 0; gx < gridSize; gx++)
            {
                for (int gz = 0; gz < gridSize; gz++)
                {
                    float wx = startX + gx * spacing + spacing * 0.5f;
                    float wz = startZ + gz * spacing + spacing * 0.5f;
                    float slope = TerrainBrush.SlopeDeg(new Vector3(wx, 0, wz));
                    Color heatColor = GetSlopeColor(slope, c.a);

                    for (int seg = 0; seg <= segments; seg++)
                    {
                        float angle = (float)seg / segments * Mathf.PI * 2f;
                        float r = s.Radius / gridSize * 0.4f;
                        float wx_pos = wx + Mathf.Cos(angle) * r;
                        float wz_pos = wz + Mathf.Sin(angle) * r;
                        float y = DrawHeight(wx_pos, wz_pos, s) + 0.1f;
                        _slopeHeatmap.SetPosition(idx++, new Vector3(wx_pos, y, wz_pos));
                    }
                    // Close the loop
                    _slopeHeatmap.SetPosition(idx - 1, _slopeHeatmap.GetPosition(idx - segments - 1));
                }
            }

            SetWidth(_slopeHeatmap, 0.02f);
            SetColor(_slopeHeatmap, c);
            SetActive(_slopeHeatmap, true);
        }

        private Color GetSlopeColor(float slopeDeg, float alpha)
        {
            // Green (0-15°) -> Yellow (15-30°) -> Orange (30-45°) -> Red (45°+)
            if (slopeDeg < 15f)
                return new Color(0.2f, 0.8f, 0.2f, alpha);
            else if (slopeDeg < 30f)
                return new Color(1f, 0.9f, 0.1f, alpha);
            else if (slopeDeg < 45f)
                return new Color(1f, 0.5f, 0.1f, alpha);
            else
                return new Color(1f, 0.2f, 0.1f, alpha);
        }

        private void DrawHeightHeatmap(PreviewState s)
        {
            if (!s.ShowHeightHeatmap)
            {
                SetActive(_heightHeatmap, false);
                return;
            }

            float w = Mathf.Clamp(s.Radius * 0.035f, 0.08f, 0.7f);
            Color c = s.Blocked ? new Color(0.45f, 0.45f, 0.48f, s.Color.a) : s.Color;

            // Draw a grid of height indicators
            const int gridSize = 16;
            const int segments = 8;
            float spacing = s.Radius * 2f / gridSize;
            float startX = s.Center.x - s.Radius;
            float startZ = s.Center.z - s.Radius;

            int totalPoints = gridSize * gridSize * (segments + 1);
            _heightHeatmap.positionCount = totalPoints;
            int idx = 0;

            // Get min/max height in the area for normalization
            float minH = float.MaxValue, maxH = float.MinValue;
            float[,] heights = new float[16, 16];
            for (int gx = 0; gx < 16; gx++)
            {
                for (int gz = 0; gz < 16; gz++)
                {
                    float wx = s.Center.x - s.Radius + (gx + 0.5f) * (s.Radius * 2f / 16f);
                    float wz = s.Center.z - s.Radius + (gz + 0.5f) * (s.Radius * 2f / 16f);
                    float h = GroundHeight(wx, wz);
                    heights[gx, gz] = h;
                    minH = Mathf.Min(minH, h);
                    maxH = Mathf.Max(maxH, h);
                }
            }

            for (int gx = 0; gx < gridSize; gx++)
            {
                for (int gz = 0; gz < gridSize; gz++)
                {
                    float wx = s.Center.x - s.Radius + (gx + 0.5f) * (s.Radius * 2f / gridSize);
                    float wz = s.Center.z - s.Radius + (gz + 0.5f) * (s.Radius * 2f / gridSize);
                    float h = GroundHeight(wx, wz);
                    float t = maxH > minH ? Mathf.InverseLerp(minH, maxH, h) : 0.5f;
                    Color heatColor = GetHeightColor(t, s.Color.a);

                    for (int seg = 0; seg <= 8; seg++)
                    {
                        float angle = (float)seg / 8f * Mathf.PI * 2f;
                        float r = s.Radius / 16f * 0.4f;
                        float wx_pos = (s.Center.x - s.Radius) + (gx + 0.5f) * (s.Radius * 2f / 16f) + Mathf.Cos(angle) * (s.Radius / 16f * 0.4f);
                        float wz_pos = (s.Center.z - s.Radius) + (gz + 0.5f) * (s.Radius * 2f / 16f) + Mathf.Sin(angle) * (s.Radius / 16f * 0.4f);
                        float y = DrawHeight(wx_pos, wz_pos, s) + 0.1f;
                        _heightHeatmap.SetPosition(idx++, new Vector3(wx_pos, y, wz_pos));
                    }
                    _heightHeatmap.SetPosition(idx - 1, _heightHeatmap.GetPosition(idx - 9));
                }
            }

            SetWidth(_heightHeatmap, 0.02f);
            SetColor(_heightHeatmap, s.Color);
            SetActive(_heightHeatmap, true);
        }

        private Color GetHeightColor(float t, float alpha)
        {
            // Blue (low) -> Cyan -> Green -> Yellow -> Red (high)
            if (t < 0.25f)
                return new Color(0.1f, 0.3f, 1f, alpha);      // Deep blue
            else if (t < 0.5f)
                return new Color(0.1f, 0.7f, 1f, alpha);      // Cyan
            else if (t < 0.75f)
                return new Color(0.2f, 0.9f, 0.2f, alpha);    // Green
            else if (t < 0.9f)
                return new Color(1f, 0.9f, 0.1f, alpha);      // Yellow
            else
                return new Color(1f, 0.2f, 0.1f, alpha);      // Red
        }

        private const float StampHighlightSeconds = 4f;
        private const int StampTileLimit = 32;

        /// <summary>
        /// Draws the stamp as a real relief surface built from the stored per-vertex
        /// height deltas, so you can recognise the shape being copied/pasted. Tile
        /// centres are moved/rotated/scaled like <see cref="TerrainEngine.PasteStamp"/>
        /// (pasted) or shown in gold at the source (just copied). Vertices are tinted
        /// warm where terrain rises and cool where it drops; the tile rim is brighter
        /// so the copied region reads clearly.
        /// </summary>
        private void DrawStampFootprint(PreviewState s)
        {
            TerrainEngine.TerrainStamp stamp = s.Stamp;
            bool has = stamp != null && stamp.Tiles != null && stamp.Tiles.Count > 0;
            bool paste = s.ShowStamp && has;
            bool highlight = !paste && has && s.CopyHighlightStart > 0f &&
                             (Time.time - s.CopyHighlightStart) < StampHighlightSeconds;

            if (!paste && !highlight)
            {
                SetActive(_stampGo, false);
                return;
            }

            // Throttle the (potentially large) relief rebuild to ~10 Hz.
            if (Time.time < _stampNext)
            {
                SetActive(_stampGo, true);
                return;
            }
            _stampNext = Time.time + 0.1f;

            Vector3 target = paste ? s.StampTarget : stamp.Center;
            float rot = paste ? s.StampRotation : 0f;
            float scl = paste ? s.StampScale : 1f;
            Vector3 origin = stamp.Center;
            float cosR = Mathf.Cos(rot * Mathf.Deg2Rad);
            float sinR = Mathf.Sin(rot * Mathf.Deg2Rad);
            bool blocked = paste && s.Blocked;

            int tileCount = Mathf.Min(stamp.Tiles.Count, StampTileLimit);

            float maxAbs = 0.5f;
            for (int ti = 0; ti < tileCount; ti++)
            {
                float[] lv = stamp.Tiles[ti].Level;
                if (lv == null)
                    continue;
                for (int k = 0; k < lv.Length; k++)
                {
                    float a = lv[k] < 0f ? -lv[k] : lv[k];
                    if (a > maxAbs)
                        maxAbs = a;
                }
            }

            var verts = new List<Vector3>();
            var cols = new List<Color>();
            var tris = new List<int>();

            Color flat = blocked ? new Color(0.45f, 0.45f, 0.48f) : new Color(0.62f, 0.62f, 0.66f);
            Color upCol = blocked ? flat : new Color(1.00f, 0.55f, 0.25f);
            Color downCol = blocked ? flat : new Color(0.35f, 0.65f, 1.00f);
            Color rim = highlight ? new Color(1f, 0.85f, 0.25f) : new Color(s.Color.r, s.Color.g, s.Color.b);
            float rimAlpha = 0.95f;

            for (int ti = 0; ti < tileCount; ti++)
            {
                TerrainEngine.StampTile tile = stamp.Tiles[ti];
                if (tile.Level == null || tile.Width <= 0)
                    continue;
                int w = tile.Width;
                int pitch = w + 1;
                if (tile.Level.Length < pitch * pitch)
                    continue;

                float st = (tile.Size > 0.5f ? tile.Size : 64f) / w;
                float half = w * 0.5f;

                Vector3 rel = tile.TilePos - origin;
                float rx = rel.x * scl;
                float rz = rel.z * scl;
                float tcx = target.x + rx * cosR - rz * sinR;
                float tcz = target.z + rx * sinR + rz * cosR;

                int tbase = verts.Count;
                for (int i = 0; i <= w; i++)
                {
                    for (int j = 0; j <= w; j++)
                    {
                        float wx = tcx + (j - half) * st;
                        float wz = tcz + (i - half) * st;
                        float d = tile.Level[i * pitch + j];
                        float wy = GroundHeight(wx, wz) + d + 0.10f;
                        verts.Add(new Vector3(wx, wy, wz));

                        bool border = i == 0 || i == w || j == 0 || j == w;
                        if (border)
                        {
                            cols.Add(new Color(rim.r, rim.g, rim.b, rimAlpha));
                        }
                        else
                        {
                            float t = Mathf.Clamp(d / maxAbs, -1f, 1f);
                            Color c = t > 0.04f ? Color.Lerp(flat, upCol, t)
                                    : t < -0.04f ? Color.Lerp(flat, downCol, -t)
                                    : flat;
                            c.a = Mathf.Lerp(0.18f, highlight ? 0.55f : 0.50f, Mathf.Abs(t));
                            cols.Add(c);
                        }
                    }
                }

                bool region = stamp.RegionMask && tile.ModH != null;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        int cA = i * pitch + j;
                        if (region && !(tile.ModH[cA] && tile.ModH[cA + 1] &&
                                        tile.ModH[cA + pitch] && tile.ModH[cA + pitch + 1]))
                            continue;
                        int a = tbase + i * pitch + j;
                        int b = a + 1;
                        int c2 = a + pitch + 1;
                        int d2 = a + pitch;
                        tris.Add(a); tris.Add(c2); tris.Add(b);
                        tris.Add(a); tris.Add(d2); tris.Add(c2);
                    }
                }
            }

            if (verts.Count == 0)
            {
                SetActive(_stampGo, false);
                return;
            }

            _stampMesh.Clear();
            _stampMesh.SetVertices(verts);
            _stampMesh.SetColors(cols);
            _stampMesh.SetTriangles(tris, 0);
            _stampMesh.RecalculateBounds();
            _stampMat.color = Color.white;
            SetActive(_stampGo, true);
        }

        private void SetRing(LineRenderer lr, float radius, TerrainShape shape)
        {
            if (shape == TerrainShape.Square)
            {
                lr.positionCount = 5;
                lr.SetPosition(0, new Vector3(-radius, 0f, -radius));
                lr.SetPosition(1, new Vector3(-radius, 0f, radius));
                lr.SetPosition(2, new Vector3(radius, 0f, radius));
                lr.SetPosition(3, new Vector3(radius, 0f, -radius));
                lr.SetPosition(4, new Vector3(-radius, 0f, -radius));
            }
            else
            {
                lr.positionCount = Segments + 1;
                for (int i = 0; i < Segments; i++)
                    lr.SetPosition(i, _unitRing[i] * radius);
                lr.SetPosition(Segments, _unitRing[0] * radius);
            }
        }

        private void UpdatePatch(PreviewState s, Color c, bool suppress)
        {
            bool want = !suppress && !s.Blocked && s.Radius > 0.5f &&
                        (s.Shape == TerrainShape.Circle || s.Shape == TerrainShape.Square);
            if (!want)
            {
                SetActive(_patchGo, false);
                return;
            }

            _patchMat.color = new Color(c.r, c.g, c.b, 0.22f);

            bool stale = _patchShape != s.Shape || !Mathf.Approximately(_patchRadius, s.Radius);
            if (stale || Time.time >= _patchNext)
            {
                _patchShape = s.Shape;
                _patchRadius = s.Radius;
                _patchNext = Time.time + 0.1f;
                RebuildPatch(s);
            }

            SetActive(_patchGo, true);
        }

        private void RebuildPatch(PreviewState s)
        {
            const int G = 18;
            float r = s.Radius;
            var verts = new List<Vector3>();
            var ids = new int[G + 1, G + 1];
            for (int i = 0; i <= G; i++)
                for (int j = 0; j <= G; j++)
                    ids[i, j] = -1;

            float step = 2f * r / G;
            for (int i = 0; i <= G; i++)
            {
                float gx = -r + i * step;
                for (int j = 0; j <= G; j++)
                {
                    float gz = -r + j * step;
                    if (s.Shape == TerrainShape.Circle && gx * gx + gz * gz > r * r)
                        continue;
                    float wx = s.Center.x + gx;
                    float wz = s.Center.z + gz;
                    ids[i, j] = verts.Count;
                    verts.Add(new Vector3(wx, DrawHeight(wx, wz, s) + 0.06f, wz));
                }
            }

            var tris = new List<int>();
            for (int i = 0; i < G; i++)
                for (int j = 0; j < G; j++)
                {
                    int a = ids[i, j], b = ids[i + 1, j], d = ids[i + 1, j + 1], e = ids[i, j + 1];
                    if (a < 0 || b < 0 || d < 0 || e < 0)
                        continue;
                    tris.Add(a); tris.Add(b); tris.Add(d);
                    tris.Add(a); tris.Add(d); tris.Add(e);
                }

            _patchMesh.Clear();
            _patchMesh.SetVertices(verts);
            _patchMesh.SetTriangles(tris, 0);
            _patchMesh.RecalculateBounds();
        }

        private static void SetWidth(LineRenderer lr, float w)
        {
            lr.startWidth = w;
            lr.endWidth = w;
        }

        private static void SetColor(LineRenderer lr, Color c)
        {
            lr.startColor = c;
            lr.endColor = c;
        }

        private static void SetActive(LineRenderer lr, bool on)
        {
            if (lr != null && lr.gameObject.activeSelf != on)
                lr.gameObject.SetActive(on);
        }

        private static void SetActive(GameObject go, bool on)
        {
            if (go != null && go.activeSelf != on)
                go.SetActive(on);
        }

        // Keeps a dark backing line in sync with a source outline so the main
        // colour stays readable over bright/busy terrain.
        private static void SyncShadow(LineRenderer src, LineRenderer dst, float width, float alpha)
        {
            if (dst == null)
                return;
            bool on = src != null && src.gameObject.activeSelf && src.positionCount > 1;
            SetActive(dst, on);
            if (!on)
                return;
            int n = src.positionCount;
            dst.positionCount = n;
            for (int i = 0; i < n; i++)
                dst.SetPosition(i, src.GetPosition(i));
            SetWidth(dst, width);
            SetColor(dst, new Color(0f, 0f, 0f, alpha));
        }

        private static Color Col(Color c, float a)
        {
            return new Color(c.r, c.g, c.b, c.a * a);
        }

        private static float DrawHeight(float x, float z, PreviewState s)
        {
            float ground = GroundHeight(x, z);
            float water = s.WaterLevel > 0f ? s.WaterLevel + 0.1f : ground;
            return Mathf.Max(ground, water);
        }

        private static float GroundHeight(float x, float z)
        {
            if (ZoneSystem.instance != null)
                return ZoneSystem.instance.GetGroundHeight(new Vector3(x, 0f, z));
            return 0f;
        }

        private static void UpdateHeightMarker(LineRenderer lr, Vector3 pos, Color c)
        {
            const float ringR = 0.35f;
            const int segs = 16;
            lr.positionCount = segs + 1;
            for (int i = 0; i < segs; i++)
            {
                float a = (float)i / segs * Mathf.PI * 2f;
                lr.SetPosition(i, new Vector3(pos.x + Mathf.Cos(a) * ringR, pos.y + 0.08f, pos.z + Mathf.Sin(a) * ringR));
            }
            lr.SetPosition(segs, lr.GetPosition(0));
            SetWidth(lr, 0.04f);
            SetColor(lr, new Color(c.r, c.g, c.b, c.a * 0.6f));
        }

        public void Destroy()
        {
            SafeDestroy(_lineMat);
            SafeDestroy(_patchMat);
            SafeDestroy(_patchMesh);
            if (_patchGo != null)
            {
                Object.Destroy(_patchGo);
                _patchGo = null;
                _patchMf = null;
                _patchMr = null;
            }
            SafeDestroy(_stampMat);
            SafeDestroy(_stampMesh);
            if (_stampGo != null)
            {
                Object.Destroy(_stampGo);
                _stampGo = null;
                _stampMf = null;
                _stampMr = null;
            }
            if (_root != null)
            {
                Object.Destroy(_root);
                _root = null;
                _edge = null;
                _full = null;
                _reach = null;
                _aim = null;
                _rampLine = null;
                _rampL = null;
                _rampR = null;
                _rampFill = null;
                _rampMarkA = null;
                _rampMarkB = null;
                _rect = null;
                _poly = null;
                _edgeShadow = null;
                _rectShadow = null;
                _polyShadow = null;
                _level = null;
            }
        }

        private static void SafeDestroy(Object o)
        {
            if (o != null)
                Object.Destroy(o);
        }

        private static Shader FindShader(params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                Shader s = Shader.Find(names[i]);
                if (s != null)
                    return s;
            }
            return null;
        }
    }
}
