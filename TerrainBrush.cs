using System.Collections.Generic;
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

        private readonly TerrainPreview _preview = new TerrainPreview();
        private float _lastApply = -999f;
        private bool _applying;
        private bool _toolGiven;
        private bool _rampHasA;
        private Vector3 _rampA;

        private GUIStyle _hud;
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

        // Stamp copy/paste
        private bool _pastePreview;
        private float _stampRotation;
        private float _stampScale = 1f;
        private Vector3 _stampPreviewCenter;

        internal static bool AdjustModeActive { get; private set; }
        private static bool s_clearMarks;

        internal static void RequestClearMarks() => s_clearMarks = true;

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

            if (p.AutoGiveTool.Value && !_toolGiven)
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

            if (equipped && !p.SeenTutorial.Value)
            {
                p.SeenTutorial.Value = true;
                try { p.Config.Save(); } catch { }
                TerrainForgePlugin.Message(Loc.T("tut.1"));
                TerrainForgePlugin.Message(Loc.T("tut.2"));
            }

            Vector3 aim;
            bool hasAim = TryGetAim(out aim);

            Vector3 groundCenter;
            if (hasAim)
            {
                groundCenter = aim;
            }
            else
            {
                float gy = GroundHeight(player.transform.position);
                groundCenter = new Vector3(player.transform.position.x, gy, player.transform.position.z);
            }

            if (!blocked && equipped)
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
            _serverAllowed = !equipped || ServerAllows(p, out _serverWhy);
            _wardOk = !equipped || WardAllows(p, groundCenter, EffectiveRadius(p), out _wardWhy);
            bool canEdit = _serverAllowed && _wardOk;
            string why = _serverAllowed ? _wardWhy : _serverWhy;

            // Stamp copy/paste (work outside normal apply flow)
            if (!blocked && equipped)
            {
                if (p.CopyStampKey.Value.IsDown())
                    HandleCopyStamp(p, groundCenter);
                if (p.PasteStampKey.Value.IsDown())
                    HandlePasteStamp(p, groundCenter, canEdit, why);
            }

            TerrainOperation op = p.DefaultOperation.Value;

            if (!blocked && equipped && !Held(p.RadialKey.Value))
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

            if (equipped)
            {
                _aimY = groundCenter.y;
                _aimPoint = groundCenter;
                _targetTxt = TargetText(p, player, groundCenter);
                _slopeDeg = SlopeDeg(groundCenter);

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

            if (equipped && p.ShowPreview.Value)
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
                    float scroll = Input.mouseScrollDelta.y;
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
                    var tilePositions = new List<Vector3>();
                    if (stamp != null)
                    {
                        foreach (var tile in stamp.Tiles)
                            tilePositions.Add(tile.TilePos);
                    }

                    _preview.Update(new PreviewState
                    {
                        ShowStamp = true,
                        StampCenter = _stampPreviewCenter,
                        StampRotation = _stampRotation,
                        StampScale = _stampScale,
                        StampTilePositions = tilePositions,
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
                            (!isRamp && (p.Shape.Value == TerrainShape.Line ||
                                         p.Shape.Value == TerrainShape.Rect ||
                                         p.Shape.Value == TerrainShape.Poly)),
                        HasRect = rectMarking,
                        RectA = _rectA,
                        RectB = groundCenter,
                        PolyPreview = polyPreview,
                        SlopeDeg = _slopeDeg,
                        GuidesOn = guides,
                        LevelY = guides ? _levelY : float.NaN,
                        Blocked = equipped && !canEdit,
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
                SnowClear = p.DefaultOperation.Value == TerrainOperation.Snow && Held(p.AimHeightModifierKey.Value),
                PaintStrength = p.PaintStrength.Value,
                MaxUp = EffectiveMaxUp(p),
                MaxDown = EffectiveMaxDown(p),
                SmoothMode = p.SmoothMode.Value,
                Falloff = p.FalloffMode.Value,
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
            else if (p.OpSnowKey.Value.IsDown()) SetOp(p, TerrainOperation.Snow);
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
            var stamp = TerrainEngine.CopyStamp(
                center,
                EffectiveRadius(p),
                p.Shape.Value,
                _rectHasA ? (Vector3?)_rectA : null,
                _rectHasA ? (Vector3?)_rectA : null, // rectB will be current aim, but we need the marked rect
                _polyPoints.Count >= 3 ? _polyPoints.ToArray() : null,
                _lineHasA, _lineHasA ? (Vector3?)_lineA : null, _lineHasA ? (Vector3?)_aimPoint : null);

            // For rect, we need both corners
            if (p.Shape.Value == TerrainShape.Rect && _rectHasA)
            {
                stamp = TerrainEngine.CopyStamp(
                    center,
                    EffectiveRadius(p),
                    p.Shape.Value,
                    _rectA,
                    _aimPoint,
                    null,
                    false, null, null);
            }

            if (stamp != null)
            {
                Flash(Loc.F("flash.stamp.copied", stamp.Tiles.Count));
                TerrainForgePlugin.Message(stamp.Label);
            }
            else
            {
                Flash(Loc.T("flash.stamp.empty"));
            }
        }

        private void HandlePasteStamp(TerrainForgePlugin p, Vector3 center, bool canEdit, string why)
        {
            if (_pastePreview)
            {
                // Confirm paste
                if (canEdit)
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
                    Flash(why);
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

            _pastePreview = true;
            _stampPreviewCenter = center;
            _stampRotation = 0f;
            _stampScale = 1f;

            // Build tile positions for preview
            var tilePositions = new List<Vector3>();
            foreach (var tile in stamp.Tiles)
            {
                tilePositions.Add(tile.TilePos);
            }

            _preview.Update(new PreviewState
            {
                ShowStamp = true,
                StampCenter = center,
                StampRotation = 0f,
                StampScale = 1f,
                StampTilePositions = tilePositions,
                Radius = EffectiveRadius(p),
                Color = BrushColor(p).WithAlpha(0.95f),
                Blocked = !canEdit,
                WaterLevel = TerrainEngine.WaterLevel(),
            });

            Flash(Loc.T("flash.stamp.preview"));
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
            bool angleMode = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) &&
                             p.DefaultOperation.Value == TerrainOperation.Ramp &&
                             p.RampMode.Value == RampMode.FixedAngle;

            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool fine = Held(p.AimHeightModifierKey.Value);
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
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
            p.DefaultOperation.Value = (TerrainOperation)(((int)p.DefaultOperation.Value + 1) % 9);
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
                case TerrainOperation.Snow:
                    return Loc.T(Held(p.AimHeightModifierKey.Value) ? "hud.snow.clear" : "hud.snow.put");
                case TerrainOperation.Paint:
                    return p.Paint.Value + " " + (p.PaintStrength.Value * 100f).ToString("0") + "%";
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

        private static float SlopeDeg(Vector3 c)
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

            if (Physics.Raycast(ray, out RaycastHit hit, MaxAimDistance, ~0, QueryTriggerInteraction.Ignore) &&
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

            float step = Mathf.Max(0.75f, MaxAimDistance * 0.05f);
            float maxDist = MaxAimDistance * 2f;

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

        internal static Color BrushColor(TerrainForgePlugin p)
        {
            bool snowClear = p.DefaultOperation.Value == TerrainOperation.Snow &&
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

            if (!IsToolEquipped(p, GetHeldItem(player)))
                return;

            EnsureHudStyle();

            float er = EffectiveRadius(p);
            string rtxt = er.ToString("0.#") + " m" +
                (er < p.Radius.Value - 0.001f ? " (" + Loc.T("hud.capped") + ")" : "");
            string status = string.Format("TerrainForge   {0}   |   {1} {2}   |   {3}   |   {4}",
                p.DefaultOperation.Value, Loc.T("hud.radius"), rtxt, p.Shape.Value, p.Paint.Value);
            if (p.DefaultOperation.Value == TerrainOperation.Ramp)
            {
                if (p.RampMode.Value == RampMode.Path)
                    status += "   |   " + (_rampPath.Count > 0
                        ? Loc.F("hud.path.mark", _rampPath.Count)
                        : Loc.T("hud.path.idle"));
                else if (_rampHasA)
                    status += "   |   " + Loc.T("hud.rampA");
            }
            if (p.DefaultOperation.Value != TerrainOperation.Ramp)
            {
                switch (p.Shape.Value)
                {
                    case TerrainShape.Line:
                        if (_lineHasA)
                        {
                            float len = HorizDist(_lineA, _aimPoint);
                            float rise = _aimPoint.y - _lineA.y;
                            status += "   |   " + Loc.T("hud.line.mark") + string.Format(" ({0}, {1}, {2})",
                                M(len), Rise(rise), Deg(PathAngle(len, rise)));
                        }
                        else
                            status += "   |   " + Loc.T("hud.line.idle");
                        break;
                    case TerrainShape.Rect:
                        if (_rectHasA)
                        {
                            float wdt = Mathf.Abs(_aimPoint.x - _rectA.x);
                            float dep = Mathf.Abs(_aimPoint.z - _rectA.z);
                            status += "   |   " + Loc.T("hud.rect.mark") + string.Format(" ({0}×{1} m, {2} m²)",
                                wdt.ToString("0.#"), dep.ToString("0.#"), (wdt * dep).ToString("0.#"));
                        }
                        else
                            status += "   |   " + Loc.T("hud.rect.idle");
                        break;
                    case TerrainShape.Poly:
                        if (_polyPoints.Count > 0)
                        {
                            float perim = ChainLength(_polyPoints, _aimPoint, false);
                            float area = PolyArea(_polyPoints, _aimPoint, true);
                            status += "   |   " + Loc.F("hud.poly.mark", _polyPoints.Count) +
                                string.Format(" ({0}, {1} m²)", M(perim), area.ToString("0.#"));
                        }
                        else
                            status += "   |   " + Loc.T("hud.poly.idle");
                        break;
                }
            }
            status += string.Format("   |   {0} {1} / {2} {3}",
                Loc.T("hud.undo"), TerrainEngine.UndoCount, Loc.T("hud.redo"), TerrainEngine.RedoCount);
            if (_adjustMode)
                status += "   |   " + Loc.T("hud.adjust");
            if (!_serverAllowed)
                status += "   |   " + Loc.T("hud.blocked");
            if (!_wardOk)
                status += "   |   " + Loc.T("hud.ward");
            if (!string.IsNullOrEmpty(_flashMsg) && Time.time < _flashUntil)
                status += "   |   " + _flashMsg;

            Color prev = GUI.color;
            Color c = BrushColor(p);
            _hud.normal.textColor = _applying ? new Color(1f, 0.85f, 0.4f) : c;
            string prefix = _applying ? "\u25B6  " : "\u2022  ";
            string full = prefix + status;

            // Bottom-right block: second line at the bottom, main line above it.
            Vector2 size = _hud.CalcSize(new GUIContent(full));
            float w = Mathf.Max(320f, size.x + 28f);
            w = Mathf.Min(w, Mathf.Max(200f, Screen.width - 24f));
            float h = Mathf.Max(30f, size.y + 12f);

            string line2 = string.Format("{0} {1:0.0} m   |   {2}   |   {3} {4:0}\u00B0",
                Loc.T("hud.aimed"), _aimY, _targetTxt, Loc.T("hud.slope"), _slopeDeg);
            Vector2 size2 = _hud.CalcSize(new GUIContent(line2));
            float w2 = Mathf.Max(320f, size2.x + 28f);
            w2 = Mathf.Min(w2, Mathf.Max(200f, Screen.width - 24f));
            float h2 = Mathf.Max(30f, size2.y + 12f);

            float margin = 12f;
            var rect2 = new Rect(Screen.width - w2 - margin, Screen.height - h2 - margin, w2, h2);
            var rect = new Rect(Screen.width - w - margin, rect2.y - 4f - h, w, h);

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect, _hudBg);
            GUI.color = prev;

            GUI.Label(new Rect(rect.x + 8f, rect.y, rect.width - 20f, rect.height), full, _hud);

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect2, _hudBg);
            GUI.color = prev;

            Color prevText = _hud.normal.textColor;
            _hud.normal.textColor = Color.white;
            GUI.Label(new Rect(rect2.x + 8f, rect2.y, rect2.width - 20f, rect2.height), line2, _hud);
            _hud.normal.textColor = prevText;
        }

        private void EnsureHudStyle()
        {
            if (_hud != null)
                return;

            _hudBg = new Texture2D(1, 1);
            _hudBg.SetPixel(0, 0, Color.white);
            _hudBg.Apply();
            _hudBg.hideFlags = HideFlags.HideAndDontSave;

            // Default IMGUI typeface (not the game's Averia): cleaner at small
            // sizes and visually distinct from vanilla text.
            _hud = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                font = null
            };
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

    internal static class ColorExtensions
    {
        public static Color WithAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }
    }

    /// <summary>
    /// Hold-to-open radial menu for picking the brush operation with the mouse.
    /// Release confirms the hovered wedge, Esc cancels. IMGUI only, no assets.
    /// </summary>
    internal sealed class TerrainRadial : MonoBehaviour
    {
        public static bool Open { get; private set; }

        private static readonly TerrainOperation[] s_ops =
            (TerrainOperation[])System.Enum.GetValues(typeof(TerrainOperation));

        private int _sel = -1;
        private bool _cancel;
        private GUIStyle _style;
        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;

        private void Update()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || !p.Enabled.Value || TerrainKeybinds.MenuOpen)
            {
                if (Open)
                    Close();
                return;
            }

            Player player = Player.m_localPlayer;
            if (player == null)
            {
                if (Open)
                    Close();
                return;
            }

            bool blocked = Console.IsVisible() || InventoryGui.IsVisible() || TextInput.IsVisible();
            if (blocked)
            {
                if (Open)
                    Close();
                return;
            }

            if (!TerrainBrush.IsToolEquipped(p, TerrainBrush.GetHeldItem(player)))
            {
                if (Open)
                    Close();
                return;
            }

            if (!Open)
            {
                if (TerrainBrush.Held(p.RadialKey.Value))
                {
                    Open = true;
                    _sel = (int)p.DefaultOperation.Value;
                    _cancel = false;
                    EnsureStyle();
                    _cursorWasVisible = Cursor.visible;
                    _cursorLock = Cursor.lockState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                _cancel = true;

            _sel = HoverFromMouse();

            if (Input.GetMouseButtonDown(0) && !_cancel && _sel >= 0)
            {
                ApplySel(p);
                Close();
                return;
            }

            if (!TerrainBrush.Held(p.RadialKey.Value))
            {
                if (!_cancel && _sel >= 0)
                    ApplySel(p);
                Close();
            }
        }

        private void ApplySel(TerrainForgePlugin p)
        {
            if (_sel < 0 || _sel >= s_ops.Length)
                return;
            if (p.DefaultOperation.Value != s_ops[_sel])
            {
                p.DefaultOperation.Value = s_ops[_sel];
                TerrainBrush.RequestClearMarks();
            }
        }

        private void Close()
        {
            Open = false;
            Cursor.visible = _cursorWasVisible;
            Cursor.lockState = _cursorLock;
        }

        private static int HoverFromMouse()
        {
            Vector2 m = Input.mousePosition;
            float dx = m.x - Screen.width * 0.5f;
            float dy = m.y - Screen.height * 0.5f;
            if (dx * dx + dy * dy < 34f * 34f)
                return -1;
            float deg = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            float cw = (90f - deg + 360f) % 360f;
            int idx = (int)((cw + 22.5f) / 45f) % 8;
            if (idx < 0 || idx >= s_ops.Length)
                return -1;
            return idx;
        }

        private void OnGUI()
        {
            if (!Open)
                return;
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;
            EnsureStyle();

            TerrainPaint paint = p.Paint.Value;

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            Color prevBg = GUI.backgroundColor;
            for (int i = 0; i < s_ops.Length; i++)
            {
                float ang = (-90f + i * 45f) * Mathf.Deg2Rad;
                float bx = cx + Mathf.Cos(ang) * 104f - 58f;
                float by = cy - Mathf.Sin(ang) * 104f - 16f;
                Color c = TerrainBrush.OpColor(s_ops[i], paint, false);
                if (i == _sel)
                    GUI.backgroundColor = new Color(Mathf.Min(1f, c.r + 0.3f), Mathf.Min(1f, c.g + 0.3f), Mathf.Min(1f, c.b + 0.3f), 0.97f);
                else
                    GUI.backgroundColor = new Color(c.r * 0.35f, c.g * 0.35f, c.b * 0.35f, 0.92f);
                GUI.Button(new Rect(bx, by, 116f, 32f), Loc.OpName(s_ops[i]), _style);
            }
            GUI.backgroundColor = prevBg;

            string center = _sel >= 0 ? Loc.OpName(s_ops[_sel]) : Loc.OpName(p.DefaultOperation.Value);
            GUI.Box(new Rect(cx - 70f, cy - 16f, 140f, 32f), center, _style);
        }

        private void EnsureStyle()
        {
            if (_style != null)
                return;
            _style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                font = TerrainBrush.GameFont()
            };
        }
    }

    /// <summary>
    /// One-shot feedback (dust + thud) when the brush writes terrain. Reuses the
    /// tool's own terrain-hit effect, so no prefab names are needed. Local only,
    /// throttled while holding, always on discrete applications.
    /// </summary>
    internal static class TerrainFx
    {
        private static float _last;

        public static void Play(Vector3 pos, bool force)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p != null && p.PlayEffects != null && !p.PlayEffects.Value)
                return;
            if (!force && Time.time - _last < 0.6f)
                return;
            _last = Time.time;

            try
            {
                EffectList fx = TerrainTool.Shared != null
                    ? TerrainTool.Shared.m_hitTerrainEffect
                    : null;
                if (fx == null)
                    return;
                fx.Create(pos + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
            }
            catch
            {
                // Feedback must never break an edit.
            }
        }
    }
}
