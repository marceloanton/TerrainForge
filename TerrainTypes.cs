using UnityEngine;

namespace TerrainForge
{
    public enum TerrainOperation
    {
        Flatten,
        Raise,
        Lower,
        Smooth,
        Paint,
        Restore,
        Ramp
    }

    public enum TerrainShape
    {
        Circle,
        Square
    }

    /// <summary>
    /// Which equipped item enables the brush.
    /// </summary>
    public enum ToolRequirement
    {
        Any,
        Pickaxe,
        TerrainForgeTool
    }

    public enum TerrainPaint
    {
        Dirt,
        Cultivated,
        Paved,
        ClearVegetation,
        DeepSnow,
        Reset
    }

    /// <summary>
    /// Fully describes a single brush application. Built by <see cref="TerrainBrush"/>
    /// from config + input state and consumed by <see cref="TerrainEngine"/>.
    /// </summary>
    public struct BrushSpec
    {
        public TerrainOperation Operation;
        public TerrainShape Shape;
        public TerrainPaint Paint;

        public float Radius;         // world units, horizontal
        public float Softness;       // 0..100, edge falloff amount
        public float Power;          // falloff exponent for raise/lower/smooth
        public float RaiseDelta;     // world units to raise/lower per application
        public float TargetWorld;    // world Y used by flatten/smooth
        public float PaintStrength;  // 0..1
        public float MaxUp;          // clamp on levelDelta above base
        public float MaxDown;        // clamp on levelDelta below base (positive number)

        // Two-point ramp: when set, a straight ramp is built along the A->B segment
        // (width = Radius) instead of using the circular brush footprint.
        public bool HasRampLine;
        public Vector3 RampA;
        public Vector3 RampB;

        public TerrainPaint PaintType => Paint;
    }

    /// <summary>Old values of one touched terrain vertex, for undo/redo.</summary>
    internal struct UndoCell
    {
        public int Index;
        public float Level;
        public float Smooth;
        public bool ModifiedHeight;
        public Color Paint;
        public bool ModifiedPaint;
    }

    /// <summary>All vertices changed on one TerrainComp by one brush application.</summary>
    internal sealed class UndoEntry
    {
        public TerrainComp Comp;
        public Heightmap Hm;
        public bool WithSmooth;
        public bool WithPaint;
        public System.Collections.Generic.List<UndoCell> Before =
            new System.Collections.Generic.List<UndoCell>();
    }

    /// <summary>One undoable step: every comp touched by a single Apply call.</summary>
    internal sealed class UndoFrame
    {
        public string Label;
        public Vector3 Center;
        public float Radius;
        public System.Collections.Generic.List<UndoEntry> Entries =
            new System.Collections.Generic.List<UndoEntry>();
    }
}
