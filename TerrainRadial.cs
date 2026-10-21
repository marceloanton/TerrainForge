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

        private enum Page { Operations, Tools }
        private enum ToolAction { Copy, Paste, Undo, Redo, PreviousStamp, NextStamp }
        private static readonly ToolAction[] s_tools = (ToolAction[])Enum.GetValues(typeof(ToolAction));

        private static readonly TerrainOperation[] s_ops =
        {
            TerrainOperation.Flatten, TerrainOperation.Raise, TerrainOperation.Lower,
            TerrainOperation.Smooth, TerrainOperation.Paint, TerrainOperation.Restore,
            TerrainOperation.Ramp, TerrainOperation.Shore, TerrainOperation.CopyPaste
        };

        private const int TexSize = 384;

        private int _sel = -1;
        private Page _page;
        private bool _cancel;
        private bool _cursorWasVisible;
        private CursorLockMode _cursorLock;
        private bool _scrollSelectionActive;
        private Vector2 _lastMousePosition;

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
                    _page = Page.Operations;
                    _sel = Array.IndexOf(s_ops, p.DefaultOperation.Value);
                    if (_sel < 0) _sel = 0;
                    _cancel = false;
                    _scrollSelectionActive = false;
                    _lastMousePosition = Input.mousePosition;
                    _cursorWasVisible = Cursor.visible;
                    _cursorLock = Cursor.lockState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
                return;
            }

            if (TerrainBrush.Pressed(p.RadialCancelKey.Value))
                _cancel = true;

            if (TerrainBrush.Pressed(p.RadialToolsKey.Value))
            {
                _page = _page == Page.Operations ? Page.Tools : Page.Operations;
                _sel = _page == Page.Operations
                    ? Mathf.Max(0, Array.IndexOf(s_ops, p.DefaultOperation.Value))
                    : 0;
                _scrollSelectionActive = false;
            }

            int count = _page == Page.Operations ? s_ops.Length : s_tools.Length;

            // A wheel step owns selection until the pointer moves. Otherwise a
            // stationary pointer over a sector would immediately undo the step.
            Vector2 mouse = Input.mousePosition;
            float scroll = TerrainBrush.WheelDelta();
            if (Mathf.Abs(scroll) > 0.01f)
            {
                int dir = scroll > 0f ? -1 : 1; // up = previous, down = next (like Valheim)
                _sel = (_sel + dir + count) % count;
                _scrollSelectionActive = true;
            }
            else
            {
                if (_scrollSelectionActive && (mouse - _lastMousePosition).sqrMagnitude > 9f)
                    _scrollSelectionActive = false;
                if (!_scrollSelectionActive)
                {
                    int hover = HoverFromMouse(mouse);
                    if (hover >= 0)
                        _sel = hover;
                }
            }
            _lastMousePosition = mouse;

            if (TerrainBrush.Pressed(p.RadialConfirmKey.Value) && !_cancel && _sel >= 0)
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
            if (_page == Page.Tools)
            {
                if (_sel < 0 || _sel >= s_tools.Length) return;
                switch (s_tools[_sel])
                {
                    case ToolAction.Copy: TerrainBrush.RequestCopyStamp(); break;
                    case ToolAction.Paste: TerrainBrush.RequestPasteStamp(); break;
                    case ToolAction.Undo: TerrainBrush.RequestUndo(); break;
                    case ToolAction.Redo: TerrainBrush.RequestRedo(); break;
                    case ToolAction.PreviousStamp: TerrainBrush.RequestCycleStampBack(); break;
                    case ToolAction.NextStamp: TerrainBrush.RequestCycleStamp(); break;
                }
                return;
            }
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

        private static int HoverFromMouse(Vector2 mouse)
        {
            float dx = mouse.x - Screen.width * 0.5f;
            float dy = mouse.y - Screen.height * 0.5f;
            float distSq = dx * dx + dy * dy;
            float r0 = InnerR, r1 = OuterR + 6f;
            if (distSq < r0 * r0 || distSq > r1 * r1)
                return -1;
            return SectorFromDirection(dx, dy, s_ops.Length);
        }

        // dx/dy are in screen-style Cartesian coordinates: +x right, +y up.
        // Operation 0 starts at the top and sectors proceed clockwise.
        private static int SectorFromDirection(float dx, float dy, int count)
        {
            float deg = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            float clockwise = Mathf.Repeat(90f - deg, 360f);
            float step = 360f / count;
            return Mathf.FloorToInt((clockwise + step * 0.5f) / step) % count;
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
            int count = _page == Page.Operations ? s_ops.Length : s_tools.Length;
            EnsureWheel(paint, count);

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.DrawTexture(new Rect(cx - Half, cy - Half, TexSize, TexSize), _wheel, ScaleMode.StretchToFill, true);

            float rmid = (InnerR + OuterR) * 0.5f;
            float step = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float rad = (90f - i * step) * Mathf.Deg2Rad;
                float lx = cx + Mathf.Cos(rad) * rmid;
                float ly = cy - Mathf.Sin(rad) * rmid;
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(lx - 15f, ly - 28f, 30f, 30f), IconAt(i),
                    ScaleMode.ScaleToFit, true);
                var rect = new Rect(lx - 54f, ly + 3f, 108f, 20f);
                GUI.Label(rect, NameAt(i), i == _sel ? _labelSelStyle : _labelStyle);
            }

            GUI.DrawTexture(new Rect(cx - InnerR, cy - InnerR, InnerR * 2f, InnerR * 2f), _hubTex);

            string sel = _sel >= 0 ? NameAt(_sel) : Loc.OpName(p.DefaultOperation.Value);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(cx - 19f, cy - 50f, 38f, 38f), _sel >= 0 ? IconAt(_sel) : TerrainUiIcons.Op(p.DefaultOperation.Value),
                ScaleMode.ScaleToFit, true);
            GUI.Label(new Rect(cx - InnerR, cy - 9f, InnerR * 2f, 28f), sel, _centerLabelStyle);

            GUI.Label(new Rect(cx - 100f, cy - 104f, 200f, 22f),
                Loc.T(_page == Page.Operations ? "radial.page.ops" : "radial.page.tools"), _centerLabelStyle);

            GUI.Label(new Rect(cx - OuterR, cy + OuterR - 26f, OuterR * 2f, 22f),
                _cancel ? Loc.T("radial.cancel") : Loc.F("radial.hint", p.RadialKey.Value, p.RadialToolsKey.Value, p.RadialConfirmKey.Value, p.RadialCancelKey.Value), _hintStyle);
        }

        private Texture IconAt(int i) => _page == Page.Operations
            ? TerrainUiIcons.Op(s_ops[i]) : TerrainUiIcons.Tool(ToolId(s_tools[i]));

        private string NameAt(int i) => _page == Page.Operations
            ? Loc.OpName(s_ops[i]) : Loc.T("radial.tool." + ToolId(s_tools[i]));

        private static string ToolId(ToolAction action)
        {
            switch (action)
            {
                case ToolAction.Copy: return "copy";
                case ToolAction.Paste: return "paste";
                case ToolAction.Undo: return "undo";
                case ToolAction.Redo: return "redo";
                case ToolAction.PreviousStamp: return "prev";
                default: return "next";
            }
        }

        private void EnsureWheel(TerrainPaint paint, int count)
        {
            int pageSel = _sel + (_page == Page.Tools ? 100 : 0);
            if (_wheel != null && _wheelSel == pageSel && _wheelPaint == paint)
                return;
            if (_wheel != null)
                Destroy(_wheel);
            _wheel = BuildWheel(_sel, paint, count, _page);
            _wheelSel = pageSel;
            _wheelPaint = paint;
        }

        private static Texture2D BuildWheel(int sel, TerrainPaint paint, int count, Page page)
        {
            int s = TexSize;
            var t = new Texture2D(s, s, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
            var px = new Color32[s * s];

            float r = s * 0.5f;
            float r0 = InnerR, r1 = OuterR;
            float step = 360f / count;

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

                    // Texture2D pixel coordinates also use +y upward. Use the same
                    // sector calculation as pointer hover so wedges, seams and labels
                    // share one angular origin.
                    float cw = Mathf.Repeat(90f - Mathf.Atan2(dy, dx) * Mathf.Rad2Deg, 360f);
                    int i = SectorFromDirection(dx, dy, count);

                    Color c = i == sel ? SectorGold : SectorDark;
                    // Sectors are centred on their labels, so seams sit half a step
                    // away from each centre rather than cutting through its label.
                    float within = Mathf.Repeat(cw + step * 0.5f, step);
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
