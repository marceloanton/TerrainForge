using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Shared Valheim-flavoured palette so the F5 panel, the radial wheel and the
    /// HUD all read as one coherent interface. Change a colour here and every
    /// surface follows.
    /// </summary>
    internal static class TerrainTheme
    {
        // Surfaces
        public static readonly Color PanelDark = new Color(0.07f, 0.07f, 0.08f, 0.97f);
        public static readonly Color PanelHeader = new Color(0.10f, 0.10f, 0.12f, 1f);
        public static readonly Color PanelBg = new Color(0.12f, 0.12f, 0.14f, 0.98f);
        public static readonly Color Button = new Color(0.16f, 0.16f, 0.18f, 1f);
        public static readonly Color ButtonHover = new Color(0.22f, 0.22f, 0.25f, 1f);
        public static readonly Color ButtonActive = new Color(0.75f, 0.60f, 0.15f, 1f);

        // Accents / text
        public static readonly Color Gold = new Color(0.92f, 0.78f, 0.18f, 1f);
        public static readonly Color GoldBright = new Color(1f, 0.88f, 0.25f, 1f);
        public static readonly Color TextGold = new Color(0.95f, 0.82f, 0.25f, 1f);
        public static readonly Color TextWhite = new Color(0.92f, 0.90f, 0.85f, 1f);
        public static readonly Color TextDim = new Color(0.65f, 0.62f, 0.55f, 1f);
        public static readonly Color TextBright = new Color(1f, 0.92f, 0.65f, 1f);
        public static readonly Color Danger = new Color(1f, 0.5f, 0.4f, 1f);

        // Radial wheel
        public static readonly Color SectorDark = new Color(0.11f, 0.11f, 0.12f, 0.92f);
        public static readonly Color SectorGold = new Color(0.79f, 0.63f, 0.20f, 0.98f);
        public static readonly Color Hub = new Color(0.08f, 0.08f, 0.09f, 0.96f);
        public static readonly Color DarkText = new Color(0.10f, 0.09f, 0.07f, 1f);

        // Default tool accent (matches Plugin.ToolTintColor default #4FC3F7).
        public static readonly Color Accent = new Color(0.31f, 0.76f, 0.97f);
    }
}
