using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Builds the TerrainForge tool icon at runtime (no asset files): a golden
    /// rod over a small terrain mound with a glowing crystal tip. Uses the same
    /// gold/celeste palette as the panel and the radial wheel so the item reads
    /// as part of the mod at a glance.
    /// </summary>
    internal static class TerrainToolIcon
    {
        private const int Size = 64;

        private static readonly Color Outline = new Color(0.10f, 0.08f, 0.05f, 1f);
        private static readonly Color GoldDark = new Color(0.42f, 0.31f, 0.10f, 1f);
        private static readonly Color Gold = new Color(0.85f, 0.69f, 0.26f, 1f);
        private static readonly Color GoldLight = new Color(0.98f, 0.86f, 0.48f, 1f);

        /// <summary>Builds and returns the icon sprite, or null if it can't be made.</summary>
        public static Sprite Build(Color accent)
        {
            try
            {
                var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };

                var px = new Color32[Size * Size];
                for (int i = 0; i < px.Length; i++)
                    px[i] = new Color(0f, 0f, 0f, 0f);

                // Ground mound (two humps) across the bottom.
                for (int x = 9; x <= 55; x++)
                {
                    int hump = 12 + Mathf.RoundToInt(3f * Mathf.Sin((x - 9) / 46f * Mathf.PI * 2f));
                    Disc(px, x, hump - 2, 3, Outline);
                }
                for (int x = 10; x <= 54; x++)
                {
                    int hump = 12 + Mathf.RoundToInt(3f * Mathf.Sin((x - 10) / 44f * Mathf.PI * 2f));
                    Disc(px, x, hump - 2, 2, GoldDark);
                    Disc(px, x, hump - 1, 1, Gold);
                }

                // Rod shaft: diagonal, dark outline first then gold fill.
                Line(px, 19, 15, 45, 50, 4, Outline);
                Line(px, 19, 15, 45, 50, 3, GoldDark);
                Line(px, 20, 16, 44, 49, 2, Gold);
                Line(px, 21, 17, 43, 48, 1, GoldLight);

                // Binding near the grip.
                Disc(px, 24, 21, 4, Outline);
                Disc(px, 24, 21, 3, GoldDark);

                // Crystal tip: dark rim, accent body, bright core.
                Disc(px, 46, 51, 8, Outline);
                Disc(px, 46, 51, 6, accent);
                Disc(px, 46, 51, 3, new Color(
                    Mathf.Min(1f, accent.r + 0.4f),
                    Mathf.Min(1f, accent.g + 0.4f),
                    Mathf.Min(1f, accent.b + 0.4f), 1f));

                tex.SetPixels32(px);
                tex.Apply();

                return Sprite.Create(tex, new Rect(0f, 0f, Size, Size), new Vector2(0.5f, 0.5f), Size);
            }
            catch (System.Exception e)
            {
                TerrainForgePlugin.Log?.LogWarning("TerrainToolIcon: build failed: " + e.Message);
                return null;
            }
        }

        private static void Line(Color32[] px, int x0, int y0, int x1, int y1, int half, Color c)
        {
            int steps = Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0)) * 2 + 1;
            for (int s = 0; s <= steps; s++)
            {
                float t = (float)s / steps;
                Disc(px,
                    Mathf.RoundToInt(Mathf.Lerp(x0, x1, t)),
                    Mathf.RoundToInt(Mathf.Lerp(y0, y1, t)),
                    half, c);
            }
        }

        private static void Disc(Color32[] px, int cx, int cy, int r, Color c)
        {
            Color32 cc = c;
            int r2 = r * r;
            for (int y = cy - r; y <= cy + r; y++)
            {
                if (y < 0 || y >= Size)
                    continue;
                int ddy = y - cy;
                for (int x = cx - r; x <= cx + r; x++)
                {
                    if (x < 0 || x >= Size)
                        continue;
                    int ddx = x - cx;
                    if (ddx * ddx + ddy * ddy <= r2)
                        px[y * Size + x] = cc;
                }
            }
        }
    }
}
