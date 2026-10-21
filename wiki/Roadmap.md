# Roadmap

*This page is currently available in English only.*

Planned direction for TerrainForge. Order and scope may change; done items ship in the listed release.

---

## Done

### 0.3.x — Brush & UI foundation
- Free-form terrain brush (Flatten, Raise, Lower, Smooth, Ramp, Shore, Paint, Restore) beyond the vanilla ±8 m limit; Snow is a Paint type (`DeepSnow`).
- 5 brush shapes: Circle, Square, Line, Rect, Polygon.
- 3 falloff modes (Two-point, Fixed-angle, Path) and configurable Shore falloff.
- Undo / Redo and terrain **snapshots** (full backup / restore).
- **Terrain stamps** (copy / paste) with an on-disk library and preview.
- **Dedicated Copy/Paste Q-wheel mode** — select a rectangle, line, polygon, circle, or square footprint, then preview and place terrain stamps.
- **F5 menu improvements** — six-tab layout, responsive sizing, dimmed background, and frozen local player/camera controls while open.
- Valheim-native UI (Averia typeface, Valheim palette), radial menu (`Q`), keybind remapping (`F5`), persistent HUD.
- Slope heatmap (`H`) and height heatmap (`J`).
- Brush profiles and presets.
- **14 in-game languages** with auto-detect, plus a full wiki manual in 14 languages.

---

## Planned

### 0.4.0 — "Advanced Tools"
- Blueprint ghost system: translucent preview with a snap grid (1 m / 2 m / 4 m) and rotation steps (90° / 45° / 15°).
- Stamp Preview Pro: real contour lines drawn over the terrain.
- Live measurement HUD while marking (distance / area / slope).

### 0.5.0 — "World Building"
- Non-biome textures + a per-biome / custom selector.
- Complete blueprint system (save / load / place).
- Multiplayer: stamp sync between clients and per-rank permissions.

### 0.6.0 — "World Builder"
- Blueprint library (save / load / share).
- Terrain prefabs (templates) and a biome painter.
- River / road tool driven by a spline.
- Performance: chunk streaming, LOD for large stamps, multithreading.

---

## Under evaluation
- **Non-biome textures**: the terrain uses a single material per heightmap with a mask that only picks dirt / cultivated / paved / snow inside the biome palette. There is no exposed biome → material map, so this is a client-side, fragile hack. A shader/texture-slot diagnostic has been added to the log to assess feasibility.
