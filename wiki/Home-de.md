# TerrainForge Wiki

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · *Deutsch* · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 – Änderungen

> Das Q-Rad hat einen eigenen Kopieren/Einfügen-Modus. Rechteck markiert A/B mit Anwenden; Linie markiert zwei Endpunkte; Polygon fügt Eckpunkte hinzu und kopiert bei Doppeldruck ab 3 Punkten; Kreis/Quadrat kopiert den Pinselbereich. Das HUD zeigt den nächsten Schritt. Schnee ist jetzt Farbe > DeepSnow (Shift halten zum Entfernen). F5 hält lokale Bewegung und Kamera an; ESC pausiert die ganze Einzelspielerwelt.

## Inhaltsverzeichnis

[Overview](Overview) · 1. [Installation](#-installation)
2. [Schnellstart](#-schnellstart)
3. [Steuerung](#-steuerung)
4. [Operationen](#-operationen)
5. [Formen](#-formen)
6. [Abfallmodi](#-abfallmodi-anhebenabsenken)
7. [Stamps (Kopieren/Einfügen)](#-stamps-kopiereneinfügen)
8. [Snapshots (Backups)](#-snapshots-backups)
9. [Profile](#-profile)
10. [Sprachen](#-sprachen)
11. [Mehrspieler](#-mehrspieler)
12. [Einstellungen](#-einstellungen)
13. [Fehlerbehebung](#-fehlerbehebung)

---

## 📥 Installation

[Overview](Overview) · ### Anforderungen
- **BepInEx 5** (x64, Mono) auf Client und Server. Thunderstore-Nutzer haben es bereits.

### Schritte
1. Lade `TerrainForge-<version>.zip` von Thunderstore oder GitHub Releases herunter.
2. Entpacke und platziere `TerrainForge.dll` unter:
   - **Client:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Server:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Starte Spiel/Server neu.

> ⚠️ **Muss auf Client UND Server installiert sein** für Bearbeitungen größer als ±8 m und eine korrekte Replikation.

---

## 🚀 Schnellstart

[Overview](Overview) · 1. Betrete eine Welt. Der **TerrainForge-Stab** (ein Hammer-Klon) wird automatisch übergeben und ausgerüstet.
2. Drücke `F5`, um das Menü zu öffnen (oder klicke im `ESC`-Pausenmenü auf die Schaltfläche **TerrainForge**).
3. Rüste den Stab aus, falls er nicht ausgerüstet ist.
4. Ziele auf das Gelände und halte die **rechte Maustaste** (`Mouse1`) gedrückt, um zu formen.
5. Nutze `,`, um Operationen zu wechseln, `.` für Formen, `/` für Bemalungen.
6. Halte `Q` → Radialmenü, zum Bestätigen loslassen.
7. `Ctrl+Z` / `Ctrl+Y` für Rückgängig/Wiederherstellen.

---

## ⌨️ Steuerung

[Overview](Overview) · | Aktion | Standardtaste | Beschreibung |
|--------|-------------|-------------|
| Pinsel anwenden | `Mouse1` halten (Rechtsklick) | Formt das Gelände unter dem Fadenkreuz |
| Radius + | `=` oder `Ctrl`+Mausrad | Erhöht den Pinselradius |
| Radius − | `-` oder `Ctrl`+Mausrad | Verringert den Pinselradius |
| Operation wechseln | `,` | Wechselt die 9 Operationen durch |
| Form wechseln | `.` | Kreis → Quadrat → Linie → Rechteck → Poly |
| Bemalung wechseln | `/` | Wechselt die 6 Bemalungstypen durch |
| Auf Füße nivellieren | `Left Shift` halten | Einebnen verwendet die Spielerhöhe |
| Rückgängig | `Ctrl+Z` | Macht den letzten Strich rückgängig |
| Wiederherstellen | `Ctrl+Y` | Stellt den rückgängig gemachten Strich wieder her |
| Pipette | `G` | Fixiert die Einebnen-Höhe am Fadenkreuz (erneut = freigeben) |
| Rampenmodus | `H` | Wechselt: Zwei-Punkt → Fester Winkel → Pfad (oder wähle mit 3 Schaltflächen in EINSTELLUNGEN) |
| Fester Rampenwinkel | `Alt` + Radius/Mausrad halten | Passt den Winkel im festen Modus an |
| Radialmenü | `Q` halten | Operation mit der Maus wählen, Loslassen bestätigt |
| Anpassungsmodus | `V` | Mausrad = Radius, friert Zoom/Leiste/Karte ein |
| Feinanpassung (0,1 m) | `Shift` + Mausrad/Radius halten | Präzise Radiusanpassung |
| **Stamp kopieren** | `Ctrl+Shift+C` | Kopiert den markierten Bereich in die Zwischenablage |
| **Stamp einfügen** | `Ctrl+Shift+V` | Geistervorschau, Klick bestätigt |
| Stab holen | `F8` | Übergibt den TerrainForge-Stab |
| Menü öffnen | `F5` | Öffnet das Menü für Tasten/Hilfe/Einstellungen |

**Extra-Tasten (kein Standard, weise sie unter TASTEN zu):**
- Direkte Operation: Einebnen, Anheben, Absenken, Glätten, Bemalen, Wiederherstellen, Rampe, Küste, Schnee
- Nächstes / vorheriges Profil
- Nächster / vorheriger Stamp (Verlauf)

**Bereich „Schnellaktionen“ (TASTEN-Tab):** Schaltflächen für Funktionen ohne obligatorische Tastenkombination — Rückgängig, Wiederherstellen, Höhe fixieren, Stamp kopieren/einfügen, Vorheriger Stamp, Markierungen löschen, die Heatmaps und die Material-Diagnose (schreibt den Shader + die Textur-Slots ins Log).

---

## 🔄 Operationen

[Overview](Overview) · | Operation | Beschreibung |
|-----------|-------------|
| **Einebnen** | Nivelliert auf die angezielte Höhe (Shift = auf deine Füße). Feste Höhe über die Pipette (`G`) oder das EINSTELLUNGEN-Textfeld. |
| **Anheben** | Hebt das Gelände um `RaiseDelta` pro Anwendung. Der Abfallmodus gibt die Form vor. |
| **Absenken** | Senkt das Gelände um `RaiseDelta` pro Anwendung. Der Abfallmodus gibt die Form vor. |
| **Glätten** | Echtes Blur: mittelt jeden Vertex mit seinen 8 Nachbarn. Halten für progressives Glätten. **Modi:** Vollständig (beide), Nur Rücken (senkt), Nur Mulden (hebt). Form Linie = einen Pfad glätten. |
| **Rampe** | **3 Modi** (wechselt mit `H`):<br>• **Zwei-Punkt**: markiere A und B → eine gerade Steigung verbindet die Höhen.<br>• **Fester Winkel**: B = Richtung/Länge; Höhe = `SlopeAngle` (auf/ab je nach Fadenkreuz). Anpassen mit `Alt`+Radius.<br>• **Pfad**: markiere eine Kurve A→B→C… (Doppelklick bei 3+ schließt) → glatte Catmull-Rom.<br>Breite = Pinselradius. Weichheit = Blend nur an den Seitenrändern. Vorschau: gefüllter Korridor + Höhenringe bei A/B. |
| **Küste** | Ebnet in Richtung des Wasserspiegels + `ShoreOffset`-Rand (Strände). Weichheit wie bei Einebnen. |
| **Bemalen** | Typen: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Einstellbare Stärke. |
| **Wiederherstellen** | Setzt das Gelände in seinen Originalzustand zurück (Höhe + Bemalung). |
| **Kopieren/Einfügen** | Eigener Q-Rad-Modus zum Auswählen und Einfügen von Gelände-Stamps. |

---

## 📐 Formen

[Overview](Overview) · Wechsle mit der Taste `.`. Alle unterstützen Rückgängig.

| Form | Markierung | Anwendungsfall |
|-------|---------|----------|
| **Kreis** | Einzelklick (Pinselzentrum) | Allgemeines Formen |
| **Quadrat** | Einzelklick (Zentrum) | Plattformen, Fundamente |
| **Linie** | 2 Klicks (A → B) | Pfade, Mauern, Wege. Halbbreite = Radius. |
| **Rechteck** | 2 Klicks (gegenüberliegende Ecken) | Fundamente, Farmen, Plattformen. Weiche Ränder. |
| **Polygon** | Klick pro Vertex, Doppelklick schließt (3+ Vertices) | Komplexe Formen, eigene Bereiche |

- Nicht-kreisförmige Formen zeigen **nur die markierte Kontur** (keine kreisförmigen Ringe).
- Weichheit beeinflusst den Rand-Blend bei allen Formen.

---

## ⚙️ Abfallmodi (Anheben/Absenken)

[Overview](Overview) · Config in EINSTELLUNGEN → „Raise/Lower falloff“:

| Modus | Verhalten |
|------|----------|
| **Spitz** | Zentrale Spitze. Gesteuert durch den Regler „Falloff power“ (1–5). |
| **Flach** | Gleichmäßige Scheibe — gleiches Gewicht über den gesamten Radius. |
| **Weich** | Kosinus-Glocke — weicher Übergang, keine harte Kante. |
| **Linear** | Gerader Verlauf von Zentrum zu Rand. |

---

## 🧱 Stamps (Kopieren/Einfügen)

[Overview](Overview) · Eine leistungsstarke Funktion zum Replizieren von Geländestrukturen.

### Rechteck-Auswahl (empfohlen)

Wähle Kopieren/Einfügen im Q-Rad. Anwenden markiert die Auswahl: zwei Ecken für Rechteck, zwei Endpunkte für Linie, Eckpunkte für Polygon (ab 3 Punkten doppelt drücken) oder den Pinselbereich für Kreis/Quadrat. Das HUD zeigt den nächsten Schritt.

### Workflow

[Overview](Overview) · 1. **Markiere den Bereich**: Nutze eine beliebige Form (Kreis, Rechteck, Linie, Poly, Rampe) — oder die obige Rechteck-Auswahl.
2. **Kopieren**: `Ctrl+Shift+C` → „Stamp kopiert: X Tiles“ (RAM-Zwischenablage).
3. **Einfüge-Vorschau**: `Ctrl+Shift+V` → Geister-Drahtgitter am Fadenkreuz.
   - `Q`/`E` = um ±15° drehen
   - `Alt`+Mausrad = kontinuierliche Drehung
   - `Shift`+Mausrad = Skalierung 0,25×–4×
   - `Esc` = abbrechen
4. **Confirm**: Drücke zum Bestätigen erneut die Einfügen-Taste.

> Auswahl-Stamps schreiben **nur innerhalb der kopierten Region**; das Gelände außerhalb bleibt unberührt.

### Verlauf
- Bis zu **10 Stamps** im RAM (neueste zuerst).
- Tasten `Nächster Stamp` / `Vorheriger Stamp` (neu belegbar) wechseln sie durch.
- Flash: „Stamp-Verlauf: Copy: 3 Tiles [2/5]“

### Persistenz
- **EINSTELLUNGEN → STAMPS**:
  - Speichere den aktuellen Stamp auf die Festplatte (`.stamp` in `BepInEx/config/TerrainForge/stamps/`, max. 12).
  - Lade einen Stamp in die Zwischenablage.
  - Gespeicherte Stamps auflisten/löschen.

### Stamp-Werkzeug
- EINSTELLUNGEN → „Stamp tool“ (4 Optionen):
  - `Same as brush` (Standard) — verwendet dasselbe Werkzeug wie der Pinsel
  - `Any` — keine Voraussetzung
  - `Pickaxe` — erfordert eine ausgerüstete Spitzhacke
  - `TerrainForge Rod` — erfordert den Stab des Mods
- Das HUD zeigt `STAMP READY [2/5]`, wenn das Werkzeug ausgerüstet ist + Stamps in der Zwischenablage sind.
- Ohne das Werkzeug: `Stamp: tool not equipped`, Kopieren/Einfügen blockiert.

---

## 💾 Snapshots (Backups)

[Overview](Overview) · Vollständiges Gelände-Backup/Restore.

### Speichern
1. EINSTELLUNGEN → **SNAPSHOTS**.
2. Name (leer = automatisches Datum/Uhrzeit).
3. **Speichern** → bearbeitete Tiles nach `BepInEx/config/TerrainForge/snapshots/<name>.dat` (max. 12).

### Laden
1. Wähle einen Snapshot.
2. **Laden** → stellt Tiles inklusive Ownership wieder her, **1 Undo-Schritt**.
3. Nicht geladene Tiles warnen sauber — importiere nahe der ursprünglichen Position.

### Funktionen
- Versioniertes Binärformat (`.dat`).
- Stellt Höhe + Smooth + Bemalung + Ownership wieder her.
- Der Import ist rückgängig machbar (`Ctrl+Z` macht den Import rückgängig).
- Nicht geladene Tiles warnen sauber.

---

## 💾 Profile

[Overview](Overview) · Speichern/Laden vollständiger Pinselkonfigurationen.

- EINSTELLUNGEN → **PROFILES**.
- Name + **Speichern** → Radius, Weichheit, Op, Form, Bemalung, Rampenmodus, Winkel, Glättungsmodus, Raise-Delta, Bemalstärke, Falloff, Shore-Offset.
- **Laden** → wird sofort angewendet.
- Max. 12 Profile (kulturinvariantes Format in `Presets > Slots`).
- Tastenbelegung: `Next Profile` (unter TASTEN zuweisen) zum Durchwechseln.

---

## 🌐 Sprachen

[Overview](Overview) · **15 Optionen:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — Standard `Auto`.

### Auto-Erkennung
- Der Standard `Auto` erkennt die Spielsprache via Reflection: `Localization.instance.GetSelectedLanguage()`.
- Ordnet die Valheim-Sprache → internem Index zu.
- Hat eine Sprache keine vollständige Übersetzung → Fallback auf English.
- Konfigurierbar in EINSTELLUNGEN → „Language“ (wechselt die 15 Optionen durch).
- Tastenlabels werden beim Öffnen des Menüs neu aufgebaut.

### Übersetzungsstatus
- **Vollständig:** English, Español
- **English-Fallback:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Community-Übersetzungen willkommen via PR.

### Diese Dokumentation
Übersetzte Wiki-Seiten: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Mehrspieler

[Overview](Overview) · ### Anforderungen
- **Mod auf Client UND Server** für Bearbeitungen größer als ±8 m und eine korrekte Replikation.
- `UnlockDeltaLimit` auch auf dem Server, sonst begrenzt der Server das Gelände.

### Server-Autorität (Config Sync)
Wenn der Server den Mod ausführt, gewinnt seine `Server`-Sektion:
- `AllowTerrainEdits` (Standard `true`) — Hauptschalter; wenn `false`, können Clients nicht anwenden/rückgängig machen/wiederherstellen.
- `AdminOnlyEdits` (Standard `false`) — nur Admins/Host können bearbeiten.
- `RespectWards` (Standard `true`) — blockiert Bearbeitungen innerhalb fremder Wards.
- `RadiusCap` / `DeltaCap` (0 = aus) — vom Server erzwungene Caps, sichtbar in HUD+Vorschau.

> Server **ohne** den Mod ignorieren die Sektion → der Client funktioniert zum Testen.

### Desync-Vorbeugung
- Halte `MaxUpDelta` / `MaxDownDelta` auf Server + Clients konsistent.
- Große Radien können Clients ohne den Mod desynchronisieren.

---

## ⚙️ Einstellungen

[Overview](Overview) · Datei: `BepInEx/config/com.terrainforge.valheim.cfg`

### Allgemein
| Einstellung | Standard | Beschreibung |
|---------|---------|-------------|
| `Enabled` | `true` | Hauptschalter |
| `PlayEffects` | `true` | Staub + Treffer beim Anwenden |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Färbt den TerrainForge-Stab (Hammer-Klon) ein, damit er leicht vom Vanilla-Hammer zu unterscheiden ist |
| `ToolTintColor` | `#4FC3F7` | Hex-Farbe des Stabs (Modell + Icon). Z. B. `#FFC400` Gold, `#8BC34A` Grün |

### Pinsel
| Einstellung | Standard | Bereich | Beschreibung |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Pinselradius (m) |
| `RadiusMin` | 2 | — | Mindestradius beim Anpassen |
| `RadiusMax` | 60 | — | Höchstradius beim Anpassen |
| `RadiusStep` | 1 | — | Radiusänderung pro Taste/Mausrad |
| `Softness` | 50 | 0–100 | Rand-Weichheit (0 = hart, 100 = sehr weich) |
| `FalloffPower` | 3 | 1–5 | Abfall-Exponent (nur Spitz-Modus) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Meter pro Anheben/Absenken-Anwendung |
| `PaintStrength` | 1 | 0–1 | Bemalstärke |
| `DefaultOperation` | Flatten | — | Startoperation |
| `Shape` | Circle | — | Startform |
| `Paint` | Dirt | — | Startbemalung |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Fester Rampenwinkel (Grad) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Küstenrand über dem Wasser (m) |
| `UseFixedHeight` | false | — | Feste Höhe verwenden (Pipetten-Zustand) |
| `FixedHeight` | 30 | -10–150 | Exakte Einebnen-Höhe (m) |

### Vanilla / Integration
- `BlockVanillaTerrainTools`: false — mit ausgerüstetem Stab wird die Geländebearbeitung der Vanilla-Hacke/des Kultivators ignoriert. Es gibt auch einen Umschalter in EINSTELLUNGEN.

### Eingabe
Alle Tastenkürzel sind im TASTEN-Tab neu belegbar. Highlights:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Direkte Op-Tasten** (9, kein Standard): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (Standard)
- `AutoGiveTool`: true (übergibt den Stab beim Beitreten)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (Standard Same as Brush)

### Vorschau
- `ShowPreview`: true, `ShowGuides`: true (Reichweitenhilfen, Steigungs-Tönung, Niveau-Ring)

### Verlauf
- `UndoDepth`: 50 (0–500, 0 deaktiviert den Verlauf)

### Server (Autoritativ via Config Sync)
- `AllowTerrainEdits`: true (Hauptschalter)
- `AdminOnlyEdits`: false (nur Admins)
- `RespectWards`: true (blockiert fremde Wards)
- `RadiusCap` / `DeltaCap`: 0 (aus) — vom Server erzwungene Caps

### Leistung
- `TilesPerFrame`: 4 (1–32 Tiles/Frame)

---

## 🐛 Fehlerbehebung

[Overview](Overview) · | Problem | Lösung |
|-------|----------|
| **Menü erscheint nicht / unsichtbarer Cursor** | Schließe das Spiel. Lösche `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\DeinProfil\BepInEx\plugins\TerrainForge\` und `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Starte TMM neu, prüfe Updates. |
| **Update erscheint nicht in TMM** | TMM → Settings → „Refresh mod list“ (Ctrl+R). TMM schließen/neu öffnen. Prüfe das richtige Profil. |
| **Thunderstore-Versionskonflikt** | Erhöhe die Version (0.3.3 → 0.3.4) in `.csproj`, `Plugin.cs`, `manifest.json`. Neu bauen + neu hochladen. |
| **Stamp Kopieren/Einfügen tut nichts** | HUD: `STAMP READY [X/Y]` = bereit. Bei „tool not equipped“ rüste das Werkzeug aus (EINSTELLUNGEN → Stamp tool). |
| **Vorschau unsichtbar unter Wasser** | Behoben in 0.3.2 — die Vorschau schwebt auf der Oberfläche. Aktualisiere. |
| **Kreise bei Linie/Rechteck/Poly** | Behoben in 0.3.2 — nur die markierte Kontur wird angezeigt. Aktualisiere. |
| **Radial-Mausrad funktioniert nicht** | Behoben in 0.3.2 (`ZInput.GetMouseScrollWheel()`) und 0.3.4 (die Auswahl wird bei zentriertem Cursor nicht mehr gelöscht). Aktualisiere. |
| **Config wird nicht gespeichert** | Schließe das Menü (F5) zum Auto-Speichern. Oder bearbeite `BepInEx/config/com.terrainforge.valheim.cfg` direkt. |
| **Mehrspieler-Desync** | Mod auf Client+Server. `UnlockDeltaLimit` auf beiden. Gleiche `MaxUpDelta`/`MaxDownDelta`. |
| **Build schlägt fehl** | Prüfe die Pfade `ValheimDir`, `BepInExCoreDir` in `.csproj` oder via CLI. .NET SDK 8+ erforderlich. |
| **Kompilieren / Paketieren** | `dotnet build -c Release` (DLL in `build/`), `-p:Deploy=true` (installiert ins Profil) oder `-p:Package=true` (Zip in `build/`). |

---

## 🔗 Links

[Overview](Overview) · - **Thunderstore-Seite:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub-Repository:** https://github.com/marceloanton/TerrainForge
- **Spenden:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [Changelog](Changelog)

---

*TerrainForge — Terraforme Valheim ohne Grenzen.*
