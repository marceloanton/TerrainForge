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
        private bool _serverAllowed = true;
        private string _serverWhy;

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
                return;
            }

            Player player = Player.m_localPlayer;
            if (player == null)
            {
                _toolGiven = false;
                _preview.Hide();
                TerrainEngine.ClearHistory();
                return;
            }

            if (TerrainKeybinds.MenuOpen)
            {
                _preview.Hide();
                return;
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
                }
                if (p.CycleShapeKey.Value.IsDown())
                    CycleShape(p);
                if (p.CyclePaintKey.Value.IsDown())
                    CyclePaint(p);
                if (p.UndoKey.Value.IsDown())
                    DoUndo();
                if (p.RedoKey.Value.IsDown())
                    DoRedo();
            }

            TerrainOperation op = p.DefaultOperation.Value;
            if (op != TerrainOperation.Ramp)
                _rampHasA = false;

            // Server authority: when the server runs the mod, its Server-section
            // values (synced to clients) decide whether this client may write
            // terrain. Servers without the mod leave the local values alone, so
            // the tool keeps working there for testing.
            _serverAllowed = !equipped || ServerAllows(p, out _serverWhy);

            if (!blocked && equipped)
            {
                if (op == TerrainOperation.Ramp)
                {
                    if (p.ApplyKey.Value.IsDown())
                    {
                        if (_serverAllowed)
                            HandleRampMark(p, player, groundCenter, hasAim);
                        else
                            Flash(_serverWhy);
                    }
                }
                else if (Held(p.ApplyKey.Value))
                {
                    if (_serverAllowed)
                        _applying = TryApply(p, player, groundCenter);
                    else
                        Flash(_serverWhy);
                }
            }

            if (equipped)
            {
                _aimY = groundCenter.y;
                _targetTxt = TargetText(p, player, groundCenter);
                _slopeDeg = SlopeDeg(groundCenter);
            }

            if (equipped && p.ShowPreview.Value)
            {
                bool guides = p.ShowGuides.Value;
                float fullRadius = p.Radius.Value * (1f - 0.5f * Mathf.Clamp01(p.Softness.Value / 100f));
                _preview.Update(new PreviewState
                {
                    Center = groundCenter,
                    PlayerPos = player.transform.position,
                    Radius = p.Radius.Value,
                    FullRadius = guides ? fullRadius : 0f,
                    ReachRadius = guides ? p.RadiusMax.Value : 0f,
                    AimReach = guides ? MaxAimDistance : 0f,
                    Shape = p.Shape.Value,
                    Color = BrushColor(p).WithAlpha(0.95f),
                    IsRamp = p.DefaultOperation.Value == TerrainOperation.Ramp,
                    HasRampA = _rampHasA,
                    RampA = _rampA,
                });
            }
            else
            {
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

            if (!_rampHasA)
            {
                _rampA = ground;
                _rampHasA = true;
                TerrainForgePlugin.Message("Rampa: punto A marcado. Apunta al punto B y volve a pulsar aplicar.");
                return;
            }

            Vector3 a = _rampA;
            _rampHasA = false;

            if ((ground - a).sqrMagnitude < 1f)
            {
                TerrainForgePlugin.Message("Rampa: los puntos A y B estan demasiado cerca.");
                return;
            }

            var spec = new BrushSpec
            {
                Operation = TerrainOperation.Ramp,
                Shape = p.Shape.Value,
                Radius = p.Radius.Value,
                Softness = p.Softness.Value,
                MaxUp = p.UnlockDeltaLimit.Value ? p.MaxUpDelta.Value : 8f,
                MaxDown = p.UnlockDeltaLimit.Value ? p.MaxDownDelta.Value : 8f,
                HasRampLine = true,
                RampA = a,
                RampB = ground
            };

            Vector3 mid = (a + ground) * 0.5f;
            if (TerrainEngine.Apply(mid, spec))
                TerrainForgePlugin.Message("Rampa creada de A a B.");
            else
                TerrainForgePlugin.Message("Rampa: no se pudo crear (puntos muy cortos o sin terreno).");
        }

        private bool TryApply(TerrainForgePlugin p, Player player, Vector3 center)
        {
            if (Time.time - _lastApply < ApplyInterval)
                return _applying;

            float playerGround = GroundHeight(player.transform.position);

            // Flatten / Smooth follow the height under the crosshair by default so
            // they conform to whatever you aim at (e.g. a ramp). Hold the modifier to
            // level to your own feet instead.
            bool ownHeight = Held(p.AimHeightModifierKey.Value);

            var spec = new BrushSpec
            {
                Operation = p.DefaultOperation.Value,
                Shape = p.Shape.Value,
                Paint = p.Paint.Value,
                Radius = p.Radius.Value,
                Softness = p.Softness.Value,
                Power = p.FalloffPower.Value,
                RaiseDelta = p.RaiseDelta.Value,
                TargetWorld = ownHeight ? playerGround : center.y,
                PaintStrength = p.PaintStrength.Value,
                MaxUp = p.UnlockDeltaLimit.Value ? p.MaxUpDelta.Value : 8f,
                MaxDown = p.UnlockDeltaLimit.Value ? p.MaxDownDelta.Value : 8f
            };

            _lastApply = Time.time;
            return TerrainEngine.Apply(center, spec);
        }

        private void HandleRadius(TerrainForgePlugin p)
        {
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (ctrl)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > 0.01f)
                    AdjustRadius(p, Mathf.Sign(scroll) * p.RadiusStep.Value);
            }

            if (p.RadiusUpKey.Value.IsDown())
                AdjustRadius(p, p.RadiusStep.Value);
            if (p.RadiusDownKey.Value.IsDown())
                AdjustRadius(p, -p.RadiusStep.Value);
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
            p.DefaultOperation.Value = (TerrainOperation)(((int)p.DefaultOperation.Value + 1) % 7);
        }

        private static void CycleShape(TerrainForgePlugin p)
        {
            p.Shape.Value = p.Shape.Value == TerrainShape.Circle ? TerrainShape.Square : TerrainShape.Circle;
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
                        reason = "Solo administradores pueden editar terreno en este servidor.";
                        return false;
                    }
                }
                return true;
            }
            reason = "El servidor no permite editar terreno.";
            return false;
        }

        private void DoUndo()
        {
            if (!_serverAllowed)
            {
                Flash(_serverWhy);
                return;
            }
            if (TerrainEngine.Undo(out string label))
                Flash("Deshecho" + (string.IsNullOrEmpty(label) ? "" : ": " + label) +
                      "  (quedan " + TerrainEngine.UndoCount + ")");
            else
                Flash("Nada que deshacer");
        }

        private void DoRedo()
        {
            if (!_serverAllowed)
            {
                Flash(_serverWhy);
                return;
            }
            if (TerrainEngine.Redo(out string label))
                Flash("Rehecho" + (string.IsNullOrEmpty(label) ? "" : ": " + label) +
                      "  (quedan " + TerrainEngine.RedoCount + ")");
            else
                Flash("Nada que rehacer");
        }

        private void Flash(string msg)
        {
            _flashMsg = msg;
            _flashUntil = Time.time + 2.5f;
        }

        private string TargetText(TerrainForgePlugin p, Player player, Vector3 center)
        {
            switch (p.DefaultOperation.Value)
            {
                case TerrainOperation.Flatten:
                {
                    bool own = Held(p.AimHeightModifierKey.Value);
                    float t = own ? GroundHeight(player.transform.position) : center.y;
                    return string.Format("objetivo {0:0.0} m", t);
                }
                case TerrainOperation.Raise:
                    return "+" + p.RaiseDelta.Value.ToString("0.#") + " m/aplic.";
                case TerrainOperation.Lower:
                    return "-" + p.RaiseDelta.Value.ToString("0.#") + " m/aplic.";
                case TerrainOperation.Smooth:
                    return "promedio vecinos";
                case TerrainOperation.Paint:
                    return p.Paint.Value + " " + (p.PaintStrength.Value * 100f).ToString("0") + "%";
                case TerrainOperation.Restore:
                    return "original";
                case TerrainOperation.Ramp:
                    if (_rampHasA)
                        return string.Format("A {0:0.0} \u2192 B {1:0.0} m",
                            GroundHeight(_rampA), center.y);
                    return "marca A y B";
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
        private static ItemDrop.ItemData GetHeldItem(Player player)
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

        private static bool IsToolEquipped(TerrainForgePlugin p, ItemDrop.ItemData weapon)
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

        private static bool Held(KeyboardShortcut shortcut)
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
            switch (p.DefaultOperation.Value)
            {
                case TerrainOperation.Raise: return new Color(0.50f, 0.82f, 1.00f);
                case TerrainOperation.Lower: return new Color(1.00f, 0.60f, 0.30f);
                case TerrainOperation.Smooth: return new Color(0.40f, 0.92f, 0.92f);
                case TerrainOperation.Restore: return new Color(0.90f, 0.45f, 0.95f);
                case TerrainOperation.Ramp: return new Color(0.98f, 0.78f, 0.40f);
                case TerrainOperation.Paint: return PaintColor(p.Paint.Value);
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

            string status = string.Format("TerrainForge   {0}   |   radio {1:0.#} m   |   {2}   |   {3}",
                p.DefaultOperation.Value, p.Radius.Value, p.Shape.Value, p.Paint.Value);
            if (p.DefaultOperation.Value == TerrainOperation.Ramp && _rampHasA)
                status += "   |   RAMPA: punto A fijado, apunta a B";
            status += string.Format("   |   deshacer {0} / rehacer {1}",
                TerrainEngine.UndoCount, TerrainEngine.RedoCount);
            if (!_serverAllowed)
                status += "   |   SERVER: BLOQUEADO";
            if (!string.IsNullOrEmpty(_flashMsg) && Time.time < _flashUntil)
                status += "   |   " + _flashMsg;

            Color prev = GUI.color;
            Color c = BrushColor(p);
            _hud.normal.textColor = _applying ? new Color(1f, 0.85f, 0.4f) : c;
            string prefix = _applying ? "\u25B6  " : "\u2022  ";
            string full = prefix + status;

            Vector2 size = _hud.CalcSize(new GUIContent(full));
            float w = Mathf.Max(320f, size.x + 28f);
            w = Mathf.Min(w, Mathf.Max(200f, Screen.width - 24f));
            float h = Mathf.Max(30f, size.y + 12f);
            var rect = new Rect(12f, 12f, w, h);

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect, _hudBg);
            GUI.color = prev;

            GUI.Label(new Rect(rect.x + 12f, rect.y, rect.width - 20f, rect.height), full, _hud);

            string line2 = string.Format("apuntado {0:0.0} m   |   {1}   |   pendiente {2:0}\u00B0",
                _aimY, _targetTxt, _slopeDeg);
            Vector2 size2 = _hud.CalcSize(new GUIContent(line2));
            float w2 = Mathf.Max(320f, size2.x + 28f);
            w2 = Mathf.Min(w2, Mathf.Max(200f, Screen.width - 24f));
            float h2 = Mathf.Max(30f, size2.y + 12f);
            var rect2 = new Rect(12f, rect.y + rect.height + 4f, w2, h2);

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect2, _hudBg);
            GUI.color = prev;

            Color prevText = _hud.normal.textColor;
            _hud.normal.textColor = Color.white;
            GUI.Label(new Rect(rect2.x + 12f, rect2.y, rect2.width - 20f, rect2.height), line2, _hud);
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

            _hud = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                font = GameFont()
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
}
