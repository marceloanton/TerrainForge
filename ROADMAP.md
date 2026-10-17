# TerrainForge — Roadmap

> Documento interno de desarrollo. No se incluye en el paquete de release.
> Versión actual en desarrollo: **0.3.5** (0.3.4 ya publicada en Thunderstore).
> Versión pública resumida: `wiki/Roadmap.md` (en inglés, se publica en la wiki).

---

## Estado de 0.3.5 (en desarrollo)

### Hecho en esta tanda (0.3.5)
- **Stamp — selección rectangular (A→B)**: copia exactamente el rectángulo marcado (forma Rect → `RMB` marca A, apuntar B, `Ctrl+Shift+C`), recortado a nivel de vértice (sin bloques enteros de 64 m ni terreno intocado alrededor). El pegado (`Ctrl+Shift+V`) solo escribe dentro de la región copiada.
- **Stamps guardados v2**: incluyen la máscara de selección; los v1 siguen cargando.
- **Cámara libre (`F`) arreglada**: sin lock-on, sin deslizamiento de brocha ni captura al RMB; `Ctrl`+rueda = radio; los ajustes tienen prioridad sobre el input de vuelo; banner con la distancia.
- **Documentación**: README/CHANGELOG en inglés, manual completo en la wiki en **14 idiomas**, este ROADMAP interno y un roadmap público en la wiki.
- **Serialización**: `StampVersion` 1 → 2 (loader acepta v1 y v2).

### Estado de 0.3.4 (publicado)
- **HUD persistente**: coordenadas, bioma actual y recursos cercanos (×cantidad, radio ~30 m).
- **Botón "Restablecer atajos"** funcional (resetea las 30 teclas a su valor por defecto).
- **UI nativa Valheim** (fuente Averia, paleta, botones/paneles).
- **Menú radial (Q)** con selección por rueda (`ZInput.GetMouseScrollWheel()`); la selección se mantiene (antes se borraba con el cursor centrado).
- **`WheelDelta()`** robusto (Unity + `ZInput`) usado en radial, Alt+Rueda y radio.
- **Heatmaps**: Pendiente (H) y Altura (J).
- **Stamp Preview Pro**: curvas de nivel en la vista previa del stamp.
- **Shore falloff** configurable (`ShoreFalloffMode`: Linear / Smooth / Flat / Pointy).
- **Stamps**: copiar/pegar + historial en RAM + persistencia en disco (`.stamp`).
- **Selector de modo de rampa**: 3 botones (2 puntos / ángulo fijo / senda) en AJUSTES.
- **Acciones rápidas** en la pestaña de teclas (undo/redo/eyedrop/stamps/limpiar/heatmaps).
- **Bloqueo de tools vanilla** (`BlockVanillaTerrainTools`).
- **Diagnóstico de material (log)**: vuelca shader + ranuras de textura del terreno.
- **Color de la herramienta** (`ToolTint` / `ToolTintColor`).
- **Panel F5 reorganizado** en secciones (Herramienta / Vista / Vanilla / Servidor / General).
- 14 idiomas + autodetección; documentación ES/EN.
- **Build a `build/`**: `OutputPath=build\`, `-p:Package=true` genera el zip ahí.

### Pendiente
- **Texturas no-bioma**: hack de material *en evaluación* (ver abajo).

### Hack de material (en evaluación)
El terreno usa un único material por heightmap (`new Material(m_material)`) y una
máscara (`_ClearedMaskTex`) que solo elige dirt/cultivated/paved/nieve dentro de la
paleta del bioma. **No hay mapa bioma→material expuesto** en el decompilado, así que
la vía es: (1) diagnosticar qué ranuras de textura expone el shader (botón de
diagnóstico ya añadido), y (2) si es viable, copiar el material de un tile del bioma
objetivo al tile pintado. Es **client-side** (no persiste en servidor y puede
desincronizar la vista entre clientes) y frágil. Pendiente de los datos del log.

### Sin verificar en juego
- **0.3.5**: selección rectangular del stamp (que el rectángulo coincida con lo marcado, sin arrastrar terreno de fuera) y el lote de cámara libre (sin lock-on/deslizamiento/capturas, `Ctrl`+rueda = radio, ajustes dominantes, banner, rueda radial, panel F5) requieren prueba en juego antes de publicar.

---

## Plan inmediato — 0.3.5 (antes de publicar)

1. ~~Stamp — selección rectangular (A→B).~~ ✅
2. ~~Cámara libre (F): neutralizar RMB / rueda / capturas.~~ ✅
3. ~~Docs en inglés + wiki en 14 idiomas + roadmap.~~ ✅
4. **Validar en juego** los puntos de 0.3.5 y quitar los logs `[TF]` de depuración.
5. Publicar 0.3.5 (0.3.4 ya estaba subida y no incluía lo último).
6. **Texturas no-bioma.** *Hack en evaluación (diagnóstico añadido).*

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
- `KeybindMenu.cs` — menú F5 (pestañas, keybinds, settings). ✅ *(panel reagrupado)*
- `TerrainRadial.cs` — menú radial Q. ✅ *(extraído)*
- `TerrainHud.cs` — HUD en pantalla. ⏳ *no existe como clase: el HUD son métodos dentro de `TerrainBrush.cs`; extraer solo si aporta.*
- `TerrainPreview.cs` — previews (brocha, rampas, stamps, heatmaps). ✅

### Herramientas
- `TerrainBrush.cs` — input, lógica de brocha, stamps.
- `TerrainFx.cs` — efectos visuales (polvo, sonido). ✅ *(extraído)*
- `ColorExtensions.cs` — `Color.WithAlpha`. ✅ *(extraído)*

### Datos
- `TerrainLoc.cs` — localización (`Loc`). ✅ *(extraído)*
- `TerrainPresets.cs` — presets de brocha. ✅ *(extraído)*
- `TerrainStamps.cs` — sistema de stamps (RAM + disco). ⏳ *hoy clases anidadas en `TerrainEngine.cs`.*
- `TerrainSnapshots.cs` — sistema de snapshots. ⏳ *hoy clases anidadas en `TerrainEngine.cs`.*
