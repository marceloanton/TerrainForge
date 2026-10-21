# TerrainForge ウィキ

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · *日本語* · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 の更新

> Qホイールに専用のコピー/貼り付けモードを追加。矩形は適用キーでA/B、線は両端、多角形は頂点を指定して3点以上からダブル押しでコピー、円/四角形はブラシ範囲をコピーします。HUDが次の操作を表示します。雪は塗装 > DeepSnow に統合（Shift長押しで除去）。F5中はローカルの移動とカメラを固定し、Escはシングルプレイ全体を一時停止します。

## 目次

[Overview](Overview) · 1. [インストール](#-インストール)
2. [クイックスタート](#-クイックスタート)
3. [キー操作](#-キー操作)
4. [操作](#-操作)
5. [形状](#-形状)
6. [フォールオフモード (上昇/下降)](#-フォールオフモード-上昇下降)
7. [スタンプ (コピー/ペースト)](#-スタンプ-コピーペースト)
8. [スナップショット (バックアップ)](#-スナップショット-バックアップ)
9. [プロファイル](#-プロファイル)
10. [言語](#-言語)
11. [マルチプレイ](#-マルチプレイ)
12. [設定](#-設定)
13. [トラブルシューティング](#-トラブルシューティング)

---

## 📥 インストール

[Overview](Overview) · ### 要件
- **BepInEx 5**（x64、Mono）をクライアントとサーバーの両方に。Thunderstore のユーザーはすでに導入済みです。

### 手順
1. Thunderstore または GitHub Releases から `TerrainForge-<version>.zip` をダウンロードします。
2. 解凍して `TerrainForge.dll` を次の場所に配置します:
   - **クライアント:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **サーバー:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. ゲーム / サーバーを再起動します。

> ⚠️ ±8 m を超える編集と正しいレプリケーションには、**クライアントとサーバーの両方に**インストールする必要があります。

---

## 🚀 クイックスタート

[Overview](Overview) · 1. ワールドに参加します。**TerrainForge の杖**（ハンマーのクローン）が自動的に付与され、装備されます。
2. `F5` を押してメニューを開きます（または `ESC` ポーズメニューの **TerrainForge** ボタンをクリック）。
3. 杖を装備していない場合は装備します。
4. 地形を狙い、**右クリック**（`Mouse1`）を押し続けて掘削します。
5. `,` で操作を切り替え、`.` で形状、`/` で塗装を切り替えます。
6. `Q` を押し続ける → ラジアルメニュー、離して確定します。
7. `Ctrl+Z` / `Ctrl+Y` で元に戻す / やり直しします。

---

## ⌨️ キー操作

[Overview](Overview) · | 操作 | 既定キー | 説明 |
|--------|-------------|-------------|
| ブラシを適用 | `Mouse1`（右クリック）を押し続ける | クロスヘア下の地形を掘削します |
| 半径 + | `=` または `Ctrl`+ホイール | ブラシ半径を拡大します |
| 半径 − | `-` または `Ctrl`+ホイール | ブラシ半径を縮小します |
| 操作を切り替え | `,` | 9 つの操作を切り替えます |
| 形状を切り替え | `.` | 円 → 正方形 → 線 → 矩形 → 多角形 |
| 塗装を切り替え | `/` | 6 種類の塗装を切り替えます |
| 足元の高さに合わせる | `Left Shift` を押し続ける | 平坦化がプレイヤーの高さを使用します |
| 元に戻す | `Ctrl+Z` | 最後のストロークを元に戻します |
| やり直す | `Ctrl+Y` | 元に戻したストロークをやり直します |
| スポイト | `G` | クロスヘア位置で平坦化の高さを固定します（もう一度 = 解除） |
| ランプモード | `H` | 切り替え: 2点 → 固定角度 → パス（または設定の3つのボタンで選択） |
| ランプの固定角度 | `Alt` + 半径/ホイールを押し続ける | 固定モードで角度を調整します |
| ラジアルメニュー | `Q` を押し続ける | マウスで操作を選び、離して確定します |
| 調整モード | `V` | ホイール = 半径、ズーム/バー/マップを凍結します |
| 微調整 (0.1 m) | `Shift` + ホイール/半径を押し続ける | 半径を精密に調整します |
| **スタンプをコピー** | `Ctrl+Shift+C` | マークした領域をクリップボードにコピーします |
| **スタンプを貼り付け** | `Ctrl+Shift+V` | ゴーストプレビュー、クリックで確定 |
| 杖を入手 | `F8` | TerrainForge の杖を付与します |
| メニューを開く | `F5` | キー/ヘルプ/設定メニューを開きます |

**追加キー（既定なし、キーで割り当て）:**
- 操作の直接指定: 平坦化、上昇、下降、スムーズ、塗装、復元、ランプ、岸、雪
- 次の / 前のプロファイル
- 次の / 前のスタンプ（履歴）

**「クイックアクション」セクション（キータブ）:** 必須ショートカットのない機能用のボタン — 元に戻す、やり直す、高さを固定、スタンプのコピー/貼り付け、前のスタンプ、マークをクリア、ヒートマップ、マテリアル診断（シェーダー + テクスチャスロットをログに出力）。

---

## 🔄 操作

[Overview](Overview) · | 操作 | 説明 |
|-----------|-------------|
| **平坦化** | 狙った高さに水平化します（Shift = 足元）。スポイト（`G`）または設定のテキストボックスで高さを固定できます。 |
| **上昇** | 1 回の適用ごとに地形を `RaiseDelta` だけ持ち上げます。フォールオフモードが形状を決めます。 |
| **下降** | 1 回の適用ごとに地形を `RaiseDelta` だけ下げます。フォールオフモードが形状を決めます。 |
| **スムーズ** | 本物のぼかし: 各頂点を隣接する 8 頂点と平均化します。押し続けると段階的に適用されます。**モード:** 全体（両方）、尾根のみ（下げる）、窪みのみ（上げる）。線形状 = パスを滑らかにします。 |
| **ランプ** | **3 モード**（`H` で切り替え）:<br>• **2点**: A と B をマーク → 直線的な傾斜が高さをつなぎます。<br>• **固定角度**: B = 方向/長さ、高さ = `SlopeAngle`（クロスヘアに応じて上下）。`Alt`+半径で調整します。<br>• **パス**: 曲線 A→B→C… をマーク（3 個以上でダブルプレスで閉じる）→ 滑らかな Catmull-Rom。<br>幅 = ブラシ半径。ソフトネス = 側面のエッジのみをブレンドします。プレビュー: 塗りつぶした回廊 + A/B の高さリング。 |
| **岸** | 水位 + `ShoreOffset` マージンに向かって平坦化します（ビーチ）。ソフトネスは平坦化と同様です。 |
| **塗装** | 種類: Dirt、Cultivated、Paved、ClearVegetation、DeepSnow、Reset。強度を調整できます。 |
| **復元** | 地形を元の状態（高さ + 塗装）に戻します。 |
| **コピー/貼り付け** | 地形範囲の選択とスタンプ配置を行うQホイール専用モード。 |

---

## 📐 形状

[Overview](Overview) · `.` キーで切り替えます。すべて元に戻すに対応しています。

| 形状 | マーク方法 | 用途 |
|-------|---------|----------|
| **円** | シングルクリック（ブラシ中心） | 一般的な掘削 |
| **正方形** | シングルクリック（中心） | プラットフォーム、基礎 |
| **線** | 2 クリック（A → B） | パス、壁、小道。半幅 = 半径。 |
| **矩形** | 2 クリック（対角の角） | 基礎、農場、プラットフォーム。エッジが滑らか。 |
| **多角形** | 頂点ごとにクリック、ダブルクリックで閉じる（3 頂点以上） | 複雑な形状、カスタム領域 |

- 円形以外の形状は**マークした輪郭のみ**を表示します（円形リングなし）。
- ソフトネスはすべての形状のエッジブレンドに影響します。

---

## ⚙️ フォールオフモード (上昇/下降)

[Overview](Overview) · 設定 → 「Raise/Lower falloff」で構成:

| モード | 動作 |
|------|----------|
| **尖り (Pointy)** | 中央のピーク。「Falloff power」スライダー（1–5）で制御します。 |
| **フラット (Flat)** | 均一な円盤 — 半径全体で同じ重み。 |
| **スムーズ (Smooth)** | コサインの鐘形 — 滑らかな遷移、ハードエッジなし。 |
| **リニア (Linear)** | 中心からエッジへの直線的なテーパー。 |

---

## 🧱 スタンプ (コピー/ペースト)

[Overview](Overview) · 地形構造を複製するための強力な機能です。

### 矩形選択 (推奨)

Qホイールでコピー/貼り付けを選択します。適用キーで範囲を指定：矩形は2つの角、線は両端、多角形は頂点（3点以上でダブル押し）、円/四角形は現在のブラシ範囲です。HUDが次の操作を案内します。

### ワークフロー

[Overview](Overview) · 1. **領域をマーク**: 任意の形状（円、矩形、線、多角形、ランプ）を使用 — または上記の矩形選択。
2. **コピー**: `Ctrl+Shift+C` → 「Stamp copied: X tiles」（RAM クリップボード）。
3. **貼り付けプレビュー**: `Ctrl+Shift+V` → クロスヘア位置にゴーストワイヤーフレーム。
   - `Q`/`E` = ±15° 回転
   - `Alt`+ホイール = 連続回転
   - `Shift`+ホイール = 0.25×–4× 拡大縮小
   - `Esc` = キャンセル
4. **確定**: 貼り付けキーをもう一度押すとプレビューを確定します。

> 選択スタンプは**コピーした領域の内側にのみ**書き込みます。その外側の地形は変更されません。

### 履歴
- RAM に最大 **10 個のスタンプ**（新しい順）。
- 「次のスタンプ」/「前のスタンプ」キー（再割り当て可能）で切り替えます。
- フラッシュ: 「Stamp history: Copy: 3 tiles [2/5]」

### 永続化
- **設定 → STAMPS**:
  - 現在のスタンプをディスクに保存（`BepInEx/config/TerrainForge/stamps/` 内の `.stamp`、最大 12）。
  - スタンプをクリップボードに読み込みます。
  - 保存したスタンプを一覧表示/削除します。

### スタンプツール
- 設定 → 「Stamp tool」（4 つのオプション）:
  - `Same as brush`（既定）— ブラシと同じツールを使用します
  - `Any` — 要件なし
  - `Pickaxe` — ピッケルの装備が必要
  - `TerrainForge Rod` — MOD の杖が必要
- ツールを装備 + クリップボードにスタンプがある場合、HUD に `STAMP READY [2/5]` が表示されます。
- ツールなしの場合: `Stamp: tool not equipped`、コピー/貼り付けがブロックされます。

---

## 💾 スナップショット (バックアップ)

[Overview](Overview) · 地形の完全なバックアップ/復元。

### 保存
1. 設定 → **SNAPSHOTS**。
2. 名前（空 = 自動の日付/時刻）。
3. **保存** → 編集したタイルを `BepInEx/config/TerrainForge/snapshots/<name>.dat` に保存（最大 12）。

### 読み込み
1. スナップショットを選択します。
2. **読み込み** → 所有権込みでタイルを復元、**1 つの元に戻すステップ**。
3. 未読み込みのタイルは明確に警告します — 元の場所の近くでインポートしてください。

### 機能
- バージョン管理されたバイナリ形式（`.dat`）。
- 高さ + スムーズ + 塗装 + 所有権を復元します。
- インポートは元に戻せます（`Ctrl+Z` でインポートを巻き戻します）。
- 未読み込みのタイルは明確に警告します。

---

## 💾 プロファイル

[Overview](Overview) · 完全なブラシ構成を保存/読み込みします。

- 設定 → **PROFILES**。
- 名前 + **保存** → 半径、ソフトネス、操作、形状、塗装、ランプモード、角度、スムーズモード、上昇量、塗装強度、フォールオフ、岸オフセット。
- **読み込み** → 即座に適用します。
- 最大 12 プロファイル（`Presets > Slots` 内のカルチャ不変形式）。
- キーバインド: `Next Profile`（キーで割り当て）で切り替え。

---

## 🌐 言語

[Overview](Overview) · **15 のオプション:** `Auto`、`English`、`Español`、`Français`、`Deutsch`、`Русский`、`中文`、`繁體中文`、`日本語`、`한국어`、`Polski`、`Português`、`Türkçe`、`Українська`、`Italiano` — 既定は `Auto`。

### 自動検出
- 既定の `Auto` はリフレクションでゲームの言語を検出します: `Localization.instance.GetSelectedLanguage()`。
- Valheim の言語 → 内部インデックスにマッピングします。
- 言語に完全な翻訳がない場合 → English にフォールバックします。
- 設定 → 「Language」で構成可能（15 のオプションを切り替え）。
- キーラベルはメニューを開くと再構築されます。

### 翻訳状況
- **完全:** English、Español
- **English フォールバック:** Français、Deutsch、Русский、中文、繁體中文、日本語、한국어、Polski、Português、Türkçe、Українська、Italiano
- コミュニティ翻訳は PR で歓迎します。

### このドキュメント
翻訳済みの Wiki ページ: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)。

---

## 🌍 マルチプレイ

[Overview](Overview) · ### 要件
- ±8 m を超える編集と正しいレプリケーションには、**クライアントとサーバーの両方に MOD** が必要です。
- サーバー側にも `UnlockDeltaLimit`、そうでなければサーバーが地形をクランプします。

### サーバー権限 (Config Sync)
サーバーで MOD が動作している場合、その `Server` セクションが優先されます:
- `AllowTerrainEdits`（既定 `true`）— マスタースイッチ、`false` の場合クライアントは適用/元に戻す/やり直しができません。
- `AdminOnlyEdits`（既定 `false`）— 管理者/ホストのみが編集できます。
- `RespectWards`（既定 `true`）— 他人の結界内での編集をブロックします。
- `RadiusCap` / `DeltaCap`（0 = オフ）— サーバー適用の上限、HUD+プレビューに表示されます。

> MOD **なし**のサーバーはこのセクションを無視します → クライアントはテスト用に動作します。

### デシンク防止
- サーバー + クライアントで `MaxUpDelta` / `MaxDownDelta` を一致させてください。
- 大きな半径は MOD なしのクライアントをデシンクさせる可能性があります。

---

## ⚙️ 設定

[Overview](Overview) · ファイル: `BepInEx/config/com.terrainforge.valheim.cfg`

### 一般
| 設定 | 既定値 | 説明 |
|---------|---------|-------------|
| `Enabled` | `true` | マスタースイッチ |
| `PlayEffects` | `true` | 適用時にほこり + ヒット |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | TerrainForge の杖（ハンマーのクローン）を着色し、バニラのハンマーと簡単に区別できるようにします |
| `ToolTintColor` | `#4FC3F7` | 杖の 16 進カラー（モデル + アイコン）。例: `#FFC400` 金、`#8BC34A` 緑 |

### ブラシ
| 設定 | 既定値 | 範囲 | 説明 |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | ブラシ半径（m） |
| `RadiusMin` | 2 | — | 調整時の最小半径 |
| `RadiusMax` | 60 | — | 調整時の最大半径 |
| `RadiusStep` | 1 | — | キー/ホイールごとの半径変化量 |
| `Softness` | 50 | 0–100 | エッジのソフトネス（0 = 硬い、100 = 非常に柔らかい） |
| `FalloffPower` | 3 | 1–5 | フォールオフ指数（尖りモードのみ） |
| `RaiseLowerMeters` | 2 | 0.5–10 | 上昇/下降 1 回あたりのメートル |
| `PaintStrength` | 1 | 0–1 | 塗装強度 |
| `DefaultOperation` | Flatten | — | 開始時の操作 |
| `Shape` | Circle | — | 開始時の形状 |
| `Paint` | Dirt | — | 開始時の塗装 |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | 固定ランプ角度（度） |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | 水面より上の岸マージン（m） |
| `UseFixedHeight` | false | — | 固定高さを使用（スポイト状態） |
| `FixedHeight` | 30 | -10–150 | 平坦化の正確な高さ（m） |

### Vanilla / 連携
- `BlockVanillaTerrainTools`: false — 杖を装備しているとき、バニラのクワ/カルティベーターの地形編集を無視します。設定にトグルもあります。

### 入力
すべてのショートカットはキータブで再割り当て可能です。主なもの:
- `ApplyKey` (Mouse1)、`RadiusUp/Down` (=/-)、`CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z)、`RedoKey` (Ctrl+Y)、`EyedropKey` (G)、`RampModeKey` (H)
- `RadialKey` (Q)、`AdjustModeKey` (V)、`GiveToolKey` (F8)、`RebindMenuKey` (F5)
- **操作の直接キー**（9 個、既定なし）: OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`、`CycleStampKey`、`CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool（既定）
- `AutoGiveTool`: true（参加時に杖を付与）

### スタンプ
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool（既定 Same as Brush）

### プレビュー
- `ShowPreview`: true、`ShowGuides`: true（リーチガイド、傾斜の着色、レベルリング）

### 履歴
- `UndoDepth`: 50（0–500、0 で履歴を無効化）

### サーバー (Config Sync による権威付け)
- `AllowTerrainEdits`: true（マスタースイッチ）
- `AdminOnlyEdits`: false（管理者のみ）
- `RespectWards`: true（他人の結界をブロック）
- `RadiusCap` / `DeltaCap`: 0（オフ）— サーバー適用の上限

### パフォーマンス
- `TilesPerFrame`: 4（1–32 タイル/フレーム）

---

## 🐛 トラブルシューティング

[Overview](Overview) · | 問題 | 解決策 |
|-------|----------|
| **メニューが表示されない / カーソルが見えない** | ゲームを閉じます。`%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` と `%APPDATA%\Thunderstore Mod Manager\PackageCache\` を削除します。TMM を再起動し、更新を確認します。 |
| **TMM に更新が表示されない** | TMM → Settings → 「Refresh mod list」（Ctrl+R）。TMM を閉じて再度開きます。正しいプロファイルを確認します。 |
| **Thunderstore のバージョン競合** | `.csproj`、`Plugin.cs`、`manifest.json` のバージョンを上げます（0.3.3 → 0.3.4）。リビルド + 再アップロード。 |
| **スタンプのコピー/貼り付けが機能しない** | HUD: `STAMP READY [X/Y]` = 準備完了。「tool not equipped」の場合はツールを装備します（設定 → Stamp tool）。 |
| **プレビューが水中で見えない** | 0.3.2 で修正済み — プレビューは水面に浮きます。更新してください。 |
| **線/矩形/多角形に円が表示される** | 0.3.2 で修正済み — マークした輪郭のみが表示されます。更新してください。 |
| **ラジアルホイールが機能しない** | 0.3.2（`ZInput.GetMouseScrollWheel()`）と 0.3.4（カーソルが中央でも選択が消去されなくなりました）で修正済み。更新してください。 |
| **設定が保存されない** | メニュー（F5）を閉じて自動保存します。または `BepInEx/config/com.terrainforge.valheim.cfg` を直接編集します。 |
| **マルチプレイのデシンク** | クライアント+サーバーに MOD。両方に `UnlockDeltaLimit`。同じ `MaxUpDelta`/`MaxDownDelta`。 |
| **ビルドが失敗する** | `.csproj` または CLI で `ValheimDir`、`BepInExCoreDir` のパスを確認します。.NET SDK 8+ が必要です。 |
| **コンパイル / パッケージ** | `dotnet build -c Release`（DLL は `build/`）、`-p:Deploy=true`（プロファイルにインストール）または `-p:Package=true`（`build/` に zip）。 |

---

## 🔗 リンク

[Overview](Overview) · - **Thunderstore ページ:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub リポジトリ:** https://github.com/marceloanton/TerrainForge
- **寄付:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **変更履歴:** [Changelog](Changelog)

---

*TerrainForge — 制限なく Valheim をテラフォーム。*
