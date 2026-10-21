# TerrainForge 위키

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · *한국어* · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 업데이트

> Q 휠에 전용 복사/붙여넣기 모드가 추가되었습니다. 사각형은 적용으로 A/B, 선은 양 끝점, 다각형은 꼭짓점을 지정하고 3개 이상에서 두 번 눌러 복사하며, 원/정사각형은 브러시 범위를 복사합니다. HUD가 다음 단계를 안내합니다. 눈은 페인트 > DeepSnow에 통합되었습니다(Shift를 누르면 제거). F5에서는 로컬 이동과 카메라가 고정되며, Esc는 싱글 플레이 세계 전체를 일시 정지합니다.

## 목차

[Overview](Overview) · 1. [설치](#-설치)
2. [빠른 시작](#-빠른-시작)
3. [조작](#-조작)
4. [작업](#-작업)
5. [모양](#-모양)
6. [감쇠 모드](#-감쇠-모드-올리기내리기)
7. [스탬프 (복사/붙여넣기)](#-스탬프-복사붙여넣기)
8. [스냅샷 (백업)](#-스냅샷-백업)
9. [프로필](#-프로필)
10. [언어](#-언어)
11. [멀티플레이어](#-멀티플레이어)
12. [설정](#-설정)
13. [문제 해결](#-문제-해결)

---

## 📥 설치

[Overview](Overview) · ### 요구 사항
- **BepInEx 5** (x64, Mono) — 클라이언트와 서버 모두. Thunderstore 사용자는 이미 설치되어 있습니다.

### 단계
1. Thunderstore 또는 GitHub Releases에서 `TerrainForge-<version>.zip`을 다운로드합니다.
2. 압축을 풀고 `TerrainForge.dll`을 다음 위치에 놓습니다:
   - **클라이언트:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **서버:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. 게임 / 서버를 재시작합니다.

> ⚠️ ±8 m를 초과하는 편집과 올바른 복제를 위해서는 **클라이언트와 서버 모두에 설치해야 합니다**.

---

## 🚀 빠른 시작

[Overview](Overview) · 1. 월드에 접속합니다. **TerrainForge 막대기**(망치 복제품)가 자동으로 지급되고 장착됩니다.
2. `F5`를 눌러 메뉴를 엽니다 (`ESC` 일시정지 메뉴의 **TerrainForge** 버튼을 클릭해도 됩니다).
3. 막대기가 장착되어 있지 않다면 장착합니다.
4. 지형을 조준하고 **오른쪽 클릭**(`Mouse1`)을 길게 눌러 조형합니다.
5. `,`로 작업을, `.`로 모양을, `/`로 페인트를 순환합니다.
6. `Q`를 길게 누르면 → 방사형 메뉴가 열리고, 놓으면 확정됩니다.
7. `Ctrl+Z` / `Ctrl+Y`로 실행 취소/다시 실행합니다.

---

## ⌨️ 조작

[Overview](Overview) · | 동작 | 기본 키 | 설명 |
|--------|-------------|-------------|
| 브러시 적용 | `Mouse1` (오른쪽 클릭) 길게 | 크로스헤어 아래의 지형을 조형합니다 |
| 반경 + | `=` 또는 `Ctrl`+휠 | 브러시 반경을 늘립니다 |
| 반경 − | `-` 또는 `Ctrl`+휠 | 브러시 반경을 줄입니다 |
| 작업 순환 | `,` | 9가지 작업을 순환합니다 |
| 모양 순환 | `.` | 원 → 사각형 → 선 → Rect → 다각형 |
| 페인트 순환 | `/` | 6가지 페인트 유형을 순환합니다 |
| 발 높이로 평탄화 | `Left Shift` 길게 | 평탄화가 플레이어 높이를 사용합니다 |
| 실행 취소 | `Ctrl+Z` | 마지막 획을 취소합니다 |
| 다시 실행 | `Ctrl+Y` | 취소한 획을 다시 실행합니다 |
| 스포이트 | `G` | 크로스헤어 지점에서 평탄화 높이를 고정합니다 (다시 누르면 해제) |
| 램프 모드 | `H` | 순환: 두 점 → 고정 각도 → 경로 (또는 SETTINGS의 3개 버튼으로 선택) |
| 고정 램프 각도 | `Alt` 길게 + 반경/휠 | 고정 모드에서 각도를 조정합니다 |
| 방사형 메뉴 | `Q` 길게 | 마우스로 작업을 고르고, 놓으면 확정됩니다 |
| 조정 모드 | `V` | 휠 = 반경, 줌/바/지도를 고정합니다 |
| 미세 조정 (0.1 m) | `Shift` 길게 + 휠/반경 | 정밀한 반경 조정 |
| **스탬프 복사** | `Ctrl+Shift+C` | 표시한 영역을 클립보드로 복사합니다 |
| **스탬프 붙여넣기** | `Ctrl+Shift+V` | 고스트 미리보기, 클릭으로 확정 |
| 막대기 받기 | `F8` | TerrainForge 막대기를 지급합니다 |
| 메뉴 열기 | `F5` | 키/도움말/설정 메뉴를 엽니다 |

**추가 키 (기본값 없음, KEYS에서 지정하세요):**
- 직접 작업: 평탄화, 올리기, 내리기, 부드럽게, 페인트, 복원, 램프, 해안, 눈
- 다음 / 이전 프로필
- 다음 / 이전 스탬프 (기록)

**"Quick actions" 섹션 (KEYS 탭):** 필수 단축키가 없는 기능을 위한 버튼 — 실행 취소, 다시 실행, 높이 고정, 스탬프 복사/붙여넣기, 이전 스탬프, 표시 지우기, 히트맵, 그리고 재질 진단(셰이더 + 텍스처 슬롯을 로그에 덤프).

---

## 🔄 작업

[Overview](Overview) · | 작업 | 설명 |
|-----------|-------------|
| **평탄화** | 조준한 높이로 평탄화합니다 (Shift = 발 위치). 스포이트(`G`) 또는 SETTINGS 텍스트박스로 고정 높이를 지정합니다. |
| **올리기** | 적용할 때마다 지형을 `RaiseDelta`만큼 올립니다. 감쇠 모드가 형태를 결정합니다. |
| **내리기** | 적용할 때마다 지형을 `RaiseDelta`만큼 내립니다. 감쇠 모드가 형태를 결정합니다. |
| **부드럽게** | 실제 블러: 각 정점을 8개 이웃과 평균냅니다. 길게 누르면 점진적으로 적용됩니다. **모드:** 전체(양쪽), 능선만(내림), 움푹한 곳만(올림). 선 모양 = 경로를 스무딩합니다. |
| **램프** | **3가지 모드** (`H` 순환):<br>• **두 점**: A와 B를 표시 → 직선 경사가 높이를 연결합니다.<br>• **고정 각도**: B = 방향/길이; 높이 = `SlopeAngle` (크로스헤어에 따라 위/아래). `Alt`+반경으로 조정합니다.<br>• **경로**: 곡선 A→B→C… 를 표시 (3개 이상에서 두 번 누르면 닫힘) → 부드러운 Catmull-Rom.<br>너비 = 브러시 반경. 부드러움 = 측면 가장자리에서만 블렌드. 미리보기: 채워진 회랑 + A/B의 높이 링. |
| **해안** | 수면 높이 + `ShoreOffset` 여백을 향해 평탄화합니다 (해변). 부드러움은 평탄화와 동일합니다. |
| **페인트** | 유형: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. 강도 조절 가능. |
| **복원** | 지형을 원래 상태로 되돌립니다 (높이 + 페인트). |
| **복사/붙여넣기** | 지형을 선택하고 스탬프를 놓는 Q 휠 전용 모드입니다. |

---

## 📐 모양

[Overview](Overview) · `.` 키로 순환합니다. 모두 실행 취소를 지원합니다.

| 모양 | 표시 | 용도 |
|-------|---------|----------|
| **원** | 단일 클릭 (브러시 중심) | 일반 조형 |
| **사각형** | 단일 클릭 (중심) | 플랫폼, 기초 |
| **선** | 2회 클릭 (A → B) | 경로, 벽, 오솔길. 반너비 = 반경. |
| **직사각형** | 2회 클릭 (반대쪽 모서리) | 기초, 농장, 플랫폼. 부드러운 가장자리. |
| **다각형** | 정점마다 클릭, 더블 클릭으로 닫기 (정점 3개 이상) | 복잡한 모양, 커스텀 영역 |

- 비원형 모양은 **표시한 윤곽만** 보여줍니다 (원형 링 없음).
- 부드러움은 모든 모양에서 가장자리 블렌드에 영향을 줍니다.

---

## ⚙️ 감쇠 모드 (올리기/내리기)

[Overview](Overview) · SETTINGS → "Raise/Lower falloff"에서 구성합니다:

| 모드 | 동작 |
|------|----------|
| **뾰족** | 중앙 피크. "Falloff power" 슬라이더(1–5)로 제어됩니다. |
| **평면** | 균일한 원판 — 전체 반경에 걸쳐 동일한 가중치. |
| **부드러움** | 코사인 종 — 부드러운 전환, 딱딱한 경계 없음. |
| **선형** | 중심에서 가장자리로 직선 테이퍼. |

---

## 🧱 스탬프 (복사/붙여넣기)

[Overview](Overview) · 지형 구조를 복제하는 강력한 기능입니다.

### 사각형 선택 (권장)

Q 휠에서 복사/붙여넣기를 선택하세요. 적용을 눌러 영역을 지정합니다. 사각형은 두 모서리, 선은 양 끝점, 다각형은 꼭짓점(3개 이상에서 두 번 누름), 원/정사각형은 현재 브러시 범위를 사용합니다. HUD가 다음 단계를 표시합니다.

### 워크플로

[Overview](Overview) · 1. **영역 표시**: 아무 모양이나 사용합니다 (원, Rect, 선, 다각형, 램프) — 또는 위의 사각형 선택.
2. **복사**: `Ctrl+Shift+C` → "스탬프 복사됨: X tiles" (RAM 클립보드).
3. **붙여넣기 미리보기**: `Ctrl+Shift+V` → 크로스헤어에 고스트 와이어프레임.
   - `Q`/`E` = ±15° 회전
   - `Alt`+휠 = 연속 회전
   - `Shift`+휠 = 0.25×–4× 크기 조절
   - `Esc` = 취소
4. **확정**: 붙여넣기 키를 다시 누르면 미리보기를 확정합니다.

> 선택 스탬프는 **복사한 영역 내부에만** 기록합니다; 그 밖의 지형은 손대지 않습니다.

### 기록
- RAM에 최대 **10개 스탬프** (최근 순).
- `다음 스탬프` / `이전 스탬프` 키(재지정 가능)로 순환합니다.
- 플래시: "스탬프 기록: Copy: 3 tiles [2/5]"

### 영속성
- **SETTINGS → STAMPS**:
  - 현재 스탬프를 디스크에 저장합니다 (`BepInEx/config/TerrainForge/stamps/`의 `.stamp`, 최대 12개).
  - 스탬프를 클립보드로 불러옵니다.
  - 저장된 스탬프를 나열/삭제합니다.

### 스탬프 도구
- SETTINGS → "Stamp tool" (4가지 옵션):
  - `Same as brush` (기본값) — 브러시와 동일한 도구를 사용합니다
  - `Any` — 요구 사항 없음
  - `Pickaxe` — 곡괭이 장착 필요
  - `TerrainForge Rod` — 모드의 막대기 필요
- 도구가 장착되어 있고 클립보드에 스탬프가 있으면 HUD가 `STAMP READY [2/5]`를 표시합니다.
- 도구가 없으면: `Stamp: tool not equipped`, 복사/붙여넣기가 차단됩니다.

---

## 💾 스냅샷 (백업)

[Overview](Overview) · 전체 지형 백업/복원.

### 저장
1. SETTINGS → **SNAPSHOTS**.
2. 이름 (비우면 자동 날짜/시간).
3. **Save** → 편집한 타일을 `BepInEx/config/TerrainForge/snapshots/<name>.dat`에 저장합니다 (최대 12개).

### 불러오기
1. 스냅샷을 선택합니다.
2. **Load** → 소유권과 함께 타일을 복원합니다, **1 실행 취소 단계**.
3. 로드되지 않은 타일은 깔끔하게 경고합니다 — 원래 위치 근처에서 가져오세요.

### 기능
- 버전이 관리되는 이진 형식 (`.dat`).
- 높이 + smooth + 페인트 + 소유권을 복원합니다.
- 가져오기는 실행 취소할 수 있습니다 (`Ctrl+Z`로 가져오기를 되돌립니다).
- 로드되지 않은 타일은 깔끔하게 경고합니다.

---

## 💾 프로필

[Overview](Overview) · 완전한 브러시 구성을 저장/불러옵니다.

- SETTINGS → **PROFILES**.
- 이름 + **Save** → 반경, 부드러움, 작업, 모양, 페인트, 램프 모드, 각도, 스무딩 모드, 올리기 델타, 페인트 강도, 감쇠, 해안 오프셋.
- **Load** → 즉시 적용됩니다.
- 최대 12개 프로필 (`Presets > Slots`의 culture-invariant 형식).
- 키 바인딩: `Next Profile` (KEYS에서 지정) 로 순환합니다.

---

## 🌐 언어

[Overview](Overview) · **15가지 옵션:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — 기본값 `Auto`.

### 자동 감지
- 기본값 `Auto`는 리플렉션을 통해 게임 언어를 감지합니다: `Localization.instance.GetSelectedLanguage()`.
- Valheim 언어 → 내부 인덱스로 매핑합니다.
- 언어에 완전한 번역이 없으면 → English로 대체됩니다.
- SETTINGS → "Language"에서 구성할 수 있습니다 (15가지 옵션 순환).
- 메뉴를 열 때 키 레이블이 다시 만들어집니다.

### 번역 상태
- **완료:** English, Español
- **English 대체:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- 커뮤니티 번역은 PR로 환영합니다.

### 이 문서
번역된 위키 페이지: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 멀티플레이어

[Overview](Overview) · ### 요구 사항
- ±8 m를 초과하는 편집과 올바른 복제를 위해서는 **클라이언트와 서버 모두에 모드**가 필요합니다.
- 서버에도 `UnlockDeltaLimit`를 설정해야 합니다, 그렇지 않으면 서버가 지형을 제한합니다.

### 서버 권한 (Config Sync)
서버가 모드를 실행하면 서버의 `Server` 섹션이 우선합니다:
- `AllowTerrainEdits` (기본값 `true`) — 마스터 스위치; `false`이면 클라이언트는 적용/실행 취소/다시 실행할 수 없습니다.
- `AdminOnlyEdits` (기본값 `false`) — 관리자/호스트만 편집할 수 있습니다.
- `RespectWards` (기본값 `true`) — 다른 사람의 ward 내부 편집을 차단합니다.
- `RadiusCap` / `DeltaCap` (0 = off) — 서버가 강제하는 상한, HUD+미리보기에 표시됩니다.

> 모드가 **없는** 서버는 이 섹션을 무시합니다 → 클라이언트는 테스트용으로 작동합니다.

### 디싱크 방지
- 서버 + 클라이언트에서 `MaxUpDelta` / `MaxDownDelta`를 일관되게 유지하세요.
- 큰 반경은 모드가 없는 클라이언트를 디싱크시킬 수 있습니다.

---

## ⚙️ 설정

[Overview](Overview) · 파일: `BepInEx/config/com.terrainforge.valheim.cfg`

### 일반
| 설정 | 기본값 | 설명 |
|---------|---------|-------------|
| `Enabled` | `true` | 마스터 스위치 |
| `PlayEffects` | `true` | 적용 시 먼지 + 타격 |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | TerrainForge 막대기(망치 복제품)에 색을 입혀 바닐라 망치와 쉽게 구분할 수 있게 합니다 |
| `ToolTintColor` | `#4FC3F7` | 막대기 헥스 색상 (모델 + 아이콘). 예: `#FFC400` 금색, `#8BC34A` 초록 |

### 브러시
| 설정 | 기본값 | 범위 | 설명 |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | 브러시 반경 (m) |
| `RadiusMin` | 2 | — | 조정 시 최소 반경 |
| `RadiusMax` | 60 | — | 조정 시 최대 반경 |
| `RadiusStep` | 1 | — | 키/휠당 반경 변화량 |
| `Softness` | 50 | 0–100 | 가장자리 부드러움 (0 = 딱딱, 100 = 매우 부드러움) |
| `FalloffPower` | 3 | 1–5 | 감쇠 지수 (뾰족 모드 전용) |
| `RaiseLowerMeters` | 2 | 0.5–10 | 올리기/내리기 적용당 미터 |
| `PaintStrength` | 1 | 0–1 | 페인트 강도 |
| `DefaultOperation` | Flatten | — | 시작 작업 |
| `Shape` | Circle | — | 시작 모양 |
| `Paint` | Dirt | — | 시작 페인트 |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | 고정 램프 각도 (도) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | 수면 위 해안 여백 (m) |
| `UseFixedHeight` | false | — | 고정 높이 사용 (스포이트 상태) |
| `FixedHeight` | 30 | -10–150 | 정확한 평탄화 높이 (m) |

### 바닐라 / 통합
- `BlockVanillaTerrainTools`: false — 막대기를 장착하면 바닐라 괭이/경작기의 지형 편집을 무시합니다. SETTINGS에도 토글이 있습니다.

### 입력
모든 단축키는 KEYS 탭에서 재지정할 수 있습니다. 주요 항목:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **직접 작업 키** (9개, 기본값 없음): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (기본값)
- `AutoGiveTool`: true (접속 시 막대기 지급)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (기본값 Same as Brush)

### Preview
- `ShowPreview`: true, `ShowGuides`: true (도달 가이드, 경사 틴트, 레벨 링)

### History
- `UndoDepth`: 50 (0–500, 0은 기록 비활성화)

### Server (Config Sync를 통한 권한 부여)
- `AllowTerrainEdits`: true (마스터 스위치)
- `AdminOnlyEdits`: false (관리자만)
- `RespectWards`: true (다른 사람의 ward 차단)
- `RadiusCap` / `DeltaCap`: 0 (off) — 서버가 강제하는 상한

### 성능
- `TilesPerFrame`: 4 (1–32 타일/프레임)

---

## 🐛 문제 해결

[Overview](Overview) · | 문제 | 해결 방법 |
|-------|----------|
| **메뉴가 나타나지 않음 / 커서가 보이지 않음** | 게임을 종료합니다. `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` 및 `%APPDATA%\Thunderstore Mod Manager\PackageCache\`를 삭제합니다. TMM을 재시작하고 업데이트를 확인합니다. |
| **TMM에 업데이트가 표시되지 않음** | TMM → Settings → "Refresh mod list" (Ctrl+R). TMM을 닫았다 다시 엽니다. 올바른 프로필인지 확인합니다. |
| **Thunderstore 버전 충돌** | `.csproj`, `Plugin.cs`, `manifest.json`에서 버전을 올립니다 (0.3.3 → 0.3.4). 다시 빌드 + 다시 업로드합니다. |
| **스탬프 복사/붙여넣기가 아무것도 하지 않음** | HUD: `STAMP READY [X/Y]` = 준비됨. "tool not equipped"라고 나오면 도구를 장착합니다 (SETTINGS → Stamp tool). |
| **물속에서 미리보기가 보이지 않음** | 0.3.2에서 수정됨 — 미리보기가 수면에 떠 있습니다. 업데이트하세요. |
| **선/Rect/Poly에 원이 표시됨** | 0.3.2에서 수정됨 — 표시한 윤곽만 표시됩니다. 업데이트하세요. |
| **방사형 휠이 작동하지 않음** | 0.3.2 (`ZInput.GetMouseScrollWheel()`) 및 0.3.4 (커서가 중앙에 있어도 선택이 더 이상 지워지지 않음)에서 수정됨. 업데이트하세요. |
| **Config가 저장되지 않음** | 메뉴를 닫으면(F5) 자동 저장됩니다. 또는 `BepInEx/config/com.terrainforge.valheim.cfg`를 직접 편집합니다. |
| **멀티플레이어 디싱크** | 클라이언트+서버에 모드. 둘 다 `UnlockDeltaLimit`. 동일한 `MaxUpDelta`/`MaxDownDelta`. |
| **빌드 실패** | `.csproj`의 `ValheimDir`, `BepInExCoreDir` 경로를 확인하거나 CLI로 확인합니다. .NET SDK 8+가 필요합니다. |
| **컴파일 / 패키징** | `dotnet build -c Release` (`build/`의 DLL), `-p:Deploy=true` (프로필에 설치) 또는 `-p:Package=true` (`build/`의 zip). |

---

## 🔗 링크

[Overview](Overview) · - **Thunderstore 페이지:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub 저장소:** https://github.com/marceloanton/TerrainForge
- **후원:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [Changelog](Changelog)

---

*TerrainForge — 한계 없이 Valheim을 조형하세요.*
