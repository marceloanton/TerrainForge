using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Replaces the hard-coded +/-8 m TerrainComp clamp with configurable limits so
    /// edits beyond 8 m survive heightmap generation. Must run on the server too,
    /// otherwise the server regenerates a clamped version of the terrain.
    /// </summary>
    [HarmonyPatch(typeof(TerrainComp), nameof(TerrainComp.ApplyToHeightmap))]
    internal static class ApplyToHeightmapPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(TerrainComp __instance, Texture2D clearedMask, List<float> heights,
            float[] baseHeights, float[] levelOnlyHeights, Heightmap hm)
        {
            var plugin = TerrainForgePlugin.Instance;
            if (plugin == null || !plugin.UnlockDeltaLimit.Value)
                return true; // run the vanilla method

            if (clearedMask == null || heights == null || baseHeights == null)
                return true;

            TerrainEngine.Cache();

            float[] level = TerrainEngine.LevelDelta(__instance);
            float[] smooth = TerrainEngine.SmoothDelta(__instance);
            bool[] modPaint = TerrainEngine.ModifiedPaint(__instance);
            Color[] paint = TerrainEngine.PaintMask(__instance);
            if (level == null || smooth == null || modPaint == null || paint == null)
                return true;

            int width = TerrainEngine.Width(__instance);
            int num = width + 1;

            float up = Mathf.Max(8f, plugin.MaxUpDelta.Value);
            float down = Mathf.Max(8f, plugin.MaxDownDelta.Value);

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int idx = i * num + j;
                    float ld = level[idx];
                    float sd = smooth[idx];
                    if (ld != 0f || sd != 0f)
                    {
                        float value = heights[idx] + ld + sd;
                        float baseH = baseHeights[idx];
                        heights[idx] = Mathf.Clamp(value, baseH - down, baseH + up);
                    }
                }
            }

            for (int k = 0; k < num; k++)
            {
                for (int l = 0; l < num; l++)
                {
                    int idx = k * num + l;
                    if (modPaint[idx])
                        clearedMask.SetPixel(l, k, paint[idx]);
                }
            }

            return false; // skip vanilla
        }
    }

    /// <summary>
    /// Optional: removes the workstation / material requirements for placing pieces,
    /// and skips resource consumption. Off by default.
    /// </summary>
    internal static class NoCostPatch
    {
        [HarmonyPatch(typeof(Player), nameof(Player.HaveRequirements),
            new[] { typeof(Piece), typeof(Player.RequirementMode) })]
        [HarmonyPrefix]
        private static bool HaveRequirementsPrefix(Player __instance, Piece piece, Player.RequirementMode mode, ref bool __result)
        {
            var plugin = TerrainForgePlugin.Instance;
            if (plugin == null || !plugin.NoBuildCost.Value)
                return true;

            if (mode == Player.RequirementMode.CanBuild || mode == Player.RequirementMode.CanAlmostBuild)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Player), "ConsumeResources")]
        [HarmonyPrefix]
        private static bool ConsumeResourcesPrefix(Player __instance)
        {
            var plugin = TerrainForgePlugin.Instance;
            if (plugin == null || !plugin.NoBuildCost.Value)
                return true;
            return false; // skip consumption
        }
    }

    /// <summary>
    /// Optional: forces placement to stay "Valid" inside no-build / ward zones.
    /// Off by default because it can bypass protection.
    /// </summary>
    internal static class NoBuildZonePatch
    {
        private static FieldInfo s_statusField;

        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        [HarmonyPostfix]
        private static void Postfix(Player __instance)
        {
            var plugin = TerrainForgePlugin.Instance;
            if (plugin == null || !plugin.AllowInProtectedZones.Value)
                return;
            if (__instance != Player.m_localPlayer)
                return;

            if (s_statusField == null)
                s_statusField = AccessTools.Field(typeof(Player), "m_placementStatus");
            if (s_statusField == null)
                return;

            var status = (Player.PlacementStatus)s_statusField.GetValue(__instance);
            if (status == Player.PlacementStatus.NoBuildZone || status == Player.PlacementStatus.PrivateZone)
                s_statusField.SetValue(__instance, Player.PlacementStatus.Valid);
        }
    }

    /// <summary>
    /// Best-effort scaling of the vanilla hoe / cultivator terrain radius. This only
    /// affects the local client's application of the op; it may not match other
    /// clients or the server.
    /// </summary>
    internal static class ToolRadiusPatch
    {
        private static PlacementRadiusScope s_scope;

        [HarmonyPatch(typeof(Player), nameof(Player.TryPlacePiece))]
        [HarmonyPrefix]
        private static void Prefix(Player __instance, Piece piece)
        {
            Restore();
            var plugin = TerrainForgePlugin.Instance;
            if (plugin == null || !plugin.ScaleVanillaToolRadius.Value)
                return;
            if (__instance != Player.m_localPlayer || piece == null)
                return;

            float target = Mathf.Clamp(plugin.VanillaToolRadius.Value, 1f, 100f);
            var scope = new PlacementRadiusScope();
            ScaleTerrainComponents(piece.gameObject, target, scope);
            if (scope.Count == 0)
            {
                scope.Restore();
                return;
            }
            s_scope = scope;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.TryPlacePiece))]
        [HarmonyFinalizer]
        private static Exception Finalizer(Exception __exception)
        {
            Restore();
            return __exception;
        }

        private static void Restore()
        {
            if (s_scope == null)
                return;
            try { s_scope.Restore(); }
            finally { s_scope = null; }
        }

        private static void ScaleTerrainComponents(GameObject root, float target, PlacementRadiusScope scope)
        {
            if (root == null)
                return;

            foreach (TerrainOp op in root.GetComponentsInChildren<TerrainOp>(true))
            {
                if (op == null || op.m_settings == null)
                    continue;

                TerrainOp.Settings s = op.m_settings;
                float max = s.GetRadius();
                if (max <= 0.001f)
                    continue;

                float scale = target / max;
                float l = s.m_levelRadius, r = s.m_raiseRadius, sm = s.m_smoothRadius, p = s.m_paintRadius;
                bool lvl = s.m_level, rse = s.m_raise, smt = s.m_smooth, pnt = s.m_paintCleared;
                scope.Add(() =>
                {
                    s.m_levelRadius = l;
                    s.m_raiseRadius = r;
                    s.m_smoothRadius = sm;
                    s.m_paintRadius = p;
                });
                if (lvl) s.m_levelRadius = Mathf.Clamp(l * scale, 0.5f, 200f);
                if (rse) s.m_raiseRadius = Mathf.Clamp(r * scale, 0.5f, 200f);
                if (smt) s.m_smoothRadius = Mathf.Clamp(sm * scale, 0.5f, 200f);
                if (pnt) s.m_paintRadius = Mathf.Clamp(p * scale, 0.5f, 200f);
            }

            foreach (TerrainModifier mod in root.GetComponentsInChildren<TerrainModifier>(true))
            {
                if (mod == null)
                    continue;

                float max = mod.GetRadius();
                if (max <= 0.001f)
                    continue;

                float scale = target / max;
                float l = mod.m_levelRadius, sm = mod.m_smoothRadius, p = mod.m_paintRadius;
                bool lvl = mod.m_level, smt = mod.m_smooth, pnt = mod.m_paintCleared;
                scope.Add(() =>
                {
                    mod.m_levelRadius = l;
                    mod.m_smoothRadius = sm;
                    mod.m_paintRadius = p;
                });
                if (lvl) mod.m_levelRadius = Mathf.Clamp(l * scale, 0.5f, 200f);
                if (smt) mod.m_smoothRadius = Mathf.Clamp(sm * scale, 0.5f, 200f);
                if (pnt) mod.m_paintRadius = Mathf.Clamp(p * scale, 0.5f, 200f);
            }
        }
    }

    /// <summary>Records undo actions so scaled radii can be restored after placement.</summary>
    internal sealed class PlacementRadiusScope
    {
        private readonly List<Action> _undo = new List<Action>();
        public int Count => _undo.Count;
        public void Add(Action undo) => _undo.Add(undo);

        public void Restore()
        {
            for (int i = _undo.Count - 1; i >= 0; i--)
            {
                try { _undo[i](); }
                catch { /* ignore */ }
            }
            _undo.Clear();
        }
    }
}
