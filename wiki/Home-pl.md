# Wiki TerrainForge

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · *Polski* · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it) · [Website](https://marceloanton.github.io/TerrainForge/)

---


### TerrainForge 0.3.8 — poprawka do 0.3.7

> Koło Q ma osobny tryb Kopiuj/Wklej. Prostokąt wyznacza A/B klawiszem Zastosuj; Linia dwa końce; Wielokąt dodaje wierzchołki i kopiuje po dwukrotnym naciśnięciu przy 3+ punktach; Koło/Kwadrat kopiuje obszar pędzla. HUD pokazuje następny krok. Śnieg jest w Maluj > DeepSnow (przytrzymaj Shift, aby usunąć). F5 blokuje lokalny ruch i kamerę; Esc pauzuje cały świat w trybie jednoosobowym. 0.3.6: F5 układa karty w dwóch rzędach i pokazuje ustawienia wybranej operacji. Ramp udostępnia tryby Dwa punkty, Stały kąt i Połączona ścieżka; rampy kołowe są planowane na 0.4.0.

## Spis treści

1. [Instalacja](#-instalacja)
2. [Szybki start](#-szybki-start)
3. [Sterowanie](#-sterowanie)
4. [Operacje](#-operacje)
5. [Kształty](#-kształty)
6. [Tryby opadania](#-tryby-opadania-podnoszenieobniżanie)
7. [Stemple (Kopiuj/Wklej)](#-stemple-kopiujwklej)
8. [Migawki (Backupy)](#-migawki-backupy)
9. [Profile](#-profile)
10. [Języki](#-języki)
11. [Wieloosobowość](#-wieloosobowość)
12. [Ustawienia](#-ustawienia)
13. [Rozwiązywanie problemów](#-rozwiązywanie-problemów)

---

## 📥 Instalacja

### Wymagania
- **BepInEx 5** (x64, Mono) na kliencie i serwerze. Użytkownicy Thunderstore już go mają.

### Kroki
1. Pobierz `TerrainForge-<version>.zip` z Thunderstore lub GitHub Releases.
2. Rozpakuj i umieść `TerrainForge.dll` w:
   - **Klient:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Serwer:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Uruchom ponownie grę / serwer.

> ⚠️ **Musi być zainstalowany na kliencie ORAZ serwerze** dla edycji większych niż ±8 m i poprawnej replikacji.

---

## 🚀 Szybki start

1. Dołącz do świata. **Różdżka TerrainForge** (klon Młota) zostaje automatycznie przyznana i założona.
2. Naciśnij `F5`, aby otworzyć menu (lub kliknij przycisk **TerrainForge** w menu pauzy `ESC`).
3. Załóż różdżkę, jeśli nie jest założona.
4. Wyceluj w teren i przytrzymaj **prawy przycisk myszy** (`Mouse1`), aby rzeźbić.
5. Użyj `,`, aby przełączać operacje, `.` dla kształtów, `/` dla farb.
6. Przytrzymaj `Q` → menu radialne, puść, aby potwierdzić.
7. `Ctrl+Z` / `Ctrl+Y`, aby cofnąć/ponowić.

---

## ⌨️ Sterowanie

| Akcja | Domyślny klawisz | Opis |
|-------|------------------|------|
| Zastosuj pędzel | Przytrzymaj `Mouse1` (prawy przycisk myszy) | Rzeźbi teren pod celownikiem |
| Promień + | `=` lub `Ctrl`+Rolka | Zwiększa promień pędzla |
| Promień − | `-` lub `Ctrl`+Rolka | Zmniejsza promień pędzla |
| Przełącz operację | `,` | Przełącza 9 operacji |
| Przełącz kształt | `.` | Koło → Kwadrat → Linia → Prostokąt → Wielokąt |
| Przełącz farbę | `/` | Przełącza 6 typów farb |
| Wyrównaj do stóp | Przytrzymaj `Left Shift` | Wyrównywanie używa wysokości gracza |
| Cofnij | `Ctrl+Z` | Cofa ostatnie pociągnięcie |
| Ponów | `Ctrl+Y` | Ponawia cofnięte pociągnięcie |
| Kroplomierz | `G` | Ustala wysokość Wyrównywania w miejscu celownika (ponownie = zwolnij) |
| Tryb rampy | `H` | Przełącza: Dwa punkty → Stały kąt → Ścieżka (lub wybierz 3 przyciskami w USTAWIENIACH) |
| Stały kąt rampy | Przytrzymaj `Alt` + promień/rolka | Dostosowuje kąt w trybie stałym |
| Menu radialne | Przytrzymaj `Q` | Wybierz operację myszą, puszczenie potwierdza |
| Tryb dostosowania | `V` | Rolka = promień, blokuje zoom/pasek/mapę |
| Precyzyjna regulacja (0.1 m) | Przytrzymaj `Shift` + rolka/promień | Dokładna regulacja promienia |
| **Kopiuj stempel** | `Ctrl+Shift+C` | Kopiuje zaznaczony obszar do schowka |
| **Wklej stempel** | `Ctrl+Shift+V` | Widmowy podgląd, kliknięcie potwierdza |
| Pobierz różdżkę | `F8` | Przyznaje Różdżkę TerrainForge |
| Otwórz menu | `F5` | Otwiera menu klawiszy/pomocy/ustawień |

**Dodatkowe klawisze (brak domyślnych, przypisz je w KLAWISZACH):**
- Operacja bezpośrednia: Wyrównaj, Podnieś, Obniż, Wygładź, Maluj, Przywróć, Rampa, Brzeg, Śnieg
- Następny / poprzedni profil
- Następny / poprzedni stempel (historia)

**Sekcja "Szybkie akcje" (zakładka KLAWISZE):** przyciski dla funkcji bez obowiązkowego skrótu — Cofnij, Ponów, Ustal wysokość, Kopiuj/Wklej stempel, Poprzedni stempel, Wyczyść znaczniki, mapy cieplne i Diagnostyka materiału (zrzuca shader + sloty tekstur do loga).

---

## 🔄 Operacje

| Operacja | Opis |
|----------|------|
| **Wyrównaj** | Wyrównuje do wskazanej wysokości (Shift = na wysokości twoich stóp). Stała wysokość przez Kroplomierz (`G`) lub pole tekstowe USTAWIEŃ. |
| **Podnieś** | Podnosi teren o `RaiseDelta` na zastosowanie. Tryb opadania nadaje mu kształt. |
| **Obniż** | Obniża teren o `RaiseDelta` na zastosowanie. Tryb opadania nadaje mu kształt. |
| **Wygładź** | Prawdziwe rozmycie: uśrednia każdy wierzchołek z jego 8 sąsiadami. Przytrzymaj dla efektu progresywnego. **Tryby:** Pełny (oba), Tylko grzbiety (obniża), Tylko zagłębienia (podnosi). Kształt Linia = wygładza ścieżkę. |
| **Rampa** | **3 tryby** (przełącz `H`):<br>• **Dwa punkty**: zaznacz A i B → prosta pochyłość łączy wysokości.<br>• **Stały kąt**: B = kierunek/długość; wysokość = `SlopeAngle` (w górę/w dół zależnie od celownika). Dostosuj przez `Alt`+promień.<br>• **Ścieżka**: zaznacz krzywą A→B→C… (podwójne naciśnięcie przy 3+ zamyka) → gładka Catmull-Rom.<br>Szerokość = promień pędzla. Miękkość = blend tylko na bocznych krawędziach. Podgląd: wypełniony korytarz + pierścienie wysokości w A/B. |
| **Brzeg** | Wyrównuje w stronę poziomu wody + margines `ShoreOffset` (plaże). Miękkość jak Wyrównywanie. |
| **Maluj** | Typy: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Regulowana siła. |
| **Przywróć** | Przywraca teren do stanu pierwotnego (wysokość + farba). |
| **Kopiuj/Wklej** | Osobny tryb koła Q do zaznaczania terenu i umieszczania stampów. |

---

## 📐 Kształty

Przełączaj klawiszem `.`. Wszystkie obsługują cofanie.

| Kształt | Zaznaczanie | Zastosowanie |
|---------|-------------|--------------|
| **Koło** | Pojedyncze kliknięcie (środek pędzla) | Ogólne rzeźbienie |
| **Kwadrat** | Pojedyncze kliknięcie (środek) | Platformy, fundamenty |
| **Linia** | 2 kliknięcia (A → B) | Ścieżki, mury, szlaki. Połowa szerokości = promień. |
| **Prostokąt** | 2 kliknięcia (przeciwległe rogi) | Fundamenty, farmy, platformy. Miękkie krawędzie. |
| **Wielokąt** | Kliknięcie na wierzchołek, podwójne kliknięcie zamyka (3+ wierzchołki) | Złożone kształty, własne obszary |

- Kształty inne niż koło pokazują **tylko zaznaczony kontur** (bez okrągłych pierścieni).
- Miękkość wpływa na blend krawędzi we wszystkich kształtach.

---

## ⚙️ Tryby opadania (Podnoszenie/Obniżanie)

Konfiguracja w USTAWIENIACH → "Opadanie Raise/Lower":

| Tryb | Zachowanie |
|------|------------|
| **Spiczasty** | Centralny szczyt. Kontrolowany suwakiem "Potęga opadania" (1–5). |
| **Płaski** | Równomierny dysk — ten sam ciężar na całym promieniu. |
| **Gładki** | Dzwon kosinusowy — płynne przejście, bez twardej krawędzi. |
| **Liniowy** | Proste zwężenie od środka do krawędzi. |

---

## 🧱 Stemple (Kopiuj/Wklej)

Potężna funkcja do replikowania struktur terenu.

### Wybierz obszar do skopiowania

Wybierz Kopiuj/Wklej w kole Q. Zastosuj wyznacza obszar: dwa rogi dla Prostokąta, dwa końce dla Linii, wierzchołki dla Wielokąta (dwuklik po 3+ punktach) lub obszar pędzla dla Koła/Kwadratu. HUD podpowiada następny krok.

### Przebieg pracy

1. **Zaznacz obszar**: użyj dowolnego kształtu (Koło, Prostokąt, Linia, Wielokąt, Rampa) — lub selekcji prostokątem powyżej.
2. **Kopiuj**: `Ctrl+Shift+C` → "Skopiowano stempel: X kafelków" (schowek w RAM).
3. **Podgląd wklejania**: `Ctrl+Shift+V` → widmowa siatka w miejscu celownika.
   - `Q`/`E` = obrót ±15°
   - `Alt`+Rolka = ciągły obrót
   - `Shift`+Rolka = skala 0.25×–4×
   - `Esc` = anuluj
4. **Potwierdź**: Naciśnij ponownie klawisz wklejania, aby zatwierdzić podgląd.

> Stemple selekcji zapisują **tylko wewnątrz skopiowanego obszaru**; teren poza nim pozostaje nietknięty.

### Historia
- Do **10 stempli** w RAM (najnowsze najpierw).
- Klawisze `Następny stempel` / `Poprzedni stempel` (przypisywalne) przełączają je.
- Komunikat: "Historia stempli: Copy: 3 kafelki [2/5]"

### Trwałość
- **USTAWIENIA → STEMPLE**:
  - Zapisz bieżący stempel na dysk (`.stamp` w `BepInEx/config/TerrainForge/stamps/`, maks. 12).
  - Wczytaj stempel do schowka.
  - Wyświetl/usuń zapisane stemple.

### Narzędzie stempla
- USTAWIENIA → "Narzędzie stempla" (4 opcje):
  - `Same as brush` (domyślnie) — używa tego samego narzędzia co pędzel
  - `Any` — brak wymagań
  - `Pickaxe` — wymaga założonego kilofa
  - `TerrainForge Rod` — wymaga różdżki z moda
- HUD pokazuje `STEMP GOTOWY [2/5]`, gdy narzędzie jest założone + stemple znajdują się w schowku.
- Bez narzędzia: `Stempel: narzędzie niezałożone`, kopiowanie/wklejanie zablokowane.

---

## 💾 Migawki (Backupy)

Pełny backup/przywracanie terenu.

### Zapisywanie
1. USTAWIENIA → **SNAPSHOTS**.
2. Nazwa (pusta = automatyczna data/godzina).
3. **Zapisz** → edytowane kafelki do `BepInEx/config/TerrainForge/snapshots/<name>.dat` (maks. 12).

### Wczytywanie
1. Wybierz migawkę.
2. **Wczytaj** → przywraca kafelki wraz z własnością, **1 krok cofania**.
3. Niewczytane kafelki ostrzegają w czytelny sposób — importuj blisko pierwotnej lokalizacji.

### Cechy
- Wersjonowany format binarny (`.dat`).
- Przywraca wysokość + smooth + farbę + własność.
- Import można cofnąć (`Ctrl+Z` wycofuje import).
- Niewczytane kafelki ostrzegają w czytelny sposób.

---

## 💾 Profile

Zapisuj/wczytuj pełne konfiguracje pędzla.

- USTAWIENIA → **PROFILE**.
- Nazwa + **Zapisz** → promień, miękkość, operacja, kształt, farba, tryb rampy, kąt, tryb wygładzania, raise delta, siła farby, falloff, shore offset.
- **Wczytaj** → stosuje natychmiast.
- Maks. 12 profili (format niezależny od kultury w `Presets > Slots`).
- Skrót: `Next Profile` (przypisz w KLAWISZACH), aby przełączać.

---

## 🌐 Języki

**15 opcji:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — domyślnie `Auto`.

### Auto-Wykrywanie
- Domyślne `Auto` wykrywa język gry przez reflection: `Localization.instance.GetSelectedLanguage()`.
- Mapuje język Valheim → indeks wewnętrzny.
- Jeśli język nie ma pełnego tłumaczenia → następuje powrót do English.
- Konfigurowalne w USTAWIENIACH → "Język" (przełącza 15 opcji).
- Etykiety klawiszy są odbudowywane przy otwarciu menu.

### Status tłumaczeń
- **Pełne:** English, Español
- **Powrót do English:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Tłumaczenia społeczności mile widziane przez PR.

### Ta dokumentacja
Przetłumaczone strony wiki: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Wieloosobowość

### Wymagania
- **Mod na kliencie ORAZ serwerze** dla edycji większych niż ±8 m i poprawnej replikacji.
- `UnlockDeltaLimit` również na serwerze, w przeciwnym razie serwer ogranicza teren.

### Autorytet serwera (Synchronizacja konfiguracji)
Gdy serwer uruchamia mod, jego sekcja `Server` ma pierwszeństwo:
- `AllowTerrainEdits` (domyślnie `true`) — główny przełącznik; jeśli `false`, klienci nie mogą stosować/cofać/ponawiać.
- `AdminOnlyEdits` (domyślnie `false`) — tylko admini/host mogą edytować.
- `RespectWards` (domyślnie `true`) — blokuje edycje wewnątrz obcych wards.
- `RadiusCap` / `DeltaCap` (0 = wyłączone) — limity narzucane przez serwer, widoczne w HUD+podglądzie.

> Serwery **bez** moda ignorują tę sekcję → klient działa do testów.

### Zapobieganie desynchronizacji
- Utrzymuj spójne `MaxUpDelta` / `MaxDownDelta` na serwerze + klientach.
- Duże promienie mogą desynchronizować klientów bez moda.

---

## ⚙️ Ustawienia

Plik: `BepInEx/config/com.terrainforge.valheim.cfg`

### Ogólne
| Ustawienie | Domyślnie | Opis |
|------------|-----------|------|
| `Enabled` | `true` | Główny przełącznik |
| `PlayEffects` | `true` | Pył + trafienie przy zastosowaniu |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Barwi Różdżkę TerrainForge (klon Młota), aby łatwo odróżnić ją od waniliowego Młota |
| `ToolTintColor` | `#4FC3F7` | Kolor hex różdżki (model + ikona). Np. `#FFC400` złoty, `#8BC34A` zielony |

### Pędzel
| Ustawienie | Domyślnie | Zakres | Opis |
|------------|-----------|--------|------|
| `Radius` | 8 | 1–100 | Promień pędzla (m) |
| `RadiusMin` | 2 | — | Minimalny promień podczas dostosowywania |
| `RadiusMax` | 60 | — | Maksymalny promień podczas dostosowywania |
| `RadiusStep` | 1 | — | Zmiana promienia na klawisz/rolkę |
| `Softness` | 50 | 0–100 | Miękkość krawędzi (0 = twarda, 100 = bardzo miękka) |
| `FalloffPower` | 3 | 1–5 | Wykładnik opadania (tylko tryb Spiczasty) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Metry na zastosowanie Podnieś/Obniż |
| `PaintStrength` | 1 | 0–1 | Siła farby |
| `DefaultOperation` | Flatten | — | Operacja początkowa |
| `Shape` | Circle | — | Kształt początkowy |
| `Paint` | Dirt | — | Farba początkowa |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Stały kąt rampy (stopnie) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Margines brzegu nad wodą (m) |
| `UseFixedHeight` | false | — | Używaj stałej wysokości (stan kroplomierza) |
| `FixedHeight` | 30 | -10–150 | Dokładna wysokość Wyrównywania (m) |

### Vanilla / integracja
- `BlockVanillaTerrainTools`: false — przy założonej różdżce ignoruje edycję terenu waniliowej motyki/kultywatora. W USTAWIENIACH jest też przełącznik.

### Wejście
Wszystkie skróty są przypisywalne na zakładce KLAWISZE. Najważniejsze:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Bezpośrednie klawisze operacji** (9, brak domyślnych): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (domyślnie)
- `AutoGiveTool`: true (przyznaje różdżkę po dołączeniu)

### Stemple
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (domyślnie Same as Brush)

### Podgląd
- `ShowPreview`: true, `ShowGuides`: true (prowadnice zasięgu, przyciemnienie nachylenia, pierścień poziomu)

### Historia
- `UndoDepth`: 50 (0–500, 0 wyłącza historię)

### Serwer (Autorytatywny przez Config Sync)
- `AllowTerrainEdits`: true (główny przełącznik)
- `AdminOnlyEdits`: false (tylko admini)
- `RespectWards`: true (blokuje obce wards)
- `RadiusCap` / `DeltaCap`: 0 (wyłączone) — limity narzucane przez serwer

### Wydajność
- `TilesPerFrame`: 4 (1–32 kafelków/klatkę)

---

## 🐛 Rozwiązywanie problemów

| Problem | Rozwiązanie |
|---------|-------------|
| **Menu się nie pojawia / niewidoczny kursor** | Zamknij grę. Usuń `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` i `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Uruchom ponownie TMM, sprawdź aktualizacje. |
| **Aktualizacja nie pojawia się w TMM** | TMM → Settings → "Refresh mod list" (Ctrl+R). Zamknij/otwórz ponownie TMM. Sprawdź właściwy profil. |
| **Konflikt wersji w Thunderstore** | Podnieś wersję (0.3.3 → 0.3.4) w `.csproj`, `Plugin.cs`, `manifest.json`. Przebuduj + wgraj ponownie. |
| **Kopiowanie/wklejanie stempla nic nie robi** | HUD: `STEMP GOTOWY [X/Y]` = gotowe. Jeśli "narzędzie niezałożone", załóż narzędzie (USTAWIENIA → Narzędzie stempla). |
| **Podgląd niewidoczny pod wodą** | Naprawione w 0.3.2 — podgląd unosi się na powierzchni. Zaktualizuj. |
| **Okręgi na Linii/Prostokącie/Wielokącie** | Naprawione w 0.3.2 — pokazywany jest tylko zaznaczony kontur. Zaktualizuj. |
| **Rolka radialna nie działa** | Naprawione w 0.3.2 (`ZInput.GetMouseScrollWheel()`) i 0.3.4 (zaznaczenie nie jest już czyszczone przy wyśrodkowanym kursorze). Zaktualizuj. |
| **Konfiguracja się nie zapisuje** | Zamknij menu (F5), aby zapisać automatycznie. Albo edytuj `BepInEx/config/com.terrainforge.valheim.cfg` bezpośrednio. |
| **Desynchronizacja w trybie wieloosobowym** | Mod na kliencie+serwerze. `UnlockDeltaLimit` na obu. Te same `MaxUpDelta`/`MaxDownDelta`. |
| **Budowanie kończy się niepowodzeniem** | Sprawdź ścieżki `ValheimDir`, `BepInExCoreDir` w `.csproj` lub przez CLI. Wymagany .NET SDK 8+. |
| **Kompilacja / pakowanie** | `dotnet build -c Release` (DLL w `build/`), `-p:Deploy=true` (instaluje w profilu) lub `-p:Package=true` (zip w `build/`). |

---

## 🔗 Linki

- **Strona Thunderstore:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **Repozytorium GitHub:** https://github.com/marceloanton/TerrainForge
- **Dotacje:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Dziennik zmian:** [Changelog](Changelog)

---

*TerrainForge — Terraformuj Valheim bez ograniczeń.*