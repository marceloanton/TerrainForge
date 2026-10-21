# TerrainForge Wiki

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · *繁體中文* · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 更新

> Q 圓盤新增專用「複製/貼上」模式。矩形用「套用」標記 A/B；直線標記兩端；多邊形標記頂點並在 3 點以上雙擊複製；圓形/正方形複製筆刷範圍。HUD 會提示下一步。雪已整合至塗裝 > DeepSnow（按住 Shift 可移除）。F5 會鎖定本機移動與鏡頭；Esc 會暫停單人世界。 0.3.6：F5 分頁改為兩列，並依目前操作顯示對應設定。Ramp 提供兩點、固定角度與連續路徑模式；圓形坡道列入 0.4.0 計畫。

## 目錄

[Overview](Overview) · 1. [安裝](#-安裝)
2. [快速開始](#-快速開始)
3. [控制](#-控制)
4. [操作](#-操作)
5. [形狀](#-形狀)
6. [衰減模式](#-衰減模式升高降低)
7. [印記（複製/貼上）](#-印記複製貼上)
8. [快照（備份）](#-快照備份)
9. [設定檔](#-設定檔)
10. [語言](#-語言)
11. [多人遊戲](#-多人遊戲)
12. [設定](#-設定)
13. [疑難排解](#-疑難排解)

---

## 📥 安裝

[Overview](Overview) · ### 需求
- **BepInEx 5**（x64、Mono）安裝於客戶端與伺服器。Thunderstore 使用者已內建。

### 步驟
1. 從 Thunderstore 或 GitHub Releases 下載 `TerrainForge-<version>.zip`。
2. 解壓縮並將 `TerrainForge.dll` 放到：
   - **客戶端：** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **伺服器：** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. 重新啟動遊戲 / 伺服器。

> ⚠️ **必須同時安裝於客戶端與伺服器**，才能進行大於 ±8 m 的編輯並正確同步。

---

## 🚀 快速開始

[Overview](Overview) · 1. 加入一個世界。**TerrainForge 法杖**（鐵鎚的複製品）會自動給予並裝備。
2. 按 `F5` 開啟選單（或點擊 `ESC` 暫停選單中的 **TerrainForge** 按鈕）。
3. 若法杖尚未裝備，請裝備它。
4. 瞄準地形並按住 **右鍵**（`Mouse1`）進行雕塑。
5. 使用 `,` 循環操作，`.` 切換形狀，`/` 切換塗裝。
6. 按住 `Q` → 環形選單，放開即確認。
7. `Ctrl+Z` / `Ctrl+Y` 復原/重做。

---

## ⌨️ 控制

[Overview](Overview) · | 動作 | 預設按鍵 | 說明 |
|--------|-------------|-------------|
| 套用筆刷 | 按住 `Mouse1`（右鍵） | 在準心下方雕塑地形 |
| 半徑 + | `=` 或 `Ctrl`+滾輪 | 增加筆刷半徑 |
| 半徑 − | `-` 或 `Ctrl`+滾輪 | 減少筆刷半徑 |
| 循環操作 | `,` | 循環 9 種操作 |
| 循環形狀 | `.` | 圓形 → 方形 → 線 → 矩形 → 多邊形 |
| 循環塗裝 | `/` | 循環 6 種塗裝類型 |
| 整平至腳下 | 按住 `Left Shift` | 整平使用玩家高度 |
| 復原 | `Ctrl+Z` | 復原上一個筆觸 |
| 重做 | `Ctrl+Y` | 重做已復原的筆觸 |
| 取色器 | `G` | 將整平高度固定在準心處（再按一次 = 解除） |
| 斜坡模式 | `H` | 循環：兩點 → 固定角度 → 路徑（或在設定中以 3 個按鈕選取） |
| 固定斜坡角度 | 按住 `Alt` + 半徑/滾輪 | 在固定模式中調整角度 |
| 環形選單 | 按住 `Q` | 以滑鼠選取操作，放開即確認 |
| 調整模式 | `V` | 滾輪 = 半徑，凍結縮放/快捷列/地圖 |
| 微調（0.1 m） | 按住 `Shift` + 滾輪/半徑 | 精確調整半徑 |
| **複製印記** | `Ctrl+Shift+C` | 將標記區域複製到剪貼簿 |
| **貼上印記** | `Ctrl+Shift+V` | 幽靈預覽，點擊即確認 |
| 取得法杖 | `F8` | 給予 TerrainForge 法杖 |
| 開啟選單 | `F5` | 開啟按鍵/說明/設定選單 |

**額外按鍵（無預設，於按鍵中指定）：**
- 直接操作：整平、升高、降低、平滑、塗裝、還原、斜坡、海岸、雪地
- 下一個 / 上一個設定檔
- 下一個 / 上一個印記（歷史）

**「快速動作」區段（按鍵分頁）：** 用於沒有強制快捷鍵的功能按鈕 — 復原、重做、固定高度、複製/貼上印記、上一個印記、清除標記、熱圖以及材質診斷（將 shader + 材質插槽傾印至日誌）。

---

## 🔄 操作

[Overview](Overview) · | 操作 | 說明 |
|-----------|-------------|
| **整平** | 整平至瞄準的高度（Shift = 在腳下）。透過取色器（`G`）或設定的文字方塊固定高度。 |
| **升高** | 每次套用將地形升高 `RaiseDelta`。由衰減模式塑形。 |
| **降低** | 每次套用將地形降低 `RaiseDelta`。由衰減模式塑形。 |
| **平滑** | 真正的模糊：將每個頂點與其 8 個鄰點平均。按住可漸進。**模式：** 完整（兩者）、僅山脊（降低）、僅凹陷（升高）。線形狀 = 平滑一條路徑。 |
| **斜坡** | **3 種模式**（循環 `H`）：<br>• **兩點**：標記 A 與 B → 一條直線斜坡連結兩處高度。<br>• **固定角度**：B = 方向/長度；高度 = `SlopeAngle`（依準心上升/下降）。以 `Alt`+半徑調整。<br>• **路徑**：標記一條曲線 A→B→C…（3 點以上時雙擊收尾）→ 平滑 Catmull-Rom。<br>寬度 = 筆刷半徑。柔和度 = 僅在側邊邊緣混合。預覽：填充的走廊 + A/B 處的高度環。 |
| **海岸** | 整平至水位 + `ShoreOffset` 邊距（海灘）。柔和度如同整平。 |
| **塗裝** | 類型：Dirt、Cultivated、Paved、ClearVegetation、DeepSnow、Reset。可調整強度。 |
| **還原** | 將地形回復至原始狀態（高度 + 塗裝）。 |
| **複製/貼上** | Q 圓盤中的專用模式，可選取地形並放置 stamp。 |

---

## 📐 形狀

[Overview](Overview) · 以 `.` 鍵循環。全部支援復原。

| 形狀 | 標記 | 用途 |
|-------|---------|----------|
| **圓形** | 單擊（筆刷中心） | 一般雕塑 |
| **方形** | 單擊（中心） | 平台、地基 |
| **線** | 2 次點擊（A → B） | 路徑、牆、步道。半寬 = 半徑。 |
| **矩形** | 2 次點擊（對角） | 地基、農場、平台。柔和邊緣。 |
| **多邊形** | 每個頂點點擊，雙擊收尾（3 個以上頂點） | 複雜形狀、自訂區域 |

- 非圓形僅顯示 **標記的輪廓**（無圓環）。
- 柔和度影響所有形狀的邊緣混合。

---

## ⚙️ 衰減模式（升高/降低）

[Overview](Overview) · 於設定 → 「升高/降低衰減」中配置：

| 模式 | 行為 |
|------|----------|
| **尖銳** | 中央峰值。由「衰減強度」滑桿（1–5）控制。 |
| **平坦** | 均勻圓盤 — 整個半徑權重相同。 |
| **柔和** | 餘弦鐘形 — 平滑過渡，無硬邊。 |
| **線性** | 從中心到邊緣直線遞減。 |

---

## 🧱 印記（複製/貼上）

[Overview](Overview) · 用於複製地形結構的強大功能。

### 矩形選取（推薦）

在 Q 圓盤選擇「複製/貼上」。按「套用」標記範圍：矩形選兩個角、直線選兩端、多邊形選頂點（3 點以上雙擊），圓形/正方形則使用目前筆刷範圍。HUD 會提示下一步。

### 工作流程

[Overview](Overview) · 1. **標記區域**：使用任意形狀（圓形、矩形、線、多邊形、斜坡）— 或上述的矩形選取。
2. **複製**：`Ctrl+Shift+C` → 「Stamp copied: X tiles」（RAM 剪貼簿）。
3. **貼上預覽**：`Ctrl+Shift+V` → 準心處的幽靈線框。
   - `Q`/`E` = 旋轉 ±15°
   - `Alt`+滾輪 = 連續旋轉
   - `Shift`+滾輪 = 縮放 0.25×–4×
   - `Esc` = 取消
4. **Confirm**: 再次按下貼上快捷鍵以確認預覽。

> 選取印記僅寫入**複製區域內**；其外的地形保持不變。

### 歷史
- RAM 中最多 **10 個印記**（最新在前）。
- `下一個印記` / `上一個印記` 按鍵（可重新綁定）可循環。
- 提示：「Stamp history: Copy: 3 tiles [2/5]」

### 持久化
- **設定 → 印記**：
  - 將目前印記儲存至磁碟（`.stamp` 位於 `BepInEx/config/TerrainForge/stamps/`，最多 12 個）。
  - 載入印記至剪貼簿。
  - 列出/刪除已儲存的印記。

### 印記工具
- 設定 → 「印記工具」（4 個選項）：
  - `與筆刷相同`（預設）— 使用與筆刷相同的工具
  - `任意` — 無要求
  - `鎬` — 需要裝備鎬
  - `TerrainForge 法杖` — 需要模組的法杖
- 當工具已裝備 + 剪貼簿中有印記時，HUD 會顯示 `STAMP READY [2/5]`。
- 若無工具：`Stamp: tool not equipped`，複製/貼上被封鎖。

---

## 💾 快照（備份）

[Overview](Overview) · 完整的地形備份/還原。

### 儲存
1. 設定 → **快照**。
2. 名稱（留空 = 自動日期/時間）。
3. **儲存** → 已編輯的 tiles 至 `BepInEx/config/TerrainForge/snapshots/<name>.dat`（最多 12 個）。

### 載入
1. 選擇一個快照。
2. **載入** → 還原 tiles 及其歸屬，**1 步復原**。
3. 未載入的 tiles 會乾淨地警告 — 請在原始位置附近匯入。

### 功能
- 版本化二進位格式（`.dat`）。
- 還原高度 + 平滑 + 塗裝 + 歸屬。
- 匯入可復原（`Ctrl+Z` 還原匯入）。
- 未載入的 tiles 會乾淨地警告。

---

## 💾 設定檔

[Overview](Overview) · 儲存/載入完整的筆刷配置。

- 設定 → **設定檔**。
- 名稱 + **儲存** → 半徑、柔和度、操作、形狀、塗裝、斜坡模式、角度、平滑模式、升高增量、塗裝強度、衰減、海岸偏移。
- **載入** → 立即套用。
- 最多 12 個設定檔（culture-invariant 格式，位於 `Presets > Slots`）。
- 按鍵綁定：`Next Profile`（於按鍵中指定）以循環。

---

## 🌐 語言

[Overview](Overview) · **15 個選項：** `Auto`、`English`、`Español`、`Français`、`Deutsch`、`Русский`、`中文`、`繁體中文`、`日本語`、`한국어`、`Polski`、`Português`、`Türkçe`、`Українська`、`Italiano` — 預設 `Auto`。

### 自動偵測
- 預設的 `Auto` 透過反射偵測遊戲語言：`Localization.instance.GetSelectedLanguage()`。
- 將 Valheim 語言 → 內部索引對應。
- 若某語言沒有完整翻譯 → 退回 English。
- 可於設定 → 「語言」中配置（循環 15 個選項）。
- 開啟選單時會重建按鍵標籤。

### 翻譯狀態
- **完整：** English、Español
- **退回 English：** Français、Deutsch、Русский、中文、繁體中文、日本語、한국어、Polski、Português、Türkçe、Українська、Italiano
- 歡迎透過 PR 提供社群翻譯。

### 本文件
已翻譯的 wiki 頁面：[English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)。

---

## 🌍 多人遊戲

[Overview](Overview) · ### 需求
- **客戶端與伺服器皆需安裝模組**，以進行大於 ±8 m 的編輯並正確同步。
- 伺服器也需 `UnlockDeltaLimit`，否則伺服器會夾限地形。

### 伺服器權威（設定同步）
當伺服器執行此模組時，其 `Server` 區段優先：
- `AllowTerrainEdits`（預設 `true`）— 主開關；若為 `false`，客戶端無法套用/復原/重做。
- `AdminOnlyEdits`（預設 `false`）— 僅管理員/主機能編輯。
- `RespectWards`（預設 `true`）— 封鎖他人結界內的編輯。
- `RadiusCap` / `DeltaCap`（0 = 關閉）— 伺服器強制的上限，顯示於 HUD+預覽中。

> **未安裝**模組的伺服器會忽略該區段 → 客戶端可用於測試。

### 防止不同步
- 在伺服器 + 客戶端保持 `MaxUpDelta` / `MaxDownDelta` 一致。
- 較大的半徑可能使未安裝模組的客戶端不同步。

---

## ⚙️ 設定

[Overview](Overview) · 檔案：`BepInEx/config/com.terrainforge.valheim.cfg`

### 一般
| 設定 | 預設 | 說明 |
|---------|---------|-------------|
| `Enabled` | `true` | 主開關 |
| `PlayEffects` | `true` | 套用時的塵土 + 打擊效果 |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | 為 TerrainForge 法杖（鐵鎚的複製品）著色，以便與原版鐵鎚輕鬆分辨 |
| `ToolTintColor` | `#4FC3F7` | 法杖的十六進位顏色（模型 + 圖示）。例如 `#FFC400` 金色、`#8BC34A` 綠色 |

### 筆刷
| 設定 | 預設 | 範圍 | 說明 |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | 筆刷半徑（m） |
| `RadiusMin` | 2 | — | 調整時的最小半徑 |
| `RadiusMax` | 60 | — | 調整時的最大半徑 |
| `RadiusStep` | 1 | — | 每次按鍵/滾輪的半徑變化 |
| `Softness` | 50 | 0–100 | 邊緣柔和度（0 = 硬，100 = 非常柔） |
| `FalloffPower` | 3 | 1–5 | 衰減指數（僅尖銳模式） |
| `RaiseLowerMeters` | 2 | 0.5–10 | 每次升高/降低套用的公尺數 |
| `PaintStrength` | 1 | 0–1 | 塗裝強度 |
| `DefaultOperation` | Flatten | — | 起始操作 |
| `Shape` | Circle | — | 起始形狀 |
| `Paint` | Dirt | — | 起始塗裝 |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | 固定斜坡角度（度） |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | 水面上方的海岸邊距（m） |
| `UseFixedHeight` | false | — | 使用固定高度（取色器狀態） |
| `FixedHeight` | 30 | -10–150 | 精確的整平高度（m） |

### 原版 / 整合
- `BlockVanillaTerrainTools`：false — 裝備法杖時，忽略原版鋤頭/耕耘機的地形編輯。設定中也有切換開關。

### 輸入
所有快捷鍵皆可在按鍵分頁中重新綁定。重點：
- `ApplyKey`（Mouse1）、`RadiusUp/Down`（=/-）、`CycleOp/Shape/Paint`（,/.//）
- `UndoKey`（Ctrl+Z）、`RedoKey`（Ctrl+Y）、`EyedropKey`（G）、`RampModeKey`（H）
- `RadialKey`（Q）、`AdjustModeKey`（V）、`GiveToolKey`（F8）、`RebindMenuKey`（F5）
- **直接操作鍵**（9 個，無預設）：OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`、`CycleStampKey`、`CycleStampBackKey`
- `RequiredTool`：Any / Pickaxe / TerrainForgeTool（預設）
- `AutoGiveTool`：true（加入時給予法杖）

### 印記
- `StampToolMode`：Same as Brush / Any / Pickaxe / TerrainForgeTool（預設 Same as Brush）

### 預覽
- `ShowPreview`：true，`ShowGuides`：true（觸及範圍輔助、坡度著色、高度環）

### 歷史
- `UndoDepth`：50（0–500，0 停用歷史）

### 伺服器（透過設定同步為權威）
- `AllowTerrainEdits`：true（主開關）
- `AdminOnlyEdits`：false（僅管理員）
- `RespectWards`：true（封鎖他人結界）
- `RadiusCap` / `DeltaCap`：0（關閉）— 伺服器強制的上限

### 效能
- `TilesPerFrame`：4（1–32 tiles/frame）

---

## 🐛 疑難排解

[Overview](Overview) · | 問題 | 解決方法 |
|-------|----------|
| **選單未出現 / 游標不可見** | 關閉遊戲。刪除 `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` 與 `%APPDATA%\Thunderstore Mod Manager\PackageCache\`。重新啟動 TMM，檢查更新。 |
| **TMM 未顯示更新** | TMM → Settings → 「Refresh mod list」（Ctrl+R）。關閉/重新開啟 TMM。確認正確的設定檔。 |
| **Thunderstore 版本衝突** | 在 `.csproj`、`Plugin.cs`、`manifest.json` 中提升版本（0.3.3 → 0.3.4）。重建 + 重新上傳。 |
| **印記複製/貼上無作用** | HUD：`STAMP READY [X/Y]` = 就緒。若顯示「工具未裝備」，裝備工具（設定 → 印記工具）。 |
| **預覽在水下不可見** | 已於 0.3.2 修正 — 預覽會浮在水面上。請更新。 |
| **線/矩形/多邊形上出現圓圈** | 已於 0.3.2 修正 — 僅顯示標記的輪廓。請更新。 |
| **環形滾輪無作用** | 已於 0.3.2（`ZInput.GetMouseScrollWheel()`）與 0.3.4（游標置中時選取不再被清除）修正。請更新。 |
| **設定無法儲存** | 關閉選單（F5）以自動儲存。或直接編輯 `BepInEx/config/com.terrainforge.valheim.cfg`。 |
| **多人遊戲不同步** | 客戶端+伺服器皆安裝模組。兩者皆需 `UnlockDeltaLimit`。相同的 `MaxUpDelta`/`MaxDownDelta`。 |
| **建置失敗** | 檢查 `.csproj` 中的 `ValheimDir`、`BepInExCoreDir` 路徑或透過 CLI。需要 .NET SDK 8+。 |
| **編譯 / 打包** | `dotnet build -c Release`（DLL 位於 `build/`）、`-p:Deploy=true`（安裝至設定檔）或 `-p:Package=true`（zip 位於 `build/`）。 |

---

## 🔗 連結

[Overview](Overview) · - **Thunderstore 頁面：** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub 儲存庫：** https://github.com/marceloanton/TerrainForge
- **贊助：** [cafecito.app/byrka](https://cafecito.app/byrka)
- **更新日誌：** [Changelog](Changelog)

---

*TerrainForge — 無限制地改造 Valheim。*
