using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// Named brush presets stored in a single config string:
    /// name|radius|softness|op|shape|paint|rampMode|slope|smooth|raiseDelta|paintStrength|falloff|shoreOffset; ...
    /// </summary>
    internal static class TerrainPresets
    {
        private const int MaxSlots = 12;

        internal sealed class Preset
        {
            public string Name = "";
            public float Radius = 8f;
            public float Softness = 50f;
            public TerrainOperation Operation = TerrainOperation.Flatten;
            public TerrainShape Shape = TerrainShape.Circle;
            public TerrainPaint Paint = TerrainPaint.Dirt;
            public RampMode RampMode = RampMode.TwoPoint;
            public float SlopeAngle = 30f;
            public SmoothMode SmoothMode = SmoothMode.Full;
            public float RaiseDelta = 2f;
            public float PaintStrength = 1f;
            public float FalloffPower = 3f;
            public FalloffMode FalloffMode = FalloffMode.Pointy;
            public float ShoreOffset = 0.5f;
        }

        public static List<Preset> Load()
        {
            var list = new List<Preset>();
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.PresetSlots == null)
                return list;
            string raw = p.PresetSlots.Value;
            if (string.IsNullOrEmpty(raw))
                return list;
            string[] entries = raw.Split(';');
            for (int i = 0; i < entries.Length && list.Count < MaxSlots; i++)
            {
                Preset pr = Parse(entries[i]);
                if (pr != null)
                    list.Add(pr);
            }
            return list;
        }

        public static void SaveCurrent(string name)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null)
                return;
            name = (name ?? "").Trim().Replace("|", " ").Replace(";", " ");
            if (name.Length == 0)
                return;

            var list = Load();
            list.RemoveAll(x => x.Name == name);
            list.Add(Capture(p, name));
            while (list.Count > MaxSlots)
                list.RemoveAt(0);
            Write(list);
        }

        public static void Delete(string name)
        {
            var list = Load();
            list.RemoveAll(x => x.Name == name);
            Write(list);
        }

        public static void ApplyPreset(Preset s)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || s == null)
                return;
            p.Radius.Value = Mathf.Clamp(s.Radius, 1f, 100f);
            p.Softness.Value = Mathf.Clamp(s.Softness, 0f, 100f);
            p.DefaultOperation.Value = s.Operation;
            p.Shape.Value = s.Shape;
            p.Paint.Value = s.Paint;
            p.RampMode.Value = s.RampMode;
            p.SlopeAngle.Value = Mathf.Clamp(s.SlopeAngle, 5f, 60f);
            p.SmoothMode.Value = s.SmoothMode;
            p.RaiseDelta.Value = Mathf.Clamp(s.RaiseDelta, 0.5f, 10f);
            p.PaintStrength.Value = Mathf.Clamp(s.PaintStrength, 0f, 1f);
            p.FalloffPower.Value = Mathf.Clamp(s.FalloffPower, 1f, 5f);
            p.FalloffMode.Value = s.FalloffMode;
            p.ShoreOffset.Value = Mathf.Clamp(s.ShoreOffset, -2f, 5f);
        }

        private static Preset Capture(TerrainForgePlugin p, string name)
        {
            return new Preset
            {
                Name = name,
                Radius = p.Radius.Value,
                Softness = p.Softness.Value,
                Operation = p.DefaultOperation.Value,
                Shape = p.Shape.Value,
                Paint = p.Paint.Value,
                RampMode = p.RampMode.Value,
                SlopeAngle = p.SlopeAngle.Value,
                SmoothMode = p.SmoothMode.Value,
                RaiseDelta = p.RaiseDelta.Value,
                PaintStrength = p.PaintStrength.Value,
                FalloffPower = p.FalloffPower.Value,
                FalloffMode = p.FalloffMode.Value,
                ShoreOffset = p.ShoreOffset.Value
            };
        }

        private static void Write(List<Preset> list)
        {
            TerrainForgePlugin p = TerrainForgePlugin.Instance;
            if (p == null || p.PresetSlots == null)
                return;
            System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
            var parts = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                Preset s = list[i];
                parts.Add(string.Join("|", new string[]
                {
                    s.Name,
                    s.Radius.ToString("0.##", inv), s.Softness.ToString("0.##", inv),
                    ((int)s.Operation).ToString(inv), ((int)s.Shape).ToString(inv), ((int)s.Paint).ToString(inv),
                    ((int)s.RampMode).ToString(inv), s.SlopeAngle.ToString("0.##", inv), ((int)s.SmoothMode).ToString(inv),
                    s.RaiseDelta.ToString("0.##", inv), s.PaintStrength.ToString("0.##", inv),
                    s.FalloffPower.ToString("0.##", inv), ((int)s.FalloffMode).ToString(inv), s.ShoreOffset.ToString("0.##", inv)
                }));
            }
            p.PresetSlots.Value = string.Join(";", parts.ToArray());
        }

        private static Preset Parse(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return null;
            string[] f = entry.Split('|');
            if (f.Length != 14)
                return null;
            try
            {
                System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
                var s = new Preset { Name = f[0].Trim() };
                if (s.Name.Length == 0)
                    return null;
                s.Radius = float.Parse(f[1], inv);
                s.Softness = float.Parse(f[2], inv);
                s.Operation = (TerrainOperation)int.Parse(f[3], inv);
                s.Shape = (TerrainShape)int.Parse(f[4], inv);
                s.Paint = (TerrainPaint)int.Parse(f[5], inv);
                s.RampMode = (RampMode)int.Parse(f[6], inv);
                s.SlopeAngle = float.Parse(f[7], inv);
                s.SmoothMode = (SmoothMode)int.Parse(f[8], inv);
                s.RaiseDelta = float.Parse(f[9], inv);
                s.PaintStrength = float.Parse(f[10], inv);
                s.FalloffPower = float.Parse(f[11], inv);
                s.FalloffMode = (FalloffMode)int.Parse(f[12], inv);
                s.ShoreOffset = float.Parse(f[13], inv);
                if (!System.Enum.IsDefined(typeof(TerrainOperation), s.Operation)) return null;
                if (!System.Enum.IsDefined(typeof(TerrainShape), s.Shape)) return null;
                if (!System.Enum.IsDefined(typeof(TerrainPaint), s.Paint)) return null;
                if (!System.Enum.IsDefined(typeof(RampMode), s.RampMode)) return null;
                if (!System.Enum.IsDefined(typeof(SmoothMode), s.SmoothMode)) return null;
                if (!System.Enum.IsDefined(typeof(FalloffMode), s.FalloffMode)) return null;
                return s;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// In-mod key rebinding UI with a custom skin. Toggle with the configured menu
    /// key (default F5); click a binding then press a key / mouse button. Saved to config.
    /// </summary>
}
