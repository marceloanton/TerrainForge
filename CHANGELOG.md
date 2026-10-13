# Changelog

All notable changes to TerrainForge are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2026-10-13

### Added

- **Undo / redo** of brush applications (`Ctrl+Z` / `Ctrl+Y`, rebindable): every
  application (including ramps, paint and restore) records the old values of
  exactly the vertices it overwrites, so one step back restores them. Configurable
  history depth (`History > UndoDepth`, 0-500, 0 disables) with a slider + exact
  textbox in the in-game menu (TECLAS tab), live undo/redo counts and flash
  messages in the status HUD. History is cleared when leaving the world.
- **Live HUD data**: a second status line shows the aimed ground height, the
  current target (Flatten height / Raise-Lower step / paint / ramp A→B heights)
  and the local slope in degrees, updated every frame while aiming.
- **Eyedropper**: `EyedropKey` (default `G`) fixes the Flatten target at the aimed
  height until released (wins over the Shift modifier); HUD shows it as `fija`.
- **Fixed-angle ramps**: `RampMode` (`TwoPoint` / `FixedAngle`, toggled with
  `RampModeKey`, default `H`). In fixed-angle mode B only sets direction and
  length while heights follow `SlopeAngle` (up or down depending on the aim);
  adjust the angle in-game with `Alt` + radius keys/wheel.
- **New brush shapes**: `Line` (two-press capsule with the brush radius as
  half-width, reuses the corridor preview), `Rect` (two opposite corners, soft
  edges from radius + softness) and `Poly` (one point per press, double-press
  with 3+ points closes and applies). All area operations work over them, with
  undo support and HUD + preview state.
- **In-game AJUSTES tab** (F5 menu): everything is now editable live without
  touching the config file — operation / shape / paint / ramp-mode / smooth-mode
  cyclers, sliders (radius, softness, ramp angle, raise-lower meters, paint
  strength, falloff, shore offset) and all toggles (preview, guides, effects,
  no-cost, protected zones, delta limit, server authority). Saved on menu close.
- **Smooth modes**: `Full` blurs both ways, `Bumps` only lowers bumps, `Dips`
  only fills dips (AJUSTES cycler). Line shape + Smooth = smoothing along a path.
- **Shore operation**: flattens toward the water level plus an offset (beaches /
  coastlines), with softness blending like Flatten.
- **Preview upgrades**: footprint edge tints toward red on steep ground (slope
  shading) and Flatten/Shore show a target-level marker ring that reads as a
  contour on slopes.
- **Feedback effects**: the tool's own terrain-hit effect (dust + thud) plays on
  every application (throttled while holding, always on ramps/shapes); toggle
  with `General > PlayEffects`.
- **EN/ES localization**: `General > Language` switches the whole menu, HUD,
  messages and help between Español and English (key labels rebuild on open).
- **Radial menu**: hold `RadialKey` (default `Q`) to pick any of the 8 operations
  with the mouse; release confirms, `Esc` cancels. Suppresses brush application
  while open.
- **Named presets**: save / load / delete up to 12 full brush setups from the
  AJUSTES tab (stored in `Presets > Slots`, culture-invariant format).
- **Live measurements**: while marking, the HUD shows line/ramp length, rise and
  angle, rectangle W×D + area, polygon perimeter + area; final numbers are also
  appended to the confirmation messages.
- **Path ramps**: `RampMode.Path` marks a curve A→B→C… (double-press with 3+
  points closes) and builds a smooth Catmull-Rom ramp along it, width = radius;
  next-segment corridor + trail preview, live length, undo support.
- **Typed target height**: the eyedropper is now config-backed (`Brush >
  UseFixedHeight` / `FixedHeight`) with a textbox + Fix/Release row in AJUSTES;
  `G` captures into the same state, which persists and drives HUD + level ring.
- **Terrain snapshots**: save the edited tiles to a versioned binary file and
  restore them later (`BepInEx/config/TerrainForge/snapshots/`, AJUSTES UI with
  save/load/delete, 12-file cap); importing resolves tiles, restores arrays
  with ownership and is itself undoable; missing tiles report cleanly.
- **Wards + server caps**: `RespectWards` blocks edits inside foreign wards via
  the game's own check; `RadiusCap`/`DeltaCap` let the server clamp radius and
  delta, visibly (HUD + preview use the effective values). HUD `WARD` tag and
  AJUSTES rows included.
- **Chunked application**: big strokes process a few heightmap tiles per frame
  (`Performance > ChunkCompsPerFrame`, AJUSTES slider) instead of hitching;
  terrain appears progressively, one undo step per completed stroke, auto-
  continues if the key is released, FIFO order kept.
- **Server authority**: new `Server` section (`AllowTerrainEdits`, `AdminOnlyEdits`).
  When the server runs the mod, its values win through config sync and clients
  cannot apply, undo or redo against them (HUD shows `SERVER: BLOQUEADO` with the
  reason). Servers without the mod ignore these, so the client tool keeps working
  there for testing.

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
