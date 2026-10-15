# TerrainForge

[![Invítame un café en cafecito.app](https://cdn.cafecito.app/imgs/buttons/button_5.svg)](https://cafecito.app/byrka)

---

## 📋 Descripción / Description

**ES:** Una brocha de terraformado libre para Valheim. Aplana, sube, baja, suaviza, crea **rampas** (dos puntos, ángulo fijo y senda), **costa**, pinta y restaura terreno de cualquier tamaño, superando el límite vanilla de ±8 m, con deshacer/rehacer, cinco formas de brocha, menú radial, perfiles guardados, **stamps (copiar/pegar terreno)**, snapshots de terreno y **14 idiomas + auto-detect** (detecta el idioma del juego automáticamente).

**EN:** A free-form terrain brush for Valheim. Flatten, raise, lower, smooth, create **ramps** (two-point, fixed-angle, and path), **shore**, paint and restore terrain of any size, surpassing the vanilla ±8 m limit, with undo/redo, five brush shapes, radial menu, saved presets, **stamps (copy/paste terrain)**, terrain snapshots, and **14 languages + auto-detect** (automatically detects the game's language).

---

## 🎯 ¿Por qué cliente Y servidor? / Why Client AND Server?

**ES:** Los cambios de terreno se guardan en el mundo y el servidor los replica a todos los jugadores. Como el juego recorta los edits a ±8 m, **TerrainForge debe instalarse también en el servidor**; si no, el servidor regenera el terreno recortado mientras los clientes ven el edit completo → desincronización.

**EN:** Terrain changes are saved in the world and replicated by the server to all players. Since the game clamps edits to ±8 m, **TerrainForge must also be installed on the server**; otherwise, the server regenerates clamped terrain while clients see the full edit → desync.

---

## 📦 Requisitos / Requirements

- **BepInEx 5** (x64, Mono) en cliente y servidor. Usuarios de Thunderstore ya lo tienen.
- **Jotunn** (`ValheimModding-Jotunn`) — registra la herramienta personalizada. Está en `manifest.json`; Thunderstore lo instala automáticamente.

---

## 📥 Instalación / Installation

1. Compila (ver abajo) o copia `TerrainForge.dll`.
2. Colócala en:
   - **Cliente:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Servidor:** `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicia el juego / servidor.

---

## 🎮 Uso / Usage

Equipa la **Varita TerrainForge** (te la dan al entrar al mundo; vuelve a conseguirla con la tecla *dar herramienta*, `F8` por defecto), apunta al suelo y mantén **aplicar** (clic derecho por defecto) para esculpir bajo la mira.

While aiming, a solid ring marks the real generated area, with faint guides in the operation color (full-strength zone, max brush radius, max aim distance). In Ramp mode the real A-B corridor is drawn while marking. The HUD (bottom right) shows operation, radius, shape, paint, and undo/redo counts; a second line shows aimed height, current target, and local slope in degrees. Guides can be toggled in `Preview > Show Guides`.

---

## ⌨️ Controles / Controls

| Acción / Action | Tecla por defecto / Default Key |
|----------------|--------------------------------|
| Aplicar brocha / Apply Brush | Mantener `Mouse1` (clic derecho) |
| Subir radio / Increase Radius | `=` (también `Ctrl` + rueda) |
| Bajar radio / Decrease Radius | `-` (también `Ctrl` + rueda) |
| Cambiar operación / Cycle Operation | `,` |
| Cambiar forma / Cycle Shape | `.` |
| Cambiar pintura / Cycle Paint | `/` |
| Usar altura apuntada / Use Aimed Height | Mantener `Shift` izq. |
| Deshacer / Undo | `Ctrl+Z` |
| Rehacer / Redo | `Ctrl+Y` |
| Cuentagotas / Eyedropper | `G` |
| Modo rampa / Ramp Mode | `H` |
| Ángulo rampa fija / Fixed Ramp Angle | Mantener `Alt` + teclas de radio / rueda |
| Menú radial / Radial Menu | `Q` (mantener) |
| Modo ajuste / Adjust Mode | `V` |
| Ajuste fino (0.1 m) / Fine Adjust | Mantener `Shift` + rueda / teclas de radio |
| **Copiar Stamp** | `Ctrl+Shift+C` |
| **Pegar Stamp** | `Ctrl+Shift+V` |
| Obtener Varita / Get Wand | `F8` |
| Menú principal / Open Menu | `F5` |

**Teclas extra (sin default, asígnalas en TECLAS):**
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa, Nieve
- Siguiente / anterior perfil
- Siguiente / anterior stamp (historial)

---

## 🔄 Operaciones / Operations

Las operaciones ciclan: `Aplanar`, `Subir`, `Bajar`, `Suavizar`, `Rampa`, `Pintar`, `Restaurar`, `Costa` (aplana hacia el nivel del agua + margen, para playas) y `Nieve` (pinta nieve como Pintar pero solo nieve; con `Shift` la quita y restaura el suelo original).

Operations cycle: `Flatten`, `Raise`, `Lower`, `Smooth`, `Ramp`, `Paint`, `Restore`, `Shore` (flattens toward water level + margin, for beaches) and `Snow` (paints snow like Paint but snow-only; hold `Shift` to remove and restore original ground).

---

## 📐 Formas / Shapes

`Círculo`, `Cuadrado`, `Línea`, `Rectángulo`, `Polígono` (tecla `.`).  
`Circle`, `Square`, `Line`, `Rectangle`, `Polygon` (key `.`).

- `Línea`/`Rectángulo`: dos pulsos de Aplicar (A y B; semiancho = radio, esquinas definen área con bordes suaves).
- `Polígono`: un vértice por pulso, doble-pulso cierra (3+ vértices).
- Todas las operaciones de área aplican sobre estas huellas con soporte de deshacer.

---

## ⚙️ Modos de caída / Falloff Modes (Subir/Bajar / Raise/Lower)

Cicla en AJUSTES → "Caída Raise/Lower":

| Modo / Mode | Efecto / Effect |
|-------------|----------------|
| **Punta** / **Pointy** | Pico central (original), controlado por "Potencia caída" |
| **Plano** / **Flat** | Disco uniforme, mismo peso en todo el radio |
| **Suave** / **Smooth** | Campana coseno, transición suave |
| **Lineal** / **Linear** | Decrece en línea recta del centro al borde |

---

## 🌐 Idiomas / Languages

**15 opciones:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

**Auto-detect:** por defecto `Auto` detecta el idioma configurado en Valheim (`Localization.instance.GetSelectedLanguage()`) vía reflection. Si el idioma no tiene traducción completa, usa English como fallback.

---

## 💾 Snapshots (Backups)

Pestaña **SNAPSHOTS** en AJUSTES: ponle un nombre para guardar todos los tiles editados en `BepInEx/config/TerrainForge/snapshots/<nombre>.dat`, luego cárgalo después (incluso en otra partida). La importación restaura con propiedad y es deshacible; tiles no cargados avisan limpiamente.

---

## 🧱 Stamps (Copiar/Pegar Terreno)

Pestaña **STAMPS** en AJUSTES:

- `Ctrl+Shift+C` copia el área marcada (forma actual) al portapapeles (RAM).
- `Ctrl+Shift+V` muestra preview fantasma (wireframe) en la mira; `Q`/`E` rotan, `Alt`+rueda rota continuo, `Shift`+rueda escala, click confirma.
- Historial de 10 stamps en RAM: teclas "Siguiente stamp" / "Stamp anterior" para ciclar.
- Guardar stamp a disco (`.stamp` en `BepInEx/config/TerrainForge/stamps/`, tope 12), cargar al portapapeles, listar y borrar desde la UI.
- **Herramienta stamp**: en AJUSTES → "Herramienta stamp" (4 opciones): `Igual a brocha` (default), `Cualquiera`, `Pico`, `Varita TerrainForge`. El HUD muestra `STAMP LISTO [2/5]` cuando la tool correcta está equipada y hay stamps en el portapapeles; si no, avisa `Stamp: herramienta no equipada`. `Ctrl+Shift+C/V` no funcionan sin la tool equipada.

---

## 🔧 Operaciones Detalladas / Detailed Operations

### Aplanar / Flatten
Sigue el suelo **bajo la mira** por defecto, se adapta a lo que apuntes (p.ej. una rampa). Mantén `Shift` izq. para nivelar a tus pies.

### Subir / Raise
Eleva terreno según modo de caída y potencia.

### Bajar / Lower
Baja terreno según modo de caída y potencia.

### Suavizar / Smooth
Promedia cada punto con sus vecinos (blur real), alisando lomos, escalones y ruido. Mantén pulsado para suavizar progresivamente.  
Modos: `Completo` (ambos sentidos), `SoloLomos` (solo baja), `SoloHoyos` (solo rellena).  
Forma Línea + Suavizar = suavizado a lo largo de un camino.

### Rampa / Ramp
Tres modos (`H`): **dos puntos** (marcas **A** y **B**, une ambas alturas con pendiente recta), **ángulo fijo** (**B** da dirección y largo, la altura sigue `Ángulo rampa`, ajustable con `Alt` + radio), y **senda** (marcas una curva **A→B→C…**, doble pulso cierra). El radio de la brocha pone el ancho de la rampa y la suavidad solo ablanda los bordes laterales. Vista previa: corredor relleno semitransparente + anillos de altura en A y B.

### Costa / Shore
Aplana hacia el nivel del agua + margen (playas/costas), con suavidad mezclando como Aplanar.

### Pintar / Paint
Tipos: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset.

### Restaurar / Restore
Devuelve el terreno a su estado original.

### Nieve / Snow
Pinta nieve como Pintar pero solo nieve; con `Shift` la quita y restaura el suelo original.

---

## 🎨 Vista Previa / Preview

- Anillo sólido: área real generada.
- Guías tenues: zona a plena fuerza, radio máximo, distancia máxima de mira.
- En Rampa: corredor A-B real (bordes + relleno semitransparente) + anillos de altura en A y B.
- En Flatten/Shore: anillo marcador de nivel objetivo.
- Bloqueado por servidor/ward: guías en gris.
- Bajo agua: preview flota en superficie.

---

## 🖥️ Menú In-Game (`F5`)

3 pestañas con **scroll global** (rueda del ratón):

| Pestaña / Tab | Función |
|---------------|---------|
| **TECLAS** | Reasigna teclas: click → presiona tecla/botón; `Esc` cancela. Combos: mantené modificadores y después tecla principal. Botones ratón soportados. |
| **AYUDA** | Referencia completa de operaciones, controles, preview, multijugador, guardado. |
| **AJUSTES** | Todo editable en vivo: sliders, toggles, ciclos, perfiles, stamps, snapshots, idioma. Se guarda al cerrar. |

**Botón en ESC:** el menú de pausa tiene un botón "TerrainForge" que abre el menú.

---

## 🌍 Servidor + Cliente / Server + Client

- **Requerido en ambos** para edits >±8m y replicación correcta.
- Sección `Servidor` (autoritaria via config sync):
  - `PermitirEditsTerreno` (default on)
  - `SoloAdmins` (default off)
  - `RespetarWards` (default on)
  - `TopeRadio` / `TopeDelta` (0 = sin límite)
- Servidor **sin** el mod ignora la sección → cliente funciona para test.

---

## ⚡ Rendimiento / Performance

`TilesPorFrame` (1–32, default 4): tiles por frame en trazos grandes. Evita lag.

---

## 🛠️ Compilar / Build

Requiere .NET SDK 8+ y las DLLs de referencia del juego + BepInEx.

```bash
dotnet build -c Release \
  -p:ValheimDir="D:\Steam\steamapps\common\Valheim" \
  -p:BepInExCoreDir="%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\core" \
  -p:JotunnDir="%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\ValheimModding-Jotunn" \
  -p:Deploy=true
```

- `ValheimDir`: carpeta raíz de Valheim (donde está `valheim_Data/Managed`).
- `BepInExCoreDir`: carpeta `core` de BepInEx del perfil que uses.
- `JotunnDir`: carpeta del plugin Jotunn instalado en ese perfil.
- `Deploy=true`: copia el `.dll` a `PluginsDir`.

---

## 📝 Notas / Notes

- Radio de herramienta vanilla grande es best-effort y puede desincronizar otros clientes; la brocha personalizada es el camino fiable para edits grandes.
- Mantén `DeltaMáxSubida` / `DeltaMáxBajada` consistentes entre servidor y clientes.

---

## 📝 Changelog

Ver [CHANGELOG.md](CHANGELOG.md).

---

## 🔗 Enlaces / Links

- **Donaciones / Donations:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Issues / PRs:** GitHub del mod