using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;

namespace TerrainForge
{
    [BepInPlugin(Guid, "TerrainForge", "0.1.0")]
    [BepInDependency("com.jotunn.jotunn")]
    public class TerrainForgePlugin : BaseUnityPlugin
    {
        public const string Guid = "com.terrainforge.valheim";

        public static TerrainForgePlugin Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }

        // General
        public ConfigEntry<bool> Enabled;

        // Brush
        public ConfigEntry<float> Radius;
        public ConfigEntry<float> RadiusMin;
        public ConfigEntry<float> RadiusMax;
        public ConfigEntry<float> RadiusStep;
        public ConfigEntry<float> Softness;
        public ConfigEntry<float> FalloffPower;
        public ConfigEntry<float> RaiseDelta;
        public ConfigEntry<float> PaintStrength;
        public ConfigEntry<TerrainOperation> DefaultOperation;
        public ConfigEntry<TerrainShape> Shape;
        public ConfigEntry<TerrainPaint> Paint;

        // Limits
        public ConfigEntry<bool> UnlockDeltaLimit;
        public ConfigEntry<float> MaxUpDelta;
        public ConfigEntry<float> MaxDownDelta;

        // Input
        public ConfigEntry<KeyboardShortcut> ApplyKey;
        public ConfigEntry<KeyboardShortcut> RadiusUpKey;
        public ConfigEntry<KeyboardShortcut> RadiusDownKey;
        public ConfigEntry<KeyboardShortcut> CycleOperationKey;
        public ConfigEntry<KeyboardShortcut> CycleShapeKey;
        public ConfigEntry<KeyboardShortcut> CyclePaintKey;
        public ConfigEntry<KeyboardShortcut> AimHeightModifierKey;
        public ConfigEntry<KeyboardShortcut> RebindMenuKey;
        public ConfigEntry<KeyboardShortcut> GiveToolKey;
        public ConfigEntry<ToolRequirement> RequiredTool;
        public ConfigEntry<bool> AutoGiveTool;

        // Server / restrictions
        public ConfigEntry<bool> ScaleVanillaToolRadius;
        public ConfigEntry<float> VanillaToolRadius;
        public ConfigEntry<bool> NoBuildCost;
        public ConfigEntry<bool> AllowInProtectedZones;

        // Preview
        public ConfigEntry<bool> ShowPreview;
        public ConfigEntry<bool> ShowGuides;

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "Master switch for TerrainForge.");

            Radius = Config.Bind("Brush", "Radius", 8f,
                new ConfigDescription("Brush radius in meters.", new AcceptableValueRange<float>(1f, 100f)));
            RadiusMin = Config.Bind("Brush", "RadiusMin", 2f, "Minimum radius when adjusting.");
            RadiusMax = Config.Bind("Brush", "RadiusMax", 60f, "Maximum radius when adjusting.");
            RadiusStep = Config.Bind("Brush", "RadiusStep", 1f, "Radius change per key press / scroll notch.");
            Softness = Config.Bind("Brush", "Softness", 50f,
                new ConfigDescription("Edge softness (0 = hard edge, 100 = very soft).",
                    new AcceptableValueRange<float>(0f, 100f)));
            FalloffPower = Config.Bind("Brush", "FalloffPower", 3f,
                "Falloff exponent used by Raise / Lower.");
            RaiseDelta = Config.Bind("Brush", "RaiseLowerMeters", 2f,
                "How many meters Raise / Lower move the terrain per application.");
            PaintStrength = Config.Bind("Brush", "PaintStrength", 1f,
                new ConfigDescription("How strongly Paint applies each application.",
                    new AcceptableValueRange<float>(0f, 1f)));
            DefaultOperation = Config.Bind("Brush", "DefaultOperation", TerrainOperation.Flatten,
                "Brush operation selected on startup.");
            Shape = Config.Bind("Brush", "Shape", TerrainShape.Circle,
                "Brush shape selected on startup.");
            Paint = Config.Bind("Brush", "Paint", TerrainPaint.Dirt,
                "Paint type selected on startup.");

            UnlockDeltaLimit = Config.Bind("Limits", "UnlockDeltaLimit", true,
                "Replace the hard-coded +/-8 m clamp so edits beyond 8 m survive terrain regeneration. " +
                "MUST also be enabled on the server, otherwise the server clamps the terrain.");
            MaxUpDelta = Config.Bind("Limits", "MaxUpDelta", 64f,
                "Maximum height above the natural terrain the brush may produce (meters).");
            MaxDownDelta = Config.Bind("Limits", "MaxDownDelta", 64f,
                "Maximum depth below the natural terrain the brush may produce (meters).");

            ApplyKey = Config.Bind("Input", "ApplyKey", new KeyboardShortcut(KeyCode.Mouse1),
                "Hold (with the tool equipped) to apply the brush.");
            RadiusUpKey = Config.Bind("Input", "RadiusUpKey", new KeyboardShortcut(KeyCode.Equals),
                "Increase radius.");
            RadiusDownKey = Config.Bind("Input", "RadiusDownKey", new KeyboardShortcut(KeyCode.Minus),
                "Decrease radius.");
            CycleOperationKey = Config.Bind("Input", "CycleOperationKey", new KeyboardShortcut(KeyCode.Comma),
                "Cycle brush operation.");
            CycleShapeKey = Config.Bind("Input", "CycleShapeKey", new KeyboardShortcut(KeyCode.Period),
                "Toggle circle / square.");
            CyclePaintKey = Config.Bind("Input", "CyclePaintKey", new KeyboardShortcut(KeyCode.Slash),
                "Cycle paint type.");
            AimHeightModifierKey = Config.Bind("Input", "AimHeightModifierKey", new KeyboardShortcut(KeyCode.LeftShift),
                "Hold to level Flatten to your own feet instead of the height under the crosshair.");
            RebindMenuKey = Config.Bind("Input", "RebindMenuKey", new KeyboardShortcut(KeyCode.F5),
                "Open the in-game key rebinding menu. Note: F5 is also Valheim's developer console key (only if the game runs with -console).");
            GiveToolKey = Config.Bind("Input", "GiveToolKey", new KeyboardShortcut(KeyCode.F8),
                "Hand out the TerrainForge tool to your inventory.");
            RequiredTool = Config.Bind("Input", "RequiredTool", ToolRequirement.TerrainForgeTool,
                "Which equipped item enables the brush: Any (no requirement), Pickaxe, or the mod's TerrainForgeTool.");
            AutoGiveTool = Config.Bind("Input", "AutoGiveTool", true,
                "Automatically give yourself the TerrainForge tool once you enter a world (only if you don't already have it).");

            ScaleVanillaToolRadius = Config.Bind("VanillaTools", "ScaleVanillaToolRadius", false,
                "Best effort: scale the vanilla hoe / cultivator terrain radius. " +
                "May not be consistent across clients when modded radius is used.");
            VanillaToolRadius = Config.Bind("VanillaTools", "VanillaToolRadius", 20f,
                "Radius applied to vanilla hoe / cultivator terrain ops when scaling is enabled.");
            NoBuildCost = Config.Bind("VanillaTools", "NoBuildCost", false,
                "Remove workstation requirements and material cost for placing build pieces.");
            AllowInProtectedZones = Config.Bind("VanillaTools", "AllowInProtectedZones", false,
                "Allow placement inside no-build zones and ward zones. Bypasses protection.");

            ShowPreview = Config.Bind("Preview", "ShowPreview", true,
                "Draw the brush footprint while aiming.");

            ShowGuides = Config.Bind("Preview", "ShowGuides", true,
                "Draw the faint reach guides: full-strength zone, maximum radius, maximum aim distance and the ramp corridor.");

            TerrainEngine.Cache();

            PrefabManager.OnVanillaPrefabsAvailable += TerrainTool.Register;

            _harmony = new Harmony(Guid);
            _harmony.PatchAll();

            gameObject.AddComponent<TerrainBrush>();
            gameObject.AddComponent<TerrainKeybinds>();

            Log.LogInfo("TerrainForge loaded. Radius=" + Radius.Value +
                        " Operation=" + DefaultOperation.Value + " UnlockDeltaLimit=" + UnlockDeltaLimit.Value +
                        " RequiredTool=" + RequiredTool.Value);
        }

        private void OnDestroy()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= TerrainTool.Register;
            _harmony?.UnpatchSelf();
        }

        internal static void Message(string text)
        {
            if (MessageHud.instance != null)
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "[TerrainForge] " + text);
        }
    }
}
