using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;

namespace TerrainForge
{
    [BepInPlugin(Guid, "TerrainForge", "0.3.4")]
    [BepInDependency("com.jotunn.jotunn")]
    public class TerrainForgePlugin : BaseUnityPlugin
    {
        public const string Guid = "com.terrainforge.valheim";

        public static TerrainForgePlugin Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }

        // General
        public ConfigEntry<bool> Enabled;
        public ConfigEntry<bool> PlayEffects;
        public ConfigEntry<ModLanguage> Language;

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
        public ConfigEntry<RampMode> RampMode;
        public ConfigEntry<float> SlopeAngle;
        public ConfigEntry<bool> UseFixedHeight;
        public ConfigEntry<float> FixedHeight;
        public ConfigEntry<SmoothMode> SmoothMode;
        public ConfigEntry<FalloffMode> FalloffMode;
        public ConfigEntry<float> ShoreOffset;

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
        public ConfigEntry<KeyboardShortcut> EyedropKey;
        public ConfigEntry<KeyboardShortcut> RampModeKey;
        public ConfigEntry<KeyboardShortcut> RadialKey;
        public ConfigEntry<KeyboardShortcut> AdjustModeKey;
        public ConfigEntry<KeyboardShortcut> OpFlattenKey;
        public ConfigEntry<KeyboardShortcut> OpRaiseKey;
        public ConfigEntry<KeyboardShortcut> OpLowerKey;
        public ConfigEntry<KeyboardShortcut> OpSmoothKey;
        public ConfigEntry<KeyboardShortcut> OpPaintKey;
        public ConfigEntry<KeyboardShortcut> OpRestoreKey;
        public ConfigEntry<KeyboardShortcut> OpRampKey;
        public ConfigEntry<KeyboardShortcut> OpShoreKey;
        public ConfigEntry<KeyboardShortcut> OpSnowKey;
        public ConfigEntry<KeyboardShortcut> CycleProfileKey;
        public ConfigEntry<KeyboardShortcut> CopyStampKey;
        public ConfigEntry<KeyboardShortcut> PasteStampKey;
        public ConfigEntry<KeyboardShortcut> CycleStampKey;
        public ConfigEntry<KeyboardShortcut> CycleStampBackKey;
        public ConfigEntry<bool> SeenTutorial;
        public ConfigEntry<string> PresetSlots;
        public ConfigEntry<ToolRequirement> RequiredTool;
        public ConfigEntry<StampToolMode> StampToolMode;
        public ConfigEntry<bool> AutoGiveTool;

        // Server / restrictions
        public ConfigEntry<bool> ScaleVanillaToolRadius;
        public ConfigEntry<float> VanillaToolRadius;
        public ConfigEntry<bool> NoBuildCost;
        public ConfigEntry<bool> AllowInProtectedZones;

        // Preview
        public ConfigEntry<bool> ShowPreview;
        public ConfigEntry<bool> ShowGuides;
        public ConfigEntry<bool> ShowSlopeHeatmap;
        public ConfigEntry<KeyboardShortcut> SlopeHeatmapKey;
        public ConfigEntry<bool> ShowHeightHeatmap;
        public ConfigEntry<KeyboardShortcut> HeightHeatmapKey;

        // History
        public ConfigEntry<int> UndoDepth;
        public ConfigEntry<KeyboardShortcut> UndoKey;
        public ConfigEntry<KeyboardShortcut> RedoKey;

        // Server authority (the server's values win through config sync when it
        // runs the mod; servers without the mod simply ignore these)
        public ConfigEntry<bool> AllowTerrainEdits;
        public ConfigEntry<bool> AdminOnlyEdits;
        public ConfigEntry<bool> RespectWards;
        public ConfigEntry<float> RadiusCap;
        public ConfigEntry<float> DeltaCap;
        public ConfigEntry<int> ChunkCompsPerFrame;

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "Master switch for TerrainForge.");
            PlayEffects = Config.Bind("General", "PlayEffects", true,
                "Play the tool's terrain-hit effect (dust + thud) when the brush applies.");
            Language = Config.Bind("General", "Language", ModLanguage.Auto,
                "Menu / HUD language: Auto (detect from game), Español, English, or other supported languages.");

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
            RampMode = Config.Bind("Brush", "RampMode", global::TerrainForge.RampMode.TwoPoint,
                "Ramp mode: TwoPoint joins the heights at A and B; FixedAngle builds " +
                "from A at a fixed slope (SlopeAngle) toward B.");
            SlopeAngle = Config.Bind("Brush", "SlopeAngle", 30f,
                new ConfigDescription("Slope used by fixed-angle ramps (degrees). " +
                    "Adjust in-game with Alt + radius keys/wheel while Ramp is selected.",
                    new AcceptableValueRange<float>(5f, 60f)));
            UseFixedHeight = Config.Bind("Brush", "UseFixedHeight", false,
                "When true, Flatten levels to FixedHeight instead of the aimed / feet height (eyedropper state).");
            FixedHeight = Config.Bind("Brush", "FixedHeight", 30f,
                new ConfigDescription("Exact Flatten target height in meters (used when UseFixedHeight is true).",
                    new AcceptableValueRange<float>(-10f, 150f)));
            SmoothMode = Config.Bind("Brush", "SmoothMode", global::TerrainForge.SmoothMode.Full,
                "Smooth behavior: Full blurs both ways, Bumps only lowers bumps, Dips only fills dips.");
            FalloffMode = Config.Bind("Brush", "FalloffMode", global::TerrainForge.FalloffMode.Pointy,
                "Raise/Lower brush falloff: Pointy (sharp center), Flat (uniform disk), Smooth (cosine), Linear (taper).");
            ShoreOffset = Config.Bind("Brush", "ShoreOffset", 0.5f,
                new ConfigDescription("Height above the water level targeted by the Shore operation (meters).",
                    new AcceptableValueRange<float>(-2f, 5f)));

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
            EyedropKey = Config.Bind("Input", "EyedropKey", new KeyboardShortcut(KeyCode.G),
                "Fix the Flatten target height at the aimed point (press again to release).");
            RampModeKey = Config.Bind("Input", "RampModeKey", new KeyboardShortcut(KeyCode.H),
                "Toggle ramp mode: two-point / fixed angle.");
            RadialKey = Config.Bind("Input", "RadialKey", new KeyboardShortcut(KeyCode.Q),
                "Hold to open the operation radial menu (release to confirm, Esc cancels).");
            AdjustModeKey = Config.Bind("Input", "AdjustModeKey", new KeyboardShortcut(KeyCode.V),
                "Toggle adjust mode: the mouse wheel changes the brush radius and vanilla " +
                "scroll uses (camera zoom, hotbar, map) are frozen while active.");
            OpFlattenKey = Config.Bind("Input", "OpFlattenKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Flatten operation.");
            OpRaiseKey = Config.Bind("Input", "OpRaiseKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Raise operation.");
            OpLowerKey = Config.Bind("Input", "OpLowerKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Lower operation.");
            OpSmoothKey = Config.Bind("Input", "OpSmoothKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Smooth operation.");
            OpPaintKey = Config.Bind("Input", "OpPaintKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Paint operation.");
            OpRestoreKey = Config.Bind("Input", "OpRestoreKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Restore operation.");
            OpRampKey = Config.Bind("Input", "OpRampKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Ramp operation.");
            OpShoreKey = Config.Bind("Input", "OpShoreKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Shore operation.");
            OpSnowKey = Config.Bind("Input", "OpSnowKey", new KeyboardShortcut(KeyCode.None),
                "Direct key to select Snow operation.");
            CycleProfileKey = Config.Bind("Input", "CycleProfileKey", new KeyboardShortcut(KeyCode.None),
                "Cycle to the next saved brush profile/preset.");
            CopyStampKey = Config.Bind("Input", "CopyStampKey", new KeyboardShortcut(KeyCode.C, KeyCode.LeftControl, KeyCode.LeftShift),
                "Copy the marked area or brush footprint to the stamp clipboard (RAM).");
            PasteStampKey = Config.Bind("Input", "PasteStampKey", new KeyboardShortcut(KeyCode.V, KeyCode.LeftControl, KeyCode.LeftShift),
                "Paste the stamp from clipboard at the aim position (shows ghost preview first).");
            CycleStampKey = Config.Bind("Input", "CycleStampKey", new KeyboardShortcut(KeyCode.None),
                "Cycle to the next stamp in clipboard history.");
            CycleStampBackKey = Config.Bind("Input", "CycleStampBackKey", new KeyboardShortcut(KeyCode.None),
                "Cycle to the previous stamp in clipboard history.");
            SeenTutorial = Config.Bind("General", "SeenTutorial", false,
                "Set once the first-time tutorial messages have been shown.");
            PresetSlots = Config.Bind("Presets", "Slots", "",
                "Named brush presets, managed from the in-game SETTINGS tab. " +
                "Format per preset: name|radius|softness|op|shape|paint|rampMode|slope|smooth|raiseDelta|paintStrength|falloff|shoreOffset; ...");
            RequiredTool = Config.Bind("Input", "RequiredTool", ToolRequirement.TerrainForgeTool,
                "Which equipped item enables the brush: Any (no requirement), Pickaxe, or the mod's TerrainForgeTool.");
            StampToolMode = Config.Bind("Input", "StampToolMode", TerrainForge.StampToolMode.MainTool,
                "Which equipped item enables stamp copy/paste: MainTool (same as brush), Any, Pickaxe, or TerrainForgeTool.");
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

            ShowSlopeHeatmap = Config.Bind("Preview", "ShowSlopeHeatmap", false,
                "Toggle slope heatmap overlay (green=flat, yellow=moderate, red=steep). Toggle with keybind.");
            SlopeHeatmapKey = Config.Bind("Input", "SlopeHeatmapKey", new KeyboardShortcut(KeyCode.H),
                "Toggle slope heatmap overlay (green=flat 0-15°, yellow=15-30°, orange=30-45°, red=45°+).");

            ShowHeightHeatmap = Config.Bind("Preview", "ShowHeightHeatmap", false,
                "Toggle height heatmap overlay (blue=low, green=mid, yellow=high, red=peak). Toggle with keybind.");
            HeightHeatmapKey = Config.Bind("Input", "HeightHeatmapKey", new KeyboardShortcut(KeyCode.J),
                "Toggle height heatmap overlay (blue=sea level, green=lowland, yellow=highland, red=mountain).");

            UndoDepth = Config.Bind("History", "UndoDepth", 50,
                new ConfigDescription("How many brush applications Ctrl+Z can step back through (0 disables history).",
                    new AcceptableValueRange<int>(0, 500)));
            UndoKey = Config.Bind("Input", "UndoKey", new KeyboardShortcut(KeyCode.Z, KeyCode.LeftControl),
                "Undo the last brush application.");
            RedoKey = Config.Bind("Input", "RedoKey", new KeyboardShortcut(KeyCode.Y, KeyCode.LeftControl),
                "Redo an undone brush application.");

            AllowTerrainEdits = Config.Bind("Server", "AllowTerrainEdits", true,
                "Master switch enforced by the SERVER (its value wins through config sync). " +
                "When false, clients cannot apply, undo or redo terrain edits. " +
                "Servers without the mod ignore this, so the client keeps working for testing.");
            AdminOnlyEdits = Config.Bind("Server", "AdminOnlyEdits", false,
                "When true, only admins / the host may edit terrain " +
                "(server-enforced through config sync).");
            RespectWards = Config.Bind("Server", "RespectWards", true,
                "Block brush edits inside wards you have no access to, using the " +
                "game's own ward check (server-enforced through config sync).");
            RadiusCap = Config.Bind("Server", "RadiusCap", 0f,
                new ConfigDescription("Maximum brush radius enforced by the server, " +
                    "visible clamped in HUD and preview (0 = no cap).",
                    new AcceptableValueRange<float>(0f, 100f)));
            DeltaCap = Config.Bind("Server", "DeltaCap", 0f,
                new ConfigDescription("Maximum terrain delta in meters enforced by " +
                    "the server on top of MaxUpDelta/MaxDownDelta (0 = no cap).",
                    new AcceptableValueRange<float>(0f, 64f)));
            ChunkCompsPerFrame = Config.Bind("Performance", "ChunkCompsPerFrame", 4,
                new ConfigDescription("Heightmap tiles processed per frame. Lower this " +
                    "if huge radii hitch; raise it for faster large strokes (1-32).",
                    new AcceptableValueRange<int>(1, 32)));

            TerrainEngine.Cache();

            PrefabManager.OnVanillaPrefabsAvailable += TerrainTool.Register;

            _harmony = new Harmony(Guid);
            _harmony.PatchAll();

            try
            {
                System.Reflection.MethodInfo scroll = HarmonyLib.AccessTools.Method(
                    typeof(ZInput), "GetMouseScrollWheel", new System.Type[0]);
                if (scroll != null && scroll.ReturnType == typeof(float) && scroll.GetParameters().Length == 0)
                {
                    _harmony.Patch(scroll, prefix: new HarmonyMethod(
                        typeof(ScrollSuppressPatch), nameof(ScrollSuppressPatch.Prefix)));
                }
                else
                {
                    Log.LogWarning("TerrainForge: ZInput.GetMouseScrollWheel not found; " +
                        "adjust mode will change radius but not freeze zoom.");
                }
            }
            catch (System.Exception e)
            {
                Log.LogWarning("TerrainForge scroll patch failed: " + e.Message);
            }

            gameObject.AddComponent<TerrainBrush>();
            gameObject.AddComponent<TerrainKeybinds>();
            gameObject.AddComponent<TerrainRadial>();

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
