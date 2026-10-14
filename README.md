# TerrainForge

[![Invitame un café en cafecito.app](https://cdn.cafecito.app/imgs/buttons/button_5.svg)](https://cafecito.app/byrka)

Una brocha de terraformado libre para Valheim.

Aplana, sube, baja, suaviza, crea **rampas** (dos puntos, ángulo fijo y senda),
**costa**, pinta y restaura terreno de cualquier tamaño, superando el límite
vanilla de ±8 m, con deshacer/rehacer, cinco formas de brocha, menú radial,
perfiles guardados, **stamps (copiar/pegar terreno)**, snapshots de terreno y
localización EN/ES.

El mod incluye su propia **Varita TerrainForge** (clon del martillo) como
herramienta de brocha, te la entrega automáticamente al entrar y lo explica
todo en una pestaña de **ayuda in-game** (tecla de menú, por defecto `F5`).

## Por qué cliente Y servidor

Los cambios de terreno se guardan en el mundo y el servidor los replica a todos
los jugadores. Como el juego recorta los edits a ±8 m, **TerrainForge debe
instalarse también en el servidor**; si no, el servidor regenera el terreno
recortado mientras los clientes ven el edit completo → desincronización.

## Requisitos

- BepInEx 5 (x64, Mono) en cliente y servidor. Usuarios de Thunderstore ya lo
  tienen.
- **Jotunn** (`ValheimModding-Jotunn`) — registra la herramienta personalizada.
  Está en `manifest.json`; Thunderstore lo instala solo.

## Instalación

1. Compila (ver abajo) o copia `TerrainForge.dll`.
2. Ponla en:
   - Cliente: `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - Servidor: `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicia el juego / servidor.

## Uso

Equipa la **Varita TerrainForge** (te la dan al entrar al mundo; vuelve a
conseguirla con la tecla *dar herramienta*, `F8` por defecto), apunta al suelo
y mantén **aplicar** (clic derecho por defecto) para esculpir bajo la mira.

Mientras apuntas, un anillo sólido marca el área real que se genera, con guías
tenues del color de la operación (zona a plena fuerza, radio máximo y distancia
máxima de mira). En Rampa se dibuja el corredor A-B real mientras marcas.
El HUD (abajo a la derecha) muestra operación, radio, forma, pintura y
contadores de deshacer/rehacer; una segunda línea muestra la altura apuntada,
el objetivo actual y la pendiente local en grados. Las guías se desactivan en
`Vista previa > Mostrar guías`.

| Acción | Tecla por defecto |
|--------|-------------------|
| Aplicar brocha | Mantener `Mouse1` (clic derecho) |
| Subir radio | `=` (también `Ctrl` + rueda) |
| Bajar radio | `-` (también `Ctrl` + rueda) |
| Cambiar operación | `,` |
| Cambiar forma (círculo/cuadrado/línea/rect/poli) | `.` |
| Cambiar tipo de pintura | `/` |
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

**Teclas extra (sin tecla por defecto, asignalas en TECLAS):**
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa, Nieve
- Siguiente / anterior perfil
- Siguiente / anterior stamp (historial)

Las operaciones ciclan: `Aplanar`, `Subir`, `Bajar`, `Suavizar`, `Rampa`,
`Pintar`, `Restaurar`, `Costa` (aplana hacia el nivel del agua + margen, para
playas) y `Nieve` (pinta nieve como Pintar pero solo nieve; con `Shift` la
quita y restaura el suelo original).

**Modos de caída para Subir/Bajar** (cicla en AJUSTES → "Caída Raise/Lower"):
- **Punta** (original, pico central, controlado por "Potencia caída")
- **Plano** (disco uniforme, mismo peso en todo el radio)
- **Suave** (campana coseno, transición suave)
- **Lineal** (decrece en línea recta del centro al borde)

Todo se edita en vivo en el menú in-game (`F5` → pestaña **AJUSTES**):
operación / forma / pintura / modo rampa / modo suavizar / **modo caída**,
todos los tamaños con sliders, cada interruptor, idioma del menú/HUD
(`Español`/`English`), **perfiles de brocha** con nombre y **stamps**. Se
guarda al cerrar el menú.

**Snapshots de terreno** (copias de seguridad) viven en la misma pestaña:
ponle un nombre para guardar todos los tiles editados en
`BepInEx/config/TerrainForge/snapshots/<nombre>.dat`, luego cárgalo después
(incluso en otra partida). La importación restaura con propiedad y es
deshacible; tiles no cargados avisan limpiamente, así que importa cerca del
lugar.

**Stamps (Copiar/Pegar terreno)** — pestaña **STAMPS** en AJUSTES:
- `Ctrl+Shift+C` copia el área marcada (forma actual) al portapapeles (RAM).
- `Ctrl+Shift+V` muestra preview fantasma (wireframe) en la mira; `Q`/`E` rotan,
  `Alt`+rueda rota continuo, `Shift`+rueda escala, click confirma.
- Historial de 10 stamps en RAM: teclas "Siguiente stamp" / "Stamp anterior"
  para ciclar.
- Guardar stamp a disco (`.stamp` en `BepInEx/config/TerrainForge/stamps/`,
  tope 12), cargar al portapapeles, listar y borrar desde la UI.
- **Herramienta stamp**: en AJUSTES → "Herramienta stamp" (4 opciones):
  `Igual a brocha` (default), `Cualquiera`, `Pico`, `Varita TerrainForge`.
  El HUD muestra `STAMP LISTO [2/5]` cuando la tool correcta está equipada y
  hay stamps en el portapapeles; si no, avisa `Stamp: herramienta no equipada`.
  `Ctrl+Shift+C/V` no funcionan sin la tool equipada.

Las formas ciclan: `Círculo`, `Cuadrado`, `Línea`, `Rectángulo`, `Polígono`
(tecla `.`). `Línea`/`Rectángulo` se marcan con dos pulsos de Aplicar (A y B;
el semiancho de la línea = radio de la brocha, las esquinas definen el área
con bordes suaves). `Polígono` suma un vértice por pulso y cierra con doble
pulso (3+ vértices). Todas las operaciones de área aplican sobre estas
huellas con soporte de deshacer.

- **Aplanar** sigue el suelo **bajo la mira** por defecto, así se adapta a lo
  que apuntes (p.ej. una rampa). Mantén el modificador (`Shift` izq.) para
  nivelar a tus pies.
- **Suavizar** promedia cada punto con sus vecinos (un blur real), alisando
  lomos, escalones y ruido. Mantenlo pulsado para suavizar progresivamente.
  Modos: `Completo` (ambos sentidos), `SoloLomos` (solo baja), `SoloHoyos`
  (solo rellena). Forma Línea + Suavizar = suavizado a lo largo de un camino.
- **Rampa** tiene tres modos (`H`): **dos puntos** (marcas **A** y **B**,
  une ambas alturas con pendiente recta), **ángulo fijo** (**B** da dirección
  y largo, la altura sigue `Ángulo rampa`, ajustable con `Alt` + radio), y
  **senda** (marcas una curva **A→B→C…**, doble pulso cierra). El radio de la
  brocha pone el ancho de la rampa y la suavidad solo ablanda los bordes
  laterales. Vista previa: corredor relleno semitransparente + anillos de
  altura en A y B.

### Ayuda in-game

Pulsa **F5** (configurable) — o el botón **TerrainForge** en el menú de pausa
`ESC` — y abre la pestaña **AYUDA** para una descripción de la herramienta y
cada operación. La pestaña **TECLAS** reasigna teclas: haz click en un
atajo, pulsa la tecla o botón nuevo, y se guarda al config al instante.
`Esc` cancela la captura.

**Para asignar combinaciones (ej. `Ctrl+Shift+C`):**
1. Click en el atajo en la pestaña TECLAS.
2. Mantené `Ctrl` + `Shift` **y después** presioná `C`.
3. Las teclas puras de modificación (Ctrl, Shift, Alt) **no se pueden** usar
   como tecla principal — solo como modificadores.
4. También funcionan botones del ratón: `Alt` + `Click derecho` → captura
   `Mouse1` con modificador `LeftAlt`.

Nota: F5 también abre la consola de desarrollador de Valheim, pero solo si
lanzas el juego con `-console`. Si la usas, reasigna el menú a F7/F9.

Todos los atajos también se editan directo en
`BepInEx/config/com.terrainforge.valheim.cfg`.

## Configuración

Archivo: `BepInEx/config/com.terrainforge.valheim.cfg`

- **General**: `Enabled`, `PlayEffects` (polvo + golpe al aplicar),
  `Language` (`Español` / `English`).
- **Brocha**: `Radio`, `RadioMin/Máx`, `PasoRadio`, `Suavidad`,
  `PotenciaCaída`, `MetrosSubirBajar`, `FuerzaPintura`,
  `OperaciónPorDefecto`, `Forma`, `Pintura`,
  `ModoRampa` (`DosPuntos` / `ÁnguloFijo` / `Senda`), `ÁnguloRampa` (5-60°),
  `ModoSuavizar` (`Completo` / `SoloLomos` / `SoloHoyos`), `ModoCaída`
  (`Punta` / `Plano` / `Suave` / `Lineal`), `MargenCosta`,
  `UsarAlturaFija` / `AlturaFija` (estado del cuentagotas).
- **Entrada**: todos los atajos (`ApplyKey`, radio, ciclos, `AimHeightModifierKey`,
  `UndoKey`/`RedoKey`, `EyedropKey`, `RampModeKey`, `RadialKey`, `GiveToolKey`,
  `RebindMenuKey`, **teclas directas por operación**, **ciclar perfil**,
  **ciclar stamp**), más `HerramientaRequerida` (`Cualquiera`, `Pico`, o la
  `TerrainForgeTool` del mod — por defecto) y `DarToolAuto` (entrega la varita
  al entrar, por defecto `true`).
- **Stamps**: `HerramientaStamp` (`Igual a brocha` / `Cualquiera` / `Pico` /
  `Varita TerrainForge` — por defecto `Igual a brocha`), gestionada en
  AJUSTES → "Herramienta stamp".
- **Límites**: `DesbloquearLímiteDelta` (por defecto **true**),
  `DeltaMáxSubida`, `DeltaMáxBajada`. Subir esto permite edits más allá de
  ±8 m vanilla; mantenlos iguales en servidor y clientes.
- **HerramientasVanilla** (opcional / best effort):
  - `EscalarRadioToolVanilla` — agranda el radio del azadón/cultivador vanilla.
    Solo afecta al cliente que lo usa y puede desincronizar a otros; **apagado
    por defecto**.
  - `SinCostoConstrucción` — quita requisitos de mesa y materiales.
  - `PermitirEnZonasProtegidas` — ignora zonas no-construcción y wards.
- **Vista previa**: `MostrarVista`, `MostrarGuías` (guías de alcance, tinte de
  pendiente, anillo de nivel).
- **Historial**: `ProfundidadDeshacer` (0-500, por defecto 50; 0 desactiva el
  historial).
- **Perfiles**: `Slots` (setups de brocha nombrados, se gestionan in-game).
- **Stamps**: guardados en `BepInEx/config/TerrainForge/stamps/` (tope 12,
  se gestionan in-game en pestaña STAMPS).
- **Rendimiento**: `TilesPorFrame` (1-32, por defecto 4 tiles/frame).
- **Servidor** (autoritario cuando el servidor corre el mod — sus valores ganan
  por config sync): `PermitirEditsTerreno` (por defecto `true`; si `false`,
  clientes no pueden aplicar/deshacer/rehacer) y `SoloAdmins` (por defecto
  `false`; solo admins/host pueden editar). `RespetarWards` (por defecto
  `true`) bloquea edits dentro de wards ajenos. `TopeRadio` / `TopeDelta`
  (`0` = off) recortan radio y delta visiblemente (HUD + vista previa). Servidores
  **sin** el mod ignoran esta sección, así que la herramienta sigue sirviendo
  para probar.

## Compilar

Requiere .NET SDK 8+ y las DLLs de referencia del juego + BepInEx.

Las rutas por defecto en el `.csproj` apuntan a **esta máquina** (Steam default +
perfil Thunderstore `Valheim` del usuario actual). En otra máquina **no
funcionarán** tal cual; sobrescribe **todas** las que necesites:

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
- `Deploy=true`: copia el `.dll` a `PluginsDir` (por defecto
  `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\TuPerfil\BepInEx\plugins\TerrainForge`).
  También podés sobrescribir `PluginsDir` si querés otra carpeta de destino.

Si solo querés compilar sin deployar (p.ej. para revisar errores):

```bash
dotnet build -c Release -p:ValheimDir="..." -p:BepInExCoreDir="..." -p:JotunnDir="..."
```

## Notas / limitaciones

- Radio de herramienta vanilla grande es best-effort y puede desincronizar
  otros clientes; la brocha personalizada es el camino fiable para edits
  grandes.
- Mantén `DeltaMáxSubida` / `DeltaMáxBajada` consistentes entre servidor y
  clientes.

## Changelog

Ver [CHANGELOG.md](CHANGELOG.md).