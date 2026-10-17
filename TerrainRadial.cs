using System;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Hold-to-open radial wheel for picking the brush operation with the mouse.
    /// Release confirms the hovered sector, Esc cancels. IMGUI only, no assets:
    /// the wheel is drawn from a texture generated at runtime so it scales to
    /// however many operations the enum holds.
    /// </summary>
    internal sealed class TerrainRadial : MonoBehaviour
    {
        public static bool Open { get; private set; }

        private static readonly TerrainOperation[] s_ops =
            (TerrainOperation[])Enum.GetValues(typeof(TerrainOperation));

        private const int TexSize = 384;

        private int _sel = -1;
        private bool _cancel;
        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;

        private Texture2D _wheel;
        private int _wheelSel = int.MinValue;
        private TerrainPaint _wheelPaint;
        private Texture2D _hubTex;

        private GUIStyle _labelStyle;
        private GUIStyle _labelSelStyle;
        private GUIStyle _centerLabelStyle;
        private GUIStyle _hintStyle;

        private static float Half => TexSize * 0.5f;
        private static float InnerR => TexSize * 0.32f;
        private static float OuterR => TexSize * 0.49f;

        // Valheim-ish palette (shared theme).
        private static readonly Color SectorDark = TerrainTheme.SectorDark;
        private static readonly Color SectorGold = TerrainTheme.SectorGold;
        private static readonly Color HubColor = TerrainTheme.Hub;
        private static readonly Color GoldText = TerrainTheme.TextGold;
        private static readonly Color WarmWhite = TerrainTheme.TextWhite;
        private static readonly Color DarkText = TerrainTheme.DarkText;
        private static readonly Color DimText = TerrainTheme.TextDim;

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

            if (Console.IsVisible() || InventoryGui.IsVisible() || TextInput.IsVisible())
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
                    _cursorWasVisible = Cursor.visible;
                    _cursorLock = Cursor.lockState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                _cancel = true;

            // Mouse wheel cycles the selection and it PERSISTS: hover only
            // overrides it while the pointer actually sits over a sector.
            float scroll = TerrainBrush.WheelDelta();
            if (Mathf.Abs(scroll) > 0.01f)
            {
                int dir = scroll > 0f ? -1 : 1; // up = previous, down = next (like Valheim)
                _sel = (_sel + dir + s_ops.Length) % s_ops.Length;
            }
            else
            {
                int hover = HoverFromMouse();
                if (hover >= 0)
                    _sel = hover;
            }

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
            float distSq = dx * dx + dy * dy;
            float r0 = InnerR, r1 = OuterR + 6f;
            if (distSq < r0 * r0 || distSq > r1 * r1)
                return -1;
            // Input.mousePosition has its origin bottom-left (+y is up), which is the
            // same "up" the labels use, so no sign flip: deg is already the label angle.
            float deg = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg; // up = +90
            float cw = ((90f - deg) % 360f + 360f) % 360f;    // 0 = top, clockwise
            float step = 360f / s_ops.Length;
            int idx = (int)((cw + step * 0.5f) / step) % s_ops.Length;
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

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            TerrainPaint paint = p.Paint.Value;
            EnsureWheel(paint);

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.DrawTexture(new Rect(cx - Half, cy - Half, TexSize, TexSize), _wheel, ScaleMode.StretchToFill, true);

            float rmid = (InnerR + OuterR) * 0.5f;
            float step = 360f / s_ops.Length;
            for (int i = 0; i < s_ops.Length; i++)
            {
                float rad = (90f - i * step) * Mathf.Deg2Rad;
                float lx = cx + Mathf.Cos(rad) * rmid;
                float ly = cy - Mathf.Sin(rad) * rmid;
                var rect = new Rect(lx - 56f, ly - 12f, 112f, 24f);
                GUI.Label(rect, Loc.OpName(s_ops[i]), i == _sel ? _labelSelStyle : _labelStyle);
            }

            GUI.DrawTexture(new Rect(cx - InnerR, cy - InnerR, InnerR * 2f, InnerR * 2f), _hubTex);

            string sel = _sel >= 0 ? Loc.OpName(s_ops[_sel]) : Loc.OpName(p.DefaultOperation.Value);
            GUI.Label(new Rect(cx - InnerR, cy - 14f, InnerR * 2f, 28f), sel, _centerLabelStyle);

            GUI.Label(new Rect(cx - OuterR, cy + OuterR - 26f, OuterR * 2f, 22f),
                _cancel ? Loc.T("radial.cancel") : Loc.T("radial.hint"), _hintStyle);
        }

        private void EnsureWheel(TerrainPaint paint)
        {
            if (_wheel != null && _wheelSel == _sel && _wheelPaint == paint)
                return;
            if (_wheel != null)
                Destroy(_wheel);
            _wheel = BuildWheel(_sel, paint);
            _wheelSel = _sel;
            _wheelPaint = paint;
        }

        private static Texture2D BuildWheel(int sel, TerrainPaint paint)
        {
            int s = TexSize;
            var t = new Texture2D(s, s, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
            var px = new Color32[s * s];

            float r = s * 0.5f;
            float r0 = InnerR, r1 = OuterR;
            float step = 360f / s_ops.Length;

            for (int y = 0; y < s; y++)
            {
                float dy = y - r + 0.5f;
                for (int x = 0; x < s; x++)
                {
                    float dx = x - r + 0.5f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    int idx = y * s + x;

                    if (d < r0 || d > r1)
                    {
                        px[idx] = new Color(0f, 0f, 0f, 0f);
                        continue;
                    }

                    // The texture's vertical axis is mirrored relative to GUI screen
                    // space (a drawn texture shows SetPixels row 0 at the TOP of the
                    // rect), so a texel's on-screen up-direction is -dy. Negating dy
                    // keeps the painted sectors aligned with the labels and the hover
                    // test, which are both computed in screen space. With a fixed sign
                    // the sector boundaries fall on the label centres (odd sector
                    // counts), which looks like broken slices with offset text.
                    float deg = Mathf.Atan2(-dy, dx) * Mathf.Rad2Deg;
                    float cw = ((90f - deg) % 360f + 360f) % 360f;
                    int i = (int)((cw + step * 0.5f) / step) % s_ops.Length;

                    Color c = i == sel ? SectorGold : SectorDark;
                    float within = Mathf.Repeat(cw, step);
                    float edge = Mathf.Min(within, step - within);
                    if (edge < 1.2f)
                        c = Color.Lerp(c, new Color(0f, 0f, 0f, c.a), 0.55f); // crisp opaque seam

                    px[idx] = new Color(c.r, c.g, c.b, c.a);
                }
            }

            t.SetPixels32(px);
            t.Apply();
            return t;
        }

        private static Texture2D BuildDisc(int size, Color c)
        {
            var t = new Texture2D(size, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
            var px = new Color32[size * size];
            float r = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                float dy = y - r + 0.5f;
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f;
                    bool inside = dx * dx + dy * dy <= r * r;
                    px[y * size + x] = inside ? (Color32)c : new Color32(0, 0, 0, 0);
                }
            }
            t.SetPixels32(px);
            t.Apply();
            return t;
        }

        private void EnsureStyle()
        {
            if (_labelStyle != null)
                return;

            Font font = TerrainBrush.GameFont();
            _hubTex = BuildDisc(TexSize, HubColor);

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                font = font
            };
            _labelStyle.normal.textColor = WarmWhite;

            _labelSelStyle = new GUIStyle(_labelStyle);
            _labelSelStyle.normal.textColor = DarkText;

            _centerLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                font = font
            };
            _centerLabelStyle.normal.textColor = GoldText;

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                font = font
            };
            _hintStyle.normal.textColor = DimText;
        }

        private void OnDestroy()
        {
            if (_wheel != null)
                Destroy(_wheel);
            if (_hubTex != null)
                Destroy(_hubTex);
        }
    }
}
