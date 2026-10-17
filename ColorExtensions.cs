using UnityEngine;

namespace TerrainForge
{
    internal static class ColorExtensions
    {
        public static Color WithAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }
    }
}
