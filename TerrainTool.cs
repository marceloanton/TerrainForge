using System;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Registers a mod-exclusive tool item (a clone of the Hammer) via Jotunn and
    /// provides helpers to detect, hand out and identify it. The cloned prefab gets
    /// its own copy of the shared data, so the vanilla Hammer is left untouched.
    /// </summary>
    internal static class TerrainTool
    {
        public const string PrefabName = "TerrainForgeTool";
        public const string DisplayName = "TerrainForge Rod";
        private const string BasePrefabName = "Hammer";

        /// <summary>Shared data of the registered tool, used for reference-equality checks.</summary>
        public static ItemDrop.ItemData.SharedData Shared { get; private set; }

        /// <summary>True once the item has been registered into the ObjectDB.</summary>
        public static bool Registered { get; private set; }

        /// <summary>
        /// Creates and registers the custom item. Must be called from
        /// <see cref="PrefabManager.OnVanillaPrefabsAvailable"/>.
        /// </summary>
        public static void Register()
        {
            if (Registered)
                return;

            try
            {
                var custom = new CustomItem(PrefabName, BasePrefabName);
                if (custom.ItemPrefab == null || custom.ItemDrop == null)
                {
                    LogWarning("could not clone the '" + BasePrefabName + "' prefab; tool unavailable.");
                    return;
                }

                ItemDrop.ItemData.SharedData shared = custom.ItemDrop.m_itemData.m_shared;
                shared.m_name = DisplayName;
                shared.m_description = "A rod attuned to the earth. Equip it to shape the terrain with TerrainForge.";
                shared.m_itemType = ItemDrop.ItemData.ItemType.Tool;
                shared.m_buildPieces = null; // never open the build menu
                shared.m_weight = 2f;
                shared.m_maxStackSize = 1;
                shared.m_maxQuality = 1;
                shared.m_value = 0;
                shared.m_teleportable = true;
                shared.m_equipDuration = 0.2f;

                // The brush fires on a held mouse key, so make sure swinging the rod
                // never costs stamina.
                if (shared.m_attack != null)
                    shared.m_attack.m_attackStamina = 0f;
                if (shared.m_secondaryAttack != null)
                    shared.m_secondaryAttack.m_attackStamina = 0f;

                Shared = shared;

                ApplyIcon(shared);

                if (!ItemManager.Instance.AddItem(custom))
                {
                    LogWarning("ItemManager.AddItem returned false; tool unavailable.");
                    Shared = null;
                    return;
                }

                Registered = true;
                ApplyTint(custom.ItemPrefab);
                LogInfo("tool registered as '" + PrefabName + "'.");
            }
            catch (Exception e)
            {
                Shared = null;
                LogError("registration failed: " + e);
            }
        }

        /// <summary>True if the given equipped item is the TerrainForge tool.</summary>
        public static bool IsEquipped(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null)
                return false;

            if (Shared != null && item.m_shared == Shared)
                return true;

            if (item.m_dropPrefab != null && item.m_dropPrefab.name == PrefabName)
                return true;

            // Resolve the registered shared data from ObjectDB at check time. This
            // survives any re-instantiation Jotunn may perform while adding the item,
            // which would make the cached Shared reference stale.
            GameObject prefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(PrefabName) : null;
            if (prefab != null)
            {
                ItemDrop drop = prefab.GetComponent<ItemDrop>();
                if (drop != null && drop.m_itemData != null && drop.m_itemData.m_shared == item.m_shared)
                    return true;
            }

            return false;
        }

        /// <summary>True if the player already carries the tool.</summary>
        public static bool Has(Player player)
        {
            if (player == null)
                return false;

            Inventory inv = player.GetInventory();
            return inv != null && inv.GetItem(PrefabName, -1, true) != null;
        }

        /// <summary>Adds the tool to the player's inventory and equips it.</summary>
        public static void Give(Player player)
        {
            if (player == null || ObjectDB.instance == null)
                return;

            GameObject prefab = ObjectDB.instance.GetItemPrefab(PrefabName);
            if (prefab == null)
            {
                LogWarning("prefab not found; cannot hand out the tool.");
                return;
            }

            Inventory inv = player.GetInventory();
            if (inv == null)
                return;

            if (inv.AddItem(prefab, 1))
                TerrainForgePlugin.Message("Herramienta anadida al inventario.");

            Equip(player);
        }

        /// <summary>Equips the tool if the player is carrying it.</summary>
        public static void Equip(Player player)
        {
            if (player == null)
                return;

            Inventory inv = player.GetInventory();
            if (inv == null)
                return;

            ItemDrop.ItemData data = inv.GetItem(PrefabName, -1, true);
            if (data == null)
                return;

            try { player.EquipItem(data, false); }
            catch { /* ignore: another action may be in progress */ }
        }

        // ---------------------------------------------------------------------
        //  Tint: recolor our Hammer clone so it is obvious at a glance which
        //  hammer is the TerrainForge tool. We only touch the clone's own
        //  materials / icons, never the vanilla Hammer. Best-effort: anything
        //  that can't be recolored is silently left untouched.
        // ---------------------------------------------------------------------

        private static void ApplyTint(GameObject prefab)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.ToolTint == null || !p.ToolTint.Value)
                return;

            GameObject live = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(PrefabName) : null;
            if (live != null)
                prefab = live;

            Color tint = ParseColor(p.ToolTintColor != null ? p.ToolTintColor.Value : null,
                new Color(0.31f, 0.76f, 0.97f)); // #4FC3F7 cyan

            int mats = TintRenderers(prefab, tint, out string shader);
            LogInfo("tint " + ColorUtility.ToHtmlStringRGB(tint) + ": mats=" + mats + " shader=" + shader);
        }

        // ---------------------------------------------------------------------
        //  Icon: replace the cloned Hammer icon with a mod-specific pictogram so
        //  the tool is unmistakable in the inventory (and no longer inherits the
        //  Hammer's atlas, which showed stray colours).
        // ---------------------------------------------------------------------

        private static void ApplyIcon(ItemDrop.ItemData.SharedData shared)
        {
            if (shared == null)
                return;

            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            Color accent = ParseColor(p != null && p.ToolTintColor != null ? p.ToolTintColor.Value : null,
                new Color(0.31f, 0.76f, 0.97f));

            Sprite icon = TerrainToolIcon.Build(accent);
            if (icon != null)
            {
                shared.m_icons = new[] { icon };
                LogInfo("custom icon applied.");
            }
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            if (!string.IsNullOrEmpty(hex))
            {
                if (hex[0] != '#')
                    hex = "#" + hex;
                Color c;
                if (ColorUtility.TryParseHtmlString(hex, out c))
                    return c;
            }
            return fallback;
        }

        private static int TintRenderers(GameObject prefab, Color tint, out string shaderName)
        {
            shaderName = "(none)";
            if (prefab == null)
                return 0;
            int count = 0;
            try
            {
                foreach (Renderer r in prefab.GetComponentsInChildren<Renderer>(true))
                {
                    Material[] src = r.sharedMaterials;
                    if (src == null || src.Length == 0)
                        continue;
                    var dst = new Material[src.Length];
                    for (int i = 0; i < src.Length; i++)
                    {
                        dst[i] = TintMaterial(src[i], tint);
                        if (dst[i] != null)
                            count++;
                    }
                    if (count > 0 && src[0] != null && src[0].shader != null && shaderName == "(none)")
                        shaderName = src[0].shader.name;
                    r.sharedMaterials = dst;
                }
            }
            catch (Exception e)
            {
                LogWarning("model tint failed: " + e.Message);
            }
            return count;
        }

        private static Material TintMaterial(Material src, Color tint)
        {
            if (src == null)
                return src;
            Material m = new Material(src);
            if (m.HasProperty("_Color"))
                m.color = m.color * tint;
            else if (m.HasProperty("_BaseColor"))
                m.SetColor("_BaseColor", m.GetColor("_BaseColor") * tint);
            if (m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                m.SetColor("_EmissionColor", tint * 0.5f);
            }
            return m;
        }

        private static void LogInfo(string msg) => TerrainForgePlugin.Log?.LogInfo("TerrainForgeTool: " + msg);
        private static void LogWarning(string msg) => TerrainForgePlugin.Log?.LogWarning("TerrainForgeTool: " + msg);
        private static void LogError(string msg) => TerrainForgePlugin.Log?.LogError("TerrainForgeTool: " + msg);
    }
}
