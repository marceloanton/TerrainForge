# Changelog

---

## [0.3.5] - 2026-10-17

### Added

- **Terrain stamps — rectangle selection copy**: mark corner A with the Rectangle shape, aim at corner B and press `Ctrl+Shift+C` to copy exactly that rectangle, cropped to the vertex level (no whole-tile blocks or surrounding untouched terrain). Pasting only writes inside the copied region.
- **Free camera (`F`) reworked**: while flying, right-click no longer triggers lock-on, brush drift or a screenshot; `Ctrl`+wheel adjusts the brush radius; brush adjustments keep priority over free-fly input; on-screen banner shows the fly distance.

### Changed

- **Saved stamps** now use version 2 (adds the selection mask); version 1 stamps still load.
- **Documentation**: README and CHANGELOG are now in English, with a complete wiki manual available in **14 languages**. Internal roadmap added.

### Fixed

- **Free-fly camera vs. brush input**: suppressed vanilla `SecondaryAttack` / `Block` / `Attack` handling and the scroll-wheel conflict while flying, so aiming, radius and adjustments work from the free camera.
- Preview of a region-cropped stamp now skips cells outside the copied rectangle.

---

## [0.3.4] - 2026-10-15

### Added

- **Persistent HUD**: position coordinates, current biome and nearby resources (with counts) always visible next to the tool status.
- **"Reset keybinds" button**: in the keys tab, resets every keybind to its default value in one click.
- **Valheim-native UI**: full UI redesign using the Averia typeface and Valheim's palette (gold, dark greys), with native-styled buttons, panels, tabs and scrollbars.
- **Native radial menu (Q)**: working mouse wheel via `ZInput.GetMouseScrollWheel()`, fixed button hover (radius 34–130 px), identical to vanilla radial menus.
- **Forced visible cursor**: in the F5 menu and the ESC button, the cursor is forced visible and unlocked every frame via `OnGUI()`.
- **Slope heatmap (H)**: slope overlay: 🟢 0–15° → 🟡 15–30° → 🟠 30–45° → 🔴 45°+.
- **Height heatmap (J)**: height overlay: 🔵 low → 🟢 mid → 🟡 high → 🔴 peak.
- **Stamp Preview Pro**: contour lines in the stamp preview — draws a 1 m curve over the stamp area in Valheim gold.
- **Improved Shore operation**: a dedicated falloff mode for Shore (`ShoreFalloffMode`: Linear, Smooth, Flat, Pointy), defaulting to Linear for more natural beaches; a linear falloff is used by default to reach the water level more reliably; configurable in SETTINGS → "Shore falloff".
- **Visible ramp-mode selector**: the ramp mode (two-point / fixed-angle / path) is now shown as three selectable buttons instead of a single cycling button.
- **"Quick actions" section** in the keys tab: buttons for Undo, Redo, Fix height, Copy/Paste stamp, Previous stamp, Clear marks and the heatmaps — no need to rely only on the hotkey.
- **Vanilla tool blocking** (`BlockVanillaTerrainTools`): while the TerrainForge tool is equipped, vanilla hoe/cultivator terrain edits are ignored.
- **Material diagnostic (log)**: a button that dumps the terrain shader and texture slots to the log, to assess the feasibility of non-biome textures.
- **Tool color** (`ToolTint` / `ToolTintColor`): tints the model and icon of the TerrainForge Rod (a Hammer clone) to tell it apart from the vanilla Hammer at a glance. Only affects this mod's tool.

### Changed

- **F5 panel reorganized**: the SETTINGS tab now groups options into labeled sections (Tool, View, Vanilla, Server, General) instead of a single block of toggles; the tool tint (`ToolTint` / `ToolTintColor`) is editable directly in the UI.

### Fixed

- Preview: circles/square hidden on Line/Rect/Poly while marking point A.
- Cursor visible in the F5 menu and the ESC button (OnGUI forces it visible).
- **Radial menu (Q): the mouse wheel now keeps the selection.** Previously, with a centered cursor, the pick was wiped instantly and the wheel looked unresponsive; now only the pointer over an option changes the selection.
- **Alt+Wheel (fixed height): more reliable** wheel reading.
- Menu window: initial size 680×720, min scroll 460 px, wider keybind rows.

---

## [0.3.3] - 2026-10-15

### Added
- **Professional bilingual documentation (ES/EN)**: README, CHANGELOG and WIKI fully rewritten in both languages with professional formatting, tables and complete coverage of every feature.

### Fixed
- Radial menu (Q): working mouse wheel (uses native `ZInput.GetMouseScrollWheel`), fixed button hover.
- Preview: circles/square hidden on Line/Rect/Poly while marking point A.
- Cursor visible in the F5 menu and the ESC button (OnGUI forces it visible).
- Mouse wheel in the radial menu (Q) using Valheim's native API.
- Menu window: initial size 560×600, min scroll 400 px.

---

## [0.3.2] - 2026-10-14

### Added
- **14 languages + auto-detect**: automatic detection via reflection (`Localization.instance.GetSelectedLanguage()`). Languages: English, Español, Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano. Default `Auto`; falls back to English.

### Fixed
- Preview: circles/square hidden on Line/Rect/Poly while marking point A.
- Cursor visible in the F5 menu and the ESC button (OnGUI forces it visible).
- Mouse wheel in the radial menu (Q) using native `ZInput.GetMouseScrollWheel()`.
- Menu window: scroll view min height 400 px, initial window 560×600 px.

---

## [0.3.1] - 2026-10-14
- Version fix to resolve a Thunderstore conflict.

---

## [0.3.0] - 2026-10-13

### Added
- **Undo / Redo** (`Ctrl+Z`/`Ctrl+Y`): every application stores exactly the vertices it touches. Configurable depth (0–500) with a slider + textbox in the KEYS tab. Live counters in the HUD. History is cleared when leaving the world.
- **Live HUD data**: a second line with the aimed height, current target and local slope in degrees.
- **Eyedropper** (`G`): fixes the Flatten height at the aimed point (takes priority over Shift). The HUD shows `fixed`.
- **Fixed-angle ramps** (`H`): a `FixedAngle` mode where B gives direction/length and the height follows `SlopeAngle`. Adjust in-game with `Alt` + radius/wheel.
- **New shapes**: `Line` (two-click capsule), `Rectangle` (two corners), `Polygon` (click per vertex, double-click closes). All with undo and preview.
- **In-game SETTINGS tab** (F5): everything editable live — cyclers, sliders, toggles. Saved on close.
- **Smooth modes**: `Full` / `Ridges only` / `Dips only` (cycle in SETTINGS). Line + Smooth = smooth along a path.
- **Shore operation**: flattens toward the water level + margin (beaches). Softness blends like Flatten.
- **Preview improvements**: the footprint edge tints red on slopes; Flatten/Shore show a target-level ring.
- **Feedback effects**: native dust + hit effect on every application (throttled while holding). Toggle `General > PlayEffects`.
- **EN/ES localization**: `General > Language` switches the menu, HUD, messages and help. Key labels are rebuilt when the menu opens.
- **Radial menu** (`Q`): choose the operation with the mouse; release confirms, `Esc` cancels. Suppresses the brush while open.
- **Named profiles**: save/load/delete up to 12 full setups (SETTINGS → Profiles).
- **Live measurements**: the HUD shows line/ramp length, rise and angle, rectangle W×D+area, polygon perimeter+area.
- **Path ramps** (`Path`): a curve A→B→C… (double-press closes) → smooth Catmull-Rom. Preview corridor + trail, live length, undo.
- **Target height by text**: `UseFixedHeight`/`FixedHeight` in config + a Fix/Release textbox in SETTINGS. `G` captures in the same state.
- **Terrain snapshots**: saves edited tiles in a versioned binary (`.dat` in `snapshots/`, max 12). Import restores arrays with ownership and is undoable; missing tiles warn.
- **Wards + server caps**: `RespectWards` blocks foreign wards; `RadiusCap`/`DeltaCap` are server-enforced caps visible in the HUD/preview.
- **Chunked application**: configurable tiles/frame (`TilesPerFrame`, 1–32). Terrain appears progressively, one undo per stroke, auto-continues if you release the key.
- **Server authority**: `Server` section (`AllowTerrainEdits`, `AdminOnlyEdits`). Server values win via config sync; clients cannot apply/undo/redo against them (HUD `SERVER: BLOCKED`). Servers without the mod ignore the section.

---

## [0.1.0] - 2026-10-12

### Added
- Free brush: **Flatten**, **Raise**, **Lower**, **Smooth**, **Ramp**, **Paint**, **Restore** under the crosshair. Flatten follows the aimed height (Shift = level at your feet); Smooth averages neighbours.
- **TerrainForge Rod** (Hammer clone): auto-given/equipped on join, give key `F8`. Registered via Jotunn.
- **Two-point ramp**: click A, click B → a straight slope joins both heights (radius = ramp width). A wide, flat corridor with a soft edge (softness = lateral blend).
- Configurable brush radius (60 m), **shape** (circle/square), **paint** cyclable in-game.
- **Live preview**: a solid ring = the real area; faint guides in the operation color = full-strength zone, max radius, max aim distance. For Ramp: the real A–B corridor (both edges). Guides toggleable via `Preview > ShowGuides`. Top-left HUD (op, radius, shape, paint).
- **In-game keybind menu** (`F5`, configurable) with its own skin + an **in-game help tab**.
- Removal of the vanilla **±8 m** clamp (`UnlockDeltaLimit`). Must also be on the server.
- Optional vanilla tweaks: scaled hoe/cultivator radius, **no build cost**, edits in **protected zones**.
- The crosshair works over water (heightmap raymarch without a collider).
- Configurable **required tool** (`Any`, `Pickaxe`, `TerrainForgeTool`).

### Notes
- Edits persist via the `TerrainComp` ZDO and replicate through the server → **required on both client and server**.
- Principle: minimal dependencies. The tool uses Jotunn for now; a future version will register the prefab via Harmony so Jotunn is not needed.
