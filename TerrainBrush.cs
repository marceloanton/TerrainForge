using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Reads input, resolves the brush target and drives <see cref="TerrainEngine"/>.
    /// Also draws the always-on status HUD.
    /// </summary>
    internal sealed class TerrainBrush : MonoBehaviour
    {
        private const float ApplyInterval = 0.1f;
        private const float MaxAimDistance = 40f;

        // Effective aim distance. The free-fly camera raises the cap so the
        // crosshair can target distant ground (e.g. marking a large stamp).
        private static float AimDistance
        {
            get
            {
                if (GameCamera.InFreeFly())
                {
                    TerrainForgePlugin p = TerrainForgePlugin.Instance;
                    if (p != null)
                        return Mathf.Max(MaxAimDistance, p.FreeCameraAimDistance.Value);
                }
                return MaxAimDistance;
            }
        }

        private readonly TerrainPreview _preview = new TerrainPreview();
        private float _lastApply = -999f;
        private bool _applying;
        private bool _toolGiven;
        private bool _rampHasA;
        private Vector3 _rampA;

        private GUIStyle _hud;
        private GUIStyle _hudTitle;
        private GUIStyle _hudSmall;
        private GUIStyle _hudRight;
        private GUIStyle _banner;
        private Texture2D _hudBg;
        private string _flashMsg;
        private float _flashUntil;
        private float _aimY;
        private string _targetTxt = "";
        private float _slopeDeg;
        private float _levelY = float.NaN;
        private Vector3 _aimPoint;
        private bool _previewLogged;
        private bool _serverAllowed = true;
        private string _serverWhy;
        private bool _wardOk = true;
        private string _wardWhy;
        private bool _lineHasA;
        private Vector3 _lineA;
        private bool _rectHasA;
        private Vector3 _rectA;
        private readonly List<Vector3> _polyPoints = new List<Vector3>();
        private float _polyLastClick = -999f;
        private readonly List<Vector3> _rampPath = new List<Vector3>();
        private readonly List<float> _rampPathH = new List<float>();
        private float _rampPathLast = -999f;
        private float _lastUndo = -999f;
        private float _lastRedo = -999f;
        private bool _adjustMode;

        // Persistent HUD info: position.
        private string _posTxt = "";

        // Stamp copy/paste
        private bool _pastePreview;
        private float _stampRotation;
        private float _stampScale = 1f;
        private Vector3 _stampPreviewCenter;
        // Timestamp of the last copy, for the short "just copied" highlight.
        private float _clipFxStart;

        internal static bool AdjustModeActive { get; private set; }
        internal static bool FreeFlyWheelBusy { get; private set; }
        private static bool s_clearMarks;
        private static bool s_reqUndo, s_reqRedo, s_reqEyedrop, s_reqGiveTool;
        private static bool s_reqCopyStamp, s_reqPasteStamp, s_reqCycleStampBack, s_reqCycleStamp;
        private static bool s_reqDumpMat;
        private static bool s_reqFreeCam;
        internal static bool SuppressFreeFlyScreenshot;
        internal static bool SuppressFreeFlyLockon;

        internal static void RequestClearMarks() => s_clearMarks = true;
        internal static void RequestUndo() => s_reqUndo = true;
        internal static void RequestRedo() => s_reqRedo = true;
        internal static void RequestEyedrop() => s_reqEyedrop = true;
        internal static void RequestGiveTool() => s_reqGiveTool = true;
        internal static void RequestCopyStamp() => s_reqCopyStamp = true;
        internal static void RequestPasteStamp() => s_reqPasteStamp = true;
        internal static void RequestCycleStampBack() => s_reqCycleStampBack = true;
        internal static void RequestCycleStamp() => s_reqCycleStamp = true;
        internal static void RequestDumpMaterial() => s_reqDumpMat = true;
        internal static void RequestFreeCamera() => s_reqFreeCam = true;

        private void OnDestroy()
        {
            _preview.Destroy();
            if (_hudBg != null)
                Object.Destroy(_hudBg);
        }

        private void Update()
        {
            _applying = false;

            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || !p.Enabled.Value)
            {
                _preview.Hide();
                _adjustMode = false;
                AdjustModeActive = false;
                return;
            }

            Player player = Player.m_localPlayer;
            if (player == null)
            {
                _toolGiven = false;
                _preview.Hide();
                TerrainEngine.ClearHistory();
                _rampHasA = false;
                p.UseFixedHeight.Value = false;
                ClearShapeMarks();
                ClearRampPath();
                _adjustMode = false;
                AdjustModeActive = false;
                return;
            }

            // Other mods may rebuild ObjectDB after CopyOtherDB (Jotunn does this
            // while adding its items). Revalidate the rod against the live database
            // before the automatic handout path runs.
            TerrainTool.EnsureRegistered();

            if (TerrainKeybinds.MenuOpen)
            {
                _preview.Hide();
                return;
            }

            if (TerrainRadial.Open)
            {
                _preview.Hide();
                return;
            }

            if (s_clearMarks)
            {
                s_clearMarks = false;
                _rampHasA = false;
                ClearShapeMarks();
                ClearRampPath();
            }

            // Keep pumping a chunked stroke across frames (auto-continues even
            // if the key was released); hold-strokes wait for it (one undo step
            // per completed stroke), discrete clicks drain it inside Apply.
            bool wasPending = TerrainEngine.HasPending;
            if (wasPending)
            {
                TerrainEngine.Pump();
                _applying = true;
                Flash(Loc.T("flash.applying"));
            }

            if (p.AutoGiveTool.Value && !_toolGiven && TerrainTool.Registered)
            {
                if (TerrainTool.Has(player))
                    TerrainTool.Equip(player);
                else
                    TerrainTool.Give(player);
                _toolGiven = true;
            }

            bool blocked = Console.IsVisible() || InventoryGui.IsVisible() || TextInput.IsVisible();

            if (!blocked)
                HandleRadius(p);

            if (!blocked && p.GiveToolKey.Value.IsDown())
                TerrainTool.Give(player);

            ItemDrop.ItemData weapon = GetHeldItem(player);
            bool equipped = IsToolEquipped(p, weapon);

            // Free-fly "photo" camera (F): while it is active the player is frozen
            // and the held item may be hidden by the game, so treat the tool as the
            // dominant mode for the HUD, preview and editing regardless of equip.
            bool freeCam = GameCamera.InFreeFly();
            bool active = equipped || freeCam;
            SuppressFreeFlyScreenshot = freeCam;
            SuppressFreeFlyLockon = freeCam;

            if (equipped && !p.SeenTutorial.Value)
            {
                p.SeenTutorial.Value = true;
                try { p.Config.Save(); } catch { }
                TerrainForgePlugin.Message(Loc.T("tut.1"));
                TerrainForgePlugin.Message(Loc.T("tut.2"));
            }

            // Free-fly camera: toggle while the brush is active. Allowed even when
            // the tool is unequipped if the camera is already flying, so the same
            // key can always be used to exit and never strands the player.
            if (!blocked && active && p.FreeCameraKey.Value.IsDown())
                ToggleFreeCamera();

            Vector3 aim;
            bool hasAim = TryGetAim(out aim);

            Vector3 groundCenter;
            if (hasAim)
            {
                groundCenter = aim;
            }
            else
            {
                // In the free-fly camera the (frozen) player may be nowhere near
                // what the crosshair is over, so fall back to the terrain below the
                // camera, never below the player.
                Camera cam = Camera.main;
                Vector3 src = freeCam && cam != null ? cam.transform.position : player.transform.position;
                float gy = GroundHeight(src);
                groundCenter = new Vector3(src.x, gy, src.z);
            }

            if (!blocked && active)
            {
                if (p.CycleOperationKey.Value.IsDown())
                {
                    CycleOperation(p);
                    _rampHasA = false;
                    ClearShapeMarks();
                    ClearRampPath();
                }
                HandleDirectOperationKeys(p);
                HandleCycleProfile(p);
                if (p.CycleShapeKey.Value.IsDown())
                {
                    CycleShape(p);
                    ClearShapeMarks();
                }
                if (p.CyclePaintKey.Value.IsDown())
                    CyclePaint(p);
                if (p.UndoKey.Value.IsDown() ||
                    (Held(p.UndoKey.Value) && Time.time - _lastUndo > 0.35f))
                {
                    _lastUndo = Time.time;
                    DoUndo();
                }
                if (p.RedoKey.Value.IsDown() ||
                    (Held(p.RedoKey.Value) && Time.time - _lastRedo > 0.35f))
                {
                    _lastRedo = Time.time;
                    DoRedo();
                }
                if (p.AdjustModeKey.Value.IsDown())
                {
                    _adjustMode = !_adjustMode;
                    AdjustModeActive = _adjustMode;
                    Flash(_adjustMode ? Loc.T("flash.adjust.on") : Loc.T("flash.adjust.off"));
                }
                if (p.EyedropKey.Value.IsDown())
                    ToggleEyedrop(groundCenter);
                if (p.UseFixedHeight.Value && !GameCamera.InFreeFly())
                {
                    // Handle Alt + Mouse Wheel for height adjustment
                    bool altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                    float scroll = WheelDelta();
                    if (altHeld && Mathf.Abs(scroll) > 0.01f)
                    {
                        float delta = scroll > 0f ? 0.1f : -0.1f;
                        p.FixedHeight.Value = Mathf.Clamp(p.FixedHeight.Value + delta, -10f, 150f);
                        Flash($"Altura fija: {p.FixedHeight.Value:0.1} m");
                    }
                }
                if (p.RampModeKey.Value.IsDown())
                {
                    p.RampMode.Value = (RampMode)(((int)p.RampMode.Value + 1) % 3);
                    _rampHasA = false;
                    ClearRampPath();
                    switch (p.RampMode.Value)
                    {
                        case RampMode.FixedAngle:
                            Flash(Loc.F("flash.rampmode.fixed", p.SlopeAngle.Value.ToString("0")));
                            break;
                        case RampMode.Path:
                            Flash(Loc.T("flash.rampmode.path"));
                            break;
                        default:
                            Flash(Loc.T("flash.rampmode.two"));
                            break;
                    }
                }
            }

            // Server authority: when the server runs the mod, its Server-section
            // values (synced to clients) decide whether this client may write
            // terrain. Servers without the mod leave the local values alone, so
            // the tool keeps working there for testing.
            _serverAllowed = !active || ServerAllows(p, out _serverWhy);
            _wardOk = !active || WardAllows(p, groundCenter, EffectiveRadius(p), out _wardWhy);
            bool canEdit = _serverAllowed && _wardOk;
            string why = _serverAllowed ? _wardWhy : _serverWhy;

            // Quick actions requested from the F5 menu.
            if (!blocked)
            {
                if (s_reqUndo) { s_reqUndo = false; DoUndo(); }
                if (s_reqRedo) { s_reqRedo = false; DoRedo(); }
                if (s_reqEyedrop) { s_reqEyedrop = false; ToggleEyedrop(groundCenter); }
                if (s_reqGiveTool) { s_reqGiveTool = false; TerrainTool.Give(player); }
                if (s_reqCopyStamp) { s_reqCopyStamp = false; CopyCurrentSelection(p, groundCenter); }
                if (s_reqPasteStamp) { s_reqPasteStamp = false; HandlePasteStamp(p, groundCenter, canEdit, why); }
                if (s_reqCycleStampBack) { s_reqCycleStampBack = false; HandleCycleStamp(p, -1); }
                if (s_reqCycleStamp) { s_reqCycleStamp = false; HandleCycleStamp(p, 1); }
                if (s_reqDumpMat) { s_reqDumpMat = false; DumpTerrainMaterial(groundCenter); }
                if (s_reqFreeCam) { s_reqFreeCam = false; ToggleFreeCamera(); }
            }

            // Stamp copy/paste (work outside normal apply flow)
            if (!blocked && active)
            {
                if (p.CopyStampKey.Value.IsDown())
                    CopyCurrentSelection(p, groundCenter);
                if (p.PasteStampKey.Value.IsDown())
                    HandlePasteStamp(p, groundCenter, canEdit, why);
                if (p.CycleStampKey.Value.IsDown())
                    HandleCycleStamp(p, 1);
                if (p.CycleStampBackKey.Value.IsDown())
                    HandleCycleStamp(p, -1);
                if (p.SlopeHeatmapKey.Value.IsDown())
                {
                    p.ShowSlopeHeatmap.Value = !p.ShowSlopeHeatmap.Value;
                    Flash(p.ShowSlopeHeatmap.Value ? "Mapa de pendiente: ON" : "Mapa de pendiente: OFF");
                }
                if (p.HeightHeatmapKey.Value.IsDown())
                {
                    p.ShowHeightHeatmap.Value = !p.ShowHeightHeatmap.Value;
                    Flash(p.ShowHeightHeatmap.Value ? "Mapa de altura: ON" : "Mapa de altura: OFF");
                }
            }

            TerrainOperation op = p.DefaultOperation.Value;

            if (!blocked && active && !Held(p.RadialKey.Value))
            {
                if (op == TerrainOperation.Ramp)
                {
                    if (p.ApplyKey.Value.IsDown())
                    {
                        if (canEdit)
                            HandleRampMark(p, player, groundCenter, hasAim);
                        else
                            Flash(why);
                    }
                }
                else if (op == TerrainOperation.CopyPaste)
                {
                    if (p.ApplyKey.Value.IsDown())
                        HandleCopySelectionPress(p, groundCenter);
                }
                else if (p.Shape.Value == TerrainShape.Line ||
                         p.Shape.Value == TerrainShape.Rect ||
                         p.Shape.Value == TerrainShape.Poly)
                {
                    if (p.ApplyKey.Value.IsDown())
                    {
                        if (canEdit)
                            _applying = HandleShapeMark(p, player, groundCenter);
                        else
                            Flash(why);
                    }
                }
                else if (Held(p.ApplyKey.Value) && !wasPending)
                {
                    if (canEdit)
                        _applying = TryApply(p, player, groundCenter);
                    else
                        Flash(why);
                }
            }

            if (active)
            {
                _aimY = groundCenter.y;
                _aimPoint = groundCenter;
                _targetTxt = TargetText(p, player, groundCenter);
                _slopeDeg = SlopeDeg(groundCenter);
                UpdatePersistentHud(player, p);

                // Level marker: the horizontal plane Flatten/Shore (or a fixed
                // eyedrop height) would produce, for circle/square brushes.
                _levelY = float.NaN;
                if (p.Shape.Value == TerrainShape.Circle || p.Shape.Value == TerrainShape.Square)
                {
                    if (p.DefaultOperation.Value == TerrainOperation.Shore)
                        _levelY = TerrainEngine.WaterLevel() + p.ShoreOffset.Value;
                    else if (p.DefaultOperation.Value == TerrainOperation.Flatten || p.UseFixedHeight.Value)
                        _levelY = p.UseFixedHeight.Value
                            ? p.FixedHeight.Value
                            : (Held(p.AimHeightModifierKey.Value)
                                ? GroundHeight(player.transform.position)
                                : groundCenter.y);
                }
            }

            if (active && p.ShowPreview.Value)
            {
                if (!_previewLogged)
                {
                    _previewLogged = true;
                    TerrainForgePlugin.Log?.LogInfo("TerrainPreview: drawing (first call).");
                }
                bool guides = p.ShowGuides.Value;
                float effRadius = EffectiveRadius(p);
                float fullRadius = effRadius * (1f - 0.5f * Mathf.Clamp01(p.Softness.Value / 100f));
                bool isRamp = p.DefaultOperation.Value == TerrainOperation.Ramp;
                bool pathMarking = isRamp && p.RampMode.Value == RampMode.Path && _rampPath.Count > 0;
                // The line footprint reuses the ramp corridor projection.
                bool lineMarking = !isRamp && p.Shape.Value == TerrainShape.Line && _lineHasA;
                bool rectMarking = !isRamp && p.Shape.Value == TerrainShape.Rect && _rectHasA;
                bool polyMarking = !isRamp && p.Shape.Value == TerrainShape.Poly && _polyPoints.Count > 0;
                Vector3[] polyPreview = null;
                if (polyMarking || pathMarking)
                {
                    List<Vector3> src = pathMarking ? _rampPath : _polyPoints;
                    polyPreview = new Vector3[src.Count + 1];
                    for (int k = 0; k < src.Count; k++)
                        polyPreview[k] = src[k];
                    polyPreview[src.Count] = groundCenter;
                }

                // Stamp paste preview overrides normal preview
                if (_pastePreview)
                {
                    _stampPreviewCenter = groundCenter;
                    // Rotation: Q/E or mouse wheel with Alt
                    bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                    float scroll = WheelDelta();
                    if (Mathf.Abs(scroll) > 0.01f)
                    {
                        if (alt)
                            _stampRotation += scroll * 15f;
                        else if (Held(p.AimHeightModifierKey.Value))
                            _stampScale = Mathf.Clamp(_stampScale + scroll * 0.1f, 0.25f, 4f);
                    }
                    if (Input.GetKeyDown(KeyCode.Q))
                        _stampRotation -= 15f;
                    if (Input.GetKeyDown(KeyCode.E))
                        _stampRotation += 15f;
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        _pastePreview = false;
                        _preview.Update(new PreviewState { ShowStamp = false });
                    }

                    var stamp = TerrainEngine.GetStamp();

                    _preview.Update(new PreviewState
                    {
                        ShowStamp = true,
                        Stamp = stamp,
                        StampTarget = _stampPreviewCenter,
                        StampRotation = _stampRotation,
                        StampScale = _stampScale,
                        Radius = effRadius,
                        Color = BrushColor(p).WithAlpha(0.95f),
                        Blocked = !canEdit,
                        WaterLevel = TerrainEngine.WaterLevel(),
                    });
                }
                else
                {
                    _preview.Update(new PreviewState
                    {
                        Center = groundCenter,
                        PlayerPos = player.transform.position,
                        Radius = effRadius,
                        FullRadius = guides ? fullRadius : 0f,
                        ReachRadius = guides ? p.RadiusMax.Value : 0f,
                        AimReach = guides ? MaxAimDistance : 0f,
                        Shape = p.Shape.Value,
                        Color = BrushColor(p).WithAlpha(0.95f),
                        IsRamp = isRamp,
                        HasRampA = isRamp
                            ? (p.RampMode.Value == RampMode.Path ? pathMarking : _rampHasA)
                            : _lineHasA,
                        RampA = isRamp
                            ? (p.RampMode.Value == RampMode.Path && pathMarking ? _rampPath[_rampPath.Count - 1] : _rampA)
                            : _lineA,
                        // Non-circular shapes never show the round footprint: only the
                        // marked outline (line corridor, rect, poly trail) is drawn.
                        SuppressCircles = pathMarking ||
                            isRamp ||
                            (!isRamp && (p.Shape.Value == TerrainShape.Line ||
                                         p.Shape.Value == TerrainShape.Rect ||
                                         p.Shape.Value == TerrainShape.Poly)) ||
                            _lineHasA || _rectHasA,
                        HasRect = rectMarking,
                        RectA = _rectA,
                        RectB = groundCenter,
                        PolyPreview = polyPreview,
                        SlopeDeg = _slopeDeg,
                        GuidesOn = guides,
                        LevelY = guides ? _levelY : float.NaN,
                        Blocked = equipped && !canEdit,
                        CopyHighlightStart = _clipFxStart,
                        Stamp = TerrainEngine.GetStamp(),
                        WaterLevel = TerrainEngine.WaterLevel(),
                    });
                }
            }
            else
            {
                if (_pastePreview)
                {
                    _pastePreview = false;
                    _preview.Update(new PreviewState { ShowStamp = false });
                }
                _preview.Hide();
            }
        }

        private void HandleRampMark(TerrainForgePlugin p, Player player, Vector3 point, bool hasAim)
        {
            Vector3 ground;
            if (hasAim)
            {
                ground = point;
            }
            else
            {
                Vector3 pos = player.transform.position;
                ground = new Vector3(pos.x, GroundHeight(pos), pos.z);
            }

            if (p.RampMode.Value == RampMode.Path)
            {
                HandleRampPath(p, ground);
                return;
            }

            bool fixedAngle = p.RampMode.Value == RampMode.FixedAngle;

            if (!_rampHasA)
            {
                _rampA = ground;
                _rampHasA = true;
                TerrainForgePlugin.Message(fixedAngle
                    ? Loc.F("msg.ramp.fixmark", p.SlopeAngle.Value.ToString("0"))
                    : Loc.T("msg.ramp.markA"));
                return;
            }

            Vector3 a = _rampA;
            _rampHasA = false;

            if ((ground - a).sqrMagnitude < 1f)
            {
                TerrainForgePlugin.Message(Loc.T("msg.ramp.close"));
                return;
            }

            var spec = new BrushSpec
            {
                Operation = TerrainOperation.Ramp,
                Shape = p.Shape.Value,
                Radius = EffectiveRadius(p),
                Softness = p.Softness.Value,
                MaxUp = EffectiveMaxUp(p),
                MaxDown = EffectiveMaxDown(p),
                HasRampLine = true,
                RampA = a,
                RampB = ground
            };

            float rlen = HorizDist(a, ground);
            float rrise = ground.y - a.y;
            string doneMsg = Loc.T("msg.ramp.doneAB") + string.Format(" ({0}, {1}, {2})",
                M(rlen), Rise(rrise), Deg(PathAngle(rlen, rrise)));
            if (fixedAngle)
            {
                // B sets direction + length; the height follows the fixed angle,
                // climbing or descending depending on where B sits vs A.
                float hA = GroundHeight(a);
                float dx = ground.x - a.x;
                float dz = ground.z - a.z;
                float dist = Mathf.Sqrt(dx * dx + dz * dz);
                float sign = GroundHeight(ground) >= hA ? 1f : -1f;
                float hB = hA + sign * dist * Mathf.Tan(p.SlopeAngle.Value * Mathf.Deg2Rad);
                spec.UseFixedHeights = true;
                spec.FixedHA = hA;
                spec.FixedHB = hB;
                doneMsg = Loc.F("msg.ramp.fixdone",
                    p.SlopeAngle.Value, sign >= 0f ? Loc.T("msg.ramp.up") : Loc.T("msg.ramp.down"),
                    Mathf.Abs(hB - hA), dist);
            }

            Vector3 mid = (a + ground) * 0.5f;
            if (TerrainEngine.Apply(mid, spec))
            {
                TerrainForgePlugin.Message(doneMsg);
                TerrainFx.Play(mid, true);
            }
            else
                TerrainForgePlugin.Message(Loc.T("msg.ramp.fail"));
        }

        /// <summary>
        /// Path ramps: each press adds a point (with its ground height); a
        /// double-press with 3+ points builds a smooth ramp along the curve.
        /// </summary>
        private void HandleRampPath(TerrainForgePlugin p, Vector3 point)
        {
            if (_rampPath.Count >= 3 && Time.time - _rampPathLast < 0.5f)
            {
                BrushSpec spec = new BrushSpec
                {
                    Operation = TerrainOperation.Ramp,
                    Radius = EffectiveRadius(p),
                    Softness = p.Softness.Value,
                    MaxUp = EffectiveMaxUp(p),
                    MaxDown = EffectiveMaxDown(p),
                    HasRampPath = true,
                    RampPath = _rampPath.ToArray(),
                    RampPathH = _rampPathH.ToArray()
                };

                Vector3 c = Vector3.zero;
                float total = 0f;
                for (int k = 0; k < _rampPath.Count; k++)
                {
                    c += _rampPath[k];
                    if (k > 0)
                        total += HorizDist(_rampPath[k - 1], _rampPath[k]);
                }
                c /= _rampPath.Count;
                float rise = _rampPathH[_rampPathH.Count - 1] - _rampPathH[0];
                int n = _rampPath.Count;
                ClearRampPath();

                bool ok = TerrainEngine.Apply(c, spec);
                TerrainForgePlugin.Message(ok
                    ? Loc.F("msg.path.done", n) + string.Format(" ({0}, {1})",
                        M(total), Rise(rise))
                    : Loc.T("msg.path.fail"));
                if (ok)
                    TerrainFx.Play(c, true);
                return;
            }

            if (_rampPath.Count == 0 ||
                (_rampPath[_rampPath.Count - 1] - point).sqrMagnitude > 0.25f)
            {
                _rampPath.Add(point);
                _rampPathH.Add(GroundHeight(point));
                TerrainForgePlugin.Message(Loc.F("msg.path.point", _rampPath.Count));
            }
            _rampPathLast = Time.time;
        }

        private void ClearShapeMarks()
        {
            _lineHasA = false;
            _rectHasA = false;
            _polyPoints.Clear();
        }

        private void ClearRampPath()
        {
            _rampPath.Clear();
            _rampPathH.Clear();
        }

        private BrushSpec BaseSpec(TerrainForgePlugin p, Player player, Vector3 center)
        {
            float playerGround = GroundHeight(player.transform.position);

            // Flatten follows the height under the crosshair by default so it
            // conforms to whatever you aim at (e.g. a ramp). Hold the modifier to
            // level to your own feet instead; a fixed eyedrop height wins over both.
            bool ownHeight = Held(p.AimHeightModifierKey.Value);

            return new BrushSpec
            {
                Operation = p.DefaultOperation.Value,
                Shape = p.Shape.Value,
                Paint = p.Paint.Value,
                Radius = EffectiveRadius(p),
                Softness = p.Softness.Value,
                Power = p.FalloffPower.Value,
                RaiseDelta = p.RaiseDelta.Value,
                TargetWorld = p.UseFixedHeight.Value ? p.FixedHeight.Value : (ownHeight ? playerGround : center.y),
                SnowClear = (p.DefaultOperation.Value == TerrainOperation.Snow ||
                    p.DefaultOperation.Value == TerrainOperation.Paint && p.Paint.Value == TerrainPaint.DeepSnow) &&
                    Held(p.AimHeightModifierKey.Value),
                PaintStrength = p.PaintStrength.Value,
                MaxUp = EffectiveMaxUp(p),
                MaxDown = EffectiveMaxDown(p),
                SmoothMode = p.SmoothMode.Value,
                Falloff = p.FalloffMode.Value,
                ShoreFalloff = p.ShoreFalloffMode.Value,
                ShoreTarget = TerrainEngine.WaterLevel() + p.ShoreOffset.Value
            };
        }

        private bool TryApply(TerrainForgePlugin p, Player player, Vector3 center)
        {
            if (Time.time - _lastApply < ApplyInterval)
                return _applying;

            BrushSpec spec = BaseSpec(p, player, center);

            _lastApply = Time.time;
            bool ok = TerrainEngine.Apply(center, spec);
            if (ok)
                TerrainFx.Play(center, false);
            return ok;
        }

        private void HandleDirectOperationKeys(TerrainForgePlugin p)
        {
            if (p.OpFlattenKey.Value.IsDown()) SetOp(p, TerrainOperation.Flatten);
            else if (p.OpRaiseKey.Value.IsDown()) SetOp(p, TerrainOperation.Raise);
            else if (p.OpLowerKey.Value.IsDown()) SetOp(p, TerrainOperation.Lower);
            else if (p.OpSmoothKey.Value.IsDown()) SetOp(p, TerrainOperation.Smooth);
            else if (p.OpPaintKey.Value.IsDown()) SetOp(p, TerrainOperation.Paint);
            else if (p.OpRestoreKey.Value.IsDown()) SetOp(p, TerrainOperation.Restore);
            else if (p.OpRampKey.Value.IsDown()) SetOp(p, TerrainOperation.Ramp);
            else if (p.OpShoreKey.Value.IsDown()) SetOp(p, TerrainOperation.Shore);
            else if (p.OpSnowKey.Value.IsDown()) { SetOp(p, TerrainOperation.Paint); p.Paint.Value = TerrainPaint.DeepSnow; }
        }

        private void SetOp(TerrainForgePlugin p, TerrainOperation op)
        {
            if (p.DefaultOperation.Value != op)
            {
                p.DefaultOperation.Value = op;
                _rampHasA = false;
                ClearShapeMarks();
                ClearRampPath();
                Flash(Loc.F("hud.flash.falloff", Loc.OpName(op)));
            }
        }

        private void HandleCycleProfile(TerrainForgePlugin p)
        {
            if (p.CycleProfileKey.Value.IsDown())
            {
                TerrainKeybinds.CycleNextProfile();
            }
        }

        private void HandleCopyStamp(TerrainForgePlugin p, Vector3 center)
        {
            Player player = Player.m_localPlayer;
            if (player == null)
                return;
            ItemDrop.ItemData weapon = GetHeldItem(player);
            if (!IsStampToolEquipped(p, weapon))
            {
                Flash(Loc.T("flash.stamp.noTool"));
                return;
            }

            bool rectSel = _rectHasA;
            var stamp = TerrainEngine.CopyStamp(
                center,
                EffectiveRadius(p),
                rectSel ? TerrainShape.Rect : p.Shape.Value,
                rectSel ? (Vector3?)_rectA : null,
                rectSel ? (Vector3?)_aimPoint : null,
                (!rectSel && _polyPoints.Count >= 3) ? _polyPoints.ToArray() : null,
                !rectSel && _lineHasA,
                (!rectSel && _lineHasA) ? (Vector3?)_lineA : null,
                (!rectSel && _lineHasA) ? (Vector3?)_aimPoint : null);

            if (stamp != null && rectSel)
                _rectHasA = false;

            if (stamp != null)
            {
                Flash(Loc.F("flash.stamp.copied", stamp.Tiles.Count));
                TerrainForgePlugin.Message(stamp.Label);
                // Remember the copy time so the preview can highlight the region briefly.
                _clipFxStart = Time.time;
            }
            else
            {
                Flash(Loc.T("flash.stamp.empty"));
            }
        }

        private void CopyCurrentSelection(TerrainForgePlugin p, Vector3 point)
        {
            HandleCopySelectionPress(p, point);
        }

        private void HandleCopySelectionPress(TerrainForgePlugin p, Vector3 point)
        {
            switch (p.Shape.Value)
            {
                case TerrainShape.Rect:
                    if (!_rectHasA)
                    {
                        _rectA = point;
                        _rectHasA = true;
                        TerrainForgePlugin.Message(Loc.T("msg.rect.markA"));
                        return;
                    }
                    HandleCopyStamp(p, point);
                    return;
                case TerrainShape.Line:
                    if (!_lineHasA)
                    {
                        _lineA = point;
                        _lineHasA = true;
                        TerrainForgePlugin.Message(Loc.T("msg.line.markA"));
                        return;
                    }
                    if ((point - _lineA).sqrMagnitude < 1f)
                    {
                        TerrainForgePlugin.Message(Loc.T("msg.line.close"));
                        return;
                    }
                    HandleCopyStamp(p, point);
                    _lineHasA = false;
                    return;
                case TerrainShape.Poly:
                    if (_polyPoints.Count >= 3 && Time.time - _polyLastClick < 0.5f)
                    {
                        HandleCopyStamp(p, point);
                        _polyPoints.Clear();
                        return;
                    }
                    if (_polyPoints.Count == 0 || (_polyPoints[_polyPoints.Count - 1] - point).sqrMagnitude > 0.25f)
                    {
                        _polyPoints.Add(point);
                        TerrainForgePlugin.Message(Loc.F("msg.poly.point", _polyPoints.Count));
                    }
                    _polyLastClick = Time.time;
                    return;
                default:
                    HandleCopyStamp(p, point);
                    return;
            }
        }

        private void HandlePasteStamp(TerrainForgePlugin p, Vector3 center, bool canEdit, string why)
        {
            Player player = Player.m_localPlayer;
            if (player == null)
                return;
            ItemDrop.ItemData weapon = GetHeldItem(player);
            bool stampToolOk = IsStampToolEquipped(p, weapon);

            if (_pastePreview)
            {
                // Confirm paste
                if (canEdit && stampToolOk)
                {
                    if (TerrainEngine.PasteStamp(_stampPreviewCenter, out string msg, _stampRotation, _stampScale))
                    {
                        Flash(Loc.T("flash.stamp.pasted"));
                        TerrainForgePlugin.Message(msg);
                        TerrainFx.Play(_stampPreviewCenter, true);
                    }
                    else
                    {
                        Flash(msg);
                    }
                }
                else
                {
                    Flash(stampToolOk ? why : Loc.T("flash.stamp.noTool"));
                }
                _pastePreview = false;
                _preview.Update(new PreviewState { ShowStamp = false });
                return;
            }

            // Start preview
            var stamp = TerrainEngine.GetStamp();
            if (stamp == null || stamp.Tiles.Count == 0)
            {
                Flash(Loc.T("flash.stamp.empty"));
                return;
            }

            if (!stampToolOk)
            {
                Flash(Loc.T("flash.stamp.noTool"));
                return;
            }

            _pastePreview = true;
            _stampPreviewCenter = center;
            _stampRotation = 0f;
            _stampScale = 1f;

            _preview.Update(new PreviewState
            {
                ShowStamp = true,
                Stamp = stamp,
                StampTarget = center,
                StampRotation = 0f,
                StampScale = 1f,
                Radius = EffectiveRadius(p),
                Color = BrushColor(p).WithAlpha(0.95f),
                Blocked = !canEdit,
                WaterLevel = TerrainEngine.WaterLevel(),
            });

            Flash(Loc.T("flash.stamp.preview"));
        }

        private void HandleCycleStamp(TerrainForgePlugin p, int delta)
        {
            var stamp = TerrainEngine.CycleClipboard(delta);
            if (stamp != null)
            {
                var clipboard = TerrainEngine.GetClipboard();
                int idx = clipboard.Count - 1 - clipboard.ToList().IndexOf(stamp);
                Flash(Loc.F("flash.stamp.cycled", stamp.Label + " [" + (idx + 1) + "/" + clipboard.Count + "]"));
                // Update preview if we're in paste preview mode
                if (_pastePreview)
                {
                    _preview.Update(new PreviewState
                    {
                        ShowStamp = true,
                        Stamp = stamp,
                        StampTarget = _stampPreviewCenter,
                        StampRotation = _stampRotation,
                        StampScale = _stampScale,
                        Radius = EffectiveRadius(p),
                        Color = BrushColor(p).WithAlpha(0.95f),
                        Blocked = false,
                        WaterLevel = TerrainEngine.WaterLevel(),
                    });
                }
            }
        }

        /// <summary>
        /// Two-press marking for Line / Rect / Poly shapes: the first press fixes
        /// the start (or adds a polygon vertex), the second applies the operation
        /// over the footprint. Polygons close on a double-press with 3+ points.
        /// </summary>
        private bool HandleShapeMark(TerrainForgePlugin p, Player player, Vector3 point)
        {
            switch (p.Shape.Value)
            {
                case TerrainShape.Line:
                {
                    if (!_lineHasA)
                    {
                        _lineA = point;
                        _lineHasA = true;
                        TerrainForgePlugin.Message(Loc.T("msg.line.markA"));
                        return false;
                    }
                    Vector3 a = _lineA;
                    _lineHasA = false;
                    if ((point - a).sqrMagnitude < 1f)
                    {
                        TerrainForgePlugin.Message(Loc.T("msg.line.close"));
                        return false;
                    }
                    BrushSpec spec = BaseSpec(p, player, point);
                    spec.HasLine = true;
                    spec.LineA = a;
                    spec.LineB = point;
                    Vector3 mid = (a + point) * 0.5f;
                    bool ok = TerrainEngine.Apply(mid, spec);
                    string msg = ok
                        ? Loc.T("msg.line.done") + " (" + M(HorizDist(a, point)) + ")"
                        : Loc.T("msg.line.fail");
                    TerrainForgePlugin.Message(msg);
                    if (ok)
                        TerrainFx.Play(mid, true);
                    return ok;
                }
                case TerrainShape.Rect:
                {
                    if (!_rectHasA)
                    {
                        _rectA = point;
                        _rectHasA = true;
                        TerrainForgePlugin.Message(Loc.T("msg.rect.markA"));
                        return false;
                    }
                    Vector3 ra = _rectA;
                    _rectHasA = false;
                    float dx = Mathf.Abs(point.x - ra.x);
                    float dz = Mathf.Abs(point.z - ra.z);
                    if (dx < 1f || dz < 1f)
                    {
                        TerrainForgePlugin.Message(Loc.T("msg.rect.small"));
                        return false;
                    }
                    BrushSpec spec = BaseSpec(p, player, point);
                    spec.HasRect = true;
                    spec.RectA = ra;
                    spec.RectB = point;
                    Vector3 mid = (ra + point) * 0.5f;
                    bool ok = TerrainEngine.Apply(mid, spec);
                    string msg = ok
                        ? Loc.T("msg.rect.done") + string.Format(" ({0}×{1} m, {2} m²)",
                            dx.ToString("0.#"), dz.ToString("0.#"), (dx * dz).ToString("0.#"))
                        : Loc.T("msg.rect.fail");
                    TerrainForgePlugin.Message(msg);
                    if (ok)
                        TerrainFx.Play(mid, true);
                    return ok;
                }
                case TerrainShape.Poly:
                {
                    if (_polyPoints.Count >= 3 && Time.time - _polyLastClick < 0.5f)
                    {
                        BrushSpec spec = BaseSpec(p, player, point);
                        spec.HasPoly = true;
                        spec.PolyPoints = _polyPoints.ToArray();
                        Vector3 c = Vector3.zero;
                        for (int k = 0; k < _polyPoints.Count; k++)
                            c += _polyPoints[k];
                        c /= _polyPoints.Count;
                        int n = _polyPoints.Count;
                        float pper = ChainLength(_polyPoints, Vector3.zero, true);
                        float parea = PolyArea(_polyPoints, Vector3.zero, false);
                        _polyPoints.Clear();
                        bool ok = TerrainEngine.Apply(c, spec);
                        TerrainForgePlugin.Message(ok
                            ? Loc.F("msg.poly.done", n) + string.Format(" ({0}, {1} m²)", M(pper), parea.ToString("0.#"))
                            : Loc.T("msg.poly.fail"));
                        if (ok)
                            TerrainFx.Play(c, true);
                        return ok;
                    }
                    if (_polyPoints.Count == 0 ||
                        (_polyPoints[_polyPoints.Count - 1] - point).sqrMagnitude > 0.25f)
                    {
                        _polyPoints.Add(point);
                        TerrainForgePlugin.Message(Loc.F("msg.poly.point", _polyPoints.Count));
                    }
                    _polyLastClick = Time.time;
                    return false;
                }
                default:
                    return false;
            }
        }

        private void HandleRadius(TerrainForgePlugin p)
        {
            // Alt + radius controls adjust the fixed ramp angle instead of the
            // radius when a fixed-angle ramp is selected.
            bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool angleMode = alt &&
                             p.DefaultOperation.Value == TerrainOperation.Ramp &&
                             p.RampMode.Value == RampMode.FixedAngle;

            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool fine = Held(p.AimHeightModifierKey.Value);

            // In normal mode the wheel belongs to the brush. In the free-fly camera
            // the plain wheel is the game's fly speed, so only take it when a modifier
            // asks for a brush change (Ctrl/Shift, adjust mode, or Alt on a ramp angle).
            bool inFly = GameCamera.InFreeFly();
            bool wheelOurs = !inFly || _adjustMode || angleMode || fine || ctrl;
            FreeFlyWheelBusy = inFly && wheelOurs;

            float scroll = WheelDelta();
            if (wheelOurs && Mathf.Abs(scroll) > 0.01f)
            {
                if (_adjustMode)
                    AdjustRadius(p, Mathf.Sign(scroll) * p.RadiusStep.Value);
                else if (angleMode)
                    AdjustAngle(p, Mathf.Sign(scroll) * 2.5f);
                else if (fine)
                    AdjustRadius(p, Mathf.Sign(scroll) * 0.1f);
                else if (ctrl)
                    AdjustRadius(p, Mathf.Sign(scroll) * p.RadiusStep.Value);
            }

            if (p.RadiusUpKey.Value.IsDown())
            {
                if (angleMode)
                    AdjustAngle(p, 2.5f);
                else if (fine)
                    AdjustRadius(p, 0.1f);
                else
                    AdjustRadius(p, p.RadiusStep.Value);
            }
            if (p.RadiusDownKey.Value.IsDown())
            {
                if (angleMode)
                    AdjustAngle(p, -2.5f);
                else if (fine)
                    AdjustRadius(p, -0.1f);
                else
                    AdjustRadius(p, -p.RadiusStep.Value);
            }
        }

        private void AdjustAngle(TerrainForgePlugin p, float delta)
        {
            float v = Mathf.Clamp(p.SlopeAngle.Value + delta, 5f, 60f);
            p.SlopeAngle.Value = v;
            Flash(Loc.F("flash.angle", v.ToString("0")));
        }

        private static void AdjustRadius(TerrainForgePlugin p, float delta)
        {
            float min = p.RadiusMin.Value;
            float max = Mathf.Max(min, p.RadiusMax.Value);
            float v = Mathf.Clamp(p.Radius.Value + delta, min, max);
            p.Radius.Value = v;
        }

        private static void CycleOperation(TerrainForgePlugin p)
        {
            TerrainOperation[] ops = { TerrainOperation.Flatten, TerrainOperation.Raise, TerrainOperation.Lower,
                TerrainOperation.Smooth, TerrainOperation.Paint, TerrainOperation.Restore,
                TerrainOperation.Ramp, TerrainOperation.Shore, TerrainOperation.CopyPaste };
            int index = System.Array.IndexOf(ops, p.DefaultOperation.Value);
            p.DefaultOperation.Value = ops[(index + 1 + ops.Length) % ops.Length];
        }

        private static void CycleShape(TerrainForgePlugin p)
        {
            p.Shape.Value = (TerrainShape)(((int)p.Shape.Value + 1) % 5);
        }

        private static float EffectiveRadius(TerrainForgePlugin p)
        {
            float cap = p.RadiusCap != null ? p.RadiusCap.Value : 0f;
            return cap > 0.01f ? Mathf.Min(p.Radius.Value, cap) : p.Radius.Value;
        }

        private static float EffectiveMaxUp(TerrainForgePlugin p)
        {
            float v = p.UnlockDeltaLimit.Value ? p.MaxUpDelta.Value : 8f;
            float cap = p.DeltaCap != null ? p.DeltaCap.Value : 0f;
            return cap > 0.01f ? Mathf.Min(v, cap) : v;
        }

        private static float EffectiveMaxDown(TerrainForgePlugin p)
        {
            float v = p.UnlockDeltaLimit.Value ? p.MaxDownDelta.Value : 8f;
            float cap = p.DeltaCap != null ? p.DeltaCap.Value : 0f;
            return cap > 0.01f ? Mathf.Min(v, cap) : v;
        }

        private static bool WardAllows(TerrainForgePlugin p, Vector3 center, float radius, out string reason)
        {
            reason = null;
            if (p.RespectWards == null || !p.RespectWards.Value)
                return true;
            bool ok = true;
            try { ok = PrivateArea.CheckAccess(center, radius, false); }
            catch { ok = true; }
            if (!ok)
                reason = Loc.T("srv.ward");
            return ok;
        }

        private static bool ServerAllows(TerrainForgePlugin p, out string reason)
        {
            reason = null;
            if (p.AllowTerrainEdits == null || p.AllowTerrainEdits.Value)
            {
                if (p.AdminOnlyEdits != null && p.AdminOnlyEdits.Value)
                {
                    bool admin = false;
                    try { admin = ZNet.instance != null && ZNet.instance.LocalPlayerIsAdminOrHost(); }
                    catch { admin = false; }
                    if (!admin)
                    {
                        reason = Loc.T("srv.adminonly");
                        return false;
                    }
                }
                return true;
            }
            reason = Loc.T("srv.blocked");
            return false;
        }

        private void DoUndo()
        {
            if (!_serverAllowed || !_wardOk)
            {
                Flash(!_serverAllowed ? _serverWhy : _wardWhy);
                return;
            }
            if (TerrainEngine.Undo(out string label))
                Flash(string.IsNullOrEmpty(label)
                    ? Loc.F("flash.undo", TerrainEngine.UndoCount)
                    : Loc.F("flash.undo.op", label, TerrainEngine.UndoCount));
            else
                Flash(Loc.T("flash.nothing.undo"));
        }

        private void DoRedo()
        {
            if (!_serverAllowed || !_wardOk)
            {
                Flash(!_serverAllowed ? _serverWhy : _wardWhy);
                return;
            }
            if (TerrainEngine.Redo(out string label))
                Flash(string.IsNullOrEmpty(label)
                    ? Loc.F("flash.redo", TerrainEngine.RedoCount)
                    : Loc.F("flash.redo.op", label, TerrainEngine.RedoCount));
            else
                Flash(Loc.T("flash.nothing.redo"));
        }

        private void Flash(string msg)
        {
            _flashMsg = msg;
            _flashUntil = Time.time + 2.5f;
        }

        // Diagnostic: log the ground material's shader and texture slots so we can see
        // what the terrain shader exposes (feasibility check for non-biome textures).
        private static void DumpTerrainMaterial(Vector3 center)
        {
            var hits = new Collider[16];
            int n = Physics.OverlapSphereNonAlloc(center + Vector3.up * 1.5f, 3f, hits, ~0, QueryTriggerInteraction.Collide);
            for (int i = 0; i < n; i++)
            {
                Heightmap hm = hits[i] != null ? hits[i].GetComponentInParent<Heightmap>() : null;
                if (hm == null || hm.m_material == null)
                    continue;

                Material m = hm.m_material;
                string[] props = m.GetTexturePropertyNames();
                var sb = new System.Text.StringBuilder();
                foreach (string prop in props)
                {
                    Texture t = m.GetTexture(prop);
                    sb.Append(prop).Append('=').Append(t != null ? t.name : "null").Append("; ");
                }
                TerrainForgePlugin.Log?.LogInfo("TerrainForge material dump: shader=" + m.shader.name +
                    " biome=" + hm.GetBiome(center) + " tex=" + props.Length + " [" + sb + "]");
                TerrainForgePlugin.Message("Material dump -> log (" + props.Length + " tex)");
                return;
            }
            TerrainForgePlugin.Message(Loc.T("act.diagfail"));
        }

        private void ToggleEyedrop(Vector3 center)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;
            if (p.UseFixedHeight.Value)
            {
                p.UseFixedHeight.Value = false;
                Flash(Loc.T("flash.released"));
            }
            else
            {
                p.FixedHeight.Value = Mathf.Clamp(center.y, -10f, 150f);
                p.UseFixedHeight.Value = true;
                Flash(Loc.F("flash.picked", p.FixedHeight.Value.ToString("0.0")));
            }
        }

        private string TargetText(TerrainForgePlugin p, Player player, Vector3 center)
        {
            switch (p.DefaultOperation.Value)
            {
                case TerrainOperation.Flatten:
                {
                    if (p.UseFixedHeight.Value)
                        return string.Format("{0} {1:0.0} m", Loc.T("hud.fixed"), p.FixedHeight.Value);
                    bool own = Held(p.AimHeightModifierKey.Value);
                    float t = own ? GroundHeight(player.transform.position) : center.y;
                    return string.Format("{0} {1:0.0} m", Loc.T("hud.target"), t);
                }
                case TerrainOperation.Raise:
                    return "+" + p.RaiseDelta.Value.ToString("0.#") + " " + Loc.T("hud.perapp");
                case TerrainOperation.Lower:
                    return "-" + p.RaiseDelta.Value.ToString("0.#") + " " + Loc.T("hud.perapp");
                case TerrainOperation.Smooth:
                    switch (p.SmoothMode.Value)
                    {
                        case SmoothMode.Bumps: return Loc.T("hud.bumps");
                        case SmoothMode.Dips: return Loc.T("hud.dips");
                        default: return Loc.T("hud.avg");
                    }
                case TerrainOperation.Shore:
                    return string.Format("{0} {1:0.0} m", Loc.T("hud.shore"),
                        TerrainEngine.WaterLevel() + p.ShoreOffset.Value);
                case TerrainOperation.Paint:
                    if (p.Paint.Value == TerrainPaint.DeepSnow)
                        return Loc.T(Held(p.AimHeightModifierKey.Value) ? "hud.snow.clear" : "hud.snow.put");
                    return p.Paint.Value + " " + (p.PaintStrength.Value * 100f).ToString("0") + "%";
                case TerrainOperation.CopyPaste:
                    return Loc.T("hud.stampReady");
                case TerrainOperation.Restore:
                    return Loc.T("hud.original");
                case TerrainOperation.Ramp:
                    if (p.RampMode.Value == RampMode.Path)
                        return _rampPath.Count > 0
                            ? string.Format("{0} {1}", Loc.T("hud.path"), M(ChainLength(_rampPath, center, false)))
                            : Loc.T("hud.path.idle");
                    if (p.RampMode.Value == RampMode.FixedAngle)
                        return _rampHasA
                            ? Loc.F("hud.ramp.fixed.mark", p.SlopeAngle.Value) +
                              " (" + M(HorizDist(_rampA, center)) + ")"
                            : Loc.F("hud.ramp.fixed.idle", p.SlopeAngle.Value);
                    if (_rampHasA)
                    {
                        float len = HorizDist(_rampA, center);
                        float rise = center.y - GroundHeight(_rampA);
                        return string.Format("A {0:0.0} → B {1:0.0} m ({2}, {3})",
                            GroundHeight(_rampA), center.y, M(len), Deg(PathAngle(len, rise)));
                    }
                    return Loc.T("hud.markAB");
                default:
                    return "";
            }
        }

        public static float SlopeDeg(Vector3 c)
        {
            const float e = 1f;
            float hx = GroundHeight(c + new Vector3(e, 0f, 0f)) - GroundHeight(c - new Vector3(e, 0f, 0f));
            float hz = GroundHeight(c + new Vector3(0f, 0f, e)) - GroundHeight(c - new Vector3(0f, 0f, e));
            float grad = Mathf.Sqrt(hx * hx + hz * hz) / (2f * e);
            return Mathf.Atan(grad) * Mathf.Rad2Deg;
        }

        // ---- live measurements (language-neutral numbers) ----------------------

        private static float HorizDist(Vector3 a, Vector3 b)
        {
            float dx = b.x - a.x, dz = b.z - a.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        private static string M(float v)
        {
            return v.ToString("0.#") + " m";
        }

        private static string Rise(float dh)
        {
            return (dh >= 0f ? "+" : "") + dh.ToString("0.0") + " m";
        }

        private static string Deg(float ang)
        {
            return ang.ToString("0") + "°";
        }

        private static float PathAngle(float dist, float dh)
        {
            return Mathf.Atan2(Mathf.Abs(dh), Mathf.Max(dist, 0.001f)) * Mathf.Rad2Deg;
        }

        private static float ChainLength(List<Vector3> pts, Vector3 end, bool closed)
        {
            if (pts.Count == 0)
                return 0f;
            float total = 0f;
            for (int i = 1; i < pts.Count; i++)
                total += HorizDist(pts[i - 1], pts[i]);
            total += closed ? HorizDist(pts[pts.Count - 1], pts[0]) : HorizDist(pts[pts.Count - 1], end);
            return total;
        }

        private static float PolyArea(List<Vector3> pts, Vector3 extra, bool useExtra)
        {
            int n = pts.Count + (useExtra ? 1 : 0);
            if (n < 3)
                return 0f;
            double a = 0.0;
            for (int i = 0; i < n; i++)
            {
                Vector3 p1 = PtAt(pts, extra, useExtra, i);
                Vector3 p2 = PtAt(pts, extra, useExtra, (i + 1) % n);
                a += (double)p1.x * p2.z - (double)p2.x * p1.z;
            }
            return (float)(System.Math.Abs(a) * 0.5);
        }

        private static Vector3 PtAt(List<Vector3> pts, Vector3 extra, bool useExtra, int i)
        {
            if (useExtra && i == pts.Count)
                return extra;
            return pts[i];
        }

        private static void CyclePaint(TerrainForgePlugin p)
        {
            p.Paint.Value = (TerrainPaint)(((int)p.Paint.Value + 1) % 6);
        }

        private static float GroundHeight(Vector3 pos)
        {
            if (ZoneSystem.instance != null)
                return ZoneSystem.instance.GetGroundHeight(pos);
            return pos.y;
        }

        // GetCurrentWeapon() only returns actual weapons (IsWeapon()); a Tool-type
        // item such as our rod or the vanilla Hammer lives in the hand slots but is
        // NOT a weapon, so it must be read from RightItem/LeftItem directly.
        internal static ItemDrop.ItemData GetHeldItem(Player player)
        {
            if (player == null)
                return null;

            try
            {
                ItemDrop.ItemData right = player.RightItem;
                if (right != null)
                    return right;

                ItemDrop.ItemData left = player.LeftItem;
                if (left != null)
                    return left;
            }
            catch { /* fall through to weapon fallback */ }

            try { return player.GetCurrentWeapon(); }
            catch { return null; }
        }

        private static bool TryGetAim(out Vector3 point)
        {
            point = default(Vector3);

            Ray ray;
            if (!TryGetRay(out ray))
                return false;

            if (Physics.Raycast(ray, out RaycastHit hit, AimDistance, ~0, QueryTriggerInteraction.Ignore) &&
                hit.collider != null && hit.collider.GetComponentInParent<Player>() == null)
            {
                if (hit.collider.GetComponentInParent<Heightmap>() != null)
                {
                    point = hit.point;
                    return true;
                }

                float gy = GroundHeight(hit.point);
                if (Mathf.Abs(hit.point.y - gy) < 3f)
                {
                    point = new Vector3(hit.point.x, gy, hit.point.z);
                    return true;
                }
            }

            // No usable collider hit (open air or water surface). March the crosshair
            // ray against the terrain heightmap so the brush tracks the cursor even
            // when a water body has no collider.
            return RaymarchGround(ray, out point);
        }

        private static bool TryGetRay(out Ray ray)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                return true;
            }

            if (GameCamera.instance != null)
            {
                Transform t = GameCamera.instance.transform;
                ray = new Ray(t.position, t.forward);
                return true;
            }

            ray = default(Ray);
            return false;
        }

        private static bool RaymarchGround(Ray ray, out Vector3 point)
        {
            point = default(Vector3);

            float step = Mathf.Max(0.75f, AimDistance * 0.05f);
            float maxDist = AimDistance * 2f;

            for (float dist = step; dist <= maxDist; dist += step)
            {
                Vector3 p = ray.origin + ray.direction * dist;
                if (p.y > GroundHeight(p))
                    continue;

                float lo = dist - step;
                float hi = dist;
                for (int i = 0; i < 8; i++)
                {
                    float mid = (lo + hi) * 0.5f;
                    Vector3 pm = ray.origin + ray.direction * mid;
                    if (pm.y <= GroundHeight(pm))
                        hi = mid;
                    else
                        lo = mid;
                }

                Vector3 fp = ray.origin + ray.direction * hi;
                point = new Vector3(fp.x, GroundHeight(fp), fp.z);
                return true;
            }

            return false;
        }

        internal static bool IsToolEquipped(TerrainForgePlugin p, ItemDrop.ItemData weapon)
        {
            switch (p.RequiredTool.Value)
            {
                case ToolRequirement.Pickaxe:
                    return IsPickaxe(weapon);

                case ToolRequirement.TerrainForgeTool:
                    // If the custom item failed to register, stay usable instead of locking out.
                    if (!TerrainTool.Registered)
                        return true;
                    return TerrainTool.IsEquipped(weapon);

                default:
                    return true;
            }
        }

        internal static bool IsStampToolEquipped(TerrainForgePlugin p, ItemDrop.ItemData weapon)
        {
            ToolRequirement req;
            switch (p.StampToolMode.Value)
            {
                case StampToolMode.Any:
                    return true;
                case StampToolMode.Pickaxe:
                    req = ToolRequirement.Pickaxe;
                    break;
                case StampToolMode.TerrainForgeTool:
                    req = ToolRequirement.TerrainForgeTool;
                    break;
                case StampToolMode.MainTool:
                default:
                    req = p.RequiredTool.Value;
                    break;
            }
            switch (req)
            {
                case ToolRequirement.Pickaxe:
                    return IsPickaxe(weapon);
                case ToolRequirement.TerrainForgeTool:
                    if (!TerrainTool.Registered)
                        return true;
                    return TerrainTool.IsEquipped(weapon);
                default:
                    return true;
            }
        }

        private static bool IsPickaxe(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null)
                return false;

            try
            {
                return item.m_shared.m_skillType == Skills.SkillType.Pickaxes;
            }
            catch { /* fall through */ }

            try
            {
                return item.GetDamage().m_pickaxe > 0f;
            }
            catch
            {
                return false;
            }
        }

        internal static bool Held(KeyboardShortcut shortcut)
        {
            KeyCode main = shortcut.MainKey;
            if (main == KeyCode.None)
                return false;

            bool down;
            switch (main)
            {
                case KeyCode.Mouse0: down = Input.GetMouseButton(0); break;
                case KeyCode.Mouse1: down = Input.GetMouseButton(1); break;
                case KeyCode.Mouse2: down = Input.GetMouseButton(2); break;
                default: down = Input.GetKey(main); break;
            }

            if (!down)
                return false;

            var mods = shortcut.Modifiers;
            if (mods != null)
            {
                foreach (KeyCode m in mods)
                    if (!Input.GetKey(m))
                        return false;
            }
            return true;
        }

        internal static bool Pressed(KeyboardShortcut shortcut)
        {
            KeyCode main = shortcut.MainKey;
            if (main == KeyCode.None) return false;
            bool down = main == KeyCode.Mouse0 ? Input.GetMouseButtonDown(0)
                : main == KeyCode.Mouse1 ? Input.GetMouseButtonDown(1)
                : main == KeyCode.Mouse2 ? Input.GetMouseButtonDown(2)
                : Input.GetKeyDown(main);
            if (!down) return false;
            var mods = shortcut.Modifiers;
            if (mods != null)
                foreach (KeyCode m in mods)
                    if (!Input.GetKey(m)) return false;
            return true;
        }

        internal static Color BrushColor(TerrainForgePlugin p)
        {
            bool snowClear = (p.DefaultOperation.Value == TerrainOperation.Snow ||
                p.DefaultOperation.Value == TerrainOperation.Paint && p.Paint.Value == TerrainPaint.DeepSnow) &&
                Held(p.AimHeightModifierKey.Value);
            return OpColor(p.DefaultOperation.Value, p.Paint.Value, snowClear);
        }

        internal static Color OpColor(TerrainOperation op, TerrainPaint paint, bool snowClear)
        {
            switch (op)
            {
                case TerrainOperation.Raise: return new Color(0.50f, 0.82f, 1.00f);
                case TerrainOperation.Lower: return new Color(1.00f, 0.60f, 0.30f);
                case TerrainOperation.Smooth: return new Color(0.40f, 0.92f, 0.92f);
                case TerrainOperation.Restore: return new Color(0.90f, 0.45f, 0.95f);
                case TerrainOperation.Ramp: return new Color(0.98f, 0.78f, 0.40f);
                case TerrainOperation.Shore: return new Color(0.93f, 0.87f, 0.66f);
                case TerrainOperation.Snow: return snowClear
                    ? new Color(0.55f, 0.55f, 0.60f) : new Color(0.90f, 0.95f, 1.00f);
                case TerrainOperation.Paint: return PaintColor(paint);
                default: return new Color(0.45f, 0.92f, 0.50f);
            }
        }

        private static Color PaintColor(TerrainPaint paint)
        {
            switch (paint)
            {
                case TerrainPaint.Cultivated: return new Color(0.50f, 0.85f, 0.30f);
                case TerrainPaint.Paved: return new Color(0.40f, 0.50f, 0.95f);
                case TerrainPaint.ClearVegetation: return new Color(0.95f, 0.90f, 0.30f);
                case TerrainPaint.DeepSnow: return new Color(0.90f, 0.95f, 1.00f);
                case TerrainPaint.Reset: return new Color(0.80f, 0.35f, 0.85f);
                default: return new Color(0.62f, 0.42f, 0.22f);
            }
        }

        // ---- status HUD ----------------------------------------------------------

        private void OnGUI()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || !p.Enabled.Value || TerrainKeybinds.MenuOpen)
                return;

            Player player = Player.m_localPlayer;
            if (player == null)
                return;

            bool freeCam = GameCamera.InFreeFly();
            if (!IsToolEquipped(p, GetHeldItem(player)) && !freeCam)
                return;

            EnsureHudStyle();

            // The free-fly mode is self-explanatory and must be visible even when
            // the tool is not equipped (the game freezes the player and may hide
            // the held item), so the banner is drawn before anything else.
            if (freeCam)
                DrawFreeCamBanner();
            DrawBrushHud(p, player);
            DrawAimBadge(p);
        }

        private void DrawBrushHud(TerrainForgePlugin p, Player player)
        {
            const float margin = 18f;
            float width = Mathf.Min(370f, Screen.width - margin * 2f);
            float x = Screen.width - width - margin;
            float bottom = Screen.height - margin;
            var clipboard = TerrainEngine.GetClipboard();
            bool hasStamp = clipboard.Count > 0;
            bool showMode = p.DefaultOperation.Value == TerrainOperation.Ramp ||
                p.DefaultOperation.Value == TerrainOperation.Paint || p.DefaultOperation.Value == TerrainOperation.CopyPaste;
            bool showContext = _adjustMode || !_serverAllowed || !_wardOk ||
                (!string.IsNullOrEmpty(_flashMsg) && Time.time < _flashUntil) ||
                _pastePreview || hasStamp || p.DefaultOperation.Value == TerrainOperation.Ramp &&
                    (p.RampMode.Value == RampMode.Path ? _rampPath.Count > 0 : _rampHasA) ||
                p.DefaultOperation.Value != TerrainOperation.Ramp &&
                    (p.Shape.Value == TerrainShape.Line && _lineHasA || p.Shape.Value == TerrainShape.Rect && _rectHasA ||
                     p.Shape.Value == TerrainShape.Poly && _polyPoints.Count > 0);
            float height = 126f + (showMode ? 22f : 0f) + (showContext ? 24f : 0f);
            var panel = new Rect(x, bottom - height, width, height);
            DrawHudSurface(panel, BrushColor(p));

            TerrainOperation op = p.DefaultOperation.Value;
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(x + 15f, panel.y + 15f, 42f, 42f), TerrainUiIcons.Op(op), ScaleMode.ScaleToFit, true);
            _hudTitle.normal.textColor = _applying ? TerrainTheme.GoldBright : TerrainTheme.TextWhite;
            GUI.Label(new Rect(x + 66f, panel.y + 15f, width - 156f, 25f), Loc.OpName(op).ToUpperInvariant(), _hudTitle);
            _hudSmall.normal.textColor = TerrainTheme.TextDim;
            GUI.Label(new Rect(x + 66f, panel.y + 39f, width - 82f, 18f), _applying ? Loc.T("flash.applying") : "TERRAINFORGE", _hudSmall);

            float radius = EffectiveRadius(p);
            string radiusText = radius.ToString("0.#") + " m" + (radius < p.Radius.Value - 0.001f ? "  ·  " + Loc.T("hud.capped") : "");
            _hudRight.normal.textColor = TerrainTheme.GoldBright;
            GUI.Label(new Rect(x + width - 112f, panel.y + 18f, 96f, 24f), radiusText, _hudRight);

            GUI.color = new Color(1f, 1f, 1f, 0.18f);
            GUI.DrawTexture(new Rect(x + 14f, panel.y + 64f, width - 28f, 1f), _hudBg);
            GUI.color = Color.white;

            _hud.normal.textColor = TerrainTheme.TextWhite;
            string shape = Loc.T("set.shape") + "  " + ShapeLabel(p.Shape.Value);
            string aimed = Loc.T("hud.aimed") + "  " + _aimY.ToString("0.0") + " m";
            GUI.Label(new Rect(x + 16f, panel.y + 68f, width * 0.52f, 22f), shape, _hud);
            _hudRight.normal.textColor = TerrainTheme.TextWhite;
            GUI.Label(new Rect(x + width * 0.52f, panel.y + 68f, width * 0.43f, 22f), aimed, _hudRight);

            float footerY = panel.y + 95f;
            if (showMode)
            {
                string mode = p.DefaultOperation.Value == TerrainOperation.Ramp
                    ? Loc.T("set.rampmode") + "  " + p.RampMode.Value
                    : p.DefaultOperation.Value == TerrainOperation.CopyPaste
                        ? Loc.T("tab.stamps")
                        : Loc.T("set.paint") + "  " + p.Paint.Value;
                _hudSmall.normal.textColor = TerrainTheme.TextDim;
                GUI.Label(new Rect(x + 16f, panel.y + 92f, width - 32f, 19f), mode, _hudSmall);
                footerY += 22f;
            }

            string footer = Loc.T("hud.undo") + " " + TerrainEngine.UndoCount + "     " +
                Loc.T("hud.redo") + " " + TerrainEngine.RedoCount;
            if (GameCamera.InFreeFly())
            {
                Camera cam = Camera.main;
                if (cam != null)
                    footer += "     ·     " + Loc.F("hud.freecam.dist", Vector3.Distance(cam.transform.position, _aimPoint).ToString("0"));
            }
            _hudSmall.normal.textColor = TerrainTheme.TextDim;
            GUI.Label(new Rect(x + 16f, footerY, width - 32f, 20f), footer, _hudSmall);

            if (showContext)
            {
                string context = BuildHudContext(p, player, hasStamp, clipboard.Count);
                Color contextColor = (!_serverAllowed || !_wardOk) ? TerrainTheme.Danger :
                    _adjustMode || hasStamp || _pastePreview ? TerrainTheme.GoldBright : TerrainTheme.TextDim;
                _hudSmall.normal.textColor = contextColor;
                GUI.Label(new Rect(x + 16f, panel.y + height - 27f, width - 32f, 20f), context, _hudSmall);
            }
            _hud.normal.textColor = Color.white;
            _hudTitle.normal.textColor = TerrainTheme.TextWhite;
            _hudSmall.normal.textColor = TerrainTheme.TextDim;
            _hudRight.normal.textColor = TerrainTheme.GoldBright;
        }

        private string BuildHudContext(TerrainForgePlugin p, Player player, bool hasStamp, int stampCount)
        {
            if (!_serverAllowed) return Loc.T("hud.blocked");
            if (!_wardOk) return Loc.T("hud.ward");
            if (!string.IsNullOrEmpty(_flashMsg) && Time.time < _flashUntil) return _flashMsg;
            if (_adjustMode) return Loc.T("hud.adjust");
            if (_pastePreview) return Loc.T("hud.stampPasteHint");
            if (p.DefaultOperation.Value == TerrainOperation.CopyPaste)
            {
                switch (p.Shape.Value)
                {
                    case TerrainShape.Rect: return Loc.T(_rectHasA ? "hud.copy.rectB" : "hud.copy.rectA");
                    case TerrainShape.Line: return Loc.T(_lineHasA ? "hud.copy.lineB" : "hud.copy.lineA");
                    case TerrainShape.Poly: return Loc.F("hud.copy.poly", _polyPoints.Count);
                    default: return Loc.T("hud.copy.circle");
                }
            }
            if (hasStamp) return Loc.T("hud.stampReady") + "  ·  " + stampCount;
            if (p.DefaultOperation.Value == TerrainOperation.Ramp)
                return p.RampMode.Value == RampMode.Path ? Loc.F("hud.path.mark", _rampPath.Count) : Loc.T("hud.rampA");
            switch (p.Shape.Value)
            {
                case TerrainShape.Line: return Loc.T("hud.line.mark");
                case TerrainShape.Rect: return Loc.T("hud.rect.mark");
                case TerrainShape.Poly: return Loc.F("hud.poly.mark", _polyPoints.Count);
                default: return Loc.T("hud.freecamHint");
            }
        }

        private static string ShapeLabel(TerrainShape shape)
        {
            switch (shape)
            {
                case TerrainShape.Circle: return Loc.T("shape.circle");
                case TerrainShape.Square: return Loc.T("shape.square");
                case TerrainShape.Line: return Loc.T("shape.line");
                case TerrainShape.Rect: return Loc.T("shape.rect");
                case TerrainShape.Poly: return Loc.T("shape.poly");
                default: return shape.ToString();
            }
        }

        private void DrawAimBadge(TerrainForgePlugin p)
        {
            Camera cam = Camera.main;
            if (cam == null || _aimPoint == Vector3.zero) return;
            Vector3 screen = cam.WorldToScreenPoint(_aimPoint);
            if (screen.z <= 0f) return;
            float x = Mathf.Clamp(screen.x + 22f, 8f, Screen.width - 150f);
            float y = Mathf.Clamp(Screen.height - screen.y - 48f, 8f, Screen.height - 48f);
            var rect = new Rect(x, y, 142f, 34f);
            GUI.color = Color.white;
            GUI.DrawTexture(rect, _hudBg);
            GUI.color = BrushColor(p);
            GUI.DrawTexture(new Rect(x + 5f, y + 3f, 28f, 28f), TerrainUiIcons.Op(p.DefaultOperation.Value), ScaleMode.ScaleToFit, true);
            GUI.color = Color.white;
            _hud.normal.textColor = TerrainTheme.TextWhite;
            GUI.Label(new Rect(x + 38f, y, 98f, 34f), Loc.OpName(p.DefaultOperation.Value), _hud);
            _hud.normal.textColor = Color.white;
        }

        private void DrawHudSurface(Rect rect, Color accent)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.68f);
            GUI.DrawTexture(new Rect(rect.x + 3f, rect.y + 4f, rect.width, rect.height), _hudBg);
            GUI.color = new Color(TerrainTheme.PanelDark.r, TerrainTheme.PanelDark.g, TerrainTheme.PanelDark.b, 0.96f);
            GUI.DrawTexture(rect, _hudBg);
            GUI.color = accent;
            GUI.DrawTexture(new Rect(rect.x, rect.y, 3f, rect.height), _hudBg);
            GUI.color = Color.white;
        }

        private string BuildInfoLine()
        {
            return _posTxt;
        }

        private void UpdatePersistentHud(Player player, TerrainForgePlugin p)
        {
            Vector3 pos = player.transform.position;
            _posTxt = string.Format("{0} {1:0}, {2:0}, {3:0}",
                Loc.T("hud.coords"), pos.x, pos.z, pos.y);
        }

        internal static float WheelDelta()
        {
            float s = Input.mouseScrollDelta.y;
            if (Mathf.Abs(s) < 0.01f)
                s = ZInput.GetMouseScrollWheel();
            return s;
        }

        internal static bool FreeCameraActive => GameCamera.InFreeFly();

        private void ToggleFreeCamera()
        {
            GameCamera gc = GameCamera.instance;
            if (gc == null)
                return;

            gc.ToggleFreeFly();
            if (GameCamera.InFreeFly())
                gc.SetFreeFlySmoothness(0.1f);

            Flash(GameCamera.InFreeFly() ? Loc.T("flash.freecam.on") : Loc.T("flash.freecam.off"));
        }

        private void EnsureHudStyle()
        {
            Font font = GameFont();

            // Valheim HUD colors
            Color valheimGold = new Color(0.95f, 0.82f, 0.25f, 1f);
            Color valheimTextWhite = new Color(0.92f, 0.90f, 0.85f, 1f);

            _hudBg = new Texture2D(1, 1);
            _hudBg.SetPixel(0, 0, Color.white);
            _hudBg.Apply();
            _hudBg.hideFlags = HideFlags.HideAndDontSave;

            _hud = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = font
            };
            _hud.normal.textColor = valheimTextWhite;

            _hudTitle = new GUIStyle(_hud)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _hudSmall = new GUIStyle(_hud)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            _hudRight = new GUIStyle(_hud)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight
            };

            _banner = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                font = font
            };
            _banner.normal.textColor = valheimGold;
        }

        private void DrawFreeCamBanner()
        {
            if (_banner == null || _hudBg == null)
                return;

            string txt = Loc.T("hud.freecam.banner");
            float w = Mathf.Min(Screen.width - 40f, 780f);
            Vector2 sz = _banner.CalcSize(new GUIContent(txt));
            float h = Mathf.Max(38f, sz.y + 18f);
            var rect = new Rect((Screen.width - w) * 0.5f, 16f, w, h);

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.62f);
            GUI.DrawTexture(rect, _hudBg);
            GUI.color = prev;

            GUI.Label(rect, txt, _banner);
        }

        internal static Font GameFont()
        {
            Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
            if (fonts != null)
            {
                for (int i = 0; i < fonts.Length; i++)
                {
                    Font f = fonts[i];
                    if (f != null && f.name != null && f.name.ToLowerInvariant().Contains("averia"))
                        return f;
                }
            }
            return null;
        }
    }
}

