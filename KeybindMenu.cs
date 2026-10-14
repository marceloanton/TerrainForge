using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Named brush presets stored in a single config string:
    /// name|radius|softness|op|shape|paint|rampMode|slope|smooth|raiseDelta|paintStrength|falloff|shoreOffset; ...
    /// </summary>
    internal static class TerrainPresets
    {
        private const int MaxSlots = 12;

        internal sealed class Preset
        {
            public string Name = "";
            public float Radius = 8f;
            public float Softness = 50f;
            public TerrainOperation Operation = TerrainOperation.Flatten;
            public TerrainShape Shape = TerrainShape.Circle;
            public TerrainPaint Paint = TerrainPaint.Dirt;
            public RampMode RampMode = RampMode.TwoPoint;
            public float SlopeAngle = 30f;
            public SmoothMode SmoothMode = SmoothMode.Full;
            public float RaiseDelta = 2f;
            public float PaintStrength = 1f;
            public float FalloffPower = 3f;
            public FalloffMode FalloffMode = FalloffMode.Pointy;
            public float ShoreOffset = 0.5f;
        }

        public static List<Preset> Load()
        {
            var list = new List<Preset>();
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.PresetSlots == null)
                return list;
            string raw = p.PresetSlots.Value;
            if (string.IsNullOrEmpty(raw))
                return list;
            string[] entries = raw.Split(';');
            for (int i = 0; i < entries.Length && list.Count < MaxSlots; i++)
            {
                Preset pr = Parse(entries[i]);
                if (pr != null)
                    list.Add(pr);
            }
            return list;
        }

        public static void SaveCurrent(string name)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;
            name = (name ?? "").Trim().Replace("|", " ").Replace(";", " ");
            if (name.Length == 0)
                return;

            var list = Load();
            list.RemoveAll(x => x.Name == name);
            list.Add(Capture(p, name));
            while (list.Count > MaxSlots)
                list.RemoveAt(0);
            Write(list);
        }

        public static void Delete(string name)
        {
            var list = Load();
            list.RemoveAll(x => x.Name == name);
            Write(list);
        }

        public static void ApplyPreset(Preset s)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || s == null)
                return;
            p.Radius.Value = Mathf.Clamp(s.Radius, 1f, 100f);
            p.Softness.Value = Mathf.Clamp(s.Softness, 0f, 100f);
            p.DefaultOperation.Value = s.Operation;
            p.Shape.Value = s.Shape;
            p.Paint.Value = s.Paint;
            p.RampMode.Value = s.RampMode;
            p.SlopeAngle.Value = Mathf.Clamp(s.SlopeAngle, 5f, 60f);
            p.SmoothMode.Value = s.SmoothMode;
            p.RaiseDelta.Value = Mathf.Clamp(s.RaiseDelta, 0.5f, 10f);
            p.PaintStrength.Value = Mathf.Clamp(s.PaintStrength, 0f, 1f);
            p.FalloffPower.Value = Mathf.Clamp(s.FalloffPower, 1f, 5f);
            p.FalloffMode.Value = s.FalloffMode;
            p.ShoreOffset.Value = Mathf.Clamp(s.ShoreOffset, -2f, 5f);
        }

        private static Preset Capture(TerrainForgePlugin p, string name)
        {
            return new Preset
            {
                Name = name,
                Radius = p.Radius.Value,
                Softness = p.Softness.Value,
                Operation = p.DefaultOperation.Value,
                Shape = p.Shape.Value,
                Paint = p.Paint.Value,
                RampMode = p.RampMode.Value,
                SlopeAngle = p.SlopeAngle.Value,
                SmoothMode = p.SmoothMode.Value,
                RaiseDelta = p.RaiseDelta.Value,
                PaintStrength = p.PaintStrength.Value,
                FalloffPower = p.FalloffPower.Value,
                FalloffMode = p.FalloffMode.Value,
                ShoreOffset = p.ShoreOffset.Value
            };
        }

        private static void Write(List<Preset> list)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.PresetSlots == null)
                return;
            System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
            var parts = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                Preset s = list[i];
                parts.Add(string.Join("|", new string[]
                {
                    s.Name,
                    s.Radius.ToString("0.##", inv), s.Softness.ToString("0.##", inv),
                    ((int)s.Operation).ToString(inv), ((int)s.Shape).ToString(inv), ((int)s.Paint).ToString(inv),
                    ((int)s.RampMode).ToString(inv), s.SlopeAngle.ToString("0.##", inv), ((int)s.SmoothMode).ToString(inv),
                    s.RaiseDelta.ToString("0.##", inv), s.PaintStrength.ToString("0.##", inv),
                    s.FalloffPower.ToString("0.##", inv), ((int)s.FalloffMode).ToString(inv), s.ShoreOffset.ToString("0.##", inv)
                }));
            }
            p.PresetSlots.Value = string.Join(";", parts.ToArray());
        }

        private static Preset Parse(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return null;
            string[] f = entry.Split('|');
            if (f.Length != 14)
                return null;
            try
            {
                System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
                var s = new Preset { Name = f[0].Trim() };
                if (s.Name.Length == 0)
                    return null;
                s.Radius = float.Parse(f[1], inv);
                s.Softness = float.Parse(f[2], inv);
                s.Operation = (TerrainOperation)int.Parse(f[3], inv);
                s.Shape = (TerrainShape)int.Parse(f[4], inv);
                s.Paint = (TerrainPaint)int.Parse(f[5], inv);
                s.RampMode = (RampMode)int.Parse(f[6], inv);
                s.SlopeAngle = float.Parse(f[7], inv);
                s.SmoothMode = (SmoothMode)int.Parse(f[8], inv);
                s.RaiseDelta = float.Parse(f[9], inv);
                s.PaintStrength = float.Parse(f[10], inv);
                s.FalloffPower = float.Parse(f[11], inv);
                s.FalloffMode = (FalloffMode)int.Parse(f[12], inv);
                s.ShoreOffset = float.Parse(f[13], inv);
                if (!System.Enum.IsDefined(typeof(TerrainOperation), s.Operation)) return null;
                if (!System.Enum.IsDefined(typeof(TerrainShape), s.Shape)) return null;
                if (!System.Enum.IsDefined(typeof(TerrainPaint), s.Paint)) return null;
                if (!System.Enum.IsDefined(typeof(RampMode), s.RampMode)) return null;
                if (!System.Enum.IsDefined(typeof(SmoothMode), s.SmoothMode)) return null;
                if (!System.Enum.IsDefined(typeof(FalloffMode), s.FalloffMode)) return null;
                return s;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// In-mod key rebinding UI with a custom skin. Toggle with the configured menu
    /// key (default F5); click a binding then press a key / mouse button. Saved to config.
    /// </summary>
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
        private Rect _window = new Rect(40f, 60f, 520f, 0f);

        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;
        private string _undoDepthText;
        private string _presetName = "";
        private ModLanguage _langBuilt;
        private string _fixedHeightText;
        private string _snapshotName = "";
        private string _stampName = "";
        private string _stampLoadName = "";

        private GUIStyle _winStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _hintStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _btnStyle;
        private GUIStyle _closeStyle;
        private GUIStyle _capturingStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _topicStyle;
        private GUIStyle _boxStyle;

        private Texture2D _texPanel;
        private Texture2D _texHeader;
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
            if (GUILayout.Toggle(_tab == 0, Loc.T("tab.keys"), _tabStyle, GUILayout.Height(28f)))
                _tab = 0;
            if (GUILayout.Toggle(_tab == 1, Loc.T("tab.help"), _tabStyle, GUILayout.Height(28f)))
                _tab = 1;
            if (GUILayout.Toggle(_tab == 2, Loc.T("tab.settings"), _tabStyle, GUILayout.Height(28f)))
                _tab = 2;
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            // Main scroll view for all tab content
            _mainScroll = GUILayout.BeginScrollView(_mainScroll, _boxStyle);

            if (_tab == 0)
                DrawKeysTab();
            else if (_tab == 1)
                DrawHelpTabContent();
            else
                DrawSettingsTabContent();

            GUILayout.EndScrollView();

            GUILayout.Space(6f);
            if (GUILayout.Button("Invitame un café en Cafecito", _btnStyle, GUILayout.Height(22f)))
            {
                try { Application.OpenURL("https://cafecito.app/byrka"); } catch { }
            }

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 34f));
        }

        private void DrawKeysTab()
        {
            GUILayout.Label(_capturing >= 0 ? Loc.T("keys.hint.capture") : Loc.T("keys.hint.idle"),
                _capturing >= 0 ? _capturingStyle : _hintStyle);
            GUILayout.Space(8f);

            for (int i = 0; i < _entries.Count; i++)
            {
                GUILayout.BeginHorizontal();

                Color prev = GUI.color;
                GUI.color = _capturing == i ? new Color(1f, 0.85f, 0.45f) : Color.white;
                GUILayout.Label(_entries[i].Label, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
                GUI.color = prev;

                string text = _capturing == i ? Loc.T("keys.press") : _entries[i].Bind.Value.ToString();
                if (GUILayout.Button(text, _capturing == i ? _capturingStyle : _btnStyle, GUILayout.Width(205f), GUILayout.Height(26f)))
                    _capturing = i;

                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }

            GUILayout.Space(10f);
            DrawHistorySection();

            GUILayout.Space(6f);
            if (GUILayout.Button(Loc.T("btn.get"), _btnStyle, GUILayout.Height(28f)))
                TerrainTool.Give(Player.m_localPlayer);

            GUILayout.Space(6f);
            if (GUILayout.Button(Loc.T("btn.close"), _btnStyle, GUILayout.Height(28f)))
                Toggle(false);
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
                GUILayout.Label(pr.Name, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
                if (GUILayout.Button(Loc.T("set.profileload"), _btnStyle, GUILayout.Width(120f), GUILayout.Height(26f)))
                {
                    TerrainPresets.ApplyPreset(pr);
                    TerrainBrush.RequestClearMarks();
                }
                if (GUILayout.Button("X", _btnStyle, GUILayout.Width(60f), GUILayout.Height(26f)))
                    TerrainPresets.Delete(pr.Name);
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
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
                        TerrainForgePlugin.Message("No hay stamp en el portapapeles para guardar.");
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
            GUILayout.Label(Loc.T("hist.exact"), _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
            string t = GUILayout.TextField(_undoDepthText ?? depth.ToString(), GUILayout.Width(205f), GUILayout.Height(26f));
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

        private void DrawSettingsTab()
        {
            DrawSettingsTabContent();

            GUILayout.Space(8f);
            if (GUILayout.Button(Loc.T("btn.close"), _btnStyle, GUILayout.Height(28f)))
                Toggle(false);
        }

        private void DrawSettingsTabContent()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            SettingHeader(Loc.T("set.brush"));
            CycleRow(Loc.T("set.operation"), p.DefaultOperation.Value.ToString(), () =>
            {
                p.DefaultOperation.Value = (TerrainOperation)(((int)p.DefaultOperation.Value + 1) % 9);
                TerrainBrush.RequestClearMarks();
            });
            CycleRow(Loc.T("set.shape"), ShapeText(p.Shape.Value), () =>
            {
                p.Shape.Value = (TerrainShape)(((int)p.Shape.Value + 1) % 5);
                TerrainBrush.RequestClearMarks();
            });
            CycleRow(Loc.T("set.paint"), p.Paint.Value.ToString(), () =>
            {
                p.Paint.Value = (TerrainPaint)(((int)p.Paint.Value + 1) % 6);
            });
            CycleRow(Loc.T("set.rampmode"), RampModeText(p.RampMode.Value), () =>
            {
                p.RampMode.Value = (RampMode)(((int)p.RampMode.Value + 1) % 3);
                TerrainBrush.RequestClearMarks();
            });
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

            DrawFixedHeight(p);

            SettingHeader(Loc.T("set.profiles"));
            DrawPresets(p);

            SettingHeader(Loc.T("set.snapshots"));
            DrawSnapshots();

            SettingHeader(Loc.T("set.stamps"));
            DrawStamps(p);

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

            SettingHeader(Loc.T("set.switches"));
            BoolToggle(Loc.T("set.preview"), p.ShowPreview);
            BoolToggle(Loc.T("set.guides"), p.ShowGuides);
            BoolToggle(Loc.T("set.effects"), p.PlayEffects);
            BoolToggle(Loc.T("set.autogive"), p.AutoGiveTool);
            BoolToggle(Loc.T("set.nocost"), p.NoBuildCost);
            BoolToggle(Loc.T("set.protected"), p.AllowInProtectedZones);
            BoolToggle(Loc.T("set.deltalimit"), p.UnlockDeltaLimit);
            BoolToggle(Loc.T("set.allow"), p.AllowTerrainEdits);
            BoolToggle(Loc.T("set.adminonly"), p.AdminOnlyEdits);
            BoolToggle(Loc.T("set.respectwards"), p.RespectWards);
            CycleRow(Loc.T("set.stamptool"), StampToolModeText(p.StampToolMode.Value), () =>
            {
                p.StampToolMode.Value = (StampToolMode)(((int)p.StampToolMode.Value + 1) % 4);
            });
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

private void DrawHelpTab()
        {
            DrawHelpTabContent();
            GUILayout.Space(8f);
            if (GUILayout.Button("Cerrar", _btnStyle, GUILayout.Height(28f)))
                Toggle(false);
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

            _texPanel = MakeTex(new Color(0.09f, 0.10f, 0.12f, 0.96f));
            _texHeader = MakeTex(new Color(0.15f, 0.16f, 0.20f, 1f));
            _texBtn = MakeTex(new Color(0.20f, 0.21f, 0.25f, 1f));
            _texBtnHover = MakeTex(new Color(0.30f, 0.32f, 0.38f, 1f));
            _texBtnActive = MakeTex(new Color(0.62f, 0.48f, 0.20f, 1f));
            _texCapture = MakeTex(new Color(0.42f, 0.34f, 0.14f, 1f));

            _winStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = _texPanel },
                focused = { background = _texPanel },
                onNormal = { background = _texPanel },
                onFocused = { background = _texPanel },
                padding = new RectOffset(12, 12, 12, 12),
                font = font,
                border = new RectOffset(0, 0, 0, 0)
            };

            _boxStyle = new GUIStyle
            {
                normal = { background = _texPanel },
                padding = new RectOffset(8, 8, 8, 8)
            };

            _headerStyle = new GUIStyle { normal = { background = _texHeader }, padding = new RectOffset(10, 6, 6, 6) };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = font
            };
            _titleStyle.normal.textColor = new Color(0.92f, 0.80f, 0.46f);

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                font = font
            };
            _hintStyle.normal.textColor = new Color(0.72f, 0.74f, 0.78f);

            _capturingStyle = new GUIStyle(_hintStyle);
            _capturingStyle.normal.textColor = new Color(1f, 0.85f, 0.45f);
            _capturingStyle.fontStyle = FontStyle.Bold;

            _rowStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(6, 6, 0, 0),
                font = font
            };
            _rowStyle.normal.textColor = new Color(0.88f, 0.89f, 0.92f);

            _btnStyle = MakeButtonStyle(font, _texBtn, _texBtnHover, _texBtnActive, new Color(0.92f, 0.93f, 0.95f));
            _closeStyle = MakeButtonStyle(font, _texBtn, _texBtnHover, _texBtnActive, new Color(0.95f, 0.6f, 0.55f));
            _closeStyle.fontSize = 16;
            _closeStyle.fontStyle = FontStyle.Bold;

            _tabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(10, 10, 4, 4),
                font = font,
                normal = { background = _texBtn, textColor = new Color(0.78f, 0.80f, 0.84f) },
                hover = { background = _texBtnHover, textColor = Color.white },
                active = { background = _texBtnActive, textColor = Color.white },
                onNormal = { background = _texBtnActive, textColor = Color.white },
                onHover = { background = _texBtnActive, textColor = Color.white },
                onActive = { background = _texBtnActive, textColor = Color.white }
            };

            _topicStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = font
            };
            _topicStyle.normal.textColor = new Color(0.92f, 0.80f, 0.46f);

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true,
                font = font
            };
            _bodyStyle.normal.textColor = new Color(0.82f, 0.84f, 0.88f);
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
