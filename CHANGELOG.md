# Changelog / Registro de Cambios

---

## [0.3.4] - 2026-10-15

### Añadido / Added

- **Interfaz nativa de Valheim / Valheim-native UI**: Rediseño completo de la UI usando la tipografía Averia, paleta de colores de Valheim (oro, grises oscuros), botones y paneles con estilo nativo.
- **Menú radial nativo (Q)**: Rueda del mouse funcional usando `ZInput.GetMouseScrollWheel()`, hover corregido en botones (radio 34-130px), comportamiento idéntico a menús radiales vanilla.
- **Cursor forzado visible**: En menú F5 y botón ESC, fuerza cursor visible y desbloqueado cada frame via `OnGUI()`.

### Corregido / Fixed

- Preview: círculos/cuadrado ocultos en Line/Rect/Poly al marcar punto A.
- Cursor visible en menú F5 y botón ESC (OnGUI fuerza visible).
- Rueda mouse en menú radial (Q) usando `ZInput.GetMouseScrollWheel()` nativo.
- Ventana menú: tamaño inicial 560×600, scroll mínimo 400px.

---

## [0.3.3] - 2026-10-15

### Añadido
- **Documentación bilingüe profesional (ES/EN)**: README, CHANGELOG y WIKI reescritos completamente en ambos idiomas con formato profesional, tablas y cobertura completa de todas las features.
- **Fix menú radial (Q)**: rueda del mouse funcional (usa `ZInput.GetMouseScrollWheel` nativo), hover corregido en botones.

### Corregido
- Preview: círculos/cuadrado ocultos en Line/Rect/Poly al marcar punto A.
- Cursor visible en menú F5 y botón ESC (OnGUI fuerza visible).
- Rueda mouse en menú radial (Q) usando API nativa de Valheim.
- Ventana menú: tamaño inicial 560×600, scroll mínimo 400px.

---

## [0.3.2] - 2026-10-14

### Añadido
- **14 idiomas + auto-detect**: detección automática vía reflection (`Localization.instance.GetSelectedLanguage()`). Idiomas: English, Español, Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano. Default `Auto`; fallback a English.

### Corregido
- Preview: círculos/cuadrado ocultos en Line/Rect/Poly al marcar punto A.
- Cursor visible en menú F5 y botón ESC (OnGUI fuerza visible).
- Rueda mouse en menú radial (Q) usando `ZInput.GetMouseScrollWheel()` nativo.
- Ventana menú: scroll view altura mínima 400px, ventana inicial 560×600px.

---

## [0.3.1] - 2026-10-14
- Fix versión para resolver conflicto en Thunderstore.

---

## [0.3.0] - 2026-10-13

### Añadido
- **Deshacer / Rehacer** (`Ctrl+Z`/`Ctrl+Y`): cada aplicación guarda exactamente los vértices que toca. Profundidad configurable (0-500) con slider + textbox en pestaña TECLAS. Contadores en vivo en HUD. Historial se limpia al salir del mundo.
- **Datos en vivo en HUD**: segunda línea con altura apuntada, objetivo actual y pendiente local en grados.
- **Cuentagotas** (`G`): fija altura de Aplanar en punto apuntado (gana a Shift). HUD muestra `fija`.
- **Rampas ángulo fijo** (`H`): modo `FixedAngle` donde B da dirección/largo y altura sigue `SlopeAngle`. Ajuste in-game con `Alt` + radio/rueda.
- **Nuevas formas**: `Línea` (cápsula 2 clicks), `Rectángulo` (2 esquinas), `Polígono` (click por vértice, doble-click cierra). Todas con undo y preview.
- **Pestaña AJUSTES in-game** (F5): todo editable en vivo — cicladores, sliders, toggles. Se guarda al cerrar.
- **Modos suavizado**: `Completo` / `SoloLomos` / `SoloHoyos` (cicla en AJUSTES). Línea + Suavizar = suavizado camino.
- **Operación Costa**: aplana hacia nivel agua + margen (playas). Suavidad mezcla como Aplanar.
- **Preview mejoras**: borde footprint tiñe rojo en pendientes; Aplanar/Costa muestran anillo nivel objetivo.
- **Efectos feedback**: efecto nativo polvo+golpe en cada aplicación (throttled en hold). Toggle `General > PlayEffects`.
- **Localización EN/ES**: `General > Language` cambia menú, HUD, mensajes, ayuda. Etiquetas teclas se reconstruyen al abrir.
- **Menú radial** (`Q`): elige operación con mouse; soltar confirma, `Esc` cancela. Suprime brocha mientras abierto.
- **Perfiles nombrados**: guarda/carga/borra hasta 12 setups completos (AJUSTES → Perfiles).
- **Medidas en vivo**: HUD muestra largo/subida/ángulo línea/rampa, W×D+área rect, perímetro+área poly.
- **Rampas senda** (`Path`): curva A→B→C… (doble-pulso cierra) → Catmull-Rom suave. Preview corredor + estela, largo en vivo, undo.
- **Altura objetivo por texto**: `UseFixedHeight`/`FixedHeight` en config + textbox Fix/Release en AJUSTES. `G` captura en mismo estado.
- **Snapshots terreno**: guarda tiles editados en binario versionado (`.dat` en `snapshots/`, tope 12). Import restaura arrays con ownership, es undoable; tiles faltantes avisan.
- **Wards + topes servidor**: `RespectWards` bloquea wards ajenos; `RadiusCap`/`DeltaCap` server-enforced caps visibles en HUD/preview.
- **Aplicación por chunks**: tiles/frame configurable (`TilesPorFrame`, 1-32). Terreno aparece progresivo, 1 undo por trazo, auto-continúa si sueltas tecla.
- **Autoridad servidor**: sección `Servidor` (`AllowTerrainEdits`, `AdminOnlyEdits`). Valores server ganan por config sync; clientes no pueden aplicar/undo/redo contra ellos (HUD `SERVER: BLOQUEADO`). Servidores sin mod ignoran sección.

---

## [0.1.0] - 2026-10-12

### Añadido
- Brocha libre: **Aplanar**, **Subir**, **Bajar**, **Suavizar**, **Rampa**, **Pintar**, **Restaurar** bajo mira. Aplanar sigue altura apuntada (Shift = nivelar a pies); Suavizar promedia vecinos.
- **Varita TerrainForge** (clon martillo): entrega/equipado auto al entrar, tecla dar `F8`. Registro via Jotunn.
- **Rampa dos puntos**: click A, click B → pendiente recta une alturas (radio = ancho rampa). Corredor plano ancho con borde suave (suavidad = blend lateral).
- Radio brocha configurable (60m), **forma** (círculo/cuadrado), **pintura** ciclables in-game.
- **Preview en vivo**: anillo sólido = área real; guías tenues color op = zona plena fuerza, radio máx, distancia máx mira. En Rampa: corredor A-B real (ambos bordes). Guías desactivables `Preview > ShowGuides`. HUD top-left (op, radio, forma, pintura).
- **Menú atajos in-game** (`F5`, configurable) con skin propio + **pestaña ayuda in-game**.
- Eliminación clamp vanilla **±8 m** (`UnlockDeltaLimit`). Debe estar en servidor también.
- Ajustes vanilla opcionales: radio escalado azadón/cultivador, **sin costo construcción**, edits en **zonas protegidas**.
- Mira funciona sobre agua (raymarch heightmap sin collider).
- **Herramienta requerida** configurable (`Cualquiera`, `Pico`, `TerrainForgeTool`).

### Notas
- Edits persisten vía ZDO `TerrainComp` y replican por servidor → **requerido en cliente y servidor**.
- Principio: dependencias mínimas. Herramienta usa Jotunn ahora; futura versión registrará prefab via Harmony para no necesitar Jotunn.