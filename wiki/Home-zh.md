# TerrainForge 维基

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · *中文* · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 更新

> Q 圓盤新增專用「複製/貼上」模式。矩形用「套用」標記 A/B；直線標記兩端；多邊形標記頂點並在 3 點以上雙擊複製；圓形/正方形複製筆刷範圍。HUD 會提示下一步。雪已整合至塗裝 > DeepSnow（按住 Shift 可移除）。F5 會鎖定本機移動與鏡頭；Esc 會暫停單人世界。 0.3.6：F5 选项卡改为两行，并按当前操作显示对应设置。Ramp 提供两点、固定角度和连续路径模式；圆形坡道列入 0.4.0 计划。

## 目录

[Overview](Overview) · 1. [安装](#-安装)
2. [快速开始](#-快速开始)
3. [控制](#-控制)
4. [操作](#-操作)
5. [形状](#-形状)
6. [衰减模式](#-衰减模式)
7. [图章（复制/粘贴）](#-图章复制粘贴)
8. [快照（备份）](#-快照备份)
9. [配置方案](#-配置方案)
10. [语言](#-语言)
11. [多人游戏](#-多人游戏)
12. [设置](#-设置)
13. [故障排查](#-故障排查)

---

## 📥 安装

[Overview](Overview) · ### 要求
- **BepInEx 5**（x64，Mono），客户端和服务器均需安装。Thunderstore 用户已自带。

### 步骤
1. 从 Thunderstore 或 GitHub Releases 下载 `TerrainForge-<version>.zip`。
2. 解压并将 `TerrainForge.dll` 放到：
   - **客户端：** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **服务器：** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. 重启游戏 / 服务器。

> ⚠️ **必须同时安装在客户端和服务器**，才能进行大于 ±8 m 的编辑并正确同步。

---

## 🚀 快速开始

[Overview](Overview) · 1. 加入一个世界。**TerrainForge 法杖**（锤子的克隆）会自动给予并装备。
2. 按 `F5` 打开菜单（或点击 `ESC` 暂停菜单中的 **TerrainForge** 按钮）。
3. 如果法杖未装备，先装备它。
4. 瞄准地形并按住**右键**（`Mouse1`）进行雕刻。
5. 用 `,` 切换操作，`.` 切换形状，`/` 切换涂装。
6. 按住 `Q` → 径向菜单，松开确认。
7. `Ctrl+Z` / `Ctrl+Y` 撤销/重做。

---

## ⌨️ 控制

[Overview](Overview) · | 操作 | 默认按键 | 说明 |
|------|----------|------|
| 应用笔刷 | 按住 `Mouse1`（右键） | 雕刻准星下的地形 |
| 半径 + | `=` 或 `Ctrl`+滚轮 | 增大笔刷半径 |
| 半径 − | `-` 或 `Ctrl`+滚轮 | 减小笔刷半径 |
| 切换操作 | `,` | 循环切换 9 种操作 |
| 切换形状 | `.` | 圆形 → 方形 → 直线 → 矩形 → 多边形 |
| 切换涂装 | `/` | 循环切换 6 种涂装类型 |
| 水平到脚下 | 按住 `Left Shift` | 平整使用玩家高度 |
| 撤销 | `Ctrl+Z` | 撤销上一笔 |
| 重做 | `Ctrl+Y` | 重做已撤销的一笔 |
| 吸管 | `G` | 将平整高度固定在准星处（再按一次 = 解除） |
| 坡道模式 | `H` | 循环：两点 → 固定角度 → 路径（或在设置中用 3 个按钮选择） |
| 固定坡道角度 | 按住 `Alt` + 半径/滚轮 | 在固定模式下调整角度 |
| 径向菜单 | 按住 `Q` | 用鼠标选择操作，松开确认 |
| 调整模式 | `V` | 滚轮 = 半径，冻结缩放/快捷栏/地图 |
| 精细调整（0.1 m） | 按住 `Shift` + 滚轮/半径 | 精确调整半径 |
| **复制图章** | `Ctrl+Shift+C` | 将标记区域复制到剪贴板 |
| **粘贴图章** | `Ctrl+Shift+V` | 幽灵预览，点击确认 |
| 获取法杖 | `F8` | 给予 TerrainForge 法杖 |
| 打开菜单 | `F5` | 打开按键/帮助/设置菜单 |

**额外按键（无默认值，在按键中分配）：**
- 直接操作：平整、抬高、降低、平滑、涂装、恢复、坡道、海岸、雪
- 下一个 / 上一个配置方案
- 下一个 / 上一个图章（历史记录）

**“快捷操作”部分（按键标签页）：** 为没有强制快捷键的功能提供按钮 —— 撤销、重做、固定高度、复制/粘贴图章、上一个图章、清除标记、热力图和材质诊断（将着色器 + 纹理槽转储到日志）。

---

## 🔄 操作

[Overview](Overview) · | 操作 | 说明 |
|------|------|
| **平整** | 平整到瞄准的高度（Shift = 到你脚下）。通过吸管（`G`）或设置文本框固定高度。 |
| **抬高** | 每次应用将地形抬高 `RaiseDelta`。衰减模式塑造其形状。 |
| **降低** | 每次应用将地形降低 `RaiseDelta`。衰减模式塑造其形状。 |
| **平滑** | 真正的模糊：将每个顶点与其 8 个相邻顶点取平均。按住可渐进。**模式：** 全部（两者）、仅山脊（降低）、仅凹陷（抬高）。直线形状 = 平滑一条路径。 |
| **坡道** | **3 种模式**（`H` 循环）：<br>• **两点**：标记 A 和 B → 一条直线斜坡连接两个高度。<br>• **固定角度**：B = 方向/长度；高度 = `SlopeAngle`（根据准星向上/向下）。用 `Alt`+半径调整。<br>• **路径**：标记曲线 A→B→C…（3+ 时双击闭合）→ 平滑 Catmull-Rom。<br>宽度 = 笔刷半径。柔和度 = 仅混合两侧边缘。预览：填充走廊 + A/B 处的高度环。 |
| **海岸** | 向水位 + `ShoreOffset` 边距平整（沙滩）。柔和度同平整。 |
| **涂装** | 类型：Dirt、Cultivated、Paved、ClearVegetation、DeepSnow、Reset。强度可调。 |
| **恢复** | 将地形恢复到原始状态（高度 + 涂装）。 |
| **複製/貼上** | Q 圓盤中的專用模式，可選取地形並放置 stamp。 |

---

## 📐 形状

[Overview](Overview) · 用 `.` 键循环。全部支持撤销。

| 形状 | 标记方式 | 用例 |
|------|----------|------|
| **圆形** | 单击（笔刷中心） | 通用雕刻 |
| **方形** | 单击（中心） | 平台、地基 |
| **直线** | 2 次点击（A → B） | 路径、墙壁、小径。半宽 = 半径。 |
| **矩形** | 2 次点击（对角） | 地基、农场、平台。边缘柔和。 |
| **多边形** | 每个顶点点击一次，双击闭合（3+ 顶点） | 复杂形状、自定义区域 |

- 非圆形只显示**标记的轮廓**（无圆形环）。
- 柔和度影响所有形状的边缘混合。

---

## ⚙️ 衰减模式（抬高/降低）

[Overview](Overview) · 在设置 → “抬高/降低衰减”中配置：

| 模式 | 行为 |
|------|------|
| **尖峰** | 中央峰值。由“衰减幂”滑块控制（1–5）。 |
| **平坦** | 均匀圆盘 —— 整个半径的权重相同。 |
| **平滑** | 余弦钟形 —— 平滑过渡，无硬边缘。 |
| **线性** | 从中心到边缘的直线渐变。 |

---

## 🧱 图章（复制/粘贴）

[Overview](Overview) · 用于复制地形结构的强大功能。

### 矩形选择（推荐）

在 Q 圓盤選擇「複製/貼上」。按「套用」標記範圍：矩形選兩個角、直線選兩端、多邊形選頂點（3 點以上雙擊），圓形/正方形則使用目前筆刷範圍。HUD 會提示下一步。

### 工作流程

[Overview](Overview) · 1. **标记区域**：使用任意形状（圆形、矩形、直线、多边形、坡道）—— 或上面的矩形选择。
2. **复制**：`Ctrl+Shift+C` → “图章已复制：X 瓦片”（RAM 剪贴板）。
3. **粘贴预览**：`Ctrl+Shift+V` → 准星处的幽灵线框。
   - `Q`/`E` = 旋转 ±15°
   - `Alt`+滚轮 = 连续旋转
   - `Shift`+滚轮 = 缩放 0.25×–4×
   - `Esc` = 取消
4. **确认**：再次按下贴上快捷键以确认预览。

> 选择图章**只写入复制的区域内**；其外的地形保持不变。

### 历史记录
- RAM 中最多 **10 个图章**（最新的在前）。
- `下一个图章` / `上一个图章` 键（可重新绑定）循环切换。
- Flash: "图章历史：复制：3 瓦片 [2/5]"

### 持久化
- **设置 → 图章**：
  - 将当前图章保存到磁盘（`.stamp` 位于 `BepInEx/config/TerrainForge/stamps/`，最多 12 个）。
  - 将图章加载到剪贴板。
  - 列出/删除已保存的图章。

### 图章工具
- 设置 → “图章工具”（4 个选项）：
  - `Same as brush`（默认）—— 使用与笔刷相同的工具
  - `Any` —— 无要求
  - `Pickaxe` —— 需要装备镐
  - `TerrainForge Rod` —— 需要本模组的法杖
- 装备了工具且剪贴板中有图章时，HUD 显示 `STAMP READY [2/5]`。
- 没有工具时：`Stamp: tool not equipped`，复制/粘贴被阻止。

---

## 💾 快照（备份）

[Overview](Overview) · 完整的地形备份/恢复。

### 保存
1. 设置 → **快照**。
2. 名称（留空 = 自动日期/时间）。
3. **保存** → 编辑过的瓦片保存到 `BepInEx/config/TerrainForge/snapshots/<name>.dat`（最多 12 个）。

### 加载
1. 选择一个快照。
2. **加载** → 带所有权恢复瓦片，**1 个撤销步骤**。
3. 未加载的瓦片会明确警告 —— 请在原始位置附近导入。

### 特性
- 带版本的二进制格式（`.dat`）。
- 恢复高度 + 平滑 + 涂装 + 所有权。
- 导入可撤销（`Ctrl+Z` 还原导入）。
- 未加载的瓦片会明确警告。

---

## 💾 配置方案

[Overview](Overview) · 保存/加载完整的笔刷配置。

- 设置 → **配置方案**。
- 名称 + **保存** → 半径、柔和度、操作、形状、涂装、坡道模式、角度、平滑模式、抬高增量、涂装强度、衰减、海岸偏移。
- **加载** → 立即应用。
- 最多 12 个配置方案（`Presets > Slots` 中的文化无关格式）。
- 快捷键：`Next Profile`（在按键中分配）以循环切换。

---

## 🌐 语言

[Overview](Overview) · **15 个选项：** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` —— 默认 `Auto`。

### 自动检测
- 默认的 `Auto` 通过反射检测游戏语言：`Localization.instance.GetSelectedLanguage()`。
- 将 Valheim 语言映射 → 内部索引。
- 如果某语言没有完整翻译 → 回退到 English。
- 可在设置 → “语言”中配置（循环切换 15 个选项）。
- 打开菜单时会重建按键标签。

### 翻译状态
- **完整：** English、Español
- **回退到 English：** Français、Deutsch、Русский、中文、繁體中文、日本語、한국어、Polski、Português、Türkçe、Українська、Italiano
- 欢迎通过 PR 提供社区翻译。

### 本文档
已翻译的 wiki 页面： [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)。

---

## 🌍 多人游戏

[Overview](Overview) · ### 要求
- **客户端和服务器都需安装模组**，才能进行大于 ±8 m 的编辑并正确同步。
- 服务器上也要 `UnlockDeltaLimit`，否则服务器会钳制地形。

### 服务器权威（Config Sync）
当服务器运行模组时，其 `Server` 部分优先生效：
- `AllowTerrainEdits`（默认 `true`）—— 主开关；如果为 `false`，客户端无法应用/撤销/重做。
- `AdminOnlyEdits`（默认 `false`）—— 仅管理员/主机可编辑。
- `RespectWards`（默认 `true`）—— 阻止在他人的结界（ward）内编辑。
- `RadiusCap` / `DeltaCap`（0 = 关闭）—— 服务器强制上限，在 HUD+预览中可见。

> **没有**模组的服务器会忽略该部分 → 客户端仍可用于测试。

### 防止失步
- 在服务器 + 客户端上保持 `MaxUpDelta` / `MaxDownDelta` 一致。
- 大半径可能导致没有模组的客户端失步。

---

## ⚙️ 设置

[Overview](Overview) · 文件：`BepInEx/config/com.terrainforge.valheim.cfg`

### 常规
| 设置 | 默认 | 说明 |
|------|------|------|
| `Enabled` | `true` | 主开关 |
| `PlayEffects` | `true` | 应用时的尘土 + 打击效果 |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | 为 TerrainForge 法杖（锤子的克隆）着色，便于与原生锤子区分 |
| `ToolTintColor` | `#4FC3F7` | 法杖十六进制颜色（模型 + 图标）。例如 `#FFC400` 金色，`#8BC34A` 绿色 |

### 笔刷
| 设置 | 默认 | 范围 | 说明 |
|------|------|------|------|
| `Radius` | 8 | 1–100 | 笔刷半径（m） |
| `RadiusMin` | 2 | — | 调整时的最小半径 |
| `RadiusMax` | 60 | — | 调整时的最大半径 |
| `RadiusStep` | 1 | — | 每次按键/滚轮的半径变化量 |
| `Softness` | 50 | 0–100 | 边缘柔和度（0 = 硬，100 = 非常柔） |
| `FalloffPower` | 3 | 1–5 | 衰减指数（仅尖峰模式） |
| `RaiseLowerMeters` | 2 | 0.5–10 | 每次抬高/降低的米数 |
| `PaintStrength` | 1 | 0–1 | 涂装强度 |
| `DefaultOperation` | Flatten | — | 起始操作 |
| `Shape` | Circle | — | 起始形状 |
| `Paint` | Dirt | — | 起始涂装 |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | 固定坡道角度（度） |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | 水位上方的海岸边距（m） |
| `UseFixedHeight` | false | — | 使用固定高度（吸管状态） |
| `FixedHeight` | 30 | -10–150 | 平整的精确高度（m） |

### 原生 / 集成
- `BlockVanillaTerrainTools`: false —— 装备法杖时，忽略原生锄头/耕具的地形编辑。设置中也有一个开关。

### 输入
所有快捷键都可在按键标签页重新绑定。重点：
- `ApplyKey`（Mouse1）、`RadiusUp/Down`（=/-）、`CycleOp/Shape/Paint`（,/.//）
- `UndoKey`（Ctrl+Z）、`RedoKey`（Ctrl+Y）、`EyedropKey`（G）、`RampModeKey`（H）
- `RadialKey`（Q）、`AdjustModeKey`（V）、`GiveToolKey`（F8）、`RebindMenuKey`（F5）
- **直接操作键**（9 个，无默认值）：OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`、`CycleStampKey`、`CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool（默认）
- `AutoGiveTool`: true（加入时给予法杖）

### 图章
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool（默认 Same as Brush）

### 预览
- `ShowPreview`: true，`ShowGuides`: true（触及范围参考、坡度着色、水平环）

### 历史记录
- `UndoDepth`: 50（0–500，0 禁用历史记录）

### 服务器（通过 Config Sync 具有权威性）
- `AllowTerrainEdits`: true（主开关）
- `AdminOnlyEdits`: false（仅管理员）
- `RespectWards`: true（阻止他人的结界）
- `RadiusCap` / `DeltaCap`: 0（关闭）—— 服务器强制上限

### 性能
- `TilesPerFrame`: 4（1–32 瓦片/帧）

---

## 🐛 故障排查

[Overview](Overview) · | 问题 | 解决方案 |
|------|----------|
| **菜单不显示 / 光标不可见** | 关闭游戏。删除 `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` 和 `%APPDATA%\Thunderstore Mod Manager\PackageCache\`。重启 TMM，检查更新。 |
| **TMM 中不显示更新** | TMM → Settings → "Refresh mod list"（Ctrl+R）。关闭/重新打开 TMM。确认配置文件正确。 |
| **Thunderstore 版本冲突** | 在 `.csproj`、`Plugin.cs`、`manifest.json` 中提升版本号（0.3.3 → 0.3.4）。重新构建 + 重新上传。 |
| **图章复制/粘贴无反应** | HUD：`STAMP READY [X/Y]` = 就绪。如果显示 “tool not equipped”，请装备工具（设置 → 图章工具）。 |
| **预览在水下不可见** | 已在 0.3.2 修复 —— 预览漂浮在水面上。请更新。 |
| **直线/矩形/多边形上出现圆形** | 已在 0.3.2 修复 —— 只显示标记的轮廓。请更新。 |
| **径向滚轮不工作** | 已在 0.3.2（`ZInput.GetMouseScrollWheel()`）和 0.3.4（光标居中时选择不再被清除）修复。请更新。 |
| **配置无法保存** | 关闭菜单（F5）以自动保存。或直接编辑 `BepInEx/config/com.terrainforge.valheim.cfg`。 |
| **多人游戏失步** | 客户端+服务器都安装模组。两者都设置 `UnlockDeltaLimit`。`MaxUpDelta`/`MaxDownDelta` 相同。 |
| **构建失败** | 检查 `.csproj` 中的 `ValheimDir`、`BepInExCoreDir` 路径或通过 CLI。需要 .NET SDK 8+。 |
| **编译 / 打包** | `dotnet build -c Release`（DLL 在 `build/`），`-p:Deploy=true`（安装到配置文件）或 `-p:Package=true`（zip 在 `build/`）。 |

---

## 🔗 链接

[Overview](Overview) · - **Thunderstore 页面：** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub 仓库：** https://github.com/marceloanton/TerrainForge
- **捐赠：** [cafecito.app/byrka](https://cafecito.app/byrka)
- **更新日志：** [Changelog](Changelog)

---

*TerrainForge —— 无限制地重塑 Valheim。*
