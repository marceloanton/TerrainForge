using System;
using HarmonyLib;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Registers a mod-exclusive tool item by cloning the vanilla Hammer and adding
    /// it to Valheim's ObjectDB and ZNetScene registries.
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

        internal static void EnsureRegistered()
        {
            ObjectDB db = ObjectDB.instance;
            if (db == null)
                return;

            if (db.GetItemPrefab(PrefabName) == null)
                Register(db);
            else
                Registered = true;

            TerrainToolNetworkPrefabPatch.EnsureRegistered();
        }

        internal static void Register(ObjectDB db)
        {
            if (db == null)
                return;

            // ObjectDB can be rebuilt when entering another world. Register against
            // the specific database instance instead of a process-wide one-time flag.
            if (db.GetItemPrefab(PrefabName) != null)
            {
                Registered = true;
                return;
            }

            try
            {
                GameObject hammer = null;
                foreach (GameObject item in db.m_items)
                {
                    if (item != null && item.name == BasePrefabName)
                    {
                        hammer = item;
                        break;
                    }
                }
                if (hammer == null)
                {
                    LogWarning("could not find the vanilla '" + BasePrefabName + "' prefab; tool unavailable.");
                    return;
                }

                GameObject prefab = UnityEngine.Object.Instantiate(hammer);
                prefab.name = PrefabName;
                prefab.SetActive(false);

                ItemDrop drop = prefab.GetComponent<ItemDrop>();
                if (drop == null || drop.m_itemData == null || drop.m_itemData.m_shared == null)
                {
                    UnityEngine.Object.Destroy(prefab);
                    LogWarning("Hammer clone has no valid ItemDrop; tool unavailable.");
                    return;
                }

                ItemDrop.ItemData.SharedData shared = CloneSharedData(drop.m_itemData.m_shared);
                drop.m_itemData.m_shared = shared;
                drop.m_itemData.m_dropPrefab = prefab;
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
                {
                    shared.m_attack = CloneAttack(shared.m_attack);
                    shared.m_attack.m_attackStamina = 0f;
                }
                if (shared.m_secondaryAttack != null)
                {
                    shared.m_secondaryAttack = CloneAttack(shared.m_secondaryAttack);
                    shared.m_secondaryAttack.m_attackStamina = 0f;
                }

                Shared = shared;

                ApplyIcon(shared);
                db.m_items.Add(prefab);
                AccessTools.Method(typeof(ObjectDB), "UpdateRegisters", Type.EmptyTypes)?.Invoke(db, null);
                Registered = db.GetItemPrefab(PrefabName) != null;
                if (!Registered)
                {
                    db.m_items.Remove(prefab);
                    UnityEngine.Object.Destroy(prefab);
                    Shared = null;
                    LogWarning("ObjectDB did not index the registered tool; tool unavailable.");
                    return;
                }
                ApplyTint(prefab);
                LogInfo("tool registered as '" + PrefabName + "'.");
            }
            catch (Exception e)
            {
                Shared = null;
                Registered = false;
                LogError("registration failed: " + e);
            }
        }

        private static ItemDrop.ItemData.SharedData CloneSharedData(ItemDrop.ItemData.SharedData source)
        {
            return (ItemDrop.ItemData.SharedData)AccessTools.Method(source.GetType(), "MemberwiseClone")
                .Invoke(source, null);
        }

        private static Attack CloneAttack(Attack source)
        {
            return (Attack)AccessTools.Method(source.GetType(), "MemberwiseClone")?.Invoke(source, null);
        }

        internal static GameObject Prefab
        {
            get
            {
                if (ObjectDB.instance == null)
                    return null;
                return ObjectDB.instance.GetItemPrefab(PrefabName);
            }
        }

        internal static void RegisterNetworkPrefab(ZNetScene scene)
        {
            if (scene == null)
                return;
            GameObject prefab = Prefab;
            if (prefab == null || scene.GetPrefab(PrefabName) != null)
                return;

            scene.m_prefabs.Add(prefab);
            try
            {
                var namedPrefabs = AccessTools.Field(typeof(ZNetScene), "m_namedPrefabs")?.GetValue(scene);
                var add = namedPrefabs?.GetType().GetMethod("Add", new[] { typeof(int), typeof(GameObject) });
                if (add == null)
                {
                    scene.m_prefabs.Remove(prefab);
                    LogWarning("could not find the network prefab index method; tool will not synchronize.");
                    return;
                }
                add.Invoke(namedPrefabs, new object[] { PrefabName.GetStableHashCode(), prefab });
                LogInfo("network prefab registered as '" + PrefabName + "'.");
            }
            catch (Exception e)
            {
                scene.m_prefabs.Remove(prefab);
                LogWarning("network prefab registration failed: " + e.GetBaseException().Message);
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

            // Resolve the registered shared data from ObjectDB at check time.
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
            if (player == null || !Registered || ObjectDB.instance == null)
                return;

            GameObject prefab = ObjectDB.instance.GetItemPrefab(PrefabName);
            if (prefab == null)
            {
                LogWarning("registered tool is missing from the active ObjectDB; skipping handout.");
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
