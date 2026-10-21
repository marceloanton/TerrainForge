# Wiki TerrainForge

[Overview](Overview) · [English](Home) · [Español](Home-es) · *Français* · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### Nouveautés TerrainForge 0.3.6

> La roue Q propose un mode Copier/Coller dédié. Rectangle marque A/B avec Appliquer ; Ligne marque deux extrémités ; Polygone ajoute des sommets et copie par double pression après 3 points ; Cercle/Carré copie l’empreinte du pinceau. Le HUD indique l’étape suivante. La neige est dans Peinture > DeepSnow (maintenez Maj pour l’enlever). F5 bloque les déplacements et la caméra locale ; Échap met en pause tout le monde en solo. 0.3.6 : F5 répartit les onglets sur deux lignes et n’affiche que les commandes utiles à l’opération. Ramp propose Deux points, Angle fixe et Connectée ; la rampe circulaire est prévue pour la 0.4.0.

## Sommaire

[Overview](Overview) · 1. [Installation](#-installation)
2. [Démarrage Rapide](#-démarrage-rapide)
3. [Contrôles](#-contrôles)
4. [Opérations](#-opérations)
5. [Formes](#-formes)
6. [Modes d'Atténuation (Monter/Descendre)](#-modes-datténuation-monterdescendre)
7. [Stamps (Copier/Coller)](#-stamps-copiercoller)
8. [Snapshots (Backups)](#-snapshots-backups)
9. [Profils](#-profils)
10. [Langues](#-langues)
11. [Multijoueur](#-multijoueur)
12. [Configuration](#-configuration)
13. [Dépannage](#-dépannage)

---

## 📥 Installation

[Overview](Overview) · ### Prérequis
- **BepInEx 5** (x64, Mono) sur le client et le serveur. Les utilisateurs de Thunderstore l'ont déjà.

### Étapes
1. Téléchargez `TerrainForge-<version>.zip` depuis Thunderstore ou GitHub Releases.
2. Extrayez et placez `TerrainForge.dll` dans :
   - **Client :** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Serveur :** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Redémarrez le jeu / serveur.

> ⚠️ **Doit être installé sur le client ET le serveur** pour les modifications supérieures à ±8 m et une réplication correcte.

---

## 🚀 Démarrage Rapide

[Overview](Overview) · 1. Rejoignez un monde. La **Baguette TerrainForge** (un clone du marteau) est donnée et équipée automatiquement.
2. Appuyez sur `F5` pour ouvrir le menu (ou cliquez sur le bouton **TerrainForge** dans le menu pause `ESC`).
3. Équipez la baguette si elle ne l'est pas.
4. Visez le terrain et maintenez le **clic droit** (`Mouse1`) pour sculpter.
5. Utilisez `,` pour faire défiler les opérations, `.` pour les formes, `/` pour les peintures.
6. Maintenez `Q` → menu radial, relâchez pour confirmer.
7. `Ctrl+Z` / `Ctrl+Y` pour annuler/rétablir.

---

## ⌨️ Contrôles

[Overview](Overview) · | Action | Touche par défaut | Description |
|--------|-------------------|-------------|
| Appliquer la brosse | Maintenir `Mouse1` (clic droit) | Sculpte le terrain sous le viseur |
| Rayon + | `=` ou `Ctrl`+molette | Augmente le rayon de la brosse |
| Rayon − | `-` ou `Ctrl`+molette | Diminue le rayon de la brosse |
| Changer d'opération | `,` | Fait défiler les 9 opérations |
| Changer de forme | `.` | Cercle → Carré → Ligne → Rect → Poly |
| Changer de peinture | `/` | Fait défiler les 6 types de peinture |
| Niveler aux pieds | Maintenir `Shift` gauche | Aplatir utilise la hauteur du joueur |
| Annuler | `Ctrl+Z` | Annule le dernier trait |
| Rétablir | `Ctrl+Y` | Rétablit le trait annulé |
| Pipette | `G` | Fixe la hauteur d'Aplatir au viseur (encore = libère) |
| Mode de rampe | `H` | Défile : Deux Points → Angle Fixe → Chemin (ou choisissez avec 3 boutons dans RÉGLAGES) |
| Angle de rampe fixe | Maintenir `Alt` + rayon/molette | Ajuste l'angle en mode fixe |
| Menu radial | Maintenir `Q` | Choisissez une opération à la souris, relâcher confirme |
| Mode ajustement | `V` | Molette = rayon, gèle le zoom/la barre/la carte |
| Ajustement fin (0.1 m) | Maintenir `Shift` + molette/rayon | Ajustement précis du rayon |
| **Copier le Stamp** | `Ctrl+Shift+C` | Copie la zone marquée dans le presse-papiers |
| **Coller le Stamp** | `Ctrl+Shift+V` | Aperçu fantôme, le clic confirme |
| Obtenir la baguette | `F8` | Donne la Baguette TerrainForge |
| Ouvrir le menu | `F5` | Ouvre le menu touches/aide/réglages |

**Touches supplémentaires (aucune par défaut, assignez-les dans TOUCHES) :**
- Opération directe : Aplatir, Monter, Descendre, Lisser, Peindre, Restaurer, Rampe, Côte, Neige
- Profil suivant / précédent
- Stamp suivant / précédent (historique)

**Section « Actions rapides » (onglet TOUCHES) :** boutons pour les fonctions sans raccourci obligatoire — Annuler, Rétablir, Fixer la hauteur, Copier/Coller le stamp, Stamp précédent, Effacer les marques, les cartes de chaleur et le Diagnostic de matériau (déverse le shader + les emplacements de texture dans le log).

---

## 🔄 Opérations

[Overview](Overview) · | Opération | Description |
|-----------|-------------|
| **Aplatir** | Nivelle à la hauteur visée (Shift = à vos pieds). Hauteur fixe via la Pipette (`G`) ou le champ de RÉGLAGES. |
| **Monter** | Élève le terrain de `RaiseDelta` par application. Le mode d'atténuation lui donne sa forme. |
| **Descendre** | Abaisse le terrain de `RaiseDelta` par application. Le mode d'atténuation lui donne sa forme. |
| **Lisser** | Vrai flou : moyenne chaque sommet avec ses 8 voisins. Maintenez pour un effet progressif. **Modes :** Complet (les deux), Crêtes seulement (abaisse), Creux seulement (élève). Forme Ligne = lisse un chemin. |
| **Rampe** | **3 modes** (défile `H`) :<br>• **Deux Points** : marquez A et B → une pente droite relie les hauteurs.<br>• **Angle Fixe** : B = direction/longueur ; hauteur = `SlopeAngle` (monte/descend selon le viseur). Ajustez avec `Alt`+rayon.<br>• **Chemin** : marquez une courbe A→B→C… (double-appui avec 3+ ferme) → Catmull-Rom lissé.<br>Largeur = rayon de la brosse. Douceur = mélange uniquement sur les bords latéraux. Aperçu : corridor rempli + anneaux de hauteur en A/B. |
| **Côte** | Aplanit vers le niveau de l'eau + marge `ShoreOffset` (plages). Douceur comme Aplatir. |
| **Peindre** | Types : Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Force ajustable. |
| **Restaurer** | Ramène le terrain à son état d'origine (hauteur + peinture). |
| **Copier/Coller** | Mode dédié de la roue Q pour sélectionner et coller des stamps de terrain. |

---

## 📐 Formes

[Overview](Overview) · Défilez avec la touche `.`. Toutes prennent en charge l'annulation.

| Forme | Marquage | Cas d'usage |
|-------|----------|-------------|
| **Cercle** | Clic simple (centre de la brosse) | Sculpture générale |
| **Carré** | Clic simple (centre) | Plateformes, fondations |
| **Ligne** | 2 clics (A → B) | Chemins, murs, sentiers. Demi-largeur = rayon. |
| **Rectangle** | 2 clics (coins opposés) | Fondations, fermes, plateformes. Bords doux. |
| **Polygone** | Clic par sommet, double-clic ferme (3+ sommets) | Formes complexes, zones personnalisées |

- Les formes non circulaires affichent **uniquement le contour marqué** (sans anneaux circulaires).
- La douceur affecte le mélange des bords sur toutes les formes.

---

## ⚙️ Modes d'Atténuation (Monter/Descendre)

[Overview](Overview) · Config dans RÉGLAGES → « Atténuation Monter/Descendre » :

| Mode | Comportement |
|------|----------------|
| **Pointu** | Pic central. Contrôlé par le curseur « Puissance d'atténuation » (1–5). |
| **Plat** | Disque uniforme — même poids sur tout le rayon. |
| **Doux** | Cloche en cosinus — transition douce, sans bord dur. |
| **Linéaire** | Dégradé rectiligne du centre vers le bord. |

---

## 🧱 Stamps (Copier/Coller)

[Overview](Overview) · Une fonctionnalité puissante pour répliquer des structures de terrain.

### Sélection par rectangle (recommandée)

Choisissez Copier/Coller dans la roue Q. Appliquer marque la zone : deux coins pour Rectangle, deux extrémités pour Ligne, des sommets pour Polygone (double pression après 3 points), ou l’empreinte du pinceau pour Cercle/Carré. Le HUD indique l’étape suivante.

### Flux de travail

[Overview](Overview) · 1. **Marquez la zone** : utilisez n'importe quelle forme (Cercle, Rect, Ligne, Poly, Rampe) — ou la sélection par rectangle ci-dessus.
2. **Copier** : `Ctrl+Shift+C` → « Stamp copié : X tuiles » (presse-papiers en RAM).
3. **Aperçu du collage** : `Ctrl+Shift+V` → filaire fantôme au viseur.
   - `Q`/`E` = rotation ±15°
   - `Alt`+molette = rotation continue
   - `Shift`+molette = échelle 0.25×–4×
   - `Esc` = annuler
4. **Confirm**: Appuyez à nouveau sur le raccourci de collage pour confirmer l’aperçu.

> Les stamps de sélection écrivent **uniquement à l'intérieur de la région copiée** ; le terrain à l'extérieur n'est pas touché.

### Historique
- Jusqu'à **10 stamps** en RAM (le plus récent en premier).
- Les touches `Stamp suivant` / `Stamp précédent` (réassignables) les font défiler.
- Flash : « Stamp history: Copy: 3 tiles [2/5] »

### Persistance
- **RÉGLAGES → STAMPS** :
  - Enregistrer le stamp actuel sur le disque (`.stamp` dans `BepInEx/config/TerrainForge/stamps/`, max 12).
  - Charger un stamp dans le presse-papiers.
  - Lister/supprimer les stamps enregistrés.

### Outil stamp
- RÉGLAGES → « Outil stamp » (4 options) :
  - `Same as brush` (défaut) — utilise le même outil que la brosse
  - `Any` — aucune exigence
  - `Pickaxe` — nécessite une pioche équipée
  - `TerrainForge Rod` — nécessite la baguette du mod
- Le HUD affiche `STAMP READY [2/5]` lorsque l'outil est équipé + des stamps dans le presse-papiers.
- Sans l'outil : `Stamp: tool not equipped`, copier/coller bloqués.

---

## 💾 Snapshots (Backups)

[Overview](Overview) · Sauvegarde/restauration complète du terrain.

### Enregistrer
1. RÉGLAGES → **SNAPSHOTS**.
2. Nom (vide = date/heure automatique).
3. **Enregistrer** → tuiles modifiées vers `BepInEx/config/TerrainForge/snapshots/<name>.dat` (max 12).

### Charger
1. Sélectionnez un snapshot.
2. **Charger** → restaure les tuiles avec l'ownership, **1 étape d'annulation**.
3. Les tuiles non chargées avertissent proprement — importez près de l'emplacement d'origine.

### Caractéristiques
- Format binaire versionné (`.dat`).
- Restaure hauteur + smooth + peinture + ownership.
- L'import est annulable (`Ctrl+Z` annule l'import).
- Les tuiles non chargées avertissent proprement.

---

## 💾 Profils

[Overview](Overview) · Enregistrez/chargez des configurations complètes de brosse.

- RÉGLAGES → **PROFILS**.
- Nom + **Enregistrer** → rayon, douceur, op, forme, peinture, mode de rampe, angle, mode de lissage, raise delta, force de peinture, falloff, shore offset.
- **Charger** → applique instantanément.
- Max 12 profils (format culture-invariant dans `Presets > Slots`).
- Raccourci : `Next Profile` (assignez dans TOUCHES) pour faire défiler.

---

## 🌐 Langues

[Overview](Overview) · **15 options :** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — par défaut `Auto`.

### Détection automatique
- Le `Auto` par défaut détecte la langue du jeu via réflexion : `Localization.instance.GetSelectedLanguage()`.
- Mappe la langue de Valheim → index interne.
- Si une langue n'a pas de traduction complète → revient à l'anglais.
- Configurable dans RÉGLAGES → « Language » (fait défiler les 15 options).
- Les libellés des touches sont reconstruits à l'ouverture du menu.

### État des traductions
- **Complètes :** English, Español
- **Repli sur l'anglais :** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Traductions de la communauté bienvenues via PR.

### Cette documentation
Pages wiki traduites : [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Multijoueur

[Overview](Overview) · ### Prérequis
- **Mod sur le client ET le serveur** pour les modifications supérieures à ±8 m et une réplication correcte.
- `UnlockDeltaLimit` aussi sur le serveur, sinon le serveur limite le terrain.

### Autorité du serveur (Config Sync)
Lorsque le serveur exécute le mod, sa section `Server` l'emporte :
- `AllowTerrainEdits` (défaut `true`) — interrupteur principal ; si `false`, les clients ne peuvent pas appliquer/annuler/rétablir.
- `AdminOnlyEdits` (défaut `false`) — seuls les admins/l'hôte peuvent modifier.
- `RespectWards` (défaut `true`) — bloque les modifications à l'intérieur des wards étrangers.
- `RadiusCap` / `DeltaCap` (0 = désactivé) — plafonds appliqués par le serveur, visibles dans le HUD+aperçu.

> Les serveurs **sans** le mod ignorent la section → le client fonctionne pour tester.

### Prévention de la désynchronisation
- Gardez `MaxUpDelta` / `MaxDownDelta` cohérents sur le serveur + les clients.
- Les grands rayons peuvent désynchroniser les clients sans le mod.

---

## ⚙️ Configuration

[Overview](Overview) · Fichier : `BepInEx/config/com.terrainforge.valheim.cfg`

### Général
| Réglage | Défaut | Description |
|---------|---------|-------------|
| `Enabled` | `true` | Interrupteur principal |
| `PlayEffects` | `true` | Poussière + impact à l'application |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Teinte la Baguette TerrainForge (un clone du marteau) pour la distinguer facilement du marteau vanilla |
| `ToolTintColor` | `#4FC3F7` | Couleur hex de la baguette (modèle + icône). Ex. `#FFC400` or, `#8BC34A` vert |

### Brosse
| Réglage | Défaut | Plage | Description |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Rayon de la brosse (m) |
| `RadiusMin` | 2 | — | Rayon min pendant l'ajustement |
| `RadiusMax` | 60 | — | Rayon max pendant l'ajustement |
| `RadiusStep` | 1 | — | Changement de rayon par touche/molette |
| `Softness` | 50 | 0–100 | Douceur des bords (0 = dur, 100 = très doux) |
| `FalloffPower` | 3 | 1–5 | Exposant d'atténuation (mode Pointu uniquement) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Mètres par application de Monter/Descendre |
| `PaintStrength` | 1 | 0–1 | Force de peinture |
| `DefaultOperation` | Flatten | — | Opération initiale |
| `Shape` | Circle | — | Forme initiale |
| `Paint` | Dirt | — | Peinture initiale |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Angle de rampe fixe (degrés) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Marge de côte au-dessus de l'eau (m) |
| `UseFixedHeight` | false | — | Utiliser une hauteur fixe (état de la pipette) |
| `FixedHeight` | 30 | -10–150 | Hauteur exacte d'Aplatir (m) |

### Vanilla / intégration
- `BlockVanillaTerrainTools` : false — avec la baguette équipée, ignore l'édition de terrain de la houe/cultivateur vanilla. Il y a aussi un interrupteur dans RÉGLAGES.

### Entrées
Tous les raccourcis sont réassignables dans l'onglet TOUCHES. Points clés :
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Touches d'op directes** (9, aucune par défaut) : OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool` : Any / Pickaxe / TerrainForgeTool (défaut)
- `AutoGiveTool` : true (donne la baguette à la connexion)

### Stamps
- `StampToolMode` : Same as Brush / Any / Pickaxe / TerrainForgeTool (défaut Same as Brush)

### Aperçu
- `ShowPreview` : true, `ShowGuides` : true (guides de portée, teinte de pente, anneau de niveau)

### Historique
- `UndoDepth` : 50 (0–500, 0 désactive l'historique)

### Serveur (autoritatif via Config Sync)
- `AllowTerrainEdits` : true (interrupteur principal)
- `AdminOnlyEdits` : false (admins uniquement)
- `RespectWards` : true (bloque les wards étrangers)
- `RadiusCap` / `DeltaCap` : 0 (désactivé) — plafonds appliqués par le serveur

### Performance
- `TilesPerFrame` : 4 (1–32 tuiles/image)

---

## 🐛 Dépannage

[Overview](Overview) · | Problème | Solution |
|----------|----------|
| **Le menu n'apparaît pas / curseur invisible** | Fermez le jeu. Supprimez `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` et `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Redémarrez TMM, vérifiez les mises à jour. |
| **Mise à jour absente dans TMM** | TMM → Settings → « Refresh mod list » (Ctrl+R). Fermez/rouvrez TMM. Vérifiez le bon profil. |
| **Conflit de version Thunderstore** | Incrémentez la version (0.3.3 → 0.3.4) dans `.csproj`, `Plugin.cs`, `manifest.json`. Rebuild + re-upload. |
| **Copier/coller de stamp sans effet** | HUD : `STAMP READY [X/Y]` = prêt. Si « outil non équipé », équipez l'outil (RÉGLAGES → Outil stamp). |
| **Aperçu invisible sous l'eau** | Corrigé en 0.3.2 — l'aperçu flotte à la surface. Mettez à jour. |
| **Cercles sur Ligne/Rect/Poly** | Corrigé en 0.3.2 — seul le contour marqué est affiché. Mettez à jour. |
| **La molette radiale ne fonctionne pas** | Corrigé en 0.3.2 (`ZInput.GetMouseScrollWheel()`) et 0.3.4 (la sélection n'est plus effacée avec un curseur centré). Mettez à jour. |
| **La config ne s'enregistre pas** | Fermez le menu (F5) pour auto-enregistrer. Ou modifiez `BepInEx/config/com.terrainforge.valheim.cfg` directement. |
| **Désynchronisation multijoueur** | Mod sur client+serveur. `UnlockDeltaLimit` sur les deux. Mêmes `MaxUpDelta`/`MaxDownDelta`. |
| **Le build échoue** | Vérifiez les chemins `ValheimDir`, `BepInExCoreDir` dans `.csproj` ou via la CLI. .NET SDK 8+ requis. |
| **Compiler / empaqueter** | `dotnet build -c Release` (DLL dans `build/`), `-p:Deploy=true` (installe dans le profil) ou `-p:Package=true` (zip dans `build/`). |

---

## 🔗 Liens

[Overview](Overview) · - **Page Thunderstore :** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **Dépôt GitHub :** https://github.com/marceloanton/TerrainForge
- **Dons :** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog :** [Changelog](Changelog)

---

*TerrainForge — Terraformez Valheim sans limites.*
