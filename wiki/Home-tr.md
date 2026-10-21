# TerrainForge Wiki

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · *Türkçe* · [Українська](Home-uk) · [Italiano](Home-it)

---


### TerrainForge 0.3.6 yenilikleri

> Q tekerinde ayrı bir Kopyala/Yapıştır modu var. Dikdörtgen Uygula ile A/B köşelerini, Çizgi iki ucu işaretler; Çokgen 3+ noktadan sonra çift basışla kopyalar; Daire/Kare fırça alanını kopyalar. HUD sonraki adımı gösterir. Kar, Boya > DeepSnow içindedir (kaldırmak için Shift'i basılı tutun). F5 yerel hareketi ve kamerayı dondurur; Esc tek oyunculu dünyayı tamamen duraklatır. 0.3.6: F5 sekmeleri iki sıraya yerleştirir ve seçili işleme uygun ayarları gösterir. Ramp; İki Nokta, Sabit Açı ve Bağlantılı Yol modlarını sunar; dairesel rampalar 0.4.0 için planlandı.

## İçindekiler

[Overview](Overview) · 1. [Kurulum](#-kurulum)
2. [Hızlı Başlangıç](#-hızlı-başlangıç)
3. [Kontroller](#-kontroller)
4. [İşlemler](#-işlemler)
5. [Şekiller](#-şekiller)
6. [Düşüş Modları](#-düşüş-modları)
7. [Stamps (Kopyala/Yapıştır)](#-stamps-kopyalayapıştır)
8. [Snapshots (Yedekler)](#-snapshots-yedekler)
9. [Profiller](#-profiller)
10. [Diller](#-diller)
11. [Çok Oyunculu](#-çok-oyunculu)
12. [Ayarlar](#-ayarlar)
13. [Sorun Giderme](#-sorun-giderme)

---

## 📥 Kurulum

[Overview](Overview) · ### Gereksinimler
- **BepInEx 5** (x64, Mono) istemci ve sunucuda. Thunderstore kullanıcılarında zaten vardır.

### Adımlar
1. `TerrainForge-<version>.zip` dosyasını Thunderstore veya GitHub Releases üzerinden indirin.
2. Çıkartın ve `TerrainForge.dll` dosyasını şuraya yerleştirin:
   - **İstemci:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Sunucu:** `<server>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Oyunu / sunucuyu yeniden başlatın.

> ⚠️ **İstemci VE sunucuda kurulu olmalıdır** ±8 m'den büyük düzenlemeler ve doğru replikasyon için.

---

## 🚀 Hızlı Başlangıç

[Overview](Overview) · 1. Bir dünyaya katılın. **TerrainForge Asası** (Çekicin bir klonu) otomatik olarak verilir ve kuşanılır.
2. Menüyü açmak için `F5` tuşuna basın (veya `ESC` duraklatma menüsündeki **TerrainForge** düğmesine tıklayın).
3. Kuşanılı değilse asayı kuşanın.
4. Araziye nişan alın ve şekillendirmek için **sağ tık** (`Mouse1`) tuşunu basılı tutun.
5. İşlemleri değiştirmek için `,`, şekiller için `.`, boyalar için `/`.
6. `Q` tuşunu basılı tutun → radyal menü, onaylamak için bırakın.
7. Geri al/yinele için `Ctrl+Z` / `Ctrl+Y`.

---

## ⌨️ Kontroller

[Overview](Overview) · | Eylem | Varsayılan tuş | Açıklama |
|--------|-------------|-------------|
| Fırçayı uygula | `Mouse1` basılı tut (Sağ Tık) | Nişangah altındaki araziyi şekillendirir |
| Yarıçap + | `=` veya `Ctrl`+Tekerlek | Fırça yarıçapını artırır |
| Yarıçap − | `-` veya `Ctrl`+Tekerlek | Fırça yarıçapını azaltır |
| İşlemi değiştir | `,` | 9 işlem arasında geçiş yapar |
| Şekli değiştir | `.` | Daire → Kare → Çizgi → Dikt → Çokgen |
| Boyayı değiştir | `/` | 6 boya tipi arasında geçiş yapar |
| Ayaklara düzle | `Left Shift` basılı tut | Düzleştirme, oyuncunun yüksekliğini kullanır |
| Geri al | `Ctrl+Z` | Son vuruşu geri alır |
| Yinele | `Ctrl+Y` | Geri alınan vuruşu yineler |
| Damlalık | `G` | Düzleştirme yüksekliğini nişangahta sabitler (tekrar = bırak) |
| Rampa modu | `H` | Geçiş: İki Nokta → Sabit Açı → Yol (veya AYARLAR'da 3 düğmeyle seç) |
| Sabit rampa açısı | `Alt` + yarıçap/tekerlek basılı tut | Sabit modda açıyı ayarlar |
| Radyal menü | `Q` basılı tut | Fareyle bir işlem seç, bırakınca onaylar |
| Ayar modu | `V` | Tekerlek = yarıçap, zoom/çubuğu/haritayı dondurur |
| İnce ayar (0.1 m) | `Shift` + tekerlek/yarıçap basılı tut | Hassas yarıçap ayarı |
| **Stamp Kopyala** | `Ctrl+Shift+C` | İşaretli alanı panoya kopyalar |
| **Stamp Yapıştır** | `Ctrl+Shift+V` | Hayalet önizleme, tıklama onaylar |
| Asa Al | `F8` | TerrainForge Asasını verir |
| Menüyü aç | `F5` | Tuşlar/yardım/ayarlar menüsünü açar |

**Ekstra tuşlar (varsayılan yok, TUŞLAR'da atayın):**
- Doğrudan işlem: Düzleştir, Yükselt, Alçalt, Yumuşat, Boya, Geri Yükle, Rampa, Kıyı, Kar
- Sonraki / önceki profil
- Sonraki / önceki stamp (geçmiş)

**"Hızlı eylemler" bölümü (TUŞLAR sekmesi):** zorunlu kısayolu olmayan işlevler için düğmeler — Geri Al, Yinele, Yüksekliği sabitle, Stamp kopyala/yapıştır, Önceki stamp, İşaretleri temizle, ısı haritaları ve Malzeme tanılaması (shader + doku yuvalarını loga döker).

---

## 🔄 İşlemler

[Overview](Overview) · | İşlem | Açıklama |
|-----------|-------------|
| **Düzleştir** | Hedeflenen yüksekliğe düzler (Shift = ayaklarınızda). Damlalık (`G`) veya AYARLAR metin kutusu üzerinden sabit yükseklik. |
| **Yükselt** | Araziyi uygulama başına `RaiseDelta` kadar yükseltir. Düşüş modu şeklini verir. |
| **Alçalt** | Araziyi uygulama başına `RaiseDelta` kadar alçaltır. Düşüş modu şeklini verir. |
| **Yumuşat** | Gerçek bulanıklık (blur): her köşeyi komşu 8 noktasıyla ortalar. Aşamalı için basılı tutun. **Modlar:** Tamamı (ikisi), Yalnız Sırtlar (alçaltır), Yalnız Çukurlar (yükseltir). Çizgi şekli = bir yolu yumuşatır. |
| **Rampa** | **3 mod** (`H` ile geçiş):<br>• **İki Nokta**: A ve B'yi işaretle → yükseklikleri düz bir eğim birleştirir.<br>• **Sabit Açı**: B = yön/uzunluk; yükseklik = `SlopeAngle` (nişangaha göre yukarı/aşağı). `Alt`+yarıçap ile ayarla.<br>• **Yol**: A→B→C… eğrisi işaretle (3+ ile çift basış kapatır) → düzgün Catmull-Rom.<br>Genişlik = fırça yarıçapı. Yumuşaklık = yalnız yan kenarlarda karışım. Önizleme: dolu koridor + A/B'de yükseklik halkaları. |
| **Kıyı** | Su seviyesine + `ShoreOffset` payına doğru düzler (plajlar). Yumuşaklık Düzleştir gibi. |
| **Boya** | Tipler: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Ayarlanabilir güç. |
| **Geri Yükle** | Araziyi orijinal durumuna döndürür (yükseklik + boya). |
| **Kopyala/Yapıştır** | Arazi seçmek ve stamp yerleştirmek için Q tekerinde özel mod. |

---

## 📐 Şekiller

[Overview](Overview) · `.` tuşuyla geçiş yapın. Hepsi geri almayı destekler.

| Şekil | İşaretleme | Kullanım alanı |
|-------|---------|----------|
| **Daire** | Tek tıklama (fırça merkezi) | Genel şekillendirme |
| **Kare** | Tek tıklama (merkez) | Platformlar, temeller |
| **Çizgi** | 2 tıklama (A → B) | Yollar, duvarlar, patikalar. Yarım genişlik = yarıçap. |
| **Dikdörtgen** | 2 tıklama (karşıt köşeler) | Temeller, çiftlikler, platformlar. Yumuşak kenarlar. |
| **Çokgen** | Köşe başına tıklama, çift tıklama kapatır (3+ köşe) | Karmaşık şekiller, özel alanlar |

- Dairesel olmayan şekiller **yalnızca işaretli dış çizgiyi** gösterir (dairesel halkalar yok).
- Yumuşaklık, tüm şekillerde kenar karışımını etkiler.

---

## ⚙️ Düşüş Modları (Yükselt/Alçalt)

[Overview](Overview) · AYARLAR → "Yükselt/Alçalt düşüşü" içinde yapılandırma:

| Mod | Davranış |
|------|----------|
| **Sivri** | Merkezi tepe. "Düşüş gücü" kaydırıcısıyla kontrol edilir (1–5). |
| **Düz** | Tekdüze disk — tüm yarıçap boyunca aynı ağırlık. |
| **Pürüzsüz** | Kosinüs çanı — pürüzsüz geçiş, sert kenar yok. |
| **Doğrusal** | Merkezden kenara düz daralma. |

---

## 🧱 Stamps (Kopyala/Yapıştır)

[Overview](Overview) · Arazi yapılarını çoğaltmak için güçlü bir özellik.

### Dikdörtgen seçimi (önerilir)

Q tekerinden Kopyala/Yapıştır'ı seçin. Uygula seçimi işaretler: Dikdörtgen için iki köşe, Çizgi için iki uç, Çokgen için köşeler (3+ noktadan sonra çift basış) veya Daire/Kare için fırça alanı. HUD sonraki adımı gösterir.

### İş akışı

[Overview](Overview) · 1. **Alanı işaretleyin**: herhangi bir şekil kullanın (Daire, Dikt, Çizgi, Çokgen, Rampa) — ya da yukarıdaki dikdörtgen seçimi.
2. **Kopyala**: `Ctrl+Shift+C` → "Stamp kopyalandı: X döşeme" (RAM panosu).
3. **Yapıştırma önizlemesi**: `Ctrl+Shift+V` → nişangahta hayalet tel kafes.
   - `Q`/`E` = ±15° döndür
   - `Alt`+Tekerlek = sürekli döndürme
   - `Shift`+Tekerlek = 0.25×–4× ölçekle
   - `Esc` = iptal
4. **Confirm**: Önizlemeyi onaylamak için yapıştırma kısayoluna tekrar basın.

> Seçim stamp'leri **yalnızca kopyalanan bölgenin içine** yazar; dışındaki arazi olduğu gibi bırakılır.

### Geçmiş
- RAM'de en fazla **10 stamp** (en yenisi önce).
- `Sonraki stamp` / `Önceki stamp` tuşları (yeniden atanabilir) aralarında geçiş yapar.
- Flaş: "Stamp geçmişi: Kopyala: 3 döşeme [2/5]"

### Kalıcılık
- **AYARLAR → STAMPS**:
  - Mevcut stamp'i diske kaydedin (`.stamp`, `BepInEx/config/TerrainForge/stamps/` içinde, en çok 12).
  - Bir stamp'i panoya yükleyin.
  - Kayıtlı stamp'leri listeleyin/silin.

### Stamp aracı
- AYARLAR → "Stamp aracı" (4 seçenek):
  - `Same as brush` (varsayılan) — fırçayla aynı aracı kullanır
  - `Any` — gereksinim yok
  - `Pickaxe` — bir kazma kuşanılmış olmalı
  - `TerrainForge Rod` — modun asasını gerektirir
- Araç kuşanılı + panoda stamp varken HUD `STAMP READY [2/5]` gösterir.
- Araç olmadan: `Stamp: tool not equipped`, kopyala/yapıştır engellenir.

---

## 💾 Snapshots (Yedekler)

[Overview](Overview) · Tam arazi yedekleme/geri yükleme.

### Kaydet
1. AYARLAR → **SNAPSHOTS**.
2. Ad (boş = otomatik tarih/saat).
3. **Kaydet** → düzenlenen döşemeler `BepInEx/config/TerrainForge/snapshots/<name>.dat` içine (en çok 12).

### Yükle
1. Bir snapshot seçin.
2. **Yükle** → döşemeleri sahiplikle geri yükler, **1 geri alma adımı**.
3. Yüklenmemiş döşemeler temiz şekilde uyarır — orijinal konuma yakın içe aktarın.

### Özellikler
- Sürümlenmiş ikili biçim (`.dat`).
- Yükseklik + yumuşatma + boya + sahiplik geri yükler.
- İçe aktarma geri alınabilir (`Ctrl+Z` içe aktarmayı geri alır).
- Yüklenmemiş döşemeler temiz şekilde uyarır.

---

## 💾 Profiller

[Overview](Overview) · Eksiksiz fırça yapılandırmalarını kaydedin/yükleyin.

- AYARLAR → **PROFILES**.
- Ad + **Kaydet** → yarıçap, yumuşaklık, işlem, şekil, boya, rampa modu, açı, yumuşatma modu, yükseltme deltası, boya gücü, düşüş, kıyı payı.
- **Yükle** → anında uygular.
- En çok 12 profil (`Presets > Slots` içinde kültürden bağımsız biçim).
- Tuş ataması: geçiş yapmak için `Next Profile` (TUŞLAR'da atayın).

---

## 🌐 Diller

[Overview](Overview) · **15 seçenek:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — varsayılan `Auto`.

### Otomatik Algılama
- Varsayılan `Auto`, oyun dilini yansıma (reflection) ile algılar: `Localization.instance.GetSelectedLanguage()`.
- Valheim dilini → dahili dizine eşler.
- Bir dilin tam çevirisi yoksa → English'e geri döner.
- AYARLAR → "Dil" içinde yapılandırılabilir (15 seçenek arasında geçiş yapar).
- Menü açıldığında tuş etiketleri yeniden oluşturulur.

### Çeviri durumu
- **Tamamlandı:** English, Español
- **English'e geri dönüş:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Topluluk çevirileri PR yoluyla memnuniyetle karşılanır.

### Bu dokümantasyon
Çevrilmiş wiki sayfaları: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Çok Oyunculu

[Overview](Overview) · ### Gereksinimler
- **İstemci VE sunucuda mod** ±8 m'den büyük düzenlemeler ve doğru replikasyon için.
- Sunucuda da `UnlockDeltaLimit`, aksi halde sunucu araziyi kısıtlar.

### Sunucu Yetkisi (Config Sync)
Sunucu modu çalıştırdığında, `Server` bölümü kazanır:
- `AllowTerrainEdits` (varsayılan `true`) — ana anahtar; `false` ise istemciler uygulayamaz/geri alamaz/yineleyemez.
- `AdminOnlyEdits` (varsayılan `false`) — yalnızca yöneticiler/sunucu sahibi düzenleyebilir.
- `RespectWards` (varsayılan `true`) — yabancı ward'lar içindeki düzenlemeleri engeller.
- `RadiusCap` / `DeltaCap` (0 = kapalı) — sunucu tarafından uygulanan üst sınırlar, HUD+önizlemede görünür.

> Modu **olmayan** sunucular bu bölümü yok sayar → istemci test için çalışır.

### Desync Önleme
- `MaxUpDelta` / `MaxDownDelta` değerlerini sunucu + istemcilerde tutarlı tutun.
- Büyük yarıçaplar, modu olmayan istemcilerin senkronsuz kalmasına neden olabilir.

---

## ⚙️ Ayarlar

[Overview](Overview) · Dosya: `BepInEx/config/com.terrainforge.valheim.cfg`

### Genel
| Ayar | Varsayılan | Açıklama |
|---------|---------|-------------|
| `Enabled` | `true` | Ana anahtar |
| `PlayEffects` | `true` | Uygularken toz + vuruş |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | TerrainForge Asasını (Çekicin bir klonu) renklendirir, böylece vanilla Çekiçten kolayca ayırt edilir |
| `ToolTintColor` | `#4FC3F7` | Asa hex rengi (model + simge). Örn. `#FFC400` altın, `#8BC34A` yeşil |

### Fırça
| Ayar | Varsayılan | Aralık | Açıklama |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Fırça yarıçapı (m) |
| `RadiusMin` | 2 | — | Ayar yaparken min yarıçap |
| `RadiusMax` | 60 | — | Ayar yaparken maks yarıçap |
| `RadiusStep` | 1 | — | Tuş/tekerlek başına yarıçap değişimi |
| `Softness` | 50 | 0–100 | Kenar yumuşaklığı (0 = sert, 100 = çok yumuşak) |
| `FalloffPower` | 3 | 1–5 | Düşüş üssü (yalnız Sivri modu) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Yükselt/Alçalt uygulaması başına metre |
| `PaintStrength` | 1 | 0–1 | Boya gücü |
| `DefaultOperation` | Flatten | — | Başlangıç işlemi |
| `Shape` | Circle | — | Başlangıç şekli |
| `Paint` | Dirt | — | Başlangıç boyası |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Sabit rampa açısı (derece) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Su üstü kıyı payı (m) |
| `UseFixedHeight` | false | — | Sabit yükseklik kullan (damlalık durumu) |
| `FixedHeight` | 30 | -10–150 | Tam Düzleştirme yüksekliği (m) |

### Vanilla / entegrasyon
- `BlockVanillaTerrainTools`: false — asa kuşanılıyken vanilla çapa/kültivatör arazi düzenlemesini yok sayar. Ayrıca AYARLAR'da bir anahtar vardır.

### Girdi
Tüm kısayollar TUŞLAR sekmesinde yeniden atanabilir. Öne çıkanlar:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Doğrudan işlem tuşları** (9, varsayılan yok): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (varsayılan)
- `AutoGiveTool`: true (katılırken asayı verir)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (varsayılan Same as Brush)

### Önizleme
- `ShowPreview`: true, `ShowGuides`: true (erişim kılavuzları, eğim tonu, seviye halkası)

### Geçmiş
- `UndoDepth`: 50 (0–500, 0 geçmişi devre dışı bırakır)

### Sunucu (Config Sync ile Yetkili)
- `AllowTerrainEdits`: true (ana anahtar)
- `AdminOnlyEdits`: false (yalnızca yöneticiler)
- `RespectWards`: true (yabancı ward'ları engeller)
- `RadiusCap` / `DeltaCap`: 0 (kapalı) — sunucu tarafından uygulanan üst sınırlar

### Performans
- `TilesPerFrame`: 4 (1–32 döşeme/kare)

---

## 🐛 Sorun Giderme

[Overview](Overview) · | Sorun | Çözüm |
|-------|----------|
| **Menü görünmüyor / görünmez imleç** | Oyunu kapatın. `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\YourProfile\BepInEx\plugins\TerrainForge\` ve `%APPDATA%\Thunderstore Mod Manager\PackageCache\` dizinlerini silin. TMM'i yeniden başlatın, güncellemeleri kontrol edin. |
| **Güncelleme TMM'de görünmüyor** | TMM → Settings → "Refresh mod list" (Ctrl+R). TMM'i kapatıp yeniden açın. Doğru profili doğrulayın. |
| **Thunderstore sürüm çakışması** | Sürümü (0.3.3 → 0.3.4) `.csproj`, `Plugin.cs`, `manifest.json` içinde yükseltin. Yeniden derleyin + yeniden yükleyin. |
| **Stamp kopyala/yapıştır hiçbir şey yapmıyor** | HUD: `STAMP READY [X/Y]` = hazır. "tool not equipped" diyorsa aracı kuşanın (AYARLAR → Stamp aracı). |
| **Önizleme su altında görünmez** | 0.3.2'de düzeltildi — önizleme yüzeyde yüzer. Güncelleyin. |
| **Çizgi/Dikt/Çokgen üzerinde daireler** | 0.3.2'de düzeltildi — yalnızca işaretli dış çizgi gösterilir. Güncelleyin. |
| **Radyal tekerlek çalışmıyor** | 0.3.2 (`ZInput.GetMouseScrollWheel()`) ve 0.3.4'te (seçim artık merkezlenmiş imleçle silinmiyor) düzeltildi. Güncelleyin. |
| **Config kaydedilmiyor** | Otomatik kaydetmek için menüyü kapatın (F5). Veya `BepInEx/config/com.terrainforge.valheim.cfg` dosyasını doğrudan düzenleyin. |
| **Çok oyunculu desync** | İstemci+sunucuda mod. İkisinde de `UnlockDeltaLimit`. Aynı `MaxUpDelta`/`MaxDownDelta`. |
| **Derleme başarısız** | `.csproj` veya CLI üzerinden `ValheimDir`, `BepInExCoreDir` yollarını kontrol edin. .NET SDK 8+ gerekir. |
| **Derleme / paketleme** | `dotnet build -c Release` (`build/` içinde DLL), `-p:Deploy=true` (profile kurar) veya `-p:Package=true` (`build/` içinde zip). |

---

## 🔗 Bağlantılar

[Overview](Overview) · - **Thunderstore sayfası:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **GitHub deposu:** https://github.com/marceloanton/TerrainForge
- **Bağışlar:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Değişiklik günlüğü:** [Changelog](Changelog)

---

*TerrainForge — Valheim'i sınırsızca şekillendir.*
