# Changelog

All notable changes to TerrainForge are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **Undo / redo** of brush applications (`Ctrl+Z` / `Ctrl+Y`, rebindable): every
  application (including ramps, paint and restore) records the old values of
  exactly the vertices it overwrites, so one step back restores them. Configurable
  history depth (`History > UndoDepth`, 0-500, 0 disables) with a slider + exact
  textbox in the in-game menu (TECLAS tab), live undo/redo counts and flash
  messages in the status HUD. History is cleared when leaving the world.

## [0.1.0] - 2026-10-12

Initial release.

### Added

- Free-form terrain brush: **Flatten**, **Raise**, **Lower**, **Smooth**,
  **Ramp**, **Paint** and **Restore** operations, applied under the crosshair while
  the apply key is held. Flatten follows the aimed ground height by default (hold
  the modifier to level to your own feet); Smooth blurs each point with its
  neighbours for a real smoothing effect.
- **TerrainForge Rod** tool item (a clone of the Hammer) used to drive the brush,
  with automatic hand-out and equip on entering a world, plus a give key (default
  `F8`). Registering the item uses Jotunn.
- **Ramp** operation (two-point): press *apply* to mark point A, then aim and press
  *apply* again to mark point B, and a straight slope joins the two heights along
  the A-B corridor (the brush radius sets the ramp width). The corridor is flat
  across its full width with a soft edge (softness controls only the side blend),
  so the ramp matches the previewed size exactly.
- Configurable brush **radius** (up to 60 m), **shape** (circle / square) and
  **paint** type, cycleable in-game.
- Live footprint **preview** while aiming: a solid ring marks the real generated
  area, and thin, faint guides in the operation colour show the **full-strength
  zone**, the **maximum brush radius** and the **maximum aim distance** around the
  player. In the Ramp operation the real **A-B corridor** (both width edges) is
  drawn while marking. Guides can be disabled with `Preview > ShowGuides`. Plus a
  top-left status **HUD** (operation, radius, shape, paint).
- **In-game key binding menu** (`F5`, configurable) with a custom skin, plus an
  **in-game help tab** documenting the tool and every operation.
- Removal of the vanilla hard-coded **+/-8 m** terrain delta clamp
  (`UnlockDeltaLimit`), so large edits survive terrain regeneration. Must also be
  enabled on the server.
- Optional vanilla-tool tweaks: scaled hoe / cultivator radius, **no build cost**,
  and edits inside **protected zones** (no-build / ward).
- Aim that works over water by raymarching against the terrain heightmap when no
  collider is hit.
- Configurable **required tool** (`Any`, `Pickaxe`, or the mod's `TerrainForgeTool`).

### Notes

- Terrain edits persist via the `TerrainComp` ZDO and are replicated by the
  server, so TerrainForge must be installed on **both client and server**.
- Design principle: keep external dependencies to a minimum. The custom tool item
  currently relies on Jotunn; a future release aims to register the prefab via a
  Harmony patch so Jotunn is not required.

[Unreleased]: https://example.com/terrainforge/compare/0.1.0...HEAD
[0.1.0]: https://example.com/terrainforge/releases/tag/0.1.0
