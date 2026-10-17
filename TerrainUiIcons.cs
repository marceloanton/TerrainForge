using System.Collections.Generic;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Generates small picker icons at runtime for the F5 panel: a coloured disc
    /// per terrain operation (with a simple glyph) and a geometric outline per
    /// brush shape. Cached per key; textures are freed on <see cref="Clear"/>.
    /// </summary>
    internal static class TerrainUiIcons
    {
        private const int N = 44;

        private static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();

        private static readonly Color Bg = new Color(0.10f, 0.10f, 0.12f, 1f);

        public static Texture2D Op(TerrainOperation op)
        {
            string key = "op:" + (int)op;
            if (Cache.TryGetValue(key, out Texture2D cached) && cached != null)
                return cached;

            Color c = TerrainBrush.OpColor(op, TerrainPaint.Cultivated, false);
            var t = NewTex(key);
            var px = Blank();
            Disc(px, N / 2f, N / 2f, 19f, Bg);
            Ring(px, N / 2f, N / 2f, 19f, 2.5f, c);
            Glyph(px, op, c);
            Finish(t, px);
            return t;
        }

        public static Texture2D Shape(TerrainShape shape)
        {
            string key = "shape:" + (int)shape;
            if (Cache.TryGetValue(key, out Texture2D cached) && cached != null)
                return cached;

            Color c = TerrainTheme.TextGold;
            var t = NewTex(key);
            var px = Blank();
            Disc(px, N / 2f, N / 2f, 19f, Bg);
            Ring(px, N / 2f, N / 2f, 19f, 2.5f, c);

            float cx = N / 2f, cy = N / 2f;
            switch (shape)
            {
                case TerrainShape.Circle:
                    Ring(px, cx, cy, 11f, 3f, c);
                    break;
                case TerrainShape.Square:
                    RectO(px, cx - 11f, cy - 11f, cx + 11f, cy + 11f, 3f, c);
                    break;
                case TerrainShape.Line:
                    Line(px, cx - 12f, cy, cx + 12f, cy, 3f, c);
                    Disc(px, cx - 12f, cy, 3f, c);
                    Disc(px, cx + 12f, cy, 3f, c);
                    break;
                case TerrainShape.Rect:
                    RectO(px, cx - 13f, cy - 8f, cx + 13f, cy + 8f, 3f, c);
                    break;
                case TerrainShape.Poly:
                    Poly(px, cx, cy, 12f, 6, 3f, c);
                    break;
            }
            Finish(t, px);
            return t;
        }

        public static void Clear()
        {
            foreach (Texture2D t in Cache.Values)
                if (t != null)
                    Object.Destroy(t);
            Cache.Clear();
        }

        // ---- glyphs ----------------------------------------------------------

        private static void Glyph(Color32[] px, TerrainOperation op, Color c)
        {
            float cx = N / 2f, cy = N / 2f;
            switch (op)
            {
                case TerrainOperation.Raise:
                    Tri(px, cx, cy, 9f, 9f, true, c);
                    break;
                case TerrainOperation.Lower:
                    Tri(px, cx, cy, 9f, 9f, false, c);
                    break;
                case TerrainOperation.Smooth:
                    Line(px, cx - 9f, cy + 3f, cx + 9f, cy + 3f, 2f, c);
                    Line(px, cx - 9f, cy - 3f, cx + 9f, cy - 3f, 2f, c);
                    break;
                case TerrainOperation.Restore:
                    Ring(px, cx, cy, 8f, 3f, c);
                    Disc(px, cx + 8f, cy + 6f, 2.5f, c);
                    break;
                case TerrainOperation.Ramp:
                    Line(px, cx - 9f, cy - 8f, cx + 8f, cy + 8f, 3f, c);
                    break;
                case TerrainOperation.Shore:
                    Line(px, cx - 10f, cy + 4f, cx + 10f, cy + 4f, 2f, c);
                    Wave(px, cx, cy - 4f, 10f, c);
                    break;
                case TerrainOperation.Snow:
                    Line(px, cx - 8f, cy, cx + 8f, cy, 2f, c);
                    Line(px, cx, cy - 8f, cx, cy + 8f, 2f, c);
                    Line(px, cx - 6f, cy - 6f, cx + 6f, cy + 6f, 2f, c);
                    Line(px, cx - 6f, cy + 6f, cx + 6f, cy - 6f, 2f, c);
                    break;
                case TerrainOperation.Paint:
                    Disc(px, cx - 2f, cy - 2f, 7f, c);
                    Line(px, cx + 3f, cy + 3f, cx + 9f, cy + 9f, 3f, c);
                    break;
                default:
                    Disc(px, cx, cy, 6f, c);
                    break;
            }
        }

        private static void Wave(Color32[] px, float cx, float cy, float w, Color c)
        {
            float prevX = cx - w, prevY = cy;
            for (int i = 1; i <= 16; i++)
            {
                float t = i / 16f;
                float x = cx - w + t * 2f * w;
                float y = cy + Mathf.Sin(t * Mathf.PI * 2f) * 4f;
                Line(px, prevX, prevY, x, y, 2f, c);
                prevX = x; prevY = y;
            }
        }

        // ---- primitives ------------------------------------------------------

        private static Color32[] Blank()
        {
            var px = new Color32[N * N];
            for (int i = 0; i < px.Length; i++)
                px[i] = new Color(0f, 0f, 0f, 0f);
            return px;
        }

        private static Texture2D NewTex(string key)
        {
            var t = new Texture2D(N, N, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Cache[key] = t;
            return t;
        }

        private static void Finish(Texture2D t, Color32[] px)
        {
            t.SetPixels32(px);
            t.Apply();
        }

        private static void Px(Color32[] px, int x, int y, Color c)
        {
            if (x < 0 || y < 0 || x >= N || y >= N)
                return;
            px[y * N + x] = c;
        }

        private static void Disc(Color32[] px, float cx, float cy, float r, Color c)
        {
            int x0 = Mathf.FloorToInt(cx - r), x1 = Mathf.CeilToInt(cx + r);
            int y0 = Mathf.FloorToInt(cy - r), y1 = Mathf.CeilToInt(cy + r);
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                    if (dx * dx + dy * dy <= r * r)
                        Px(px, x, y, c);
                }
        }

        private static void Ring(Color32[] px, float cx, float cy, float r, float th, Color c)
        {
            int x0 = Mathf.FloorToInt(cx - r - 1), x1 = Mathf.CeilToInt(cx + r + 1);
            int y0 = Mathf.FloorToInt(cy - r - 1), y1 = Mathf.CeilToInt(cy + r + 1);
            float inner = r - th;
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d <= r && d >= inner)
                        Px(px, x, y, c);
                }
        }

        private static void Line(Color32[] px, float x0, float y0, float x1, float y1, float th, Color c)
        {
            int steps = Mathf.Max(1, Mathf.RoundToInt(Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0)) * 2f));
            for (int s = 0; s <= steps; s++)
            {
                float t = (float)s / steps;
                Disc(px, Mathf.Lerp(x0, x1, t), Mathf.Lerp(y0, y1, t), th * 0.5f, c);
            }
        }

        private static void RectO(Color32[] px, float x0, float y0, float x1, float y1, float th, Color c)
        {
            Line(px, x0, y0, x1, y0, th, c);
            Line(px, x1, y0, x1, y1, th, c);
            Line(px, x1, y1, x0, y1, th, c);
            Line(px, x0, y1, x0, y0, th, c);
        }

        private static void Poly(Color32[] px, float cx, float cy, float r, int sides, float th, Color c)
        {
            float px0 = cx + Mathf.Cos(0f) * r, py0 = cy + Mathf.Sin(0f) * r;
            float firstX = px0, firstY = py0;
            float lastX = px0, lastY = py0;
            for (int i = 1; i <= sides; i++)
            {
                float a = i / (float)sides * Mathf.PI * 2f;
                float x = cx + Mathf.Cos(a) * r, y = cy + Mathf.Sin(a) * r;
                Line(px, lastX, lastY, x, y, th, c);
                lastX = x; lastY = y;
            }
            Line(px, lastX, lastY, firstX, firstY, th, c);
        }

        private static void Tri(Color32[] px, float cx, float cy, float w, float h, bool up, Color c)
        {
            for (int y = Mathf.FloorToInt(cy - h); y <= Mathf.CeilToInt(cy + h); y++)
            {
                float t = up ? (y - (cy - h)) / (2f * h) : 1f - (y - (cy - h)) / (2f * h);
                t = Mathf.Clamp01(t);
                int half = Mathf.RoundToInt(w * t);
                for (int x = Mathf.RoundToInt(cx) - half; x <= Mathf.RoundToInt(cx) + half; x++)
                    Px(px, x, y, c);
            }
        }
    }
}
