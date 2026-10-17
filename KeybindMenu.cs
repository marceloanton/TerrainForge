using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    internal sealed class TerrainKeybinds : MonoBehaviour
    {
        private const int WindowId = 9911735;

        private struct Entry
        {
            public string Label;
            public ConfigEntry<KeyboardShortcut> Bind;
        }

        private static readonly KeyCode[] AllKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));

        private readonly List<Entry> _entries = new List<Entry>();
        private bool _open;
        private int _capturing = -1;
        private int _tab;
        private Vector2 _mainScroll;
        private Rect _window = new Rect(40f, 60f, 680f, 720f);

        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;
        private string _undoDepthText;
        private string _presetName = "";
        private ModLanguage _langBuilt;
        private string _fixedHeightText;
        private string _snapshotName = "";
        private string _stampName = "";
        private string _stampLoadName = "";
        private string _keySearch = "";
        private bool _resizing;
        private GUIStyle _gripStyle;

        private GUIStyle _winStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _hintStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _btnStyle;
        private GUIStyle _closeStyle;
        private GUIStyle _capturingStyle;
        private GUIStyle _capturingBtnStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _topicStyle;
        private GUIStyle _boxStyle;
        private GUIStyle _scrollStyle;
        private GUIStyle _scrollBarStyle;
        private GUIStyle _scrollBarThumbStyle;
        private GUIStyle _iconBtnStyle;
        private GUIStyle _iconBtnSelStyle;
        private GUIStyle _iconCapStyle;
        private GUIStyle _iconCapSelStyle;
        private GUIStyle _iconOnlyStyle;

        private Texture2D _texPanel;
        private Texture2D _texHeader;
        private Texture2D _texPanelBg;
        private Texture2D _texBtn;
        private Texture2D _texBtnHover;
        private Texture2D _texBtnActive;
        private Texture2D _texCapture;

        public static bool MenuOpen { get; private set; }

        private static TerrainKeybinds s_instance;

        internal static void OpenMenu()
        {
            if (s_instance != null)
                s_instance.Toggle(true);
        }

        private void Start()
        {
            s_instance = this;
            BuildEntries();
        }

        private void BuildEntries()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            _entries.Clear();
            _entries.Add(new Entry { Label = Loc.T("key.apply"), Bind = p.ApplyKey });
            _entries.Add(new Entry { Label = Loc.T("key.radiusUp"), Bind = p.RadiusUpKey });
            _entries.Add(new Entry { Label = Loc.T("key.radiusDown"), Bind = p.RadiusDownKey });
            _entries.Add(new Entry { Label = Loc.T("key.cycleOp"), Bind = p.CycleOperationKey });
            _entries.Add(new Entry { Label = Loc.T("key.cycleShape"), Bind = p.CycleShapeKey });
            _entries.Add(new Entry { Label = Loc.T("key.cyclePaint"), Bind = p.CyclePaintKey });
            _entries.Add(new Entry { Label = Loc.T("key.aimMod"), Bind = p.AimHeightModifierKey });
            _entries.Add(new Entry { Label = Loc.T("key.undo"), Bind = p.UndoKey });
            _entries.Add(new Entry { Label = Loc.T("key.redo"), Bind = p.RedoKey });
            _entries.Add(new Entry { Label = Loc.T("key.eyedrop"), Bind = p.EyedropKey });
            _entries.Add(new Entry { Label = Loc.T("key.rampMode"), Bind = p.RampModeKey });
            _entries.Add(new Entry { Label = Loc.T("key.radial"), Bind = p.RadialKey });
            _entries.Add(new Entry { Label = Loc.T("key.adjust"), Bind = p.AdjustModeKey });
            _entries.Add(new Entry { Label = Loc.T("key.freecam"), Bind = p.FreeCameraKey });
            _entries.Add(new Entry { Label = Loc.T("key.give"), Bind = p.GiveToolKey });
            _entries.Add(new Entry { Label = Loc.T("key.menu"), Bind = p.RebindMenuKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.flatten"), Bind = p.OpFlattenKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.raise"), Bind = p.OpRaiseKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.lower"), Bind = p.OpLowerKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.smooth"), Bind = p.OpSmoothKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.paint"), Bind = p.OpPaintKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.restore"), Bind = p.OpRestoreKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.ramp"), Bind = p.OpRampKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.shore"), Bind = p.OpShoreKey });
            _entries.Add(new Entry { Label = Loc.T("key.op.snow"), Bind = p.OpSnowKey });
            _entries.Add(new Entry { Label = Loc.T("key.cycleProfile"), Bind = p.CycleProfileKey });
            _entries.Add(new Entry { Label = Loc.T("key.copyStamp"), Bind = p.CopyStampKey });
            _entries.Add(new Entry { Label = Loc.T("key.pasteStamp"), Bind = p.PasteStampKey });
            _entries.Add(new Entry { Label = Loc.T("key.cycleStamp"), Bind = p.CycleStampKey });
            _entries.Add(new Entry { Label = Loc.T("key.cycleStampBack"), Bind = p.CycleStampBackKey });
            _entries.Add(new Entry { Label = Loc.T("key.slopeHeatmap"), Bind = p.SlopeHeatmapKey });
            _entries.Add(new Entry { Label = Loc.T("key.heightHeatmap"), Bind = p.HeightHeatmapKey });
            _langBuilt = p.Language != null ? p.Language.Value : ModLanguage.Español;
        }

        private void Update()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            PauseMenuPatch.Ensure();

            // Force cursor visible while menu is open (game may try to hide/lock it)
            if (_open)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (_capturing >= 0)
            {
                PollCapture();
                return;
            }

            if (ShortcutDown(p.RebindMenuKey.Value))
                Toggle(!_open);
        }

        private void PollCapture()
        {
            // Check mouse buttons first (they're not in AllKeys)
            if (Input.GetMouseButtonDown(0)) { Capture(KeyCode.Mouse0); return; }
            if (Input.GetMouseButtonDown(1)) { Capture(KeyCode.Mouse1); return; }
            if (Input.GetMouseButtonDown(2)) { Capture(KeyCode.Mouse2); return; }

            for (int i = 0; i < AllKeys.Length; i++)
            {
                KeyCode kc = AllKeys[i];
                if (kc == KeyCode.None)
                    continue;

                // Ignore pure modifier keys as main key - they can only be modifiers
                if (kc == KeyCode.LeftControl || kc == KeyCode.RightControl ||
                    kc == KeyCode.LeftShift || kc == KeyCode.RightShift ||
                    kc == KeyCode.LeftAlt || kc == KeyCode.RightAlt ||
                    kc == KeyCode.LeftCommand || kc == KeyCode.RightCommand)
                    continue;

                bool down;
                try { down = Input.GetKeyDown(kc); }
                catch { continue; }

                if (!down)
                    continue;

                if (kc == KeyCode.Escape)
                {
                    _capturing = -1;
                    return;
                }

                Capture(kc);
                return;
            }
        }

        private void Capture(KeyCode main)
        {
            var mods = new List<KeyCode>();
            if (!IsModifier(main, KeyCode.LeftControl, KeyCode.RightControl) &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                mods.Add(KeyCode.LeftControl);
            if (!IsModifier(main, KeyCode.LeftShift, KeyCode.RightShift) &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                mods.Add(KeyCode.LeftShift);
            if (!IsModifier(main, KeyCode.LeftAlt, KeyCode.RightAlt) &&
                (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                mods.Add(KeyCode.LeftAlt);

            _entries[_capturing].Bind.Value = new KeyboardShortcut(main, mods.ToArray());
            TerrainForgePlugin.Instance?.Config.Save();
            _capturing = -1;
        }

        private static bool IsModifier(KeyCode main, KeyCode a, KeyCode b) => main == a || main == b;

        private void Toggle(bool open)
        {
            _open = open;
            MenuOpen = open;

            if (open)
            {
                _cursorWasVisible = Cursor.visible;
                _cursorLock = Cursor.lockState;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                TerrainForgePlugin p = TerrainForgePlugin.Instance;
                if (p != null)
                {
                    ModLanguage lang = p.Language != null ? p.Language.Value : ModLanguage.Español;
                    if (_entries.Count == 0 || lang != _langBuilt)
                        BuildEntries();
                    if (p.UndoDepth != null)
                        _undoDepthText = p.UndoDepth.Value.ToString();
                    if (p.FixedHeight != null)
                        _fixedHeightText = p.FixedHeight.Value.ToString("0.0");
                }
            }
            else
            {
                _capturing = -1;
                try { TerrainForgePlugin.Instance?.Config.Save(); } catch { }
                Cursor.visible = _cursorWasVisible;
                Cursor.lockState = _cursorLock;
            }
        }

        private void OnGUI()
        {
            if (!_open)
                return;

            // Force cursor visible and unlocked for this menu (game may try to hide/lock it)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            EnsureSkin();
            _window = GUILayout.Window(WindowId, _window, DrawWindow, GUIContent.none, _winStyle);
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(_headerStyle);
            GUILayout.Label("TERRAINFORGE", _titleStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("\u2715", _closeStyle, GUILayout.Width(30f), GUILayout.Height(24f)))
                Toggle(false);
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_tab == 0, Loc.T("tab.tool"), _tabStyle, GUILayout.Height(30f)))
                _tab = 0;
            if (GUILayout.Toggle(_tab == 1, Loc.T("tab.keys"), _tabStyle, GUILayout.Height(30f)))
                _tab = 1;
            if (GUILayout.Toggle(_tab == 2, Loc.T("tab.advanced"), _tabStyle, GUILayout.Height(30f)))
                _tab = 2;
            if (GUILayout.Toggle(_tab == 3, Loc.T("tab.settings"), _tabStyle, GUILayout.Height(30f)))
                _tab = 3;
            if (GUILayout.Toggle(_tab == 4, Loc.T("tab.help"), _tabStyle, GUILayout.Height(30f)))
                _tab = 4;
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            // Main scroll view for all tab content
            _mainScroll = GUILayout.BeginScrollView(_mainScroll, _scrollStyle, GUILayout.MinHeight(380f), GUILayout.ExpandHeight(true));

            if (_tab == 0)
                DrawToolTab();
            else if (_tab == 1)
                DrawKeysTab();
            else if (_tab == 2)
                DrawAdvancedTab();
            else if (_tab == 3)
                DrawSettingsTabContent();
            else
                DrawHelpTabContent();

            GUILayout.EndScrollView();

            GUILayout.Space(6f);
            if (GUILayout.Button("Invitame un café en Cafecito", _btnStyle, GUILayout.Height(22f)))
            {
                try { Application.OpenURL("https://cafecito.app/byrka"); } catch { }
            }

            GUILayout.EndVertical();

            DrawResizeGrip();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 34f));
        }

        private void DrawResizeGrip()
        {
            var grip = new Rect(_window.width - 22f, _window.height - 22f, 22f, 22f);
            GUI.Label(grip, "\u25E2", _gripStyle);

            Event e = Event.current;
            if (e == null)
                return;

            if (e.type == EventType.MouseDown && e.button == 0 && grip.Contains(e.mousePosition))
            {
                _resizing = true;
                e.Use();
            }
            else if (_resizing && e.type == EventType.MouseDrag)
            {
                _window.width = Mathf.Clamp(_window.width + e.delta.x, 560f, Screen.width - 30f);
                _window.height = Mathf.Clamp(_window.height + e.delta.y, 440f, Screen.height - 30f);
                e.Use();
            }
            else if (_resizing && e.type == EventType.MouseUp)
            {
                _resizing = false;
                e.Use();
            }
        }

        private void DrawToolTab()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            SettingHeader(Loc.T("set.operation"));
            DrawOpPicker(p);

            SettingHeader(Loc.T("set.shape"));
            DrawShapePicker(p);

            SettingHeader(Loc.T("set.paint"));
            CycleRow(Loc.T("set.paint"), p.Paint.Value.ToString(), () =>
            {
                p.Paint.Value = (TerrainPaint)(((int)p.Paint.Value + 1) % 6);
            });
            SegmentedRow(Loc.T("set.rampmode"),
                new[] { Loc.T("rampmode.two"), Loc.T("rampmode.fixed"), Loc.T("rampmode.path") },
                (int)p.RampMode.Value,
                i => { p.RampMode.Value = (RampMode)i; TerrainBrush.RequestClearMarks(); });
            CycleRow(Loc.T("set.smoothmode"), SmoothModeText(p.SmoothMode.Value), () =>
            {
                p.SmoothMode.Value = (SmoothMode)(((int)p.SmoothMode.Value + 1) % 3);
            });
            CycleRow(Loc.T("set.falloff"), FalloffModeText(p.FalloffMode.Value), () =>
            {
                p.FalloffMode.Value = (FalloffMode)(((int)p.FalloffMode.Value + 1) % 4);
            });

            SettingHeader(Loc.T("set.sizes"));
            FloatSlider(Loc.T("set.radius"), p.Radius, 1f, 100f, "0.#");
            FloatSlider(Loc.T("set.softness"), p.Softness, 0f, 100f, "0");
            FloatSlider(Loc.T("set.angle"), p.SlopeAngle, 5f, 60f, "0");
            FloatSlider(Loc.T("set.raisedelta"), p.RaiseDelta, 0.5f, 10f, "0.#");
            FloatSlider(Loc.T("set.paintstrength"), p.PaintStrength, 0f, 1f, "0.00");
            FloatSlider(Loc.T("set.falloff"), p.FalloffPower, 1f, 5f, "0.#");
            FloatSlider(Loc.T("set.shoreoffset"), p.ShoreOffset, -2f, 5f, "0.#");
            CycleRow(Loc.T("set.shorefalloff"), ShoreFalloffModeText(p.ShoreFalloffMode.Value), () =>
            {
                p.ShoreFalloffMode.Value = (ShoreFalloffMode)(((int)p.ShoreFalloffMode.Value + 1) % 4);
            });

            DrawFixedHeight(p);

            SettingHeader(Loc.T("set.profiles"));
            DrawPresets(p);
        }

        private void DrawOpPicker(TerrainForgePlugin p)
        {
            TerrainOperation cur = p.DefaultOperation.Value;
            var ops = (TerrainOperation[])System.Enum.GetValues(typeof(TerrainOperation));
            const int perRow = 5;
            const float cell = 74f;

            for (int i = 0; i < ops.Length; i += perRow)
            {
                GUILayout.BeginHorizontal();
                for (int j = i; j < ops.Length && j < i + perRow; j++)
                {
                    TerrainOperation op = ops[j];
                    bool sel = op == cur;
                    GUILayout.BeginVertical(GUILayout.Width(cell));
                    if (GUILayout.Button(new GUIContent(TerrainUiIcons.Op(op), Loc.OpName(op)),
                        sel ? _iconBtnSelStyle : _iconBtnStyle, GUILayout.Width(cell - 6f), GUILayout.Height(46f)))
                    {
                        p.DefaultOperation.Value = op;
                        TerrainBrush.RequestClearMarks();
                    }
                    GUILayout.Label(Loc.OpName(op), sel ? _iconCapSelStyle : _iconCapStyle,
                        GUILayout.Width(cell), GUILayout.Height(14f));
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(2f);
        }

        private void DrawShapePicker(TerrainForgePlugin p)
        {
            TerrainShape cur = p.Shape.Value;
            var shapes = (TerrainShape[])System.Enum.GetValues(typeof(TerrainShape));
            const float cell = 64f;

            GUILayout.BeginHorizontal();
            foreach (TerrainShape sh in shapes)
            {
                bool sel = sh == cur;
                GUILayout.BeginVertical(GUILayout.Width(cell));
                if (GUILayout.Button(new GUIContent(TerrainUiIcons.Shape(sh), ShapeText(sh)),
                    sel ? _iconBtnSelStyle : _iconBtnStyle, GUILayout.Width(cell - 6f), GUILayout.Height(46f)))
                {
                    p.Shape.Value = sh;
                    TerrainBrush.RequestClearMarks();
                }
                GUILayout.Label(ShapeText(sh), sel ? _iconCapSelStyle : _iconCapStyle,
                    GUILayout.Width(cell), GUILayout.Height(14f));
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2f);
        }

        private void DrawKeysTab()
        {
            GUILayout.Label(_capturing >= 0 ? Loc.T("keys.hint.capture") : Loc.T("keys.hint.idle"),
                _capturing >= 0 ? _capturingStyle : _hintStyle);
            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.search"), _rowStyle, GUILayout.Width(90f), GUILayout.Height(26f));
            _keySearch = GUILayout.TextField(_keySearch ?? "", GUILayout.Height(26f));
            if (GUILayout.Button("X", _btnStyle, GUILayout.Width(44f), GUILayout.Height(26f)))
                _keySearch = "";
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);

            string filter = (_keySearch ?? "").Trim().ToLowerInvariant();
            for (int i = 0; i < _entries.Count; i++)
            {
                if (filter.Length > 0 && !_entries[i].Label.ToLowerInvariant().Contains(filter))
                    continue;

                GUILayout.BeginHorizontal();

                Color prev = GUI.color;
                GUI.color = _capturing == i ? new Color(1f, 0.85f, 0.45f) : Color.white;
                GUILayout.Label(_entries[i].Label, _rowStyle, GUILayout.Width(250f), GUILayout.Height(26f));
                GUI.color = prev;

                string text = _capturing == i ? Loc.T("keys.press") : _entries[i].Bind.Value.ToString();
                if (GUILayout.Button(text, _capturing == i ? _capturingBtnStyle : _btnStyle, GUILayout.Width(235f), GUILayout.Height(26f)))
                    _capturing = i;

                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }

            GUILayout.Space(6f);
            if (GUILayout.Button(Loc.T("set.resetkeys"), _btnStyle, GUILayout.Height(26f)))
                ResetKeybinds();

            GUILayout.Space(10f);
            if (GUILayout.Button(Loc.T("btn.get"), _btnStyle, GUILayout.Height(28f)))
                TerrainTool.Give(Player.m_localPlayer);
        }

        private void DrawAdvancedTab()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            DrawQuickActions();

            SettingHeader(Loc.T("set.snapshots"));
            DrawSnapshots();

            SettingHeader(Loc.T("set.stamps"));
            DrawStamps(p);

            GUILayout.Space(10f);
            DrawHistorySection();
        }

        private void DrawQuickActions()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            SettingHeader(Loc.T("set.quick"));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Loc.T("key.undo"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestUndo();
            if (GUILayout.Button(Loc.T("key.redo"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestRedo();
            if (GUILayout.Button(Loc.T("act.eyedrop"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestEyedrop();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Loc.T("act.copystamp"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestCopyStamp();
            if (GUILayout.Button(Loc.T("act.pastestamp"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestPasteStamp();
            if (GUILayout.Button(Loc.T("act.stampprev"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestCycleStampBack();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Loc.T("act.clearmarks"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestClearMarks();
            if (GUILayout.Button(Loc.T("set.slopeheatmap"), _btnStyle, GUILayout.Height(26f)))
                p.ShowSlopeHeatmap.Value = !p.ShowSlopeHeatmap.Value;
            if (GUILayout.Button(Loc.T("set.heightheatmap"), _btnStyle, GUILayout.Height(26f)))
                p.ShowHeightHeatmap.Value = !p.ShowHeightHeatmap.Value;
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Loc.T("act.diag"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestDumpMaterial();
            if (GUILayout.Button(Loc.T("key.freecam"), _btnStyle, GUILayout.Height(26f)))
                TerrainBrush.RequestFreeCamera();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void ResetKeybinds()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            for (int i = 0; i < _entries.Count; i++)
                _entries[i].Bind.Value = (KeyboardShortcut)_entries[i].Bind.DefaultValue;

            _capturing = -1;
            TerrainForgePlugin.Message(Loc.T("keys.resetdone"));
        }

        private static bool TryParseLenient(string t, out float v)
        {
            if (float.TryParse(t, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v))
                return true;
            return float.TryParse(t, out v);
        }

        private void DrawFixedHeight(TerrainForgePlugin p)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.fixedheight"), _rowStyle, GUILayout.Width(140f), GUILayout.Height(26f));
            string t = GUILayout.TextField(_fixedHeightText ?? p.FixedHeight.Value.ToString("0.0"),
                GUILayout.Width(90f), GUILayout.Height(26f));
            if (t != _fixedHeightText)
            {
                _fixedHeightText = t;
                if (TryParseLenient(t, out float v))
                    p.FixedHeight.Value = Mathf.Clamp(v, -10f, 150f);
            }
            if (GUILayout.Button(Loc.T("set.fix"), _btnStyle, GUILayout.Width(85f), GUILayout.Height(26f)))
            {
                if (TryParseLenient(_fixedHeightText ?? "", out float v))
                    p.FixedHeight.Value = Mathf.Clamp(v, -10f, 150f);
                p.UseFixedHeight.Value = true;
                _fixedHeightText = p.FixedHeight.Value.ToString("0.0");
            }
            if (GUILayout.Button(Loc.T("set.release"), _btnStyle, GUILayout.Width(85f), GUILayout.Height(26f)))
                p.UseFixedHeight.Value = false;
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void DrawPresets(TerrainForgePlugin p)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.profilename"), _rowStyle, GUILayout.Width(90f), GUILayout.Height(26f));
            _presetName = GUILayout.TextField(_presetName ?? "", GUILayout.Height(26f));
            if (GUILayout.Button(Loc.T("set.profilesave"), _btnStyle, GUILayout.Width(130f), GUILayout.Height(26f)))
            {
                if (!string.IsNullOrEmpty((_presetName ?? "").Trim()))
                {
                    TerrainPresets.SaveCurrent(_presetName.Trim());
                    _presetName = "";
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            List<TerrainPresets.Preset> presets = TerrainPresets.Load();
            if (presets.Count == 0)
            {
                GUILayout.Label(Loc.T("set.profileempty"), _hintStyle);
                return;
            }
            for (int i = 0; i < presets.Count; i++)
            {
                TerrainPresets.Preset pr = presets[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(TerrainUiIcons.Op(pr.Operation)), _iconOnlyStyle,
                    GUILayout.Width(24f), GUILayout.Height(24f));
                GUILayout.Label(new GUIContent(TerrainUiIcons.Shape(pr.Shape)), _iconOnlyStyle,
                    GUILayout.Width(24f), GUILayout.Height(24f));
                GUILayout.Label(pr.Name, _rowStyle, GUILayout.Width(120f), GUILayout.Height(26f));
                GUILayout.Label(PresetSummary(pr), _hintStyle, GUILayout.Width(190f), GUILayout.Height(26f));
                if (GUILayout.Button(Loc.T("set.profileload"), _btnStyle, GUILayout.Width(110f), GUILayout.Height(26f)))
                {
                    TerrainPresets.ApplyPreset(pr);
                    TerrainBrush.RequestClearMarks();
                }
                if (GUILayout.Button("X", _btnStyle, GUILayout.Width(50f), GUILayout.Height(26f)))
                    TerrainPresets.Delete(pr.Name);
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
        }

        private string PresetSummary(TerrainPresets.Preset pr)
        {
            return string.Format("{0} · {1} · r{2}",
                Loc.OpName(pr.Operation), ShapeText(pr.Shape), pr.Radius.ToString("0.#"));
        }

        private void DrawSnapshots()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.profilename"), _rowStyle, GUILayout.Width(90f), GUILayout.Height(26f));
            _snapshotName = GUILayout.TextField(_snapshotName ?? "", GUILayout.Height(26f));
            if (GUILayout.Button(Loc.T("set.profilesave"), _btnStyle, GUILayout.Width(130f), GUILayout.Height(26f)))
            {
                string nm = (_snapshotName ?? "").Trim();
                if (nm.Length == 0)
                    nm = "Snapshot-" + System.DateTime.Now.ToString("yyyyMMdd-HHmm");
                if (TerrainEngine.ExportSnapshot(nm, out string msg))
                    _snapshotName = "";
                TerrainForgePlugin.Message(msg);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            List<string> names = TerrainEngine.SnapshotNames();
            if (names.Count == 0)
            {
                GUILayout.Label(Loc.T("set.snapshotempty"), _hintStyle);
                return;
            }
            for (int i = 0; i < names.Count; i++)
            {
                string nm = names[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(nm, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
                if (GUILayout.Button(Loc.T("set.profileload"), _btnStyle, GUILayout.Width(120f), GUILayout.Height(26f)))
                {
                    TerrainEngine.ImportSnapshot(nm, out string msg);
                    TerrainForgePlugin.Message(msg);
                }
                if (GUILayout.Button("X", _btnStyle, GUILayout.Width(60f), GUILayout.Height(26f)))
                    TerrainEngine.DeleteSnapshot(nm);
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
        }

        private void DrawStamps(TerrainForgePlugin p)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.stampname"), _rowStyle, GUILayout.Width(90f), GUILayout.Height(26f));
            _stampName = GUILayout.TextField(_stampName ?? "", GUILayout.Height(26f));
            if (GUILayout.Button(Loc.T("set.stampsave"), _btnStyle, GUILayout.Width(130f), GUILayout.Height(26f)))
            {
                if (!string.IsNullOrEmpty((_stampName ?? "").Trim()))
                {
                    var stamp = TerrainEngine.GetStamp();
                    if (stamp != null)
                    {
                        TerrainEngine.SaveStamp(_stampName.Trim(), stamp, out string msg);
                        TerrainForgePlugin.Message(msg);
                        _stampName = "";
                    }
                    else
                    {
                        TerrainForgePlugin.Message(Loc.T("set.stampnoclip"));
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            // Load from disk to clipboard
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("set.stampload"), _rowStyle, GUILayout.Width(90f), GUILayout.Height(26f));
            _stampLoadName = GUILayout.TextField(_stampLoadName ?? "", GUILayout.Height(26f));
            if (GUILayout.Button(Loc.T("set.stamploadbtn"), _btnStyle, GUILayout.Width(130f), GUILayout.Height(26f)))
            {
                if (!string.IsNullOrEmpty((_stampLoadName ?? "").Trim()))
                {
                    TerrainEngine.LoadStampToClipboard(_stampLoadName.Trim(), out string msg);
                    TerrainForgePlugin.Message(msg);
                    _stampLoadName = "";
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            List<string> names = TerrainEngine.StampNames();
            if (names.Count == 0)
            {
                GUILayout.Label(Loc.T("set.stampempty"), _hintStyle);
                return;
            }
            for (int i = 0; i < names.Count; i++)
            {
                string nm = names[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(nm, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
                if (GUILayout.Button(Loc.T("set.stamploadbtn"), _btnStyle, GUILayout.Width(120f), GUILayout.Height(26f)))
                {
                    TerrainEngine.LoadStampToClipboard(nm, out string msg);
                    TerrainForgePlugin.Message(msg);
                }
                if (GUILayout.Button("X", _btnStyle, GUILayout.Width(60f), GUILayout.Height(26f)))
                    TerrainEngine.DeleteStamp(nm);
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
        }

        private void DrawHistorySection()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.UndoDepth == null)
                return;

            GUILayout.Label(Loc.T("hist.title"), _hintStyle);

            int depth = Mathf.Clamp(p.UndoDepth.Value, 0, 500);
            float f = GUILayout.HorizontalSlider(depth, 0f, 200f);
            int fromSlider = Mathf.RoundToInt(f);
            if (fromSlider != depth)
            {
                p.UndoDepth.Value = fromSlider;
                _undoDepthText = fromSlider.ToString();
                if (fromSlider <= 0)
                    TerrainEngine.ClearHistory();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("hist.exact"), _rowStyle, GUILayout.Width(250f), GUILayout.Height(26f));
            string t = GUILayout.TextField(_undoDepthText ?? depth.ToString(), GUILayout.Width(235f), GUILayout.Height(26f));
            if (t != _undoDepthText)
            {
                _undoDepthText = t;
                if (int.TryParse(t, out int v))
                {
                    v = Mathf.Clamp(v, 0, 500);
                    if (v != p.UndoDepth.Value)
                    {
                        p.UndoDepth.Value = v;
                        if (v <= 0)
                            TerrainEngine.ClearHistory();
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(Loc.F("hist.saved", TerrainEngine.UndoCount, TerrainEngine.RedoCount), _hintStyle);
        }

        private void DrawSettingsTabContent()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            SettingHeader(Loc.T("set.general"));
            CycleRow(Loc.T("set.language"), p.Language.Value.ToString(), () =>
            {
                var langs = new[]
                {
                    ModLanguage.Auto,
                    ModLanguage.English,
                    ModLanguage.Español,
                    ModLanguage.Français,
                    ModLanguage.Deutsch,
                    ModLanguage.Русский,
                    ModLanguage.中文,
                    ModLanguage.繁體中文,
                    ModLanguage.日本語,
                    ModLanguage.한국어,
                    ModLanguage.Polski,
                    ModLanguage.Português,
                    ModLanguage.Türkçe,
                    ModLanguage.Українська,
                    ModLanguage.Italiano
                };
                int idx = System.Array.IndexOf(langs, p.Language.Value);
                p.Language.Value = langs[(idx + 1) % langs.Length];
            });

            SettingHeader(Loc.T("set.tool"));
            BoolToggle(Loc.T("set.tooltint"), p.ToolTint);
            CycleRow(Loc.T("set.toolcolor"), p.ToolTintColor.Value, () =>
            {
                string[] colors = { "#4FC3F7", "#FFC400", "#8BC34A", "#FF5252", "#AB47BC", "#FFFFFF" };
                int idx = System.Array.IndexOf(colors, p.ToolTintColor.Value);
                p.ToolTintColor.Value = colors[(idx + 1) % colors.Length];
            });
            BoolToggle(Loc.T("set.autogive"), p.AutoGiveTool);
            BoolToggle(Loc.T("set.blockvanilla"), p.BlockVanillaTerrainTools);
            CycleRow(Loc.T("set.stamptool"), StampToolModeText(p.StampToolMode.Value), () =>
            {
                p.StampToolMode.Value = (StampToolMode)(((int)p.StampToolMode.Value + 1) % 4);
            });

            SettingHeader(Loc.T("set.view"));
            BoolToggle(Loc.T("set.preview"), p.ShowPreview);
            BoolToggle(Loc.T("set.guides"), p.ShowGuides);
            BoolToggle(Loc.T("set.slopeheatmap"), p.ShowSlopeHeatmap);
            BoolToggle(Loc.T("set.heightheatmap"), p.ShowHeightHeatmap);
            BoolToggle(Loc.T("set.effects"), p.PlayEffects);

            SettingHeader(Loc.T("set.vanilla"));
            BoolToggle(Loc.T("set.nocost"), p.NoBuildCost);
            BoolToggle(Loc.T("set.protected"), p.AllowInProtectedZones);

            SettingHeader(Loc.T("set.server"));
            BoolToggle(Loc.T("set.deltalimit"), p.UnlockDeltaLimit);
            BoolToggle(Loc.T("set.allow"), p.AllowTerrainEdits);
            BoolToggle(Loc.T("set.adminonly"), p.AdminOnlyEdits);
            BoolToggle(Loc.T("set.respectwards"), p.RespectWards);
            FloatSlider(Loc.T("set.radiuscap"), p.RadiusCap, 0f, 100f, "0.#");
            FloatSlider(Loc.T("set.deltacap"), p.DeltaCap, 0f, 64f, "0.#");
            IntSlider(Loc.T("set.chunkcomps"), p.ChunkCompsPerFrame, 1, 32);
        }

        private void SettingHeader(string text)
        {
            GUILayout.Space(6f);
            GUILayout.Label(text, _topicStyle);
        }

        private void CycleRow(string label, string value, System.Action onClick)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
            if (GUILayout.Button(value, _btnStyle, GUILayout.Width(205f), GUILayout.Height(26f)))
                onClick();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void SegmentedRow(string label, string[] options, int current, System.Action<int> onSelect)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
            for (int i = 0; i < options.Length; i++)
            {
                if (GUILayout.Button(options[i], i == current ? _capturingBtnStyle : _btnStyle, GUILayout.Height(26f)))
                    onSelect(i);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void FloatSlider(string label, ConfigEntry<float> entry, float min, float max, string fmt)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ": " + entry.Value.ToString(fmt), _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
            float v = GUILayout.HorizontalSlider(entry.Value, min, max, GUILayout.Width(205f), GUILayout.Height(26f));
            if (!Mathf.Approximately(v, entry.Value))
                entry.Value = v;
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void IntSlider(string label, ConfigEntry<int> entry, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ": " + entry.Value, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
            int v = Mathf.RoundToInt(GUILayout.HorizontalSlider(entry.Value, min, max, GUILayout.Width(205f), GUILayout.Height(26f)));
            if (v != entry.Value)
                entry.Value = v;
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        private void BoolToggle(string label, ConfigEntry<bool> entry)
        {
            bool v = GUILayout.Toggle(entry.Value, label, _btnStyle, GUILayout.Height(26f));
            if (v != entry.Value)
                entry.Value = v;
            GUILayout.Space(3f);
        }

        private static string ShapeText(TerrainShape s)
        {
            switch (s)
            {
                case TerrainShape.Square: return Loc.T("shape.square");
                case TerrainShape.Line: return Loc.T("shape.line");
                case TerrainShape.Rect: return Loc.T("shape.rect");
                case TerrainShape.Poly: return Loc.T("shape.poly");
                default: return Loc.T("shape.circle");
            }
        }

        private static string RampModeText(RampMode m)
        {
            switch (m)
            {
                case RampMode.FixedAngle: return Loc.T("rampmode.fixed");
                case RampMode.Path: return Loc.T("rampmode.path");
                default: return Loc.T("rampmode.two");
            }
        }

        private static string SmoothModeText(SmoothMode m)
        {
            switch (m)
            {
                case SmoothMode.Bumps: return Loc.T("smooth.bumps");
                case SmoothMode.Dips: return Loc.T("smooth.dips");
                default: return Loc.T("smooth.full");
            }
        }

        private static string FalloffModeText(FalloffMode m)
        {
            switch (m)
            {
                case FalloffMode.Flat: return Loc.T("falloff.flat");
                case FalloffMode.Smooth: return Loc.T("falloff.smooth");
                case FalloffMode.Linear: return Loc.T("falloff.linear");
                default: return Loc.T("falloff.pointy");
            }
        }

        private static string ShoreFalloffModeText(ShoreFalloffMode m)
        {
            switch (m)
            {
                case ShoreFalloffMode.Linear: return Loc.T("shorefalloff.linear");
                case ShoreFalloffMode.Smooth: return Loc.T("shorefalloff.smooth");
                case ShoreFalloffMode.Flat: return Loc.T("shorefalloff.flat");
                default: return Loc.T("shorefalloff.pointy");
            }
        }

        private static string StampToolModeText(StampToolMode m)
        {
            switch (m)
            {
                case StampToolMode.Any: return Loc.T("stamptool.any");
                case StampToolMode.Pickaxe: return Loc.T("stamptool.pickaxe");
                case StampToolMode.TerrainForgeTool: return Loc.T("stamptool.rod");
                default: return Loc.T("stamptool.main");
            }
        }

        private void DrawHelpTabContent()
        {
            Topic(Loc.T("help.what.t"));
            Body(Loc.T("help.what.b"));

            Topic(Loc.T("help.tool.t"));
            Body(Loc.T("help.tool.b"));

            Topic(Loc.T("help.ops.t"));
            Body(Loc.T("help.ops.b"));

            Topic(Loc.T("help.controls.t"));
            Body(Loc.T("help.controls.b"));

            Topic(Loc.T("help.preview.t"));
            Body(Loc.T("help.preview.b"));

            Topic(Loc.T("help.stamp.t"));
            Body(Loc.T("help.stamp.b"));

            Topic(Loc.T("help.mp.t"));
            Body(Loc.T("help.mp.b"));

            Topic(Loc.T("help.save.t"));
            Body(Loc.T("help.save.b"));
        }

        private void Topic(string text)
        {
            GUILayout.Space(10f);
            GUILayout.Label(text, _topicStyle);
        }

        private void Body(string text)
        {
            GUILayout.Label(text, _bodyStyle);
        }

        // ---- skin ----------------------------------------------------------------

        private void EnsureSkin()
        {
            if (_winStyle != null)
                return;

            Font font = TerrainBrush.GameFont();

            // Valheim color palette (shared theme)
            Color valheimDark = TerrainTheme.PanelDark;
            Color valheimHeader = TerrainTheme.PanelHeader;
            Color valheimGold = TerrainTheme.Gold;
            Color valheimGoldBright = TerrainTheme.GoldBright;
            Color valheimPanel = TerrainTheme.PanelBg;
            Color valheimBtn = TerrainTheme.Button;
            Color valheimBtnHover = TerrainTheme.ButtonHover;
            Color valheimBtnActive = TerrainTheme.ButtonActive;
            Color valheimTextGold = TerrainTheme.TextGold;
            Color valheimTextWhite = TerrainTheme.TextWhite;
            Color valheimTextDim = TerrainTheme.TextDim;
            Color valheimTextBright = TerrainTheme.TextBright;

            _texPanel = MakeTex(valheimDark);
            _texHeader = MakeTex(valheimHeader);
            _texPanelBg = MakeTex(valheimPanel);
            _texBtn = MakeTex(valheimBtn);
            _texBtnHover = MakeTex(valheimBtnHover);
            _texBtnActive = MakeTex(valheimBtnActive);
            _texCapture = MakeTex(valheimBtnActive);

            _winStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = _texPanel },
                focused = { background = _texPanel },
                onNormal = { background = _texPanel },
                onFocused = { background = _texPanel },
                padding = new RectOffset(16, 16, 16, 16),
                font = font,
                border = new RectOffset(4, 4, 4, 4),
                stretchWidth = true,
                stretchHeight = true
            };

            _boxStyle = new GUIStyle
            {
                normal = { background = _texPanelBg },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(4, 4, 4, 4),
                stretchWidth = true
            };

            _headerStyle = new GUIStyle { normal = { background = _texHeader }, padding = new RectOffset(12, 12, 8, 8) };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = font
            };
            _titleStyle.normal.textColor = valheimGoldBright;

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                font = font
            };
            _hintStyle.normal.textColor = valheimTextDim;

            _capturingStyle = new GUIStyle(_hintStyle);
            _capturingStyle.normal.textColor = valheimGoldBright;
            _capturingStyle.fontStyle = FontStyle.Bold;

            _rowStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(8, 8, 2, 2),
                font = font
            };
            _rowStyle.normal.textColor = valheimTextWhite;

            _capturingBtnStyle = MakeButtonStyle(font, _texCapture, _texCapture, _texCapture, valheimGoldBright);
            _capturingBtnStyle.fontStyle = FontStyle.Bold;
            _capturingBtnStyle.fontSize = 13;

            _btnStyle = MakeButtonStyle(font, _texBtn, _texBtnHover, _texBtnActive, valheimTextWhite);
            _closeStyle = MakeButtonStyle(font, _texBtn, _texBtnHover, _texBtnActive, new Color(1f, 0.5f, 0.4f, 1f));
            _closeStyle.fontSize = 14;
            _closeStyle.fontStyle = FontStyle.Bold;

            _gripStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                font = font
            };
            _gripStyle.normal.textColor = valheimTextDim;

            _iconBtnStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(3, 3, 3, 3),
                margin = new RectOffset(2, 2, 2, 2),
                font = font,
                normal = { background = _texBtn },
                hover = { background = _texBtnHover },
                active = { background = _texBtnActive }
            };
            _iconBtnSelStyle = new GUIStyle(_iconBtnStyle)
            {
                normal = { background = _texBtnActive },
                hover = { background = _texBtnActive },
                active = { background = _texBtnActive }
            };

            _iconCapStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip,
                font = font
            };
            _iconCapStyle.normal.textColor = valheimTextDim;
            _iconCapSelStyle = new GUIStyle(_iconCapStyle);
            _iconCapSelStyle.normal.textColor = valheimGoldBright;

            _iconOnlyStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(1, 1, 0, 0)
            };

            _tabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(12, 12, 6, 6),
                font = font,
                normal = { background = _texBtn, textColor = valheimTextDim },
                hover = { background = _texBtnHover, textColor = valheimGold },
                active = { background = _texBtnActive, textColor = valheimGoldBright },
                onNormal = { background = _texBtnActive, textColor = valheimGoldBright },
                onHover = { background = _texBtnActive, textColor = valheimGoldBright },
                onActive = { background = _texBtnActive, textColor = valheimGoldBright },
                stretchWidth = true,
                fixedHeight = 30
            };

            _topicStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = font
            };
            _topicStyle.normal.textColor = valheimGold;

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true,
                font = font
            };
            _bodyStyle.normal.textColor = valheimTextWhite;

            _scrollStyle = new GUIStyle(GUI.skin.scrollView)
            {
                normal = { background = _texPanelBg },
                padding = new RectOffset(0, 12, 0, 0)
            };
            _scrollBarStyle = new GUIStyle(GUI.skin.verticalScrollbar)
            {
                normal = { background = MakeTex(new Color(0.1f, 0.1f, 0.12f, 0.5f)) },
                fixedWidth = 14,
                stretchWidth = false
            };
            _scrollBarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb)
            {
                normal = { background = MakeTex(new Color(0.3f, 0.3f, 0.35f, 0.8f)) },
                hover = { background = MakeTex(new Color(0.4f, 0.4f, 0.45f, 0.9f)) },
                active = { background = MakeTex(new Color(0.5f, 0.5f, 0.55f, 1f)) },
                fixedWidth = 12,
                stretchWidth = false,
                padding = new RectOffset(1, 1, 4, 4),
                margin = new RectOffset(0, 0, 4, 4)
            };
        }

        private static GUIStyle MakeButtonStyle(Font font, Texture2D normal, Texture2D hover, Texture2D active, Color text)
        {
            var s = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(8, 8, 4, 4),
                font = font,
                normal = { background = normal, textColor = text },
                hover = { background = hover, textColor = Color.white },
                active = { background = active, textColor = Color.white },
                focused = { background = normal, textColor = text }
            };
            return s;
        }

        private static Texture2D MakeTex(Color c)
        {
            var t = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        private static bool ShortcutDown(KeyboardShortcut s)
        {
            KeyCode main = s.MainKey;
            if (main == KeyCode.None)
                return false;

            bool down;
            switch (main)
            {
                case KeyCode.Mouse0: down = Input.GetMouseButtonDown(0); break;
                case KeyCode.Mouse1: down = Input.GetMouseButtonDown(1); break;
                case KeyCode.Mouse2: down = Input.GetMouseButtonDown(2); break;
                default: down = Input.GetKeyDown(main); break;
            }

            if (!down)
                return false;

            var mods = s.Modifiers;
            if (mods != null)
                foreach (KeyCode m in mods)
                    if (!Input.GetKey(m))
                        return false;

            return true;
        }

        /// <summary>
        /// Cycles to the next saved brush profile/preset.
        /// </summary>
        internal static void CycleNextProfile()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;
            var presets = TerrainPresets.Load();
            if (presets.Count == 0)
                return;

            int idx = presets.FindIndex(x =>
                x.Radius == p.Radius.Value &&
                x.Softness == p.Softness.Value &&
                x.Operation == p.DefaultOperation.Value &&
                x.Shape == p.Shape.Value &&
                x.Paint == p.Paint.Value &&
                x.RampMode == p.RampMode.Value &&
                x.SlopeAngle == p.SlopeAngle.Value &&
                x.SmoothMode == p.SmoothMode.Value &&
                x.RaiseDelta == p.RaiseDelta.Value &&
                x.PaintStrength == p.PaintStrength.Value &&
                x.FalloffPower == p.FalloffPower.Value &&
                x.ShoreOffset == p.ShoreOffset.Value);

            int next = (idx + 1) % presets.Count;
            TerrainPresets.ApplyPreset(presets[next]);
            TerrainBrush.RequestClearMarks();
            TerrainForgePlugin.Message("Perfil: " + presets[next].Name);
        }
    }
}
