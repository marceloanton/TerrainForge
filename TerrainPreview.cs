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

            _edge = MakeLine("edge", 2, false);
            _full = MakeLine("full", 2, false);
            _reach = MakeLine("reach", 2, false);
            _aim = MakeLine("aim", 2, true);
            _rampLine = MakeLine("ramp", 0, true);
            _rampL = MakeLine("rampL", 0, true);
            _rampR = MakeLine("rampR", 0, true);

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
        }

        public void Update(PreviewState s)
        {
            Ensure();
            if (_root == null)
                return;

            _root.transform.position = new Vector3(s.Center.x, s.Center.y + 0.05f, s.Center.z);
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

            float w = Mathf.Clamp(s.Radius * 0.03f, 0.05f, 0.6f);
            SetWidth(_edge, w);
            SetWidth(_full, w * 0.45f);
            SetWidth(_reach, w * 0.35f);
            SetWidth(_aim, w * 0.30f);
            SetWidth(_rampLine, w * 0.50f);
            SetWidth(_rampL, w * 0.42f);
            SetWidth(_rampR, w * 0.42f);

            Color c = s.Color;
            SetColor(_edge, c);
            SetColor(_full, Col(c, 0.42f));
            SetColor(_reach, Col(c, 0.20f));
            SetColor(_aim, Col(c, 0.15f));

            // Once the ramp corridor projection exists it replaces the round
            // footprint, so hide the circular guides to avoid clutter.
            bool suppress = s.IsRamp && s.HasRampA;

            SetActive(_edge, !suppress);
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
                    _aim.SetPosition(i, new Vector3(wx, GroundHeight(wx, wz) + 0.25f, wz));
                }
                _aim.SetPosition(Segments, _aim.GetPosition(0));
                SetActive(_aim, true);
            }
            else
            {
                SetActive(_aim, false);
            }

            Color cCorr = Col(c, 0.55f);
            if (s.HasRampA)
            {
                Vector3 a = s.RampA;
                Vector3 b = s.Center;

                Vector3 dir = new Vector3(b.x - a.x, 0f, b.z - a.z);
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Vector3 perp = Vector3.Cross(dir.normalized, Vector3.up) * s.Radius;

                    Vector3 a0 = new Vector3(a.x, a.y + 0.15f, a.z);
                    Vector3 b0 = new Vector3(b.x, b.y + 0.15f, b.z);

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
                }
                else
                {
                    SetActive(_rampLine, false);
                    SetActive(_rampL, false);
                    SetActive(_rampR, false);
                }
            }
            else
            {
                SetActive(_rampLine, false);
                SetActive(_rampL, false);
                SetActive(_rampR, false);
            }

            if (!_root.activeSelf)
                _root.SetActive(true);
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

        private static Color Col(Color c, float a)
        {
            return new Color(c.r, c.g, c.b, c.a * a);
        }

        private static float GroundHeight(float x, float z)
        {
            if (ZoneSystem.instance != null)
                return ZoneSystem.instance.GetGroundHeight(new Vector3(x, 0f, z));
            return 0f;
        }

        public void Destroy()
        {
            SafeDestroy(_lineMat);
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
