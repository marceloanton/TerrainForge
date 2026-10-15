# Changelog / Registro de Cambios

All notable changes to TerrainForge are documented here.  
Todos los cambios notables de TerrainForge se documentan aquí.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),  
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).  
El formato sigue [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) y  
el proyecto usa [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added / Añadido

- **Donations / Donaciones (Cafecito)**: badge in README (shown on Thunderstore), `website_url` in manifest, and discreet button at bottom of in-game menu opening `https://cafecito.app/byrka`.

- **14 Languages + Auto-Detect / 14 Idiomas + Auto-Detect**: the mod automatically detects the game's language via reflection of `Localization.instance.GetSelectedLanguage()`. Supported: English, Español, Français, Deutsch, Русский, 中文 (Simplified), 繁體中文 (Traditional), 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano. Default `Auto`; falls back to English if translation incomplete. Configurable in `General > Language` (cycles through 15 options).

- **Falloff Modes for Raise/Lower (4 modes, cyclable in SETTINGS / Modos de Caída para Subir/Bajar — 4 modos, ciclables en AJUSTES)**: *Pointy/Punta* (original, sharp center), *Flat/Plano* (uniform disk), *Smooth/Suave* (cosine bell), *Linear/Lineal* (linear taper). The "Falloff Power" slider only affects Pointy mode.

- **Direct Operation Keys (9 new, no default key / Teclas Directas por Operación — 9 nuevas, sin tecla por defecto)**: Flatten, Raise, Lower, Smooth, Paint, Restore, Ramp, Shore, Snow. Assign in KEYS tab to jump directly to an operation without cycling.

- **Profile Cycle Key / Tecla para Ciclar Perfiles**: switches to next saved preset with one press.

- **Improved Ramp Preview / Vista Previa de Rampa Mejorada**: semi-transparent filled corridor between side edges + height markers (rings) at points A and B.

- **Underwater Preview / Vista Previa Bajo el Agua**: when aimed terrain is submerged, preview floats at water surface instead of staying at seabed.

- **Terrain Stamp (Copy/Paste Terrain in RAM / Terrain Stamp — Copiar/Pegar Terreno en RAM)**:
  - `Ctrl+Shift+C` → copies marked area (current shape: circle, rect, line, poly, ramp) to clipboard. Shows "Stamp copied: X tiles".
  - `Ctrl+Shift+V` → ghost preview (semi-transparent wireframe) centered at aim. Controls: `Q`/`E` rotate ±15°, `Alt`+wheel continuous rotate, `Shift`+wheel scale 0.25×–4×, `Esc` cancel, click (Apply) confirms.
  - Paste is a **single undo step**.
- **Stamp History (clipboard) / Historial de Stamps (portapapeles)**: up to 10 stamps in memory (newest first). Keys `Next Stamp` / `Previous Stamp` (rebindable) to cycle; flash shows "Stamp history: Copy: 3 tiles [2/5]".
- **Persistent Stamps on Disk / Stamps Persistentes en Disco**: in SETTINGS → **STAMPS** save current stamp with name (`.stamp` in `BepInEx/config/TerrainForge/stamps/`, max 12), load to clipboard, list and delete.
- **STAMPS Tab in SETTINGS / Pestaña STAMPS en AJUSTES**: full UI to save/load/delete saved stamps and view clipboard history.
- **Dedicated Stamp Tool / Herramienta Dedicada para Stamps**: config `Stamp Tool` (SETTINGS) with 4 options: `Same as Brush` (default), `Any`, `Pickaxe`, `TerrainForge Rod`. HUD shows `STAMP READY [2/5]` when correct tool equipped and stamps in clipboard; otherwise warns `Stamp: tool not equipped`. `Ctrl+Shift+C/V` do nothing without tool equipped.

### Fixed / Corregido

- **Preview: circles/square hidden on Line/Rect/Poly when marking**: when marking point A of line, rectangle, or polygon, only the marked outline (corridor/rectangle) is shown, no circular background rings.

- **Cursor visible in F5 menu and ESC button**: forces `Cursor.visible = true` and `CursorLockMode.None` in `OnGUI()` of `TerrainKeybinds`, overriding the game's attempt to hide/lock it.

- **Mouse wheel in radial menu (Q)**: uses `ZInput.GetMouseScrollWheel()` (Valheim's native API) to rotate selection — identical to vanilla radial menus (building, map). Scroll up = previous, down = next.

- **Menu window sizing**: scroll view with minimum height (400px) and initial window 560×600px; full content visible, scrollable with wheel.

- **Keybind capture**: now ignores pure modifier keys (Ctrl, Shift, Alt) as main key — they can only be modifiers. Supports mouse buttons (Mouse0/1/2) as main key. To assign `Ctrl+Shift+C`: hold `Ctrl+Shift` then press `C`.

- **Global scroll menu**: single scroll view wraps all 3 tabs (KEYS, HELP, SETTINGS). Scroll with wheel on any tab. "Close" and Cafecito buttons always visible at bottom.

- **Cursor forced visible on menu open**: forces `Cursor.visible = true` and `CursorLockMode.None` every frame while menu open, preventing game from hiding/locking it.

- **Line/Rect/Poly shapes no longer show circular guides** (edge, full-strength, max radius, aim distance were meaningless); only the marked outline (corridor, rectangle, trail) is drawn.

- **Seamless Smooth**: border vertices now sample neighbouring tile across edge (exact vertex mapping), so smoothing blends both sides instead of opening a visible seam/crack between tiles. Applies to all footprints; other ops were already continuous.

---

## [0.3.2] - 2026-10-14

### Fixed / Corregido

- **Preview: circles/square hidden on Line/Rect/Poly when marking** (point A) — only marked outline shown.
- **Cursor visible in F5 menu and ESC pause button** — forced via `OnGUI()`.
- **Mouse wheel in radial menu (Q)** — uses `ZInput.GetMouseScrollWheel()` (Valheim native API), identical to vanilla radial menus.
- **Menu window sizing** — initial 560×600, scroll view min 400px, fully scrollable.

### Added / Añadido

- **14 Languages + Auto-Detect** — auto-detects game language via reflection; 14 languages + English fallback.

---

## [0.3.1] - 2026-10-14

### Fixed / Corregido

- Version bump to resolve Thunderstore version conflict.

---

## [0.3.0] - 2026-10-13

### Added / Añadido

- **Undo/Redo** brush strokes (`Ctrl+Z`/`Ctrl+Y`, rebindable): each application (ramps, paint, restore) records exactly the vertices it overwrites. Configurable depth (`History > UndoDepth`, 0-500, 0 disables) with slider + textbox in KEYS tab, live undo/redo counts and flash messages. History clears on world leave.
- **Live HUD Data**: second status line shows aimed ground height, current target (Flatten height / Raise-Lower step / paint / ramp A→B heights) and local slope in degrees, updated every frame.
- **Eyedropper**: `EyedropKey` (default `G`) fixes Flatten target at aimed height until released (wins over Shift); HUD shows `fixed`.
- **Fixed-Angle Ramps**: `RampMode` (`TwoPoint`/`FixedAngle`, toggled with `H`). In fixed-angle B only sets direction/length; height follows `SlopeAngle` (up/down depending on aim); adjust angle in-game with `Alt` + radius keys/wheel.
- **New Brush Shapes**: `Line` (two-press capsule, radius as half-width, reuses corridor preview), `Rect` (two opposite corners, soft edges from radius+softness), `Poly` (one point per press, double-press with 3+ points closes and applies). All area ops work over them with undo support and HUD+preview state.
- **In-Game SETTINGS Tab (F5 menu)**: everything editable live — operation/shape/paint/ramp-mode/smooth-mode cyclers, sliders (radius, softness, ramp angle, raise-lower meters, paint strength, falloff, shore offset) and all toggles (preview, guides, effects, no-cost, protected zones, delta limit, server authority). Saved on menu close.
- **Smooth Modes**: `Full` blurs both ways, `Bumps` only lowers bumps, `Dips` only fills dips (SETTINGS cycler). Line shape + Smooth = path smoothing.
- **Shore Operation**: flattens toward water level + margin (beaches/coastlines), softness blends like Flatten.
- **Preview Upgrades**: footprint edge tints red on steep ground (slope shading); Flatten/Shore show target-level marker ring reading as contour on slopes.
- **Feedback Effects**: vanilla terrain-hit effect (dust+thud) plays on every application (throttled while holding, always on ramps/shapes); toggle `General > PlayEffects`.
- **EN/ES Localization**: `General > Language` switches entire menu, HUD, messages, help between Español and English (key labels rebuild on open).
- **Radial Menu**: hold `RadialKey` (default `Q`) to pick any of 8 operations with mouse; release confirms, `Esc` cancels. Suppresses brush while open.
- **Named Presets**: save/load/delete up to 12 full brush setups from SETTINGS tab (stored in `Presets > Slots`, culture-invariant format).
- **Live Measurements**: while marking, HUD shows line/ramp length, rise and angle, rectangle W×D + area, polygon perimeter + area; final numbers appended to confirmation messages.
- **Path Ramps**: `RampMode.Path` marks curve A→B→C… (double-press with 3+ closes) and builds smooth Catmull-Rom ramp along it, width = radius; next-segment corridor + trail preview, live length, undo support.
- **Typed Target Height**: eyedropper now config-backed (`Brush > UseFixedHeight` / `FixedHeight`) with textbox + Fix/Release row in SETTINGS; `G` captures into same state, persists and drives HUD + level ring.
- **Terrain Snapshots**: save edited tiles to versioned binary file and restore later (`BepInEx/config/TerrainForge/snapshots/`, SETTINGS UI with save/load/delete, 12-file cap); import resolves tiles, restores arrays with ownership, is itself undoable; missing tiles report cleanly.
- **Wards + Server Caps**: `RespectWards` blocks edits inside foreign wards via game's own check; `RadiusCap`/`DeltaCap` let server clamp radius/delta, visible in HUD+preview. HUD `WARD` tag and SETTINGS rows included.
- **Chunked Application**: large strokes process few heightmap tiles per frame (`Performance > TilesPerFrame`, SETTINGS slider) instead of hitching; terrain appears progressively, one undo step per completed stroke, auto-continues if key released, FIFO order.
- **Server Authority**: new `Server` section (`AllowTerrainEdits`, `AdminOnlyEdits`). When server runs mod, its values win via config sync; clients cannot apply/undo/redo against them (HUD shows `SERVER: BLOCKED` with reason). Servers without mod ignore this, client works for testing.

---

## [0.2.0] - 2026-10-13

*(Same as 0.3.0 above — version alignment)*

---

## [0.1.0] - 2026-10-12

### Added / Añadido

- Free-form terrain brush: **Flatten**, **Raise**, **Lower**, **Smooth**, **Ramp**, **Paint**, **Restore** applied under crosshair while apply key held. Flatten follows aimed ground height by default (hold modifier to level to feet); Smooth averages each point with neighbours for real smoothing.
- **TerrainForge Rod** (Hammer clone) as tool item driving the brush, with auto hand-out and equip on world enter, plus give key (`F8` default). Item registration uses Jotunn.
- **Ramp Operation** (two-point): press *apply* to mark point A, aim and press *apply* again for point B, straight slope joins both heights along A-B corridor (brush radius = ramp width). Corridor is flat across width with soft edge (softness controls side blend only), so ramp matches preview exactly.
- Configurable brush **radius** (up to 60 m), **shape** (circle/square), **paint type**, cyclable in-game.
- **Live Preview** while aiming: solid ring marks real generated area; faint guides in operation color show **full-strength zone**, **max brush radius**, **max aim distance** around player. In Ramp, real **A-B corridor** (both width edges) drawn while marking. Guides disableable with `Preview > ShowGuides`. Top-left status HUD (operation, radius, shape, paint).
- **In-Game Key Binding Menu** (`F5`, configurable) with custom skin, plus **in-game help tab** documenting tool and every operation.
- Removal of vanilla hard-coded **±8 m** terrain delta clamp (`UnlockDeltaLimit`), so large edits survive terrain regeneration. Must also be enabled on server.
- Optional vanilla-tool tweaks: scaled hoe/cultivator radius, **no build cost**, edits in **protected zones** (no-build / ward).
- Aim works over water by raymarching against terrain heightmap when no collider hit.
- **Required Tool** configurable (`Any`, `Pickaxe`, or mod's `TerrainForgeTool`).

### Notes / Notas

- Terrain edits persist via `TerrainComp` ZDO and are replicated by server, so TerrainForge must be on **both client and server**.
- Design principle: minimize external dependencies. Custom tool currently uses Jotunn; future version aims to register prefab via Harmony patch to remove Jotunn dependency.

---

## Links / Enlaces

[Unreleased]: https://github.com/marceloanton/TerrainForge/compare/0.3.2...HEAD  
[0.3.2]: https://github.com/marceloanton/TerrainForge/releases/tag/v0.3.2  
[0.3.1]: https://github.com/marceloanton/TerrainForge/releases/tag/v0.3.1  
[0.3.0]: https://github.com/marceloanton/TerrainForge/releases/tag/v0.3.0  
[0.1.0]: https://github.com/marceloanton/TerrainForge/releases/tag/v0.1.0