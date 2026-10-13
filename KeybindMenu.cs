using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
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
        private Vector2 _helpScroll;
        private Rect _window = new Rect(40f, 60f, 520f, 0f);

        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;
        private string _undoDepthText;

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

        private void Start()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

            _entries.Add(new Entry { Label = "Aplicar (mantener)", Bind = p.ApplyKey });
            _entries.Add(new Entry { Label = "Subir radio", Bind = p.RadiusUpKey });
            _entries.Add(new Entry { Label = "Bajar radio", Bind = p.RadiusDownKey });
            _entries.Add(new Entry { Label = "Cambiar operacion", Bind = p.CycleOperationKey });
            _entries.Add(new Entry { Label = "Cambiar forma", Bind = p.CycleShapeKey });
            _entries.Add(new Entry { Label = "Cambiar pintura", Bind = p.CyclePaintKey });
            _entries.Add(new Entry { Label = "Nivelar a tus pies (mod.)", Bind = p.AimHeightModifierKey });
            _entries.Add(new Entry { Label = "Deshacer", Bind = p.UndoKey });
            _entries.Add(new Entry { Label = "Rehacer", Bind = p.RedoKey });
            _entries.Add(new Entry { Label = "Obtener herramienta", Bind = p.GiveToolKey });
            _entries.Add(new Entry { Label = "Abrir este menu", Bind = p.RebindMenuKey });
        }

        private void Update()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;

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
            for (int i = 0; i < AllKeys.Length; i++)
            {
                KeyCode kc = AllKeys[i];
                if (kc == KeyCode.None)
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
                if (p != null && p.UndoDepth != null)
                    _undoDepthText = p.UndoDepth.Value.ToString();
            }
            else
            {
                _capturing = -1;
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
            if (GUILayout.Toggle(_tab == 0, "TECLAS", _tabStyle, GUILayout.Height(28f)))
                _tab = 0;
            if (GUILayout.Toggle(_tab == 1, "AYUDA", _tabStyle, GUILayout.Height(28f)))
                _tab = 1;
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (_tab == 0)
                DrawKeysTab();
            else
                DrawHelpTab();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 34f));
        }

        private void DrawKeysTab()
        {
            GUILayout.Label(_capturing >= 0
                ? "Pulsa una tecla o boton del raton...   (Esc cancela)"
                : "Haz click en una tecla y luego pulsa la nueva.", _capturing >= 0 ? _capturingStyle : _hintStyle);
            GUILayout.Space(8f);

            for (int i = 0; i < _entries.Count; i++)
            {
                GUILayout.BeginHorizontal();

                Color prev = GUI.color;
                GUI.color = _capturing == i ? new Color(1f, 0.85f, 0.45f) : Color.white;
                GUILayout.Label(_entries[i].Label, _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
                GUI.color = prev;

                string text = _capturing == i ? "< pulsa una tecla... >" : _entries[i].Bind.Value.ToString();
                if (GUILayout.Button(text, _capturing == i ? _capturingStyle : _btnStyle, GUILayout.Width(205f), GUILayout.Height(26f)))
                    _capturing = i;

                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }

            GUILayout.Space(10f);
            DrawHistorySection();

            GUILayout.Space(6f);
            if (GUILayout.Button("Obtener la herramienta ahora", _btnStyle, GUILayout.Height(28f)))
                TerrainTool.Give(Player.m_localPlayer);

            GUILayout.Space(6f);
            if (GUILayout.Button("Cerrar", _btnStyle, GUILayout.Height(28f)))
                Toggle(false);
        }

        private void DrawHistorySection()
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.UndoDepth == null)
                return;

            GUILayout.Label("Historial de deshacer (pasos guardados)", _hintStyle);

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
            GUILayout.Label("Exacto (0-500):", _rowStyle, GUILayout.Width(215f), GUILayout.Height(26f));
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

            GUILayout.Label("Guardados: deshacer " + TerrainEngine.UndoCount +
                            "  ·  rehacer " + TerrainEngine.RedoCount, _hintStyle);
        }

        private void DrawHelpTab()
        {
            _helpScroll = GUILayout.BeginScrollView(_helpScroll, _boxStyle, GUILayout.Height(400f));

            Topic("Que hace");
            Body("TerrainForge esculpe el terreno libremente: sube/baja mas de 8 m, " +
                 "suaviza, crea rampas, pinta y restaura, sin las ataduras del pico o la " +
                 "azada vanilla. Los cambios se guardan en el mundo y se replican a los " +
                 "demas jugadores.");

            Topic("Herramienta");
            Body("\u2022 'TerrainForge Rod': la varita del mod (clon del martillo). Se te " +
                 "entrega automaticamente al entrar al mundo y se equipa sola; tambien " +
                 "podes obtenerla con la tecla \"Obtener herramienta\" o desde esta ventana.\n" +
                 "\u2022 Debe estar EQUIPADA para que la brocha funcione. Si el registro del " +
                 "objeto fallara, la brocha queda usable igual.");

            Topic("Operaciones  (cambiar con \"Cambiar operacion\")");
            Body("\u2022 Flatten: aplana a la altura apuntada (mantene Shift para nivelar a tus pies).\n" +
                 "\u2022 Raise: sube el terreno.\n" +
                 "\u2022 Lower: baja el terreno.\n" +
                 "\u2022 Smooth: promedia cada punto con sus vecinos y suaviza el terreno " +
                 "de verdad (bordes, escalones y ruido). Mantenelo para suavizar mas.\n" +
                 "\u2022 Ramp: marca un punto A y luego un punto B con la tecla Aplicar; se " +
                 "crea una rampa recta entre ambos que une las dos alturas. El ancho es " +
                 "el radio de la brocha y \"Softness\" suaviza solo los bordes laterales.\n" +
                 "\u2022 Paint: pinta el terreno (Dirt, Cultivated, Paved, hierba/vegetacion...).\n" +
                 "\u2022 Restore: devuelve el terreno a su estado original.");

            Topic("Controles");
            Body("\u2022 Mantener \"Aplicar\" para esculpir (en Ramp: cada pulsacion marca " +
                 "un punto A o B).\n" +
                 "\u2022 \"Subir/Bajar radio\" o Ctrl + rueda del raton: tamano de la brocha " +
                 "(en Ramp es el ancho de la rampa).\n" +
                 "\u2022 \"Cambiar forma\": circulo o cuadrado.\n" +
                 "\u2022 \"Cambiar pintura\": tipo de pintura.\n" +
                 "\u2022 \"Nivelar a tus pies\" (Shift): Flatten toma esa altura en " +
                 "vez de la apuntada.\n" +
                 "\u2022 El HUD (arriba a la izquierda) muestra operacion, radio, forma y " +
                 "pintura actuales.\n" +
                 "\u2022 \"Deshacer\" (Ctrl+Z) revierte la ultima aplicacion y \"Rehacer\" " +
                 "(Ctrl+Y) la recupera. La profundidad del historial se ajusta en la " +
                 "pesta\u00F1a TECLAS (0 lo desactiva y borra lo guardado).");

            Topic("Vista previa");
            Body("\u2022 La linea solida marca el borde real: donde se genera terreno.\n" +
                 "\u2022 Guias tenues alrededor (mismo color de cada operacion): zona de fuerza " +
                 "plena, radio maximo y distancia maxima de apuntado.\n" +
                 "\u2022 En Ramp, marca A y B y veras el corredor real (las dos lineas del ancho).\n" +
                 "\u2022 Se pueden desactivar en la config (Preview > ShowGuides).");

            Topic("Multijugador");
            Body("\u2022 El mod debe estar en el cliente Y en el servidor.\n" +
                 "\u2022 \"UnlockDeltaLimit\" debe estar activado tambien en el servidor; si no, " +
                 "el servidor recorta los cambios de mas de 8 m.\n" +
                 "\u2022 Radios muy grandes pueden desincronizar a clientes que no tengan el mod.");

            Topic("Recargar / guardar");
            Body("Las teclas se guardan solas al reasignarlas. La herramienta y el resto de " +
                 "ajustes viven en el archivo de configuracion del mod (BepInEx/config).");

            GUILayout.EndScrollView();

            GUILayout.Space(8f);
            if (GUILayout.Button("Cerrar", _btnStyle, GUILayout.Height(28f)))
                Toggle(false);
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
    }
}
