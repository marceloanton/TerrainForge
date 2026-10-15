# TerrainForge Wiki / Wiki de TerrainForge

---

## 📖 Tabla de Contenidos / Table of Contents

1. [Instalación / Installation](#-instalación--installation)
2. [Inicio Rápido / Quick Start](#-inicio-rápido--quick-start)
3. [Controles / Controls](#-controles--controls)
4. [Operaciones / Operations](#-operaciones--operations)
12. [Formas / Shapes](#-formas--shapes)
13. [Modos de Caída / Falloff Modes](#-modos-de-caída--falloff-modes)
14. [Stamps (Copiar/Pegar) / Stamps (Copy/Paste)](#-stamps-copiarpegar--stamps-copypaste)
15. [Snapshots (Backups) / Snapshots (Backups)](#-snapshots-backups--snapshots-backups)
16. [Perfiles / Profiles](#-perfiles--profiles)
17. [Idiomas / Languages](#-idiomas--languages)
18. [Multijugador / Multiplayer](#-multijugador--multiplayer)
19. [Configuración / Settings](#-configuración--settings)
20. [Solución de Problemas / Troubleshooting](#-solución-de-problemas--troubleshooting)

---

## 📥 Instalación / Installation

### Requisitos / Requirements
- **BepInEx 5** (x64, Mono) en cliente y servidor. Usuarios de Thunderstore ya lo tienen.
- **Jotunn** (`ValheimModding-Jotunn`) — registra la herramienta personalizada. Está en `manifest.json`; Thunderstore lo instala automáticamente.

### Pasos / Steps
1. Descarga `TerrainForge-<version>.zip` desde Thunderstore o GitHub Releases.
2. Extrae y coloca `TerrainForge.dll` en:
   - **Cliente:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Servidor:** `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicia el juego / servidor.

> ⚠️ **Debe instalarse en cliente Y servidor** para edits >±8m y replicación correcta.

---

## 🚀 Inicio Rápido / Quick Start

1. Entra a un mundo. La **Varita TerrainForge** (clon del martillo) se entrega y equipa automáticamente.
2. Presiona `F5` para abrir el menú (o click en botón **TerrainForge** en menú pausa `ESC`).
3. Equipa la varita si no lo está.
4. Apunta al terreno, mantén **clic derecho** (`Mouse1`) para esculpir.
5. Usa `,` para ciclar operaciones, `.` para formas, `/` para pinturas.
3. `Q` (mantener) → menú radial, soltar para confirmar.
4. `Ctrl+Z` / `Ctrl+Y` para deshacer/rehacer.

---

## ⌨️ Controles / Controls

| Acción / Action | Tecla Default / Default Key | Descripción / Description |
|----------------|----------------------------|---------------------------|
| Aplicar brocha / Apply Brush | Hold `Mouse1` (Right Click) | Esculpe terreno bajo la mira |
| Subir radio / Radius + | `=` o `Ctrl`+Rueda | Aumenta radio brocha |
| Bajar radio / Radius − | `-` o `Ctrl`+Rueda | Disminuye radio brocha |
| Cambiar operación / Cycle Op | `,` | Cicla 9 operaciones |
| Cambiar forma / Cycle Shape | `.` | Círculo → Cuadrado → Línea → Rect → Poli |
| Cambiar pintura / Cycle Paint | `/` | Cicla 6 tipos pintura |
| Nivelar a pies / Level to Feet | Hold `Left Shift` | Aplanar usa altura jugador |
| Deshacer / Undo | `Ctrl+Z` | Deshace último trazo |
| Rehacer / Redo | `Ctrl+Y` | Rehace trazo deshecho |
| Cuentagotas / Eyedropper | `G` | Fija altura Aplanar en mira (otra vez = libera) |
| Modo rampa / Ramp Mode | `H` | Cicla: Dos Puntos → Ángulo Fijo → Senda |
| Ángulo rampa fija | Hold `Alt` + radio/rueda | Ajusta ángulo en modo fijo |
| Menú radial / Radial Menu | Hold `Q` | Elige operación con mouse, soltar confirma |
| Modo ajuste / Adjust Mode | `V` | Rueda = radio, congela zoom/barra/mapa |
| Ajuste fino (0.1 m) | Hold `Shift` + rueda/radio | Ajuste preciso radio |
| **Copiar Stamp** | `Ctrl+Shift+C` | Copia área marcada al portapapeles |
| **Pegar Stamp** | `Ctrl+Shift+V` | Preview fantasma, click confirma |
| Obtener Varita / Get Wand | `F8` | Entrega Varita TerrainForge |
| Abrir menú / Open Menu | `F5` | Abre menú teclas/ayuda/ajustes |

**Teclas extra (sin default, asígnalas en TECLAS):**
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa, Nieve
- Siguiente / anterior perfil
- Siguiente / anterior stamp (historial)

---

## 🔄 Operaciones / Operations

| Operación / Operation | Descripción / Description |
|----------------------|---------------------------|
| **Aplanar / Flatten** | Nivela a altura apuntada (Shift = a tus pies). Altura fija via Cuentagotas (`G`) o textbox AJUSTES. |
| **Subir / Raise** | Eleva terreno `RaiseDelta` por aplicación. Modo caída afecta forma. |
| **Bajar / Lower** | Baja terreno `RaiseDelta` por aplicación. Modo caída afecta forma. |
| **Suavizar / Smooth** | Blur real: promedia cada vértice con sus 8 vecinos. Mantén para progresivo. **Modos:** Completo (ambos), Solo Lomos (baja), Solo Hoyos (sube). Forma Línea = suaviza camino. |
| **Rampa / Ramp** | **3 Modos** (cicla `H`):<br>• **Dos Puntos**: Marca A y B → pendiente recta une alturas.<br>• **Ángulo Fijo**: B = dirección/largo; altura = `SlopeAngle` (sube/baja según mira). Ajusta con `Alt`+radio.<br>• **Senda**: Marca curva A→B→C… (doble-pulso 3+ cierra) → Catmull-Rom suave.<br>Ancho = radio brocha. Suavidad = blend solo bordes laterales. Preview: corredor relleno + anillos altura A/B. |
| **Costa / Shore** | Aplana hacia nivel agua + `ShoreOffset` margen (playas). Suavidad como Aplanar. |
| **Pintar / Paint** | Tipos: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Fuerza ajustable. |
| **Restaurar / Restore** | Devuelve terreno a estado original (altura + pintura). |
| **Nieve / Snow** | Pintura solo nieve (misma máscara juego). `Shift` = borra y restaura original. |

---

## 📐 Formas / Shapes

Cicla con `.` key. Todas soportan undo.

| Forma / Shape | Marcado / Marking | Caso Uso / Use Case |
|--------------|-------------------|------------------------|
| **Círculo / Circle** | Click simple (centro brocha) | Esculpir general |
| **Cuadrado / Square** | Click simple (centro) | Plataformas, cimientos |
| **Línea / Line** | 2 clicks (A → B) | Caminos, muros, senderos. Semiancho = radio. |
| **Rectángulo / Rect** | 2 clicks (esquinas opuestas) | Cimientos, granjas, plataformas. Bordes suaves. |
| **Polígono / Poly** | Click/vértice, doble-click cierra (3+ vértices) | Formas complejas, áreas custom |

- Formas no circulares muestran **solo contorno marcado** (sin anillos circulares).
- Suavidad afecta blend bordes en todas formas.

---

## ⚙️ Modos de Caída / Falloff Modes (Subir/Bajar)

Config en AJUSTES → "Caída Raise/Lower":

| Modo / Mode | Comportamiento / Behavior |
|-------------|---------------------------|
| **Punta / Pointy** | Pico central. Controlado por "Potencia caída" slider (1–5). |
| **Plano / Flat** | Disco uniforme — mismo peso en todo radio. |
| **Suave / Smooth** | Campana coseno — transición suave, sin borde duro. |
| **Lineal / Linear** | Taper recto centro→borde. |

---

## 🧱 Stamps (Copiar/Pegar Terreno)

Feature potente para replicar estructuras de terreno.

### Flujo / Workflow

1. **Marca Área**: Usa cualquier forma (Círculo, Rect, Línea, Poli, Rampa).
2. **Copiar**: `Ctrl+Shift+C` → "Stamp copiado: X tiles" (RAM clipboard).
3. **Preview Pegar**: `Ctrl+Shift+V` → wireframe fantasma en mira.
   - `Q`/`E` = rotar ±15°
   - `Alt`+Rueda = rotar continuo
   - `Shift`+Rueda = escalar 0.25×–4×
   - `Esc` = cancelar
4. **Confirmar**: Click (`Mouse1`) → aplica como **un solo paso Undo**.

### Historial / History
- Hasta **10 stamps** en RAM (más reciente primero).
- Teclas `Siguiente stamp` / `Stamp anterior` (reasignables) para ciclar.
- Flash: "Stamp historial: Copy: 3 tiles [2/5]"

### Persistencia / Persistence
- **AJUSTES → STAMPS**:
  - Guardar stamp actual a disco (`.stamp` en `BepInEx/config/TerrainForge/stamps/`, máx 12).
  - Cargar stamp a portapapeles.
  - Listar/borrar stamps guardados.

### Herramienta Stamp / Stamp Tool
- AJUSTES → "Herramienta stamp" (4 opciones):
  - `Igual a brocha` (default) — usa misma tool que brocha
  - `Cualquiera` — sin requisito
  - `Pico` — requiere pico equipado
  - `Varita TerrainForge` — requiere varita del mod
- HUD muestra `STAMP LISTO [2/5]` si tool equipada + stamps en clipboard.
- Sin tool: `Stamp: herramienta no equipada`, copy/paste bloqueados.

---

## 💾 Snapshots (Backups)

Backup/restore completo de terreno.

### Guardar / Save
1. AJUSTES → **SNAPSHOTS**.
2. Nombre (vacío = auto fecha/hora).
3. **Guardar** → tiles editados en `BepInEx/config/TerrainForge/snapshots/<nombre>.dat` (máx 12).

### Cargar / Load
1. Selecciona snapshot.
2. **Cargar** → restaura tiles con ownership, **1 paso undo**.
3. Tiles no cargados avisan limpiamente — importa cerca del original.

### Características
- Formato binario versionado (`.dat`).
- Restaura altura + smooth + paint + ownership.
- Import es undoable (`Ctrl+Z` revierte import).
- Tiles no cargados avisan limpiamente.

---

## 💾 Perfiles / Profiles

Guarda/carga configuraciones completas de brocha.

- AJUSTES → **PERFILES**.
- Nombre + **Guardar** → radio, suavidad, op, forma, pintura, modo rampa, ángulo, smooth mode, raise delta, paint strength, falloff, shore offset.
- **Cargar** → aplica instantáneo.
- Máx 12 perfiles (formato culture-invariant en `Presets > Slots`).
- Keybind: `Next Profile` (asigna en TECLAS) para ciclar.

---

## 🌐 Idiomas / Languages

**15 opciones:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

### Auto-Detect
- Default `Auto` detecta idioma juego vía reflection: `Localization.instance.GetSelectedLanguage()`.
- Mapea idioma Valheim → índice interno.
- Si idioma sin traducción completa → fallback a English.
- Configurable en AJUSTES → "Idioma" (cicla 15 opciones).
- Etiquetas teclas se reconstruyen al abrir menú.

### Estado Traducciones
- **Completas:** English, Español
- **Fallback English:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Traducciones comunitarias bienvenidas via PR.

---

## 🌍 Multijugador / Multiplayer

### Requisitos
- **Mod en cliente Y servidor** para edits >±8m y replicación correcta.
- `UnlockDeltaLimit` en servidor también, si no servidor clampa terreno.

### Autoridad Servidor (Config Sync)
Cuando servidor corre mod, su sección `Servidor` gana:
- `PermitirEditsTerreno` (default `true`) — master switch; si `false`, clientes no aplican/undo/redo.
- `SoloAdmins` (default `false`) — solo admins/host editan.
- `RespetarWards` (default `true`) — bloquea edits en wards ajenos.
- `TopeRadio` / `TopeDelta` (0 = off) — caps server-enforced, visibles HUD+preview.

> Servidores **sin** mod ignoran sección → cliente funciona para test.

### Prevención Desync
- `MaxUpDelta` / `MaxDownDelta` consistentes server+clientes.
- Radios grandes pueden desincronizar clientes sin mod.

---

## ⚙️ Configuración / Settings

Archivo: `BepInEx/config/com.terrainforge.valheim.cfg`

### General
| Setting | Default | Descripción |
|---------|---------|-------------|
| `Enabled` | `true` | Master switch |
| `PlayEffects` | `true` | Polvo + golpe al aplicar |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |

### Brocha / Brush
| Setting | Default | Rango | Descripción |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Radio brocha (m) |
| `RadiusMin` | 2 | — | Radio mín ajustando |
| `RadiusMax` | 60 | — | Radio máx ajustando |
| `RadiusStep` | 1 | — | Cambio radio por tecla/rueda |
| `Softness` | 50 | 0–100 | Suavidad borde (0=duro, 100=muy suave) |
| `FalloffPower` | 3 | 1–5 | Exponente caída (solo modo Punta) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Metros por aplicación Subir/Bajar |
| `PaintStrength` | 1 | 0–1 | Fuerza pintura |
| `DefaultOperation` | Flatten | — | Operación inicio |
| `Shape` | Circle | — | Forma inicio |
| `Paint` | Dirt | — | Pintura inicio |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Ángulo rampa fija (grados) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Margen costa sobre agua (m) |
| `UseFixedHeight` | false | — | Usar altura fija (estado cuentagotas) |
| `FixedHeight` | 30 | -10–150 | Altura exacta Aplanar (m) |

### Entrada / Input
Todos atajos reasignables en pestaña TECLAS. Destacados:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Teclas directas op** (9, sin default): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (default)
- `AutoGiveTool`: true (entrega varita al entrar)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (default Same as Brush)

### Preview
- `ShowPreview`: true, `ShowGuides`: true (guías alcance, tinte pendiente, anillo nivel)

### Historial
- `UndoDepth`: 50 (0–500, 0 desactiva historial)

### Servidor (Autoritario via Config Sync)
- `AllowTerrainEdits`: true (master switch)
- `AdminOnlyEdits`: false (solo admins)
- `RespectWards`: true (bloquea wards ajenos)
- `RadiusCap` / `DeltaCap`: 0 (off) — caps server-enforced

### Rendimiento
- `ChunkCompsPerFrame`: 4 (1–32 tiles/frame)

---

## 🐛 Solución de Problemas / Troubleshooting

| Problema / Issue | Solución / Solution |
|------------------|---------------------|
| **Menú no aparece / cursor invisible** | Cierra juego. Borra `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\TerrainForge\` y `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Reinicia TMM, check updates. |
| **Update no aparece en TMM** | TMM → Settings → "Refresh mod list" (Ctrl+R). Cierra/reabre TMM. Verifica perfil correcto. |
| **Conflicto versión Thunderstore** | Bump version (0.3.3 → 0.3.4) en `.csproj`, `Plugin.cs`, `manifest.json`. Rebuild + re-upload. |
| **Stamp copy/paste no hace nada** | HUD: `STAMP LISTO [X/Y]` = listo. Si "herramienta no equipada", equipa tool (AJUSTES → Herramienta stamp). |
| **Preview invisible bajo agua** | Fix en 0.3.2 — preview flota en superficie. Actualiza. |
| **Círculos en Line/Rect/Poly** | Fix en 0.3.2 — solo contorno marcado. Actualiza. |
| **Rueda radial no funciona** | Fix en 0.3.2 — usa `ZInput.GetMouseScrollWheel()` como vanilla. Actualiza. |
| **Config no guarda** | Cierra menú (F5) para auto-guardar. O edita `BepInEx/config/com.terrainforge.valheim.cfg` directo. |
| **Desync multijugador** | Mod en cliente+servidor. `UnlockDeltaLimit` en ambos. Mismos `MaxUpDelta`/`MaxDownDelta`. |
| **Build falla** | Verifica paths `ValheimDir`, `BepInExCoreDir`, `JotunnDir` en `.csproj` o CLI. .NET SDK 8+ requerido. |

---

## 🔗 Enlaces / Links

- **Página Thunderstore:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **Repositorio GitHub:** https://github.com/marceloanton/TerrainForge
- **Donaciones / Donations:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [CHANGELOG.md](CHANGELOG.md)
- **Código Fuente:** https://github.com/marceloanton/TerrainForge

---

*TerrainForge — Terraforma Valheim sin límites.*