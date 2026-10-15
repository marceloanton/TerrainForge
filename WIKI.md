# TerrainForge — Wiki

### 📥 Instalación
1. **Cliente + Servidor** (requerido en ambos para edits >±8m y replicación).
2. Descarga `TerrainForge-0.3.0.zip` → extrae en `BepInEx/plugins/TerrainForge/`.
3. Dependencias auto: **BepInEx 5** + **Jotunn** (Thunderstore las instala solo).
4. Reinicia juego/servidor.

---

### 🎮 Controles rápidos

| Acción | Tecla default |
|--------|---------------|
| Aplicar brocha | `Mouse1` (clic derecho) |
| Radio ± | `=` / `-` · `Ctrl`+rueda |
| Cambiar operación | `,` |
| Cambiar forma | `.` |
| Cambiar pintura | `/` |
| Nivelar a pies (Flatten) | `Shift` izq. |
| Deshacer / Rehacer | `Ctrl+Z` / `Ctrl+Y` |
| Cuentagotas (fijar altura) | `G` |
| Modo rampa | `H` |
| Ángulo rampa fija | `Alt` + radio/rueda |
| Menú radial | `Q` (mantener) |
| Modo ajuste (rueda = radio) | `V` |
| Ajuste fino (0.1 m) | `Shift` + rueda/radio |
| **Copiar stamp** | `Ctrl+Shift+C` |
| **Pegar stamp** | `Ctrl+Shift+V` |
| Varita TerrainForge | `F8` |
| Menú principal | `F5` |

**Teclas extra** (sin default, asígnalas en `F5 → TECLAS`):
- Operación directa: Aplanar, Subir, Bajar, Suavizar, Pintar, Restaurar, Rampa, Costa, Nieve
- Siguiente/Anterior perfil · Siguiente/Anterior stamp

---

### 🖌️ Operaciones (9)

| Op | Qué hace |
|----|----------|
| **Aplanar** | Nivela a la altura apuntada (Shift = a tus pies). |
| **Subir** | Eleva terreno. |
| **Bajar** | Baja terreno. |
| **Suavizar** | Blur real (promedia vecinos). Modos: Completo / Solo lomos / Solo hoyos. |
| **Rampa** | 3 modos: Dos puntos (A→B), Ángulo fijo (B = dirección/largo, altura = ángulo), Senda (A→B→C… curva Catmull-Rom). |
| **Costa** | Aplana hacia nivel agua + margen (playas). |
| **Pintar** | Dirt / Cultivated / Paved / ClearVegetation / DeepSnow / Reset. |
| **Restaurar** | Vuelve al terreno original. |
| **Nieve** | Pinta nieve (Shift = borra y restaura original). |

---

### 📐 Formas (5)
`Círculo` · `Cuadrado` · `Línea` (2 clicks, semiancho = radio) · `Rectángulo` (2 esquinas) · `Polígono` (1 click/vértice, doble-click cierra).

---

### ⚙️ Modos de caída (Subir/Bajar) — AJUSTES → "Caída Raise/Lower"
| Modo | Efecto |
|------|--------|
| **Punta** | Pico central (original), controlado por "Potencia caída". |
| **Plano** | Disco uniforme. |
| **Suave** | Campana coseno. |
| **Lineal** | Taper recto centro→borde. |

---

### 🧱 Stamps (Copiar/Pegar terreno)

| Paso | Qué hace |
|------|----------|
| 1. Marca área (cualquier forma) | |
| 2. `Ctrl+Shift+C` | Copia al portapapeles RAM ("Stamp copiado: X tiles"). |
| 3. `Ctrl+Shift+V` | Preview fantasma (wireframe) en mira. |
| 4. `Q`/`E` = rotar ±15° · `Alt`+rueda = rotar continuo · `Shift`+rueda = escalar 0.25×–4× | |
| 5. Click (`Mouse1`) | Confirma y pega (1 solo paso de Undo). |

**Historial**: hasta 10 stamps en RAM. Teclas "Siguiente stamp" / "Stamp anterior" para ciclar.

**Persistencia**: `F5 → AJUSTES → STAMPS` → guardar `.stamp` (tope 12), cargar al portapapeles, listar/borrar.

**Herramienta stamp** (AJUSTES → "Herramienta stamp"): `Igual a brocha` (default) / `Cualquiera` / `Pico` / `Varita TerrainForge`.  
HUD muestra `STAMP LISTO [2/5]` si tool equipada + hay stamps; si no, `Stamp: herramienta no equipada`.

---

### 💾 Snapshots (Backups de terreno)
`F5 → AJUSTES → Snapshots` → nombre → **Guardar** (`.dat` en `BepInEx/config/TerrainForge/snapshots/`, tope 12).  
**Cargar** restaura tiles + ownership + deshacible. Tiles no cargados avisan limpiamente.

---

### 🌐 Idiomas (14 + auto-detect)
`Auto` (detecta del juego) · English · Español · Français · Deutsch · Русский · 中文 · 繁體中文 · 日本語 · 한국어 · Polski · Português · Türkçe · Українська · Italiano.  
Default `Auto` → usa reflection para leer `Localization.instance.GetSelectedLanguage()`.

---

### 🖥️ Menú in-game (`F5` o botón en ESC)
3 pestañas con **scroll global** (rueda del ratón):
- **TECLAS**: reasigna (click → presiona tecla/botón; `Esc` cancela).  
  *Combos*: mantené modificadores (Ctrl/Shift/Alt) **y después** la tecla principal.
- **AYUDA**: referencia completa.
- **AJUSTES**: todo editable en vivo (sliders, toggles, ciclos, perfiles, stamps, snapshots, idioma). Guarda al cerrar.

---

### 🌍 Servidor + Cliente
- **Requerido en ambos** para edits >±8m y replicación correcta.
- Sección `Servidor` (autoritaria via config sync):
  - `PermitirEditsTerreno` (default on)
  - `SoloAdmins` (default off)
  - `RespetarWards` (default on)
  - `TopeRadio` / `TopeDelta` (0 = sin límite)
- Servidor **sin** el mod ignora la sección → cliente funciona para test.

---

### ⚡ Rendimiento
`TilesPorFrame` (1–32, default 4): tiles por frame en trazos grandes. Evita lag.

---

### 🐛 Problemas conocidos / Tips
- **Consola F5**: si usás `-console`, reasigná menú a F7/F9 en TECLAS.
- **Cursor no aparece**: forzado visible cada frame al abrir menú; si falla, `Esc` → reabre.
- **Keybinds**: Ctrl/Shift/Alt **no** pueden ser tecla principal; mantenelos y después la tecla. Botones ratón sí funcionan.
- **Preview bajo agua**: flota en superficie automáticamente.
- **Ramp preview**: corredor relleno + anillos altura en A/B.

---

### 🔗 Enlaces
- **Donaciones**: [cafecito.app/byrka](https://cafecito.app/byrka)
- **Issues / PRs**: GitHub del mod
- **Changelog completo**: `CHANGELOG.md` en el zip