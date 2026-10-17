using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// One-shot feedback (dust + thud) when the brush writes terrain. Reuses the
    /// tool's own terrain-hit effect, so no prefab names are needed. Local only,
    /// throttled while holding, always on discrete applications.
    /// </summary>
    internal static class TerrainFx
    {
        private static float _last;

        public static void Play(Vector3 pos, bool force)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p != null && p.PlayEffects != null && !p.PlayEffects.Value)
                return;
            if (!force && Time.time - _last < 0.6f)
                return;
            _last = Time.time;

            try
            {
                EffectList fx = TerrainTool.Shared != null
                    ? TerrainTool.Shared.m_hitTerrainEffect
                    : null;
                if (fx == null)
                    return;
                fx.Create(pos + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
            }
            catch
            {
                // Feedback must never break an edit.
            }
        }
    }
}
