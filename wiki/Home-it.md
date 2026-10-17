# Wiki di TerrainForge

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · *Italiano*

---

## Indice

[Overview](Overview) · 1. [Installazione](#-installazione)
2. [Avvio Rapido](#-avvio-rapido)
3. [Comandi](#-comandi)
4. [Operazioni](#-operazioni)
5. [Forme](#-forme)
6. [Modalità di Caduta](#-modalità-di-caduta-alzaabbassa)
7. [Stamps (Copia/Incolla)](#-stamps-copiaincolla)
8. [Snapshots (Backup)](#-snapshots-backup)
9. [Profili](#-profili)
10. [Lingue](#-lingue)
11. [Multigiocatore](#-multigiocatore)
12. [Impostazioni](#-impostazioni)
13. [Risoluzione dei Problemi](#-risoluzione-dei-problemi)

---

## 📥 Installazione

[Overview](Overview) · ### Requisiti
- **BepInEx 5** (x64, Mono) su client e server. Gli utenti Thunderstore ce l'hanno già.
- **Jotunn** (`ValheimModding-Jotunn`) — registra lo strumento personalizzato. È elencato in `manifest.json`; Thunderstore lo installa automaticamente.

### Passaggi
1. Scarica `TerrainForge-<version>.zip` da Thunderstore o GitHub Releases.
2. Estrai e colloca `TerrainForge.dll` in:
   - **Client:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Server:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Riavvia il gioco / il server.

> ⚠️ **Deve essere installato su client E server** per modifiche superiori a ±8 m e una corretta replicazione.

---

## 🚀 Avvio Rapido

[Overview](Overview) · 1. Entra in un mondo. La **Bacchetta TerrainForge** (un clone del Martello) viene fornita ed equipaggiata automaticamente.
2. Premi `F5` per aprire il menu (o clicca il pulsante **TerrainForge** nel menu di pausa `ESC`).
3. Equipaggia la bacchetta se non lo è.
4. Mira al terreno e tieni premuto il **clic destro** (`Mouse1`) per scolpire.
5. Usa `,` per scorrere le operazioni, `.` per le forme, `/` per le pitture.
6. Tieni premuto `Q` → menu radiale, rilascia per confermare.
7. `Ctrl+Z` / `Ctrl+Y` per annullare/ripetere.

---

## ⌨️ Comandi

[Overview](Overview) · | Azione | Tasto predefinito | Descrizione |
|--------|-------------------|-------------|
| Applica pennello | Tieni premuto `Mouse1` (clic destro) | Scolpisce il terreno sotto il mirino |
| Raggio + | `=` o `Ctrl`+Rotellina | Aumenta il raggio del pennello |
| Raggio − | `-` o `Ctrl`+Rotellina | Riduce il raggio del pennello |
| Cambia operazione | `,` | Scorre le 9 operazioni |
| Cambia forma | `.` | Cerchio → Quadrato → Linea → Rett → Poli |
| Cambia pittura | `/` | Scorre i 6 tipi di pittura |
| Livella ai piedi | Tieni premuto `Left Shift` | Appiattisci usa l'altezza del giocatore |
| Annulla | `Ctrl+Z` | Annulla l'ultimo tratto |
| Ripeti | `Ctrl+Y` | Ripete il tratto annullato |
| Contagocce | `G` | Fissa l'altezza di Appiattisci sul mirino (di nuovo = rilascia) |
| Modalità rampa | `H` | Scorre: Due Punti → Angolo Fisso → Percorso (o scegli con 3 pulsanti in IMPOSTAZIONI) |
| Angolo rampa fissa | Tieni premuto `Alt` + raggio/rotellina | Regola l'angolo in modalità fissa |
| Menu radiale | Tieni premuto `Q` | Scegli un'operazione con il mouse, rilascia per confermare |
| Modalità regolazione | `V` | Rotellina = raggio, blocca zoom/barra/mappa |
| Regolazione fine (0,1 m) | Tieni premuto `Shift` + rotellina/raggio | Regolazione precisa del raggio |
| **Copia Stamp** | `Ctrl+Shift+C` | Copia l'area marcata negli appunti |
| **Incolla Stamp** | `Ctrl+Shift+V` | Anteprima fantasma, il clic conferma |
| Ottieni Bacchetta | `F8` | Fornisce la Bacchetta TerrainForge |
| Apri menu | `F5` | Apre il menu di tasti/aiuto/impostazioni |

**Tasti extra (nessun predefinito, assegnali in TASTI):**
- Operazione diretta: Appiattisci, Alza, Abbassa, Leviga, Pittura, Ripristina, Rampa, Costa, Neve
- Profilo successivo / precedente
- Stamp successivo / precedente (cronologia)

**Sezione "Azioni rapide" (scheda TASTI):** pulsanti per funzioni senza scorciatoia obbligatoria — Annulla, Ripeti, Fissa altezza, Copia/Incolla stamp, Stamp precedente, Cancella marcature, le mappe di calore e la Diagnostica materiale (scarica lo shader + gli slot delle texture nel log).

---

## 🔄 Operazioni

[Overview](Overview) · | Operazione | Descrizione |
|-----------|-------------|
| **Appiattisci** | Livella all'altezza mirata (Shift = ai tuoi piedi). Altezza fissa tramite il Contagocce (`G`) o la casella di testo in IMPOSTAZIONI. |
| **Alza** | Alza il terreno di `RaiseDelta` per applicazione. La modalità di caduta ne definisce la forma. |
| **Abbassa** | Abbassa il terreno di `RaiseDelta` per applicazione. La modalità di caduta ne definisce la forma. |
| **Leviga** | Vero blur: media ogni vertice con i suoi 8 vicini. Tieni premuto per progressivo. **Modalità:** Completa (entrambi), Solo Creste (abbassa), Solo Avvallamenti (alza). Forma Linea = leviga un percorso. |
| **Rampa** | **3 modalità** (scorri `H`):<br>• **Due Punti**: marca A e B → una pendenza retta unisce le altezze.<br>• **Angolo Fisso**: B = direzione/lunghezza; altezza = `SlopeAngle` (su/giù in base al mirino). Regola con `Alt`+raggio.<br>• **Percorso**: marca una curva A→B→C… (doppio clic con 3+ chiude) → Catmull-Rom levigato.<br>Larghezza = raggio del pennello. Morbidezza = blend solo sui bordi laterali. Anteprima: corridoio riempito + anelli di altezza su A/B. |
| **Costa** | Appiattisce verso il livello dell'acqua + margine `ShoreOffset` (spiagge). Morbidezza come Appiattisci. |
| **Pittura** | Tipi: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Intensità regolabile. |
| **Ripristina** | Riporta il terreno al suo stato originale (altezza + pittura). |
| **Neve** | Pittura solo neve (stessa maschera del gioco). `Shift` = la rimuove e ripristina l'originale. |

---

## 📐 Forme

[Overview](Overview) · Scorri con il tasto `.`. Tutte supportano l'annullamento.

| Forma | Marcatura | Caso d'uso |
|-------|---------|----------|
| **Cerchio** | Clic singolo (centro del pennello) | Scolpitura generale |
| **Quadrato** | Clic singolo (centro) | Piattaforme, fondazioni |
| **Linea** | 2 clic (A → B) | Sentieri, muri, piste. Semilarghezza = raggio. |
| **Rettangolo** | 2 clic (angoli opposti) | Fondazioni, fattorie, piattaforme. Bordi morbidi. |
| **Poligono** | Clic per vertice, doppio clic chiude (3+ vertici) | Forme complesse, aree personalizzate |

- Le forme non circolari mostrano **solo il contorno marcato** (nessun anello circolare).
- La morbidezza influisce sul blend dei bordi su tutte le forme.

---

## ⚙️ Modalità di Caduta (Alza/Abbassa)

[Overview](Overview) · Config in IMPOSTAZIONI → "Caduta Raise/Lower":

| Modalità | Comportamento |
|------|----------|
| **Appuntita** | Picco centrale. Controllata dallo slider "Potenza caduta" (1–5). |
| **Piatta** | Disco uniforme — stesso peso su tutto il raggio. |
| **Morbida** | Campana coseno — transizione fluida, senza bordo netto. |
| **Lineare** | Rastremazione retta dal centro al bordo. |

---

## 🧱 Stamps (Copia/Incolla)

[Overview](Overview) · Una funzione potente per replicare strutture del terreno.

### Selezione rettangolare (consigliata)

[Overview](Overview) · 1. Imposta la forma su `Rectangle` (tasto `.`).
2. Premi Applica (`Mouse1`) **una volta** per marcare l'angolo **A** (questo **non** apporta alcuna modifica).
3. Mira all'angolo **B** (basta muovere la telecamera).
4. Premi `Ctrl+Shift+C` → copia esattamente quel rettangolo, **ritagliato alla selezione** (non trascina interi tile da 64 m né il terreno non toccato attorno). La marcatura dell'angolo A viene consumata dopo una copia riuscita.

### Flusso di lavoro

[Overview](Overview) · 1. **Marca l'area**: usa una qualsiasi forma (Cerchio, Rett, Linea, Poli, Rampa) — o la selezione rettangolare qui sopra.
2. **Copia**: `Ctrl+Shift+C` → "Stamp copiato: X tile" (appunti in RAM).
3. **Anteprima Incolla**: `Ctrl+Shift+V` → wireframe fantasma sul mirino.
   - `Q`/`E` = ruota ±15°
   - `Alt`+Rotellina = rotazione continua
   - `Shift`+Rotellina = scala 0,25×–4×
   - `Esc` = annulla
4. **Conferma**: clic (`Mouse1`) → applica come **un singolo passo di Annulla**.

> Gli stamp di selezione scrivono **solo dentro la regione copiata**; il terreno esterno resta intatto.

### Cronologia
- Fino a **10 stamp** in RAM (più recente per primo).
- Tasti `Next stamp` / `Previous stamp` (riassegnabili) per scorrerli.
- Flash: "Cronologia stamp: Copy: 3 tile [2/5]"

### Persistenza
- **IMPOSTAZIONI → STAMPS**:
  - Salva lo stamp corrente su disco (`.stamp` in `BepInEx/config/TerrainForge/stamps/`, max 12).
  - Carica uno stamp negli appunti.
  - Elenca/elimina gli stamp salvati.

### Strumento stamp
- IMPOSTAZIONI → "Stamp tool" (4 opzioni):
  - `Same as brush` (predefinito) — usa lo stesso strumento del pennello
  - `Any` — nessun requisito
  - `Pickaxe` — richiede un piccone equipaggiato
  - `TerrainForge Rod` — richiede la bacchetta della mod
- L'HUD mostra `STAMP READY [2/5]` quando lo strumento è equipaggiato + ci sono stamp negli appunti.
- Senza lo strumento: `Stamp: tool not equipped`, copia/incolla bloccati.

---

## 💾 Snapshots (Backup)

[Overview](Overview) · Backup/ripristino completo del terreno.

### Salva
1. IMPOSTAZIONI → **SNAPSHOTS**.
2. Nome (vuoto = data/ora automatica).
3. **Salva** → tile modificati in `BepInEx/config/TerrainForge/snapshots/<name>.dat` (max 12).

### Carica
1. Seleziona uno snapshot.
2. **Carica** → ripristina i tile con ownership, **1 passo di annulla**.
3. I tile non caricati avvisano in modo pulito — importa vicino alla posizione originale.

### Caratteristiche
- Formato binario versionato (`.dat`).
- Ripristina altezza + smooth + pittura + ownership.
- L'importazione è annullabile (`Ctrl+Z` annulla l'importazione).
- I tile non caricati avvisano in modo pulito.

---

## 💾 Profili

[Overview](Overview) · Salva/carica configurazioni complete del pennello.

- IMPOSTAZIONI → **PROFILES**.
- Nome + **Salva** → raggio, morbidezza, op, forma, pittura, modalità rampa, angolo, modalità levigatura, raise delta, intensità pittura, falloff, shore offset.
- **Carica** → applica immediatamente.
- Max 12 profili (formato culture-invariant in `Presets > Slots`).
- Scorciatoia: `Next Profile` (assegna in TASTI) per scorrere.

---

## 🌐 Lingue

[Overview](Overview) · **15 opzioni:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — predefinito `Auto`.

### Auto-Detect
- Il default `Auto` rileva la lingua del gioco tramite reflection: `Localization.instance.GetSelectedLanguage()`.
- Mappa la lingua di Valheim → indice interno.
- Se una lingua non ha una traduzione completa → fallback all'inglese.
- Configurabile in IMPOSTAZIONI → "Language" (scorre le 15 opzioni).
- Le etichette dei tasti vengono ricostruite all'apertura del menu.

### Stato delle traduzioni
- **Complete:** English, Español
- **Fallback all'inglese:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Traduzioni della community benvenute via PR.

### Questa documentazione
Pagine wiki tradotte: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Multigiocatore

[Overview](Overview) · ### Requisiti
- **Mod su client E server** per modifiche superiori a ±8 m e una corretta replicazione.
- `UnlockDeltaLimit` anche sul server, altrimenti il server limita il terreno.

### Autorità del Server (Config Sync)
Quando il server esegue la mod, la sua sezione `Server` prevale:
- `AllowTerrainEdits` (default `true`) — interruttore principale; se `false`, i client non possono applicare/annullare/ripetere.
- `AdminOnlyEdits` (default `false`) — solo admin/host possono modificare.
- `RespectWards` (default `true`) — blocca le modifiche dentro ward altrui.
- `RadiusCap` / `DeltaCap` (0 = off) — limiti imposti dal server, visibili in HUD+anteprima.

> I server **senza** la mod ignorano la sezione → il client funziona per i test.

### Prevenzione del Desync
- Mantieni `MaxUpDelta` / `MaxDownDelta` coerenti su server + client.
- Raggi grandi possono desincronizzare i client senza la mod.

---

## ⚙️ Impostazioni

[Overview](Overview) · File: `BepInEx/config/com.terrainforge.valheim.cfg`

### Generale
| Impostazione | Default | Descrizione |
|---------|---------|-------------|
| `Enabled` | `true` | Interruttore principale |
| `PlayEffects` | `true` | Polvere + colpo all'applicazione |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Colora la Bacchetta TerrainForge (clone del Martello) per distinguerla facilmente dal Martello vanilla |
| `ToolTintColor` | `#4FC3F7` | Colore hex della bacchetta (modello + icona). Es.: `#FFC400` oro, `#8BC34A` verde |

### Pennello
| Impostazione | Default | Intervallo | Descrizione |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Raggio del pennello (m) |
| `RadiusMin` | 2 | — | Raggio minimo durante la regolazione |
| `RadiusMax` | 60 | — | Raggio massimo durante la regolazione |
| `RadiusStep` | 1 | — | Variazione di raggio per tasto/rotellina |
| `Softness` | 50 | 0–100 | Morbidezza dei bordi (0 = duro, 100 = molto morbido) |
| `FalloffPower` | 3 | 1–5 | Esponente di caduta (solo modalità Appuntita) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Metri per applicazione di Alza/Abbassa |
| `PaintStrength` | 1 | 0–1 | Intensità della pittura |
| `DefaultOperation` | Flatten | — | Operazione iniziale |
| `Shape` | Circle | — | Forma iniziale |
| `Paint` | Dirt | — | Pittura iniziale |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Angolo della rampa fissa (gradi) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Margine della costa sopra l'acqua (m) |
| `UseFixedHeight` | false | — | Usa un'altezza fissa (stato del contagocce) |
| `FixedHeight` | 30 | -10–150 | Altezza esatta di Appiattisci (m) |

### Vanilla / integrazione
- `BlockVanillaTerrainTools`: false — con la bacchetta equipaggiata, ignora la modifica del terreno di zappa/coltivatore vanilla. C'è anche un interruttore in IMPOSTAZIONI.

### Input
Tutte le scorciatoie sono riassegnabili nella scheda TASTI. In evidenza:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Tasti diretti op** (9, nessun default): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (default)
- `AutoGiveTool`: true (fornisce la bacchetta all'ingresso)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (default Same as Brush)

### Anteprima
- `ShowPreview`: true, `ShowGuides`: true (guide di portata, tinta della pendenza, anello di livello)

### Cronologia
- `UndoDepth`: 50 (0–500, 0 disattiva la cronologia)

### Server (Autoritativo via Config Sync)
- `AllowTerrainEdits`: true (interruttore principale)
- `AdminOnlyEdits`: false (solo admin)
- `RespectWards`: true (blocca ward altrui)
- `RadiusCap` / `DeltaCap`: 0 (off) — limiti imposti dal server

### Prestazioni
- `TilesPerFrame`: 4 (1–32 tile/frame)

---

## 🐛 Risoluzione dei Problemi

[Overview](Overview) · | Problema | Soluzione |
|----------|----------|
| **Il menu non appare / cursore invisibile** | Chiudi il gioco. Elimina `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` e `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Riavvia TMM, controlla gli aggiornamenti. |
| **L'aggiornamento non compare in TMM** | TMM → Settings → "Refresh mod list" (Ctrl+R). Chiudi/riapri TMM. Verifica il profilo corretto. |
| **Conflitto di versione su Thunderstore** | Incrementa la versione (0.3.3 → 0.3.4) in `.csproj`, `Plugin.cs`, `manifest.json`. Ricompila + ricarica. |
| **Copia/incolla stamp non fa nulla** | HUD: `STAMP READY [X/Y]` = pronto. Se dice "tool not equipped", equipaggia lo strumento (IMPOSTAZIONI → Stamp tool). |
| **Anteprima invisibile sott'acqua** | Corretto in 0.3.2 — l'anteprima galleggia in superficie. Aggiorna. |
| **Cerchi su Linea/Rett/Poli** | Corretto in 0.3.2 — viene mostrato solo il contorno marcato. Aggiorna. |
| **La rotellina radiale non funziona** | Corretto in 0.3.2 (`ZInput.GetMouseScrollWheel()`) e 0.3.4 (la selezione non viene più cancellata con il cursore centrato). Aggiorna. |
| **La config non si salva** | Chiudi il menu (F5) per il salvataggio automatico. Oppure modifica direttamente `BepInEx/config/com.terrainforge.valheim.cfg`. |
| **Desync in multigiocatore** | Mod su client+server. `UnlockDeltaLimit` su entrambi. Stessi `MaxUpDelta`/`MaxDownDelta`. |
| **La build fallisce** | Controlla i percorsi `ValheimDir`, `BepInExCoreDir`, `JotunnDir` in `.csproj` o tramite CLI. Richiesto .NET SDK 8+. |
| **Compila / pacchetta** | `dotnet build -c Release` (DLL in `build/`), `-p:Deploy=true` (installa nel profilo) o `-p:Package=true` (zip in `build/`). |

---

## 🔗 Link

[Overview](Overview) · - **Pagina Thunderstore:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **Repository GitHub:** https://github.com/marceloanton/TerrainForge
- **Donazioni:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [Changelog](Changelog)

---

*TerrainForge — Trasforma Valheim senza limiti.*
