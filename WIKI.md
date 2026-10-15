# TerrainForge Wiki / Wiki de TerrainForge

---

## 📖 Table of Contents / Tabla de Contenidos

1. [Installation / Instalación](#-installation--instalación)
2. [Quick Start / Inicio Rápido](#-quick-start--inicio-rápido)
3. [Controls Reference / Referencia de Controles](#-controls-reference--referencia-de-controles)
4. [Operations Guide / Guía de Operaciones](#-operations-guide--guía-de-operaciones)
5. [Shapes / Formas](#-shapes--formas)
6. [Falloff Modes / Modos de Caída](#-falloff-modes--modos-de-caída)
7. [Stamps (Copy/Paste) / Stamps (Copiar/Pegar)](#-stamps-copypaste--stamps-copiarpegar)
8. [Snapshots (Backups) / Snapshots (Backups)](#-snapshots-backups--snapshots-backups)
9. [Profiles / Perfiles](#-profiles--perfiles)
10. [Languages / Idiomas](#-languages--idiomas)
11. [Multiplayer / Multijugador](#-multiplayer--multijugador)
12. [Settings Reference / Referencia de Configuración](#-settings-reference--referencia-de-configuración)
13. [Building from Source / Compilar desde Código](#-building-from-source--compilar-desde-código)
14. [Troubleshooting / Solución de Problemas](#-troubleshooting--solución-de-problemas)

---

## 📥 Installation / Instalación

### Requirements / Requisitos
- **BepInEx 5** (x64, Mono) on both client and server. Thunderstore Mod Manager users already have it.
- **Jotunn** (`ValheimModding-Jotunn`) — registers the custom tool item. Listed in `manifest.json`; Thunderstore installs it automatically.

### Steps / Pasos
1. Download `TerrainForge-<version>.zip` from Thunderstore or GitHub Releases.
2. Extract and place `TerrainForge.dll` in:
   - **Client:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Server:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Restart game / server.

> ⚠️ **Must be installed on both client AND server** for edits beyond ±8 m and proper replication.

---

## 🚀 Quick Start / Inicio Rápido

1. Enter a world. The **TerrainForge Rod** (hammer clone) is automatically given and equipped.
2. Press `F5` to open the menu (or click **TerrainForge** button in ESC pause menu).
3. Equip the rod if not already equipped.
4. Aim at terrain, hold **Right Click** (`Mouse1`) to sculpt.
5. Use `,` to cycle operations, `.` to cycle shapes, `/` to cycle paint types.
6. Press `Q` (hold) for radial operation menu, release to confirm.
7. `Ctrl+Z` / `Ctrl+Y` for undo/redo.

---

## ⌨️ Controls Reference / Referencia de Controles

| Action / Acción | Default Key / Tecla Default | Description / Descripción |
|----------------|----------------------------|---------------------------|
| **Apply Brush** / Aplicar Brocha | Hold `Mouse1` (Right Click) | Sculpt terrain under crosshair |
| **Radius +** / Subir Radio | `=` or `Ctrl`+Wheel | Increase brush radius |
| **Radius −** / Bajar Radio | `-` or `Ctrl`+Wheel | Decrease brush radius |
| **Cycle Operation** / Cambiar Operación | `,` | Cycle through 9 operations |
| **Cycle Shape** / Cambiar Forma | `.` | Circle → Square → Line → Rect → Poly |
| **Cycle Paint** / Cambiar Pintura | `/` | Cycle 6 paint types |
| **Level to Feet** / Nivelar a Pies | Hold `Left Shift` | Flatten uses player height instead of aimed |
| **Undo** / Deshacer | `Ctrl+Z` | Undo last brush stroke |
| **Redo** / Rehacer | `Ctrl+Y` | Redo undone stroke |
| **Eyedropper** / Cuentagotas | `G` | Fix Flatten height at aim point (press again to release) |
| **Ramp Mode** / Modo Rampa | `H` | Cycle: Two-Point → Fixed-Angle → Path |
| **Fixed Ramp Angle** | Hold `Alt` + Radius keys/Wheel | Adjust slope angle in Fixed-Angle mode |
| **Radial Menu** / Menú Radial | Hold `Q` | Pick operation with mouse, release to confirm |
| **Adjust Mode** / Modo Ajuste | `V` | Wheel = radius, freezes camera zoom/hotbar/map |
| **Fine Adjust (0.1m)** | Hold `Shift` + Wheel/Radius Keys | Precise radius adjustment |
| **Copy Stamp** | `Ctrl+Shift+C` | Copy marked area to clipboard (RAM) |
| **Paste Stamp** | `Ctrl+Shift+V` | Show ghost preview, click to confirm |
| **Get Wand** / Obtener Varita | `F8` | Give yourself the TerrainForge Rod |
| **Open Menu** / Abrir Menú | `F5` | Open keybinds/help/settings menu |

**Extra Keys (no default, assign in KEYS tab):**
- Direct Operation keys: Flatten, Raise, Lower, Smooth, Paint, Restore, Ramp, Shore, Snow
- Next/Previous Profile
- Next/Previous Stamp (history)

---

## 🔄 Operations Guide / Guía de Operaciones

| Operation / Operación | Description / Descripción |
|----------------------|---------------------------|
| **Flatten / Aplanar** | Levels terrain to aimed height (hold `Shift` = level to your feet). Fixed height via Eyedropper (`G`) or SETTINGS textbox overrides both. |
| **Raise / Subir** | Raises terrain by `RaiseDelta` per application. Falloff mode affects shape. |
| **Lower / Bajar** | Lowers terrain by `RaiseDelta` per application. Falloff mode affects shape. |
| **Smooth / Suavizar** | Real blur: averages each vertex with its 8 neighbours. Hold to smooth progressively. **Modes:** Full (both), Bumps Only (lower only), Dips Only (raise only). Line shape = smooth along path. |
| **Ramp / Rampa** | **3 Modes** (cycle with `H`):<br>• **Two-Point**: Mark A then B → straight slope joining both heights.<br>• **Fixed-Angle**: B sets direction/length; height follows `SlopeAngle` (up/down by aim). Adjust angle with `Alt`+radius.<br>• **Path**: Mark curve A→B→C… (double-press with 3+ closes) → Catmull-Rom smoothed ramp along curve.<br>Width = brush radius. Softness = side edge blend only. Preview shows filled corridor + height rings at A/B. |
| **Shore / Costa** | Flattens toward water level + `ShoreOffset` margin (beaches). Softness blends like Flatten. |
| **Paint / Pintar** | Types: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Strength adjustable. |
| **Restore / Restaurar** | Returns terrain to original state (height + paint). |
| **Snow / Nieve** | Snow-only paint (same mask as game snow). Hold `Shift` to erase and restore original ground. |

---

## 📐 Shapes / Formas

Cycle with `.` key. All shapes support undo.

| Shape / Forma | Marking / Marcado | Use Case / Caso de Uso |
|--------------|-------------------|------------------------|
| **Circle / Círculo** | Single click (brush center) | General sculpting |
| **Square / Cuadrado** | Single click (center) | Flat platforms, foundations |
| **Line / Línea** | 2 clicks (A → B) | Paths, roads, walls. Half-width = radius. |
| **Rect / Rectángulo** | 2 clicks (opposite corners) | Foundations, farms, platforms. Soft edges. |
| **Poly / Polígono** | Click per vertex, double-click to close (3+ vertices) | Complex shapes, custom areas |

- Non-circular shapes show **only the marked outline** (no circular rings).
- Softness affects edge blend on all shapes.

---

## ⚙️ Falloff Modes / Modos de Caída (Raise/Lower)

Set in SETTINGS → "Caída Raise/Lower":

| Mode / Modo | Behavior / Comportamiento |
|-------------|---------------------------|
| **Pointy / Punta** | Sharp center peak. Controlled by "Falloff Power" slider (1–5). |
| **Flat / Plano** | Uniform disk — same weight across entire radius. |
| **Smooth / Suave** | Cosine bell curve — smooth transition, no hard edge. |
| **Linear / Lineal** | Linear taper from center to edge. |

---

## 🧱 Stamps (Copy/Paste Terrain) / Stamps (Copiar/Pegar Terreno)

Powerful feature to replicate terrain structures.

### Workflow / Flujo

1. **Mark Area**: Use any shape (Circle, Rect, Line, Poly, Ramp).
2. **Copy**: `Ctrl+Shift+C` → "Stamp copied: X tiles" (saved to RAM clipboard).
3. **Paste Preview**: `Ctrl+Shift+V` → ghost wireframe at aim.
   - `Q`/`E` = rotate ±15°
   - `Alt`+Wheel = continuous rotate
   - `Shift`+Wheel = scale 0.25×–4×
   - `Esc` = cancel
4. **Confirm**: Click (`Mouse1`) → applies as **single undo step**.

### History / Historial
- Up to **10 stamps** in RAM (newest first).
- Keys: `Next Stamp` / `Previous Stamp` (assign in KEYS) to cycle.
- Flash shows: "Stamp history: Copy: 3 tiles [2/5]"

### Persistence / Persistencia
- **SETTINGS → STAMPS** tab:
  - Save current stamp to disk (`.stamp` in `BepInEx/config/TerrainForge/stamps/`, max 12).
  - Load stamp from disk → puts in clipboard as active stamp.
  - List/delete saved stamps.

### Stamp Tool Requirement / Herramienta Requerida
- SETTINGS → "Herramienta stamp" (4 options):
  - `Igual a brocha` (default) — uses same tool as brush
  - `Cualquiera` — no requirement
  - `Pico` — requires pickaxe equipped
  - `Varita TerrainForge` — requires mod's rod equipped
- HUD shows `STAMP LISTO [2/5]` when tool equipped + stamps in clipboard.
- Without correct tool: `Stamp: herramienta no equipada`, copy/paste disabled.

---

## 💾 Snapshots (Terrain Backups)

Full terrain backup/restore system.

### Save / Guardar
1. SETTINGS → **SNAPSHOTS** tab.
2. Enter name (empty = auto datetime).
3. Click **Guardar** → saves all edited tiles to `BepInEx/config/TerrainForge/snapshots/<name>.dat` (max 12 files).

### Load / Cargar
1. Select snapshot from list.
2. Click **Cargar** → restores tiles with ownership, **single undo step**.
3. Missing tiles (not loaded in memory) reported cleanly — import near original location.

### Features
- Versioned binary format (`.dat`).
- Restores height + smooth + paint + ownership.
- Import is itself undoable (`Ctrl+Z` reverts the import).
- Tiles not currently loaded report cleanly.

---

## 💾 Profiles / Perfiles

Save/load complete brush configurations.

- SETTINGS → **PERFILES** tab.
- Name + **Guardar** → saves radius, softness, operation, shape, paint, ramp mode, angle, smooth mode, raise delta, paint strength, falloff, shore offset.
- Click **Cargar** on any profile → instant apply.
- Max 12 profiles (culture-invariant format in `Presets > Slots`).
- Keybind: `Next Profile` (assign in KEYS) to cycle.

---

## 🌐 Languages / Idiomas

**15 options:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

### Auto-Detect
- Default `Auto` detects game language via reflection: `Localization.instance.GetSelectedLanguage()`.
- Maps Valheim language → internal index.
- If language has incomplete translations → falls back to English.
- Configurable in SETTINGS → "Idioma" (cycles through all 15).
- Key labels rebuild on menu open.

### Translation Status
- **Complete:** English, Español
- **Fallback to English:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Community translations welcome via PR!

---

## 🌍 Multiplayer / Multijugador

### Requirements
- **Mod must be on both client AND server** for edits >±8 m and correct replication.
- `UnlockDeltaLimit` must be enabled on server too, otherwise server clamps terrain.

### Server Authority (Config Sync)
When server runs the mod, its `Server` section wins:
- `AllowTerrainEdits` (default `true`) — master switch; if `false`, clients cannot apply/undo/redo.
- `AdminOnlyEdits` (default `false`) — only admins/host may edit.
- `RespectWards` (default `true`) — blocks edits inside foreign wards.
- `RadiusCap` / `DeltaCap` (0 = off) — server-enforced max radius/delta, visible in HUD+preview.

> Servers **without** the mod ignore this section → client tool works for testing.

### Desync Prevention
- Keep `MaxUpDelta` / `MaxDownDelta` consistent between server and clients.
- Large radii may desync clients without mod.

---

## ⚙️ Settings Reference / Referencia de Configuración

File: `BepInEx/config/com.terrainforge.valheim.cfg`

### General
| Setting | Default | Description |
|---------|---------|-------------|
| `Enabled` | `true` | Master switch |
| `PlayEffects` | `true` | Dust + thud on apply |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |

### Brush / Brocha
| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Brush radius (meters) |
| `RadiusMin` | 2 | — | Min radius when adjusting |
| `RadiusMax` | 60 | — | Max radius when adjusting |
| `RadiusStep` | 1 | — | Radius change per key/scroll |
| `Softness` | 50 | 0–100 | Edge softness (0=hard, 100=very soft) |
| `FalloffPower` | 3 | 1–5 | Falloff exponent (Pointy mode only) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Meters per Raise/Lower application |
| `PaintStrength` | 1 | 0–1 | Paint application strength |
| `DefaultOperation` | Flatten | — | Startup operation |
| `Shape` | Circle | — | Startup shape |
| `Paint` | Dirt | — | Startup paint type |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Fixed-angle ramp slope (degrees) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Shore height above water (meters) |
| `UseFixedHeight` | false | — | Use fixed height (eyedropper state) |
| `FixedHeight` | 30 | -10–150 | Exact Flatten target (meters) |

### Input / Entrada
All hotkeys rebindable in KEYS tab. Notable:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Direct Op Keys** (9, no default): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (default)
- `AutoGiveTool`: true (give rod on world enter)

### Limits / Límites
- `UnlockDeltaLimit`: true (bypass vanilla ±8m clamp; must be on server too)
- `MaxUpDelta` / `MaxDownDelta`: 64 (max height above/below natural terrain)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (default Same as Brush)

### Preview
- `ShowPreview`: true, `ShowGuides`: true (reach guides, slope tint, level ring)

### History
- `UndoDepth`: 50 (0–500, 0 disables history)

### Server (Authoritative via Config Sync)
- `AllowTerrainEdits`: true (master switch)
- `AdminOnlyEdits`: false (admins only)
- `RespectWards`: true (block foreign wards)
- `RadiusCap` / `DeltaCap`: 0 (off) — server-enforced caps

### Performance
- `ChunkCompsPerFrame`: 4 (1–32 tiles/frame)

---

## 🛠️ Building from Source / Compilar desde Código

Requires .NET SDK 8+ and reference DLLs (Valheim `Managed` + BepInEx `core`).

```bash
dotnet build -c Release \
  -p:ValheimDir="D:\Steam\steamapps\common\Valheim" \
  -p:BepInExCoreDir="%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\core" \
  -p:JotunnDir="%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\ValheimModding-Jotunn" \
  -p:Deploy=true
```

- `ValheimDir`: Valheim root (contains `valheim_Data/Managed`).
- `BepInExCoreDir`: BepInEx `core` folder of your profile.
- `JotunnDir`: Jotunn plugin folder in that profile.
- `Deploy=true`: copies DLL to profile's `plugins/TerrainForge/`.

---

## 🐛 Troubleshooting / Solución de Problemas

| Issue / Problema | Solution / Solución |
|------------------|---------------------|
| **Menu doesn't appear / cursor missing** | Close game completely. Delete `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\TerrainForge\` and `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Restart TMM, check for updates. |
| **Update not showing in TMM** | TMM → Settings → "Refresh mod list" (or Ctrl+R). Close/restart TMM. Verify correct profile selected. |
| **Version conflict on Thunderstore** | Bump version (0.3.2 → 0.3.3) in `.csproj`, `Plugin.cs`, `manifest.json`. Rebuild and re-upload. |
| **Stamp copy/paste does nothing** | Check HUD: `STAMP LISTO [X/Y]` means ready. If shows "herramienta no equipada", equip required tool (SETTINGS → Herramienta stamp). |
| **Preview invisible underwater** | Fixed in 0.3.2 — preview now floats at water surface. Update to latest. |
| **Circles showing on Line/Rect/Poly** | Fixed in 0.3.2 — only marked outline shows. Update to latest. |
| **Radial menu wheel not working** | Fixed in 0.3.2 — uses `ZInput.GetMouseScrollWheel()` like vanilla. Update to latest. |
| **Config not saving** | Close menu (F5) to auto-save. Or edit `BepInEx/config/com.terrainforge.valheim.cfg` directly. |
| **Desync in multiplayer** | Ensure mod on both client+server. `UnlockDeltaLimit` enabled on both. Match `MaxUpDelta`/`MaxDownDelta`. |
| **Build fails** | Verify `ValheimDir`, `BepInExCoreDir`, `JotunnDir` paths in `.csproj` or command line. Ensure .NET SDK 8+ installed. |

---

## 🔗 Links / Enlaces

- **Thunderstore Page:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub Repository:** https://github.com/marceloanton/TerrainForge
- **Donations / Donaciones:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [CHANGELOG.md](CHANGELOG.md)
- **Source Code:** https://github.com/marceloanton/TerrainForge

---

*TerrainForge — Terraform Valheim without limits. / Terraforma Valheim sin límites.*