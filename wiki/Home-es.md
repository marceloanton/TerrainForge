# Wiki de TerrainForge

[Overview](Overview) · [English](Home) · *Español* · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it) · [Sitio web](https://marceloanton.github.io/TerrainForge/)

---


### Novedades de TerrainForge 0.3.8 — Hotfix de 0.3.7

- **Registro de la varita:** evita repetir el registro de un prefab de red con la misma clave y limpia entradas inválidas de ObjectDB. Los logs de 0.3.7 mostraron esos fallos, aunque también informaron que la herramienta se registró; por eso aún hay que confirmar el funcionamiento dentro del juego.
- **Estado de la herramienta:** la edición del terreno y su HUD se detienen al guardar o desequipar la herramienta, incluso con la cámara libre activa.
- **Antes de publicar:** confirmar en Valheim que se entrega la varita, funciona la edición y no se repiten los avisos de registro.

### Funciones incluidas desde 0.3.6

- **Menú F5:** pestañas organizadas en dos filas, tamaño adaptable a la pantalla y controles contextuales según la operación.
- **Rampa:** panel propio con Dos Puntos, Ángulo Fijo y Rampa Conectada, además de ancho, suavidad y ángulo donde corresponde.
- **Copiar/Pegar:** modo exclusivo en el wheel Q. Permite seleccionar con Rectángulo, Línea, Polígono, Círculo o Cuadrado y luego previsualizar y colocar el stamp. El HUD indica el paso siguiente.
- **Pintura y cámara:** la nieve está en Pintar > DeepSnow; la cámara libre se mantiene sobre el terreno al descender.
- **Dependencias:** TerrainForge registra su propia varita; ya no requiere Jotunn.

### Roadmap de la 0.4.0

- Estamos explorando rampas circulares en dos variantes: un anillo o terraza a nivel, y una espiral conectada que sube o baja alrededor de un centro.
- También están previstos un fantasma de colocación de blueprints con ajuste a grilla, una vista de contornos mejorada para stamps y un HUD de medición de distancia, área y pendiente. Los detalles pueden cambiar después de prototipar.

## Capturas dentro del juego

La portada siguiente es el arte de actualización 0.3.8. Las capturas muestran el menú F5, la vista previa de una rampa, una rampa terminada y una selección de Restaurar.

| TerrainForge 0.3.8 | Menú de herramientas F5 |
|---|---|
| ![Arte de actualización TerrainForge 0.3.8](https://marceloanton.github.io/TerrainForge/terrainforge-update-0.3.8.png) | ![Menú F5 de TerrainForge en Valheim](https://marceloanton.github.io/TerrainForge/images/terrainforge-menu-f5.jpg) |

| Vista previa de rampa | Rampa terminada |
|---|---|
| ![Vista previa de rampa TerrainForge](https://marceloanton.github.io/TerrainForge/images/terrainforge-ramp-preview.jpg) | ![Rampa TerrainForge construida en Valheim](https://marceloanton.github.io/TerrainForge/images/terrainforge-ramp-built.jpg) |

| Selección para Restaurar |
|---|
| ![Vista previa de selección para Restaurar](https://marceloanton.github.io/TerrainForge/images/terrainforge-restore-preview.jpg) |

## Índice

1. [Instalación](#-instalación)
2. [Inicio Rápido](#-inicio-rápido)
3. [Controles](#-controles)
4. [Operaciones](#-operaciones)
5. [Formas](#-formas)
6. [Modos de Caída](#-modos-de-caída)
7. [Stamps (Copiar/Pegar)](#-stamps-copiarpegar)
8. [Snapshots (Backups)](#-snapshots-backups)
9. [Perfiles](#-perfiles)
10. [Idiomas](#-idiomas)
11. [Multijugador](#-multijugador)
12. [Configuración](#-configuración)
13. [Solución de Problemas](#-solución-de-problemas)

---

## 📥 Instalación

### Requisitos
- **BepInEx 5** (x64, Mono) en cliente y servidor. Los usuarios de Thunderstore ya lo tienen. TerrainForge registra su propia herramienta; no requiere Jotunn.

### Pasos
1. Descarga `TerrainForge-<version>.zip` desde Thunderstore o GitHub Releases.
2. Extrae y coloca `TerrainForge.dll` en:
   - **Cliente:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Servidor:** `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicia el juego / servidor.

> ⚠️ **Debe instalarse en cliente Y servidor** para edits mayores a ±8 m y una replicación correcta.

---

## 🚀 Inicio Rápido

1. Entra a un mundo. La **Varita TerrainForge** (clon del martillo) se entrega y equipa automáticamente.
2. Pulsa `F5` para abrir el menú (o haz clic en el botón **TerrainForge** del menú de pausa `ESC`).
3. Equipa la varita si no lo está.
4. Apunta al terreno y mantén **clic derecho** (`Mouse1`) para esculpir.
5. Usa `,` para ciclar operaciones, `.` para formas, `/` para pinturas.
6. Mantén `Q` → menú radial, suelta para confirmar.
7. `Ctrl+Z` / `Ctrl+Y` para deshacer/rehacer.

---

## ⌨️ Controles

| Acción | Tecla por defecto | Descripción |
|--------|-------------------|-------------|
| Aplicar brocha | Mantener `Mouse1` (clic derecho) | Esculpe el terreno bajo la mira |
| Subir radio | `=` o `Ctrl`+Rueda | Aumenta el radio de la brocha |
| Bajar radio | `-` o `Ctrl`+Rueda | Disminuye el radio de la brocha |
| Cambiar operación | `,` | Cicla las 9 operaciones |
| Cambiar forma | `.` | Círculo → Cuadrado → Línea → Rect → Poli |
| Cambiar pintura | `/` | Cicla los 6 tipos de pintura |
| Nivelar a pies | Mantener `Shift` izq. | Aplanar usa la altura del jugador |
| Deshacer | `Ctrl+Z` | Deshace el último trazo |
| Rehacer | `Ctrl+Y` | Rehace el trazo deshecho |
| Cuentagotas | `G` | Fija la altura de Aplanar en la mira (otra vez = libera) |
| Modo de rampa | `H` | Cicla: Dos Puntos → Ángulo Fijo → Conectada (o elige en la pestaña Ramp de F5) |
| Ángulo de rampa fija | Mantener `Alt` + radio/rueda | Ajusta el ángulo en modo fijo |
| Menú radial | Mantener `Q` | Elige operación con el mouse, soltar confirma |
| Modo ajuste | `V` | Rueda = radio, congela zoom/barra/mapa |
| Ajuste fino (0.1 m) | Mantener `Shift` + rueda/radio | Ajuste preciso del radio |
| **Copiar Stamp** | `Ctrl+Shift+C` | Copia el área marcada al portapapeles |
| **Pegar Stamp** | `Ctrl+Shift+V` | Preview fantasma, clic confirma |
| Obtener Varita | `F8` | Entrega la Varita TerrainForge |
| Abrir menú | `F5` | Abre el menú de teclas/ayuda/ajustes |

**Teclas extra (sin default, asígnalas en TECLAS):**
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa. La tecla heredada de Nieve selecciona Pintar + DeepSnow.
- Siguiente / anterior perfil
- Siguiente / anterior stamp (historial)

**Sección "Acciones rápidas" (pestaña TECLAS):** botones para funciones sin atajo obligatorio — Deshacer, Rehacer, Fijar altura, Copiar/Pegar stamp, Stamp anterior, Limpiar marcas, los mapas de calor y el Diagnóstico de material (vuelca el shader + ranuras de textura al log).

---

## 🔄 Operaciones

| Operación | Descripción |
|-----------|-------------|
| **Aplanar** | Nivela a la altura apuntada (Shift = a tus pies). Altura fija vía Cuentagotas (`G`) o el textbox de AJUSTES. |
| **Subir** | Eleva el terreno `RaiseDelta` por aplicación. El modo de caída le da forma. |
| **Bajar** | Baja el terreno `RaiseDelta` por aplicación. El modo de caída le da forma. |
| **Suavizar** | Blur real: promedia cada vértice con sus 8 vecinos. Mantén para progresivo. **Modos:** Completo (ambos), Solo Lomos (baja), Solo Hoyos (sube). Forma Línea = suaviza un camino. |
| **Rampa** | **3 modos** (cicla `H`):<br>• **Dos Puntos**: marca A y B → una pendiente recta une las alturas.<br>• **Ángulo Fijo**: B = dirección/largo; altura = `SlopeAngle` (sube/baja según la mira). Ajusta con `Alt`+radio.<br>• **Conectada**: marca 3+ puntos A→B→C… para crear una rampa continua y suave.<br>Ancho = radio de la brocha. Suavidad = blend solo en los bordes laterales. Preview: corredor relleno + anillos de altura en A/B. |
| **Costa** | Aplana hacia el nivel del agua + margen `ShoreOffset` (playas). Suavidad como Aplanar. |
| **Pintar** | Tipos: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Fuerza ajustable. |
| **Restaurar** | Devuelve el terreno a su estado original (altura + pintura). |
| **Copiar/Pegar** | Modo del wheel Q para seleccionar terreno y colocar stamps. |

---

## 📐 Formas

Cicla con la tecla `.`. Todas soportan deshacer.

| Forma | Marcado | Caso de uso |
|-------|---------|-------------|
| **Círculo** | Clic simple (centro de la brocha) | Esculpido general |
| **Cuadrado** | Clic simple (centro) | Plataformas, cimientos |
| **Línea** | 2 clics (A → B) | Caminos, muros, senderos. Semiancho = radio. |
| **Rectángulo** | 2 clics (esquinas opuestas) | Cimientos, granjas, plataformas. Bordes suaves. |
| **Polígono** | Clic por vértice, doble clic cierra (3+ vértices) | Formas complejas, áreas custom |

- Las formas no circulares muestran **solo el contorno marcado** (sin anillos circulares).
- La suavidad afecta el blend de bordes en todas las formas.

---

## ⚙️ Modos de Caída (Subir/Bajar)

Config en AJUSTES → "Caída Raise/Lower":

| Modo | Comportamiento |
|------|----------------|
| **Punta** | Pico central. Controlado por el slider "Potencia caída" (1–5). |
| **Plano** | Disco uniforme — mismo peso en todo el radio. |
| **Suave** | Campana coseno — transición suave, sin borde duro. |
| **Lineal** | Taper recto de centro a borde. |

---

## 🧱 Stamps (Copiar/Pegar)

Función potente para replicar estructuras de terreno.

### Selección por rectángulo (recomendado)

Elegí Copiar/Pegar en el wheel Q. Aplicar marca la selección: dos esquinas en Rectángulo, dos extremos en Línea, vértices en Polígono (doble pulso tras 3+ puntos), o la huella actual en Círculo/Cuadrado. El HUD indica el paso siguiente.

### Flujo

1. **Marca el área**: usa cualquier forma (Círculo, Rect, Línea, Poli, Rampa) — o la selección por rectángulo de arriba.
2. **Copiar**: `Ctrl+Shift+C` → "Stamp copiado: X tiles" (portapapeles en RAM).
3. **Preview Pegar**: `Ctrl+Shift+V` → wireframe fantasma en la mira.
   - `Q`/`E` = rotar ±15°
   - `Alt`+Rueda = rotación continua
   - `Shift`+Rueda = escalar 0.25×–4×
   - `Esc` = cancelar
4. **Confirm**: Volvé a pulsar el atajo de pegar para confirmar la vista previa.

> Los stamps de selección escriben **solo dentro de la región copiada**; el terreno de fuera no se toca.

### Historial
- Hasta **10 stamps** en RAM (más reciente primero).
- Teclas `Siguiente stamp` / `Stamp anterior` (reasignables) para ciclar.
- Flash: "Stamp historial: Copy: 3 tiles [2/5]"

### Persistencia
- **AJUSTES → STAMPS**:
  - Guardar el stamp actual a disco (`.stamp` en `BepInEx/config/TerrainForge/stamps/`, máx 12).
  - Cargar un stamp al portapapeles.
  - Listar/borrar stamps guardados.

### Herramienta stamp
- AJUSTES → "Herramienta stamp" (4 opciones):
  - `Igual a brocha` (default) — usa la misma tool que la brocha
  - `Cualquiera` — sin requisito
  - `Pico` — requiere un pico equipado
  - `Varita TerrainForge` — requiere la varita del mod
- El HUD muestra `STAMP LISTO [2/5]` si la tool está equipada + hay stamps en el portapapeles.
- Sin la tool: `Stamp: herramienta no equipada`, copiar/pegar bloqueados.

---

## 💾 Snapshots (Backups)

Backup/restore completo del terreno.

### Guardar
1. AJUSTES → **SNAPSHOTS**.
2. Nombre (vacío = fecha/hora automática).
3. **Guardar** → tiles editados en `BepInEx/config/TerrainForge/snapshots/<nombre>.dat` (máx 12).

### Cargar
1. Selecciona un snapshot.
2. **Cargar** → restaura los tiles con ownership, **1 paso de undo**.
3. Los tiles no cargados avisan limpiamente — importa cerca de la ubicación original.

### Características
- Formato binario versionado (`.dat`).
- Restaura altura + smooth + pintura + ownership.
- La importación es deshacible (`Ctrl+Z` revierte la importación).
- Los tiles no cargados avisan limpiamente.

---

## 💾 Perfiles

Guarda/carga configuraciones completas de brocha.

- AJUSTES → **PERFILES**.
- Nombre + **Guardar** → radio, suavidad, op, forma, pintura, modo de rampa, ángulo, modo de suavizado, raise delta, fuerza de pintura, falloff, shore offset.
- **Cargar** → aplica al instante.
- Máx 12 perfiles (formato culture-invariant en `Presets > Slots`).
- Atajo: `Next Profile` (asigna en TECLAS) para ciclar.

---

## 🌐 Idiomas

**15 opciones:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

### Auto-Detect
- El default `Auto` detecta el idioma del juego vía reflection: `Localization.instance.GetSelectedLanguage()`.
- Mapea el idioma de Valheim → índice interno.
- Si un idioma no tiene traducción completa → fallback a English.
- Configurable en AJUSTES → "Idioma" (cicla las 15 opciones).
- Las etiquetas de teclas se reconstruyen al abrir el menú.

### Estado de las traducciones
- **Completas:** English, Español
- **Fallback a English:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Traducciones de la comunidad bienvenidas vía PR.

### Esta documentación
Páginas de la wiki traducidas: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Multijugador

### Requisitos
- **Mod en cliente Y servidor** para edits mayores a ±8 m y una replicación correcta.
- `UnlockDeltaLimit` también en el servidor, si no el servidor clampa el terreno.

### Autoridad del Servidor (Config Sync)
Cuando el servidor corre el mod, su sección `Server` gana:
- `AllowTerrainEdits` (default `true`) — master switch; si es `false`, los clientes no pueden aplicar/deshacer/rehacer.
- `AdminOnlyEdits` (default `false`) — solo admins/host editan.
- `RespectWards` (default `true`) — bloquea edits dentro de wards ajenos.
- `RadiusCap` / `DeltaCap` (0 = off) — caps aplicados por el servidor, visibles en HUD+preview.

> Los servidores **sin** el mod ignoran la sección → el cliente funciona para test.

### Prevención de Desync
- Mantén `MaxUpDelta` / `MaxDownDelta` consistentes en servidor + clientes.
- Radios grandes pueden desincronizar clientes sin el mod.

---

## ⚙️ Configuración

Archivo: `BepInEx/config/com.terrainforge.valheim.cfg`

### General
| Setting | Default | Descripción |
|---------|---------|-------------|
| `Enabled` | `true` | Master switch |
| `PlayEffects` | `true` | Polvo + golpe al aplicar |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Tiñe la Varita TerrainForge (clon del Martillo) para distinguirla a simple vista del Martillo vanilla |
| `ToolTintColor` | `#4FC3F7` | Color hex de la varita (modelo + icono). Ej: `#FFC400` oro, `#8BC34A` verde |

### Brocha
| Setting | Default | Rango | Descripción |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Radio de la brocha (m) |
| `RadiusMin` | 2 | — | Radio mínimo al ajustar |
| `RadiusMax` | 60 | — | Radio máximo al ajustar |
| `RadiusStep` | 1 | — | Cambio de radio por tecla/rueda |
| `Softness` | 50 | 0–100 | Suavidad de borde (0 = duro, 100 = muy suave) |
| `FalloffPower` | 3 | 1–5 | Exponente de caída (solo modo Punta) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Metros por aplicación de Subir/Bajar |
| `PaintStrength` | 1 | 0–1 | Fuerza de pintura |
| `DefaultOperation` | Flatten | — | Operación inicial |
| `Shape` | Circle | — | Forma inicial |
| `Paint` | Dirt | — | Pintura inicial |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Ángulo de rampa fija (grados) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Margen de costa sobre el agua (m) |
| `UseFixedHeight` | false | — | Usar altura fija (estado del cuentagotas) |
| `FixedHeight` | 30 | -10–150 | Altura exacta de Aplanar (m) |

### Vanilla / integración
- `BlockVanillaTerrainTools`: false — con la varita equipada, ignora la edición de terreno de la azada/cultivador vanilla. También hay un interruptor en AJUSTES.

### Entrada
Todos los atajos son reasignables en la pestaña TECLAS. Destacados:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Teclas directas de op** (9, sin default): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (default)
- `AutoGiveTool`: true (entrega la varita al entrar)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (default Same as Brush)

### Preview
- `ShowPreview`: true, `ShowGuides`: true (guías de alcance, tinte de pendiente, anillo de nivel)

### Historial
- `UndoDepth`: 50 (0–500, 0 desactiva el historial)

### Servidor (Autoritativo vía Config Sync)
- `AllowTerrainEdits`: true (master switch)
- `AdminOnlyEdits`: false (solo admins)
- `RespectWards`: true (bloquea wards ajenos)
- `RadiusCap` / `DeltaCap`: 0 (off) — caps aplicados por el servidor

### Rendimiento
- `TilesPerFrame`: 4 (1–32 tiles/frame)

---

## 🐛 Solución de Problemas

| Problema | Solución |
|----------|----------|
| **El menú no aparece / cursor invisible** | Cierra el juego. Borra `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\TerrainForge\` y `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Reinicia TMM, check updates. |
| **Update no aparece en TMM** | TMM → Settings → "Refresh mod list" (Ctrl+R). Cierra/reabre TMM. Verifica el perfil correcto. |
| **Conflicto de versión en Thunderstore** | Usa la misma versión nueva `X.Y.Z` en `.csproj`, `Plugin.cs` y `manifest.json`; debe ser superior a la ya publicada. Vuelve a compilar y empaquetar. |
| **Copiar/pegar stamp no hace nada** | HUD: `STAMP LISTO [X/Y]` = listo. Si dice "herramienta no equipada", equipa la tool (AJUSTES → Herramienta stamp). |
| **Preview invisible bajo el agua** | Corregido en 0.3.2 — el preview flota en la superficie. Actualiza. |
| **Círculos en Línea/Rect/Poli** | Corregido en 0.3.2 — se muestra solo el contorno marcado. Actualiza. |
| **La rueda radial no funciona** | Corregido en 0.3.2 (`ZInput.GetMouseScrollWheel()`) y 0.3.4 (la selección ya no se borra con el cursor centrado). Actualiza. |
| **La config no guarda** | Cierra el menú (F5) para auto-guardar. O edita `BepInEx/config/com.terrainforge.valheim.cfg` directamente. |
| **Desync en multijugador** | Mod en cliente+servidor. `UnlockDeltaLimit` en ambos. Mismos `MaxUpDelta`/`MaxDownDelta`. |
| **El build falla** | Verifica los paths `ValheimDir` y `BepInExCoreDir` en `.csproj` o vía CLI. Se requiere .NET SDK 8+. |
| **Compilar / empaquetar** | `dotnet build -c Release` (DLL en `build/`), `-p:Deploy=true` (instala en el perfil) o `-p:Package=true` (zip en `build/`). |

---

## 🔗 Enlaces

- **Sitio web del proyecto:** [marceloanton.github.io/TerrainForge](https://marceloanton.github.io/TerrainForge/)
- **Página de Thunderstore:** [TerrainForge en Thunderstore](https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/)
- **Repositorio GitHub:** [TerrainForge en GitHub](https://github.com/marceloanton/TerrainForge)
- **Donaciones:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [Changelog](Changelog)

---

*TerrainForge — Terraforma Valheim sin límites.*
