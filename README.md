# TerrainForge

[![Invítame un café en cafecito.app](https://cdn.cafecito.app/imgs/buttons/button_5.svg)](https://cafecito.app/byrka)

---

## 🎨 Novedades 0.3.4 / What's New 0.3.4

- **Interfaz nativa de Valheim**: Rediseño completo usando la tipografía **Averia** y la paleta de colores oficial de Valheim (oro, grises oscuros). Botones, paneles, pestañas y scrollbars con estilo nativo.
- **Menú radial (Q) nativo**: La rueda del mouse ahora funciona como en los menús radiales vanilla de Valheim (construcción, mapa). Hover corregido en los botones del radial.
- **Cursor siempre visible**: Al abrir el menú (F5 o botón ESC), el cursor se fuerza visible y desbloqueado cada frame, sin que el juego lo oculte.
- **Ventana redimensionada**: Ventana inicial 560×600px con scroll suave (mínimo 400px), contenido completo visible.

---

## 📋 Descripción

Una brocha de terraformado libre para Valheim. Aplana, sube, baja, suaviza, crea **rampas** (dos puntos, ángulo fijo y senda), **costa**, pinta y restaura terreno de cualquier tamaño, superando el límite vanilla de ±8 m, con deshacer/rehacer, cinco formas de brocha, menú radial, perfiles guardados, **stamps (copiar/pegar terreno)**, snapshots de terreno y **14 idiomas + auto-detect** (detecta el idioma del juego automáticamente).

El mod incluye su propia **Varita TerrainForge** (clon del martillo) como herramienta de brocha, te la entrega automáticamente al entrar y lo explica todo en una pestaña de **ayuda in-game** (tecla de menú, por defecto `F5`).

---

## 🎯 Por qué cliente Y servidor

Los cambios de terreno se guardan en el mundo y el servidor los replica a todos los jugadores. Como el juego recorta los edits a ±8 m, **TerrainForge debe instalarse también en el servidor**; si no, el servidor regenera el terreno recortado mientras los clientes ven el edit completo → desincronización.

---

## 📦 Requisitos

- **BepInEx 5** (x64, Mono) en cliente y servidor. Usuarios de Thunderstore ya lo tienen.
- **Jotunn** (`ValheimModding-Jotunn`) — registra la herramienta personalizada. Está en `manifest.json`; Thunderstore lo instala solo.

---

## 📥 Instalación

1. Descarga la última versión desde Thunderstore o GitHub Releases.
2. Colócala en:
   - **Cliente:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Servidor:** `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicia el juego / servidor.

> ⚠️ **Debe estar en ambos** (cliente y servidor) para edits >±8m y replicación correcta.

---

## 🎮 Uso

Equipa la **Varita TerrainForge** (te la dan al entrar al mundo; vuelve a conseguirla con `F8`), apunta al suelo y mantén **clic derecho** para esculpir.

Mientras apuntas, un anillo sólido marca el área real que se genera, con guías tenues del color de la operación (zona a plena fuerza, radio máximo, distancia máxima de mira). En Rampa se dibuja el corredor A-B real mientras marcas. El HUD (abajo a la derecha) muestra operación, radio, forma, pintura y contadores de deshacer/rehacer; una segunda línea muestra la altura apuntada, el objetivo actual y la pendiente local en grados. Las guías se desactivan en `Vista previa > Mostrar guías`.

---

## ⌨️ Controles

| Acción | Tecla por defecto |
|--------|-------------------|
| Aplicar brocha | Mantener `Mouse1` (clic derecho) |
| Subir radio | `=` (también `Ctrl` + rueda) |
| Bajar radio | `-` (también `Ctrl` + rueda) |
| Cambiar operación | `,` |
| Cambiar forma | `.` |
| Cambiar pintura | `/` |
| Usar altura apuntada (en vez de tus pies) para Aplanar | Mantener `Shift` izq. |
| Deshacer último trazo | `Ctrl+Z` |
| Rehacer trazo deshecho | `Ctrl+Y` |
| Fijar / liberar altura de Aplanar (cuentagotas) | `G` |
| Cambiar modo de rampa (dos puntos / ángulo fijo / senda) | `H` |
| Ajustar ángulo de rampa fija (con Rampa seleccionada) | Mantener `Alt` + teclas de radio / rueda |
| Menú radial de operaciones (mantener, soltar para confirmar) | `Q` |
| Modo ajuste (rueda = radio, zoom congelado) | `V` |
| Ajuste fino del radio (pasos de 0.1 m) | Mantener `Shift` + rueda / teclas de radio |
| **Copiar terreno (Stamp)** | `Ctrl+Shift+C` |
| **Pegar terreno (Stamp)** | `Ctrl+Shift+V` |
| Obtener la Varita TerrainForge | `F8` |
| Abrir menú de teclas + ayuda | `F5` |

**Teclas extra (sin tecla por defecto, asígnalas en TECLAS):**
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa, Nieve
- Siguiente / anterior perfil
- Siguiente / anterior stamp (historial)

---

## 🔄 Operaciones

Las operaciones ciclan: `Aplanar`, `Subir`, `Bajar`, `Suavizar`, `Rampa`, `Pintar`, `Restaurar`, `Costa` (aplana hacia el nivel del agua + margen, para playas) y `Nieve` (pinta nieve como Pintar pero solo nieve; con `Shift` la quita y restaura el suelo original).

---

## 📐 Formas

`Círculo`, `Cuadrado`, `Línea`, `Rectángulo`, `Polígono` (tecla `.`).

- `Línea`/`Rectángulo`: dos pulsos de Aplicar (A y B; el semiancho de la línea = radio de la brocha, las esquinas definen el área con bordes suaves).
- `Polígono`: suma un vértice por pulso y cierra con doble pulso (3+ vértices).
- Todas las operaciones de área aplican sobre estas huellas con soporte de deshacer.

---

## ⚙️ Modos de caída (Subir/Bajar)

Cicla en AJUSTES → "Caída Raise/Lower":

| Modo | Efecto |
|------|--------|
| **Punta** (original) | Pico central, controlado por "Potencia caída" |
| **Plano** | Disco uniforme, mismo peso en todo el radio |
| **Suave** | Campana coseno, transición suave |
| **Lineal** | Decrece en línea recta del centro al borde |

---

## 🌐 Idiomas

**15 opciones:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — default `Auto`.

**Auto-detect:** por defecto `Auto` detecta el idioma configurado en Valheim vía reflection. Si el idioma no tiene traducción completa, usa English como fallback.

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

## 🔧 Operaciones Detalladas

### Aplanar
Sigue el suelo **bajo la mira** por defecto, se adapta a lo que apuntes (p.ej. una rampa). Mantén `Shift` izq. para nivelar a tus pies.

### Subir / Bajar
Eleva/baja terreno según modo de caída y potencia.

### Suavizar
Promedia cada punto con sus vecinos (blur real), alisando lomos, escalones y ruido. Mantén pulsado para suavizar progresivamente.  
Modos: `Completo` (ambos sentidos), `SoloLomos` (solo baja), `SoloHoyos` (solo rellena).  
Forma Línea + Suavizar = suavizado a lo largo de un camino.

### Rampa
Tres modos (`H`): **dos puntos** (marcas **A** y **B**, une ambas alturas con pendiente recta), **ángulo fijo** (**B** da dirección y largo, la altura sigue `Ángulo rampa`, ajustable con `Alt` + radio), y **senda** (marcas una curva **A→B→C…**, doble pulso cierra). El radio de la brocha pone el ancho de la rampa y la suavidad solo ablanda los bordes laterales. Vista previa: corredor relleno semitransparente + anillos de altura en A y B.

### Costa
Aplana hacia el nivel del agua + margen (playas/costas), con suavidad mezclando como Aplanar.

### Pintar
Tipos: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset.

### Restaurar
Devuelve el terreno a su estado original.

### Nieve
Pinta nieve como Pintar pero solo nieve; con `Shift` la quita y restaura el suelo original.

---

## 🎨 Vista Previa

- Anillo sólido: área real generada.
- Guías tenues: zona a plena fuerza, radio máximo, distancia máxima de mira.
- En Rampa: corredor A-B real (bordes + relleno semitransparente) + anillos de altura en A y B.
- En Aplanar/Costa: anillo marcador de nivel objetivo.
- Bloqueado por servidor/ward: guías en gris.
- Bajo agua: preview flota en superficie.

---

## 🖥️ Menú In-Game (`F5`)

3 pestañas con **scroll global** (rueda del ratón):

| Pestaña | Función |
|---------|---------|
| **TECLAS** | Reasigna teclas: click → presiona tecla/botón; `Esc` cancela. Combos: mantené modificadores y después tecla principal. Botones ratón soportados. |
| **AYUDA** | Referencia completa de operaciones, controles, preview, multijugador, guardado. |
| **AJUSTES** | Todo editable en vivo: sliders, toggles, ciclos, perfiles, stamps, snapshots, idioma. Se guarda al cerrar. |

**Botón en ESC:** el menú de pausa tiene un botón "TerrainForge" que abre el menú.

---

## 🌍 Servidor + Cliente

- **Requerido en ambos** para edits >±8m y replicación correcta.
- Sección `Servidor` (autoritaria via config sync):
  - `PermitirEditsTerreno` (default on)
  - `SoloAdmins` (default off)
  - `RespetarWards` (default on)
  - `TopeRadio` / `TopeDelta` (0 = sin límite)
- Servidor **sin** el mod ignora la sección → cliente funciona para test.

---

## ⚡ Rendimiento

`TilesPorFrame` (1–32, default 4): tiles por frame en trazos grandes. Evita lag.

---

## 📝 Changelog

Ver [CHANGELOG.md](CHANGELOG.md).

---

## 🔗 Enlaces

- **Donaciones / Donations:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Issues / PRs:** GitHub del mod