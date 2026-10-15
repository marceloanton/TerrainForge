# TerrainForge — Roadmap

> Documento interno de desarrollo. No se incluye en el paquete de release.
> Versión actual en desarrollo: **0.3.4** (aún NO publicada en Thunderstore).

---

## Estado de 0.3.4 (en desarrollo)

### Listo y en el código
- UI nativa Valheim (fuente Averia, paleta de colores, botones/paneles).
- Menú radial (Q) con selección por rueda (`ZInput.GetMouseScrollWheel()`).
- Heatmaps: Pendiente (H) y Altura (J).
- Stamp Preview Pro: curvas de nivel en la vista previa del stamp.
- Shore falloff configurable (`ShoreFalloffMode`: Linear / Smooth / Flat / Pointy).
- Stamps: copiar/pegar (`Ctrl+Shift+C` / `Ctrl+Shift+V`) + historial en RAM + persistencia en disco (`.stamp`).
- Rampas: preview de corredor relleno + anillos de marcas A/B; ya no dibuja el círculo del footprint.
- Keybinds reasignables (captura de teclas y botones del ratón).
- 14 idiomas + autodetección; traducciones completas ES/EN.
- Documentación ES/EN (README, CHANGELOG, WIKI).

### A medias / sin verificar en juego
- Rueda del menú radial: se reportó que solo funciona con ESC abierto (pendiente confirmar tras reinicio limpio).
- `Alt` + Rueda para altura fija: implementado, sin confirmar.
- Ventana del menú F5 con "textura antigua".
- Cursor forzado visible: aplicado, sin confirmar.

### Pendiente (no implementado)
- **Botón "Restablecer atajos"** en el menú (claves de locale ya presentes, falta el método y el botón).
- Reorganización del panel (funciones repetidas, se queda chico).
- Más info en pantalla (queja 2 del usuario).
- Texturas no-bioma (snow, gravel, paved, cultivated, etc.).
- Acceso a funciones de herramientas que quedaron sin botón/atajo.

---

## Plan inmediato — 0.3.4 (antes de publicar)

1. **HUD persistente con más info**: coordenadas, bioma, recursos cercanos, estado del stamp.
2. **Measurement HUD persistente**: distancia, volumen y pendiente siempre visibles.
3. **Biome / Resource Mini-HUD**: bioma actual + recursos cercanos (radio ~50 m).
4. **Botón "Restablecer atajos"** funcional.
5. **Fix rueda radial** (que no requiera ESC) y **Alt+Rueda** para altura fija.
6. **Reorganizar panel** F5 (agrupar, quitar repetidos).
7. **Texturas no-bioma** (snow, gravel, paved, cultivated, etc.).
8. **Documentar** los cambios en WIKI.md y CHANGELOG.md.

---

## Plan futuro

### 0.4.0 — "Advanced Tools"
- Blueprint Ghost System: preview fantasma con snap grid (1 m / 2 m / 4 m) y rotación en pasos (90° / 45° / 15°).
- Stamp Preview Pro mejorado (curvas de nivel reales sobre el terreno).
- Measurement HUD en vivo durante el marcado.

### 0.5.0 — "World Building"
- Texturas no-bioma + selector por bioma / personalizado.
- Blueprint System completo (guardar/cargar/colocar).
- Multiplayer: sync de stamps entre clientes y permisos por rango.

### 0.6.0 — "World Builder"
- Biblioteca de blueprints (guardar/cargar/compartir).
- Plantillas de terreno (prefabs) y biome painter.
- Herramienta de ríos/caminos por spline.
- Rendimiento: chunk streaming, LOD para stamps grandes, multithreading.

---

## Arquitectura modular (refactor pendiente)

Objetivo: dejar de tener todo en pocos archivos gigantes. Un documento/archivo por
herramienta, para que un problema en una herramienta se resuelva en su propio archivo.

### Core
- `Plugin.cs` — plugin principal, config, ciclo de vida.
- `TerrainEngine.cs` — operaciones de terreno, undo/redo, escritura a `TerrainComp`.
- `TerrainTypes.cs` — enums y structs (`TerrainOperation`, `TerrainShape`, `BrushSpec`, …).

### UI
- `KeybindMenu.cs` — menú F5 (pestañas, keybinds, settings).
- `TerrainRadial.cs` — menú radial Q *(hoy dentro de `TerrainBrush.cs`)*.
- `TerrainHud.cs` — HUD en pantalla *(hoy dentro de `TerrainBrush.cs`)*.
- `TerrainPreview.cs` — previews (brocha, rampas, stamps, heatmaps).

### Herramientas
- `TerrainBrush.cs` — input, lógica de brocha, stamps.
- `TerrainFx.cs` — efectos visuales (polvo, sonido) *(hoy dentro de `TerrainBrush.cs`)*.

### Datos
- `TerrainLoc.cs` — localización (`Loc`) *(hoy dentro de `TerrainTypes.cs`)*.
- `TerrainPresets.cs` — presets de brocha *(hoy dentro de `KeybindMenu.cs`)*.
- `TerrainStamps.cs` — sistema de stamps (RAM + disco).
- `TerrainSnapshots.cs` — sistema de snapshots.
