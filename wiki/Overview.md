# TerrainForge Overview

[Manual (English)](Home) · [Changelog](Changelog)

[![Buy me a coffee at cafecito.app](https://cdn.cafecito.app/imgs/buttons/button_5.svg)](https://cafecito.app/byrka)

A free-form terrain brush for Valheim. Flatten, raise, lower, smooth, build **ramps** (two-point, fixed-angle and path), shape **shorelines**, paint and restore terrain of any size — beyond the vanilla ±8 m limit — with undo/redo, five brush shapes, a radial menu, saved profiles, **terrain stamps (copy/paste)**, terrain snapshots and **14 in-game languages with auto-detect**.

> 📖 **Full documentation & translations (14 languages):** [Wiki](https://github.com/marceloanton/TerrainForge/wiki) — see the **Documentation languages** list below.

---

## What's New in 0.3.5

- **Terrain stamps — rectangle selection (A→B)** — with the Rectangle shape, mark corner **A** (right-click once), aim at **B** and press `Ctrl+Shift+C` to copy **exactly that rectangle**, cropped to the vertex level (no whole 64 m tiles, no surrounding untouched terrain). Pasting writes **only inside the copied region**.
- **Free camera (`F`) fixed** — while flying, right-click no longer triggers lock-on, brush drift or a screenshot; `Ctrl`+wheel adjusts the brush radius; brush adjustments take priority.
- **Docs & wiki in 14 languages** — README and CHANGELOG in English, and the full manual on the [Wiki](https://github.com/marceloanton/TerrainForge/wiki) (`Home` + 13 translations).

---

## Description

TerrainForge ships its own **TerrainForge Rod** (a Hammer clone) as the brush tool. It is handed to you automatically when you join a world and everything is explained in an in-game **Help** tab (menu key, default `F5`).

---

## Why client AND server

Terrain edits are stored in the world and the server replicates them to every player. Because the game clamps edits to ±8 m, **TerrainForge must also be installed on the server**; otherwise the server regenerates the clamped terrain while clients see the full edit → desync.

---

## Requirements

- **BepInEx 5** (x64, Mono) on both client and server. Thunderstore users already have it.
- **Jotunn** (`ValheimModding-Jotunn`) — registers the custom tool. It is listed in `manifest.json`; Thunderstore installs it automatically.

---

## Installation

1. Download the latest version from Thunderstore or GitHub Releases.
2. Place it at:
   - **Client:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Server:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Restart the game / server.

> ⚠️ **Must be on both** (client and server) for edits greater than ±8 m and correct replication.

---

## Usage

Equip the **TerrainForge Rod** (given on world join; get it again with `F8`), aim at the ground and hold **right-click** to sculpt.

While aiming, a solid ring marks the real brush area, with faint guides in the operation color (full-strength zone, max radius, max aim distance). For Ramp, the real A–B corridor is drawn while you mark. The HUD (bottom-right) shows operation, radius, shape, paint and undo/redo counters; a second line shows the aimed height, current target and local slope in degrees. Guides can be disabled in `Preview > Show guides`.

---

## Controls

| Action | Default key |
|--------|-------------|
| Apply brush | Hold `Mouse1` (right-click) |
| Radius + | `=` (also `Ctrl` + wheel) |
| Radius − | `-` (also `Ctrl` + wheel) |
| Cycle operation | `,` |
| Cycle shape | `.` |
| Cycle paint | `/` |
| Use aimed height (instead of your feet) for Flatten | Hold `Left Shift` |
| Undo last stroke | `Ctrl+Z` |
| Redo undone stroke | `Ctrl+Y` |
| Fix / release Flatten height (eyedropper) | `G` |
| Cycle ramp mode (two-point / fixed-angle / path) | `H` |
| Adjust fixed-ramp angle (with Ramp selected) | Hold `Alt` + radius keys / wheel |
| Radial operation menu (hold, release to confirm) | `Q` |
| Adjust mode (wheel = radius, zoom frozen) | `V` |
| Fine radius steps (0.1 m) | Hold `Shift` + wheel / radius keys |
| **Copy terrain (Stamp)** | `Ctrl+Shift+C` |
| **Paste terrain (Stamp)** | `Ctrl+Shift+V` |
| Get the TerrainForge Rod | `F8` |
| Open the keys + help menu | `F5` |

**Extra keys (no default, assign them in KEYS):**
- Direct operation: Flatten, Raise, Lower, Smooth, Paint, Restore, Ramp, Shore, Snow
- Next / previous profile
- Next / previous stamp (history)

---

## Operations

Operations cycle: `Flatten`, `Raise`, `Lower`, `Smooth`, `Ramp`, `Paint`, `Restore`, `Shore` (flattens toward the water level + margin, for beaches) and `Snow` (paints snow like Paint but snow only; `Shift` removes it and restores the original ground).

### Details

- **Flatten** — follows the ground **under the crosshair** by default and adapts to what you aim at (e.g. a ramp). Hold `Left Shift` to level at your feet.
- **Raise / Lower** — raise or lower terrain according to the falloff mode and power.
- **Smooth** — averages each point with its neighbours (a real blur), smoothing ridges, steps and noise. Hold to smooth progressively. Modes: `Full` (both ways), `Ridges only` (lowers only), `Dips only` (fills only). Shape Line + Smooth = smooth along a path.
- **Ramp** — three modes (`H`): **two-point** (mark **A** and **B**, joins both heights with a straight slope), **fixed-angle** (**B** gives direction and length, height follows `Ramp angle`, adjustable with `Alt` + radius), and **path** (mark a curve **A→B→C…**, double-press to close). The brush radius sets the ramp width and softness only softens the side edges. Preview: filled semi-transparent corridor + height rings at A and B.
- **Shore** — flattens toward the water level + margin (beaches/coastlines), with softness blending like Flatten.
- **Paint** — types: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset.
- **Restore** — returns the terrain to its original state.
- **Snow** — paints snow like Paint but snow only; `Shift` removes it and restores the original ground.

---

## Shapes

`Circle`, `Square`, `Line`, `Rectangle`, `Polygon` (key `.`).

- `Line` / `Rectangle`: two Apply presses (A and B; line half-width = brush radius, corners define the area with soft edges).
- `Polygon`: adds a vertex per press and closes on a double-press (3+ vertices).
- All area operations apply over these footprints with undo support.

---

## Falloff modes (Raise/Lower)

Cycle in SETTINGS → "Raise/Lower falloff":

| Mode | Effect |
|------|--------|
| **Pointy** (original) | Central peak, controlled by "Falloff power" |
| **Flat** | Uniform disc, same weight across the whole radius |
| **Smooth** | Cosine bell, smooth transition |
| **Linear** | Decreases in a straight line from centre to edge |

---

## In-game languages

**15 options:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

**Auto-detect:** by default `Auto` detects the language configured in Valheim via reflection. If a language has no complete translation, it falls back to English.

> Documentation is available in other languages on the [Wiki](https://github.com/marceloanton/TerrainForge/wiki). See the **Documentation languages** list at the bottom of this page.

---

## Snapshots (Backups)

**SNAPSHOTS** tab in SETTINGS: give it a name to save all edited tiles to `BepInEx/config/TerrainForge/snapshots/<name>.dat`, then load it later (even in another save). Import restores with ownership and is undoable; unloaded tiles warn cleanly.

---

## Stamps (Copy/Paste Terrain)

**STAMPS** tab in SETTINGS.

- **Copy with a rectangle selection:** switch the shape to `Rectangle` (key `.`), press Apply once to mark corner **A** (no edit is made), aim at corner **B**, then press `Ctrl+Shift+C` → copies exactly that rectangle (cropped to the selection, it does not drag whole 64 m tiles or untouched ground around it).
- `Ctrl+Shift+V` shows a ghost (wireframe) preview at the crosshair; `Q`/`E` rotate, `Alt`+wheel rotates continuously, `Shift`+wheel scales, click confirms.
- Pasting writes **only inside the copied region**; it does not erase terrain outside it.
- History of 10 stamps in RAM: "Next stamp" / "Previous stamp" keys cycle them.
- Save a stamp to disk (`.stamp` in `BepInEx/config/TerrainForge/stamps/`, max 12), load it into the clipboard, list and delete from the UI.
- **Stamp tool**: in SETTINGS → "Stamp tool" (4 options): `Same as brush` (default), `Any`, `Pickaxe`, `TerrainForge Rod`. The HUD shows `STAMP READY [2/5]` when the right tool is equipped and the clipboard has stamps; otherwise it warns `Stamp: tool not equipped`. `Ctrl+Shift+C/V` do nothing without the tool equipped.

---

## Preview

- Solid ring: the real generated area.
- Faint guides: full-strength zone, max radius, max aim distance.
- For Ramp: the real A–B corridor (edges + semi-transparent fill) + height rings at A and B.
- For Flatten/Shore: a marker ring for the target level.
- Blocked by server/ward: guides in grey.
- Underwater: the preview floats on the surface.

---

## In-game menu (`F5`)

3 tabs with a **global scroll** (mouse wheel):

| Tab | Purpose |
|-----|---------|
| **KEYS** | Rebind keys: click → press a key/button; `Esc` cancels. Combos: hold modifiers, then the main key. Mouse buttons supported. |
| **HELP** | Complete reference of operations, controls, preview, multiplayer and saving. |
| **SETTINGS** | Everything editable live: sliders, toggles, cycles, profiles, stamps, snapshots, language. Saved on close. |

**ESC button:** the pause menu has a "TerrainForge" button that opens the menu.

---

## Server + Client

- **Required on both** for edits greater than ±8 m and correct replication.
- `Server` section (authoritative via config sync):
  - `AllowTerrainEdits` (default on)
  - `AdminOnlyEdits` (default off)
  - `RespectWards` (default on)
  - `RadiusCap` / `DeltaCap` (0 = no limit)
- A server **without** the mod ignores the section → the client still works for testing.

---

## Performance

`TilesPerFrame` (1–32, default 4): tiles per frame on large strokes. Prevents lag.

---

## Changelog

See [Changelog](Changelog).

---

## Links

- **Thunderstore:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub repository:** https://github.com/marceloanton/TerrainForge
- **Wiki / documentation:** https://github.com/marceloanton/TerrainForge/wiki
- **Donations:** [cafecito.app/byrka](https://cafecito.app/byrka)

### Documentation languages

[English manual](Home) · *English overview* · [Español](https://github.com/marceloanton/TerrainForge/wiki/Home-es) · [Français](https://github.com/marceloanton/TerrainForge/wiki/Home-fr) · [Deutsch](https://github.com/marceloanton/TerrainForge/wiki/Home-de) · [Русский](https://github.com/marceloanton/TerrainForge/wiki/Home-ru) · [中文](https://github.com/marceloanton/TerrainForge/wiki/Home-zh) · [繁體中文](https://github.com/marceloanton/TerrainForge/wiki/Home-zh-tw) · [日本語](https://github.com/marceloanton/TerrainForge/wiki/Home-ja) · [한국어](https://github.com/marceloanton/TerrainForge/wiki/Home-ko) · [Polski](https://github.com/marceloanton/TerrainForge/wiki/Home-pl) · [Português](https://github.com/marceloanton/TerrainForge/wiki/Home-pt) · [Türkçe](https://github.com/marceloanton/TerrainForge/wiki/Home-tr) · [Українська](https://github.com/marceloanton/TerrainForge/wiki/Home-uk) · [Italiano](https://github.com/marceloanton/TerrainForge/wiki/Home-it)

<!--
All 14 wiki pages are live: Home (en), Home-es, Home-fr, Home-de, Home-ru,
Home-zh, Home-zh-tw, Home-ja, Home-ko, Home-pl, Home-pt, Home-tr, Home-uk, Home-it.
When a new translated page is published, add it to the list above and to wiki/_Sidebar.md.
-->
