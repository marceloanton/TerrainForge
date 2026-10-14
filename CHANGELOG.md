# Changelog

Todos los cambios notables de TerrainForge se documentan aquí.

El formato sigue [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) y
el proyecto usa [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Añadido

- **Donaciones Cafecito**: badge en el README (se ve en Thunderstore),
  `website_url` en el manifest y botón discreto al final del menú in-game
  que abre `https://cafecito.app/byrka`.

### Añadido

- **Modo ajuste** (`V`, reasignable): la rueda cambia el radio de la brocha sin
  mover la cámara — usos vanilla de la rueda (zoom, barra rápida, mapa,
  rotación de piezas) se congelan mientras está activo, con etiqueta en el HUD.
- **Deshacer/rehacer mantenidos**: manteniendo `Ctrl+Z` / `Ctrl+Y` se avanza
  continuo (con límite de velocidad), sin volver a pulsar.
- **Guías bloqueadas**: la vista previa se vuelve gris cuando el servidor o un
  ward niegan la edición, para que se vea antes de intentar.
- **Tutorial primera vez**: dos mensajes cortos al equipar la varita por
  primera vez (`General > SeenTutorial`), luego nunca más.
- **Etiqueta de radio topeado**: el HUD muestra `20 (tope)` cuando el límite
  del servidor recorta tu radio.
- **Auto-nombre de snapshots**: guardar sin nombre usa fecha y hora.
- **Ajuste fino del radio**: `Shift` + rueda / teclas de radio mueven en
  pasos de 0.1 m.

### Corregido

- Las formas Línea/Rectángulo/Polígono ya no muestran las guías circulares
  (borde, zona a plena fuerza, radio máx. y distancia de mira no tenían
  sentido ahí); solo se dibuja el contorno marcado (corredor, rectángulo,
  estela).

### Corregido

- **Suavizado sin costuras**: los vértices de borde ahora muestrean el tile
  vecino al otro lado del borde (mapeo exacto), así el suavizado funde ambos
  lados en lugar de abrir una grieta visible en la malla entre tiles. Aplica
  a toda huella; las demás operaciones ya eran continuas por construcción.

### Añadido

- **Operación Nieve**: brocha de solo nieve (misma máscara que la nieve del
  juego); con `Shift` la borra y restaura el suelo original. La vista previa
  se vuelve gris en modo borrar, el HUD muestra el modo, funciona sobre toda
  huella con soporte de deshacer.
- **Botón en menú de pausa**: el menú `ESC` ahora tiene una entrada
  `TerrainForge` (clonada del botón de ajustes, mismo estilo) que abre el menú
  del mod. `F5` sigue funcionando igual.

### Corregido

- Guías mucho más visibles (líneas más gruesas, alfas más fuertes) más una
  línea de log al arranque con el shader de la vista previa para diagnosticar
  previews invisibles vía `LogOutput.log`.
- HUD de estado movido a la **esquina inferior derecha** con la fuente IMGUI
  por defecto en vez de la Averia del juego.

## [0.2.0] - 2026-10-13

### Añadido

- **Deshacer / rehacer** de trazos de brocha (`Ctrl+Z` / `Ctrl+Y`,
  reasignables): cada aplicación (incluyendo rampas, pintura y restaurar)
  guarda los valores anteriores de exactamente los vértices que toca, así un
  paso atrás los restaura. Profundidad configurable
  (`Historial > ProfundidadDeshacer`, 0-500, 0 desactiva) con slider + caja
  exacta en el menú in-game (pestaña TECLAS), contadores en vivo de
  deshacer/rehacer y mensajes flash en el HUD. El historial se limpia al
  salir del mundo.
- **Datos en vivo en el HUD**: una segunda línea muestra la altura del suelo
  apuntada, el objetivo actual (altura de Aplanar / paso de Subir-Bajar /
  pintura / alturas A→B de Rampa) y la pendiente local en grados, actualizado
  cada frame mientras apuntas.
- **Cuentagotas**: tecla `EyedropKey` (por defecto `G`) fija el objetivo de
  Aplanar en la altura apuntada hasta soltarla (gana al modificador `Shift`);
  el HUD la muestra como `fija`.
- **Rampas de ángulo fijo**: `ModoRampa` (`DosPuntos` / `ÁnguloFijo`, con
  `RampModeKey`, por defecto `H`). En ángulo fijo B solo da dirección y
  largo; la altura sigue `ÁnguloRampa` (sube o baja según dónde apuntes);
  ajustas el ángulo in-game con `Alt` + teclas de radio/rueda.
- **Nuevas formas de brocha**: `Línea` (cápsula de dos pulsos con el radio
  como semiancho, reusa la vista previa de corredor), `Rectángulo` (dos
  esquinas opuestas, bordes suaves de radio + suavidad) y `Polígono` (un
  punto por pulso, doble pulso con 3+ puntos cierra y aplica). Todas las
  operaciones de área funcionan sobre ellas, con soporte de deshacer y
  estado en HUD + vista previa.
- **Pestaña AJUSTES in-game** (menú F5): todo editable en vivo sin tocar el
  config — cicladores de operación / forma / pintura / modo rampa / modo
  suavizar, sliders (radio, suavidad, ángulo rampa, metros subir-bajar,
  fuerza pintura, caída, margen costa) y todos los interruptores (vista
  previa, guías, efectos, sin costo, zonas protegidas, límite delta,
  autoridad servidor). Se guarda al cerrar el menú.
- **Modos de suavizado**: `Completo` blureea ambos sentidos, `SoloLomos`
  solo baja lomos, `SoloHoyos` solo rellena hoyos (cicla en AJUSTES). Forma
  Línea + Suavizar = suavizado a lo largo de un camino.
- **Operación Costa**: aplana hacia el nivel del agua más un margen (playas /
  costas), con suavidad mezclando como Aplanar.
- **Mejoras en vista previa**: el borde del footprint se tiñe hacia rojo en
  terreno empinado (sombreado de pendiente) y Aplanar/Costa muestran un
  anillo marcador del nivel objetivo que se lee como curva de nivel en
  pendientes.
- **Efectos de feedback**: el efecto propio de golpe de terreno del juego
  (polvo + golpe) suena en cada aplicación (limitado al mantener, siempre en
  rampas/formas); se apaga con `General > PlayEffects`.
- **Localización EN/ES**: `General > Language` cambia todo el menú, HUD,
  mensajes y ayuda entre Español e Inglés (etiquetas de teclas se reconstruyen
  al abrir).
- **Menú radial**: mantén `RadialKey` (por defecto `Q`) para elegir cualquiera
  de las 8 operaciones con el ratón; soltar confirma, `Esc` cancela. Suprime
  la aplicación de brocha mientras está abierto.
- **Perfiles nombrados**: guarda / carga / borra hasta 12 setups completos de
  brocha desde la pestaña AJUSTES (guardados en `Perfiles > Slots`, formato
  invariante de cultura).
- **Medidas en vivo**: mientras marcas, el HUD muestra largo/subida/ángulo de
  línea/rampa, ancho×fondo + área de rectángulo, perímetro + área de
  polígono; los números finales también se añaden a los mensajes de
  confirmación.
- **Rampas senda**: `ModoRampa.Senda` marca una curva A→B→C… (doble pulso con
  3+ puntos cierra) y construye una rampa Catmull-Rom suave a lo largo,
  ancho = radio; vista previa de corredor del siguiente tramo + estela, largo
  en vivo, soporte de deshacer.
- **Altura objetivo por texto**: el cuentagotas ahora vive en el config
  (`Brocha > UsarAlturaFija` / `AlturaFija`) con caja de texto + fila
  Fijar/Liberar en AJUSTES; `G` captura en el mismo estado, que persiste y
  mueve HUD + anillo de nivel.
- **Snapshots de terreno**: guarda los tiles editados en un archivo binario
  versionado y restáuralos luego (`BepInEx/config/TerrainForge/snapshots/`,
  UI en AJUSTES con guardar/cargar/borrar, tope 12 archivos); la importación
  resuelve tiles, restaura arrays con propiedad y es ella misma deshacible;
  tiles faltantes avisan limpiamente.
- **Wards + topes de servidor**: `RespetarWards` bloquea edits dentro de
  wards ajenos usando la comprobación del juego; `TopeRadio`/`TopeDelta`
  dejan al servidor recortar radio y delta, visible (HUD + vista previa usan
  los valores efectivos). Etiqueta `WARD` en HUD y filas en AJUSTES.
- **Aplicación por chunks**: trazos grandes procesan unos pocos tiles de
  heightmap por frame (`Rendimiento > TilesPorFrame`, slider en AJUSTES) en
  lugar de trabar; el terreno aparece progresivamente, un paso de deshacer
  por trazo completado, auto-continúa si sueltas la tecla, orden FIFO.
- **Autoridad de servidor**: nueva sección `Servidor` (`PermitirEditsTerreno`,
  `SoloAdmins`). Cuando el servidor corre el mod, sus valores mandan por
  config sync y los clientes no pueden aplicar/deshacer/rehacer contra ellos
  (HUD muestra `SERVER: BLOQUEADO` con la razón). Servidores sin el mod
  ignoran esto, así que la herramienta sigue sirviendo para probar.

## [0.1.0] - 2026-10-12

### Añadido

- Brocha de terreno libre: operaciones **Aplanar**, **Subir**, **Bajar**,
  **Suavizar**, **Rampa**, **Pintar** y **Restaurar**, aplicadas bajo la mira
  mientras mantienes la tecla de aplicar. Aplanar sigue la altura del suelo
  apuntado por defecto (mantén el modificador para nivelar a tus pies);
  Suavizar promedia cada punto con sus vecinos para un suavizado real.
- **Varita TerrainForge** (clon del martillo) como objeto herramienta que
  maneja la brocha, con entrega y equipado automático al entrar al mundo,
  más tecla de dar (`F8` por defecto). El registro del objeto usa Jotunn.
- **Operación Rampa** (dos puntos): pulsa *aplicar* para marcar punto A, luego
  apunta y pulsa *aplicar* otra vez para marcar punto B, y una pendiente
  recta une ambas alturas a lo largo del corredor A-B (el radio de la brocha
  pone el ancho de la rampa). El corredor es plano a lo ancho con borde
  suave (la suavidad solo controla la mezcla lateral), así la rampa coincide
  exactamente con la vista previa.
- Radio de brocha configurable (hasta 60 m), **forma** (círculo / cuadrado) y
  **tipo de pintura**, ciclables in-game.
- **Vista previa en vivo** al apuntar: anillo sólido marca el área real que
  se genera, y guías tenues del color de la operación muestran la **zona a
  plena fuerza**, el **radio máximo** y la **distancia máxima de mira** alrededor
  del jugador. En Rampa se dibuja el **corredor A-B real** (ambos bordes del
  ancho) mientras marcas. Guías desactivables con `Vista previa > MostrarGuías`.
  Más HUD superior izquierdo (operación, radio, forma, pintura).
- **Menú de atajos in-game** (`F5`, configurable) con skin propio, más
  **pestaña de ayuda in-game** documentando la herramienta y cada operación.
- Eliminación del clamp hardcodeado vanilla de **±8 m** de delta de terreno
  (`DesbloquearLímiteDelta`), así edits grandes sobreviven a la regeneración
  del terreno. Debe estar activado también en el servidor.
- Ajustes opcionales de herramientas vanilla: radio escalado de azadón/
  cultivador, **sin costo de construcción**, y edits en **zonas protegidas**
  (no-construcción / ward).
- Mira que funciona sobre agua raymarcheando contra el heightmap cuando no
  hay collider.
- **Herramienta requerida** configurable (`Cualquiera`, `Pico`, o la
  `TerrainForgeTool` del mod).

### Notas

- Los edits de terreno persisten vía el ZDO del `TerrainComp` y los replica
  el servidor, así que TerrainForge debe ir en **cliente y servidor**.
- Principio de diseño: dependencias externas al mínimo. La herramienta
  personalizada usa Jotunn ahora; una futura versión registrará el prefab
  via Harmony para no necesitar Jotunn.

---

[Unreleased]: https://example.com/terrainforge/compare/0.1.0...HEAD
[0.1.0]: https://example.com/terrainforge/releases/tag/0.1.0