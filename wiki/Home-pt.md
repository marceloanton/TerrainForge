# Wiki do TerrainForge

[Overview](Overview) · [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · *Português* · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it) · [Website](https://marceloanton.github.io/TerrainForge/)

---


### TerrainForge 0.3.8 — correção da 0.3.7

> A roda Q tem um modo dedicado de Copiar/Colar. Retângulo marca A/B com Aplicar; Linha marca duas pontas; Polígono adiciona vértices e copia com duplo toque após 3+ pontos; Círculo/Quadrado copia a área do pincel. O HUD mostra o próximo passo. Neve fica em Pintar > DeepSnow (segure Shift para remover). F5 congela movimento e câmera locais; Esc pausa o mundo inteiro no modo solo. 0.3.6: o F5 organiza as abas em duas linhas e mostra os controles da operação selecionada. Ramp inclui Dois Pontos, Ângulo Fixo e Conectada; rampas circulares estão planejadas para a 0.4.0.

## Índice

1. [Instalação](#-instalação)
2. [Início Rápido](#-início-rápido)
3. [Controles](#-controles)
4. [Operações](#-operações)
5. [Formas](#-formas)
6. [Modos de Queda](#-modos-de-queda)
7. [Stamps (Copiar/Colar)](#-stamps-copiarcolar)
8. [Snapshots (Backups)](#-snapshots-backups)
9. [Perfis](#-perfis)
10. [Idiomas](#-idiomas)
11. [Multijogador](#-multijogador)
12. [Configurações](#-configurações)
13. [Solução de Problemas](#-solução-de-problemas)

---

## 📥 Instalação

### Requisitos
- **BepInEx 5** (x64, Mono) no cliente e no servidor. Usuários do Thunderstore já o possuem.

### Passos
1. Baixe `TerrainForge-<version>.zip` do Thunderstore ou do GitHub Releases.
2. Extraia e coloque `TerrainForge.dll` em:
   - **Cliente:** `...\BepInEx\plugins\TerrainForge\TerrainForge.dll`
   - **Servidor:** `<servidor>\BepInEx\plugins\TerrainForge\TerrainForge.dll`
3. Reinicie o jogo / servidor.

> ⚠️ **Deve ser instalado no cliente E no servidor** para edições maiores que ±8 m e replicação correta.

---

## 🚀 Início Rápido

1. Entre em um mundo. A **Varinha TerrainForge** (clone do Martelo) é entregue e equipada automaticamente.
2. Pressione `F5` para abrir o menu (ou clique no botão **TerrainForge** no menu de pausa `ESC`).
3. Equipe a varinha se ela não estiver equipada.
4. Mire no terreno e segure o **clique direito** (`Mouse1`) para esculpir.
5. Use `,` para alternar operações, `.` para formas, `/` para pinturas.
6. Segure `Q` → menu radial, solte para confirmar.
7. `Ctrl+Z` / `Ctrl+Y` para desfazer/refazer.

---

## ⌨️ Controles

| Ação | Tecla padrão | Descrição |
|--------|-------------|-------------|
| Aplicar pincel | Segurar `Mouse1` (clique direito) | Esculpe o terreno sob a mira |
| Raio + | `=` ou `Ctrl`+Roda | Aumenta o raio do pincel |
| Raio − | `-` ou `Ctrl`+Roda | Diminui o raio do pincel |
| Alternar operação | `,` | Alterna as 9 operações |
| Alternar forma | `.` | Círculo → Quadrado → Linha → Ret → Poli |
| Alternar pintura | `/` | Alterna os 6 tipos de pintura |
| Nivelar aos pés | Segurar `Shift` esq. | Aplanar usa a altura do jogador |
| Desfazer | `Ctrl+Z` | Desfaz o último traço |
| Refazer | `Ctrl+Y` | Refaz o traço desfeito |
| Conta-gotas | `G` | Fixa a altura de Aplanar na mira (de novo = libera) |
| Modo de rampa | `H` | Alterna: Dois Pontos → Ângulo Fixo → Caminho (ou escolha com 3 botões em CONFIGURAÇÕES) |
| Ângulo de rampa fixo | Segurar `Alt` + raio/roda | Ajusta o ângulo no modo fixo |
| Menu radial | Segurar `Q` | Escolha uma operação com o mouse, soltar confirma |
| Modo de ajuste | `V` | Roda = raio, congela zoom/barra/mapa |
| Ajuste fino (0,1 m) | Segurar `Shift` + roda/raio | Ajuste preciso do raio |
| **Copiar Stamp** | `Ctrl+Shift+C` | Copia a área marcada para a área de transferência |
| **Colar Stamp** | `Ctrl+Shift+V` | Preview fantasma, clique confirma |
| Obter Varinha | `F8` | Entrega a Varinha TerrainForge |
| Abrir menu | `F5` | Abre o menu de teclas/ajuda/configurações |

**Teclas extras (sem padrão, atribua-as em TECLAS):**
- Operação direta: Aplanar, Levantar, Baixar, Suavizar, Pintar, Restaurar, Rampa, Costa, Neve
- Próximo / anterior perfil
- Próximo / anterior stamp (histórico)

**Seção "Ações rápidas" (aba TECLAS):** botões para funções sem atalho obrigatório — Desfazer, Refazer, Fixar altura, Copiar/Colar stamp, Stamp anterior, Limpar marcas, os mapas de calor e o Diagnóstico de material (despeja o shader + slots de textura no log).

---

## 🔄 Operações

| Operação | Descrição |
|-----------|-------------|
| **Aplanar** | Nivela até a altura apontada (Shift = aos seus pés). Altura fixa via Conta-gotas (`G`) ou a caixa de texto de CONFIGURAÇÕES. |
| **Levantar** | Eleva o terreno em `RaiseDelta` por aplicação. O modo de queda dá forma a ele. |
| **Baixar** | Rebaixa o terreno em `RaiseDelta` por aplicação. O modo de queda dá forma a ele. |
| **Suavizar** | Blur de verdade: faz a média de cada vértice com seus 8 vizinhos. Segure para progressivo. **Modos:** Completo (ambos), Apenas Cristas (rebaixa), Apenas Depressões (eleva). Forma Linha = suaviza um caminho. |
| **Rampa** | **3 modos** (alterna `H`):<br>• **Dois Pontos**: marque A e B → uma inclinação reta une as alturas.<br>• **Ângulo Fixo**: B = direção/comprimento; altura = `SlopeAngle` (sobe/desce conforme a mira). Ajuste com `Alt`+raio.<br>• **Caminho**: marque uma curva A→B→C… (duplo toque com 3+ fecha) → Catmull-Rom suave.<br>Largura = raio do pincel. Suavidade = blend apenas nas bordas laterais. Preview: corredor preenchido + anéis de altura em A/B. |
| **Costa** | Aplana em direção ao nível da água + margem `ShoreOffset` (praias). Suavidade como Aplanar. |
| **Pintar** | Tipos: Dirt, Cultivated, Paved, ClearVegetation, DeepSnow, Reset. Força ajustável. |
| **Restaurar** | Devolve o terreno ao seu estado original (altura + pintura). |
| **Copiar/Colar** | Modo dedicado na roda Q para selecionar terreno e posicionar stamps. |

---

## 📐 Formas

Alterne com a tecla `.`. Todas suportam desfazer.

| Forma | Marcação | Caso de uso |
|-------|---------|----------|
| **Círculo** | Clique único (centro do pincel) | Escultura geral |
| **Quadrado** | Clique único (centro) | Plataformas, fundações |
| **Linha** | 2 cliques (A → B) | Caminhos, muros, trilhas. Meia-largura = raio. |
| **Retângulo** | 2 cliques (cantos opostos) | Fundações, fazendas, plataformas. Bordas suaves. |
| **Polígono** | Clique por vértice, duplo clique fecha (3+ vértices) | Formas complexas, áreas personalizadas |

- As formas não circulares mostram **apenas o contorno marcado** (sem anéis circulares).
- A suavidade afeta o blend das bordas em todas as formas.

---

## ⚙️ Modos de Queda (Levantar/Baixar)

Configuração em CONFIGURAÇÕES → "Queda de Raise/Lower":

| Modo | Comportamento |
|------|----------|
| **Pontudo** | Pico central. Controlado pelo slider "Potência da queda" (1–5). |
| **Plano** | Disco uniforme — mesmo peso em todo o raio. |
| **Suave** | Campana de cosseno — transição suave, sem borda dura. |
| **Linear** | Afunilamento reto do centro até a borda. |

---

## 🧱 Stamps (Copiar/Colar)

Uma função poderosa para replicar estruturas de terreno.

### Seleção por retângulo (recomendada)

Escolha Copiar/Colar na roda Q. Aplicar marca a seleção: dois cantos no Retângulo, duas pontas na Linha, vértices no Polígono (duplo toque após 3+ pontos), ou a área do pincel no Círculo/Quadrado. O HUD indica o próximo passo.

### Fluxo de trabalho

1. **Marque a área**: use qualquer forma (Círculo, Ret, Linha, Poli, Rampa) — ou a seleção por retângulo acima.
2. **Copiar**: `Ctrl+Shift+C` → "Stamp copiado: X tiles" (área de transferência na RAM).
3. **Preview de colagem**: `Ctrl+Shift+V` → wireframe fantasma na mira.
   - `Q`/`E` = rotacionar ±15°
   - `Alt`+Roda = rotação contínua
   - `Shift`+Roda = escalar 0.25×–4×
   - `Esc` = cancelar
4. **Confirm**: Pressione o atalho de colar novamente para confirmar a prévia.

> Os stamps de seleção gravam **apenas dentro da região copiada**; o terreno fora dela permanece intocado.

### Histórico
- Até **10 stamps** na RAM (mais recente primeiro).
- Teclas `Próximo stamp` / `Stamp anterior` (reatribuíveis) os alternam.
- Flash: "Histórico de stamps: Copy: 3 tiles [2/5]"

### Persistência
- **CONFIGURAÇÕES → STAMPS**:
  - Salvar o stamp atual em disco (`.stamp` em `BepInEx/config/TerrainForge/stamps/`, máx 12).
  - Carregar um stamp para a área de transferência.
  - Listar/excluir stamps salvos.

### Ferramenta de stamp
- CONFIGURAÇÕES → "Ferramenta de stamp" (4 opções):
  - `Igual ao pincel` (padrão) — usa a mesma ferramenta que o pincel
  - `Qualquer` — sem requisito
  - `Picareta` — requer uma picareta equipada
  - `Varinha TerrainForge` — requer a varinha do mod
- O HUD mostra `STAMP PRONTO [2/5]` quando a ferramenta está equipada + stamps na área de transferência.
- Sem a ferramenta: `Stamp: ferramenta não equipada`, copiar/colar bloqueados.

---

## 💾 Snapshots (Backups)

Backup/restauração completa do terreno.

### Salvar
1. CONFIGURAÇÕES → **SNAPSHOTS**.
2. Nome (vazio = data/hora automática).
3. **Salvar** → tiles editados para `BepInEx/config/TerrainForge/snapshots/<nome>.dat` (máx 12).

### Carregar
1. Selecione um snapshot.
2. **Carregar** → restaura os tiles com ownership, **1 passo de desfazer**.
3. Tiles não carregados avisam de forma limpa — importe perto do local original.

### Características
- Formato binário versionado (`.dat`).
- Restaura altura + smooth + pintura + ownership.
- A importação é desfazível (`Ctrl+Z` reverte a importação).
- Tiles não carregados avisam de forma limpa.

---

## 💾 Perfis

Salvar/carregar configurações completas de pincel.

- CONFIGURAÇÕES → **PERFIS**.
- Nome + **Salvar** → raio, suavidade, op, forma, pintura, modo de rampa, ângulo, modo de suavização, raise delta, força de pintura, falloff, shore offset.
- **Carregar** → aplica instantaneamente.
- Máx 12 perfis (formato culture-invariant em `Presets > Slots`).
- Atalho: `Next Profile` (atribua em TECLAS) para alternar.

---

## 🌐 Idiomas

**15 opções:** `Auto`, `English`, `Español`, `Français`, `Deutsch`, `Русский`, `中文`, `繁體中文`, `日本語`, `한국어`, `Polski`, `Português`, `Türkçe`, `Українська`, `Italiano` — padrão `Auto`.

### Auto-Detect
- O `Auto` padrão detecta o idioma do jogo via reflection: `Localization.instance.GetSelectedLanguage()`.
- Mapeia o idioma do Valheim → índice interno.
- Se um idioma não tiver tradução completa → fallback para English.
- Configurável em CONFIGURAÇÕES → "Idioma" (alterna as 15 opções).
- Os rótulos das teclas são reconstruídos ao abrir o menu.

### Status das traduções
- **Completas:** English, Español
- **Fallback para English:** Français, Deutsch, Русский, 中文, 繁體中文, 日本語, 한국어, Polski, Português, Türkçe, Українська, Italiano
- Traduções da comunidade são bem-vindas via PR.

### Esta documentação
Páginas da wiki traduzidas: [English](Home) · [Español](Home-es) · [Français](Home-fr) · [Deutsch](Home-de) · [Русский](Home-ru) · [中文](Home-zh) · [繁體中文](Home-zh-tw) · [日本語](Home-ja) · [한국어](Home-ko) · [Polski](Home-pl) · [Português](Home-pt) · [Türkçe](Home-tr) · [Українська](Home-uk) · [Italiano](Home-it).

---

## 🌍 Multijogador

### Requisitos
- **Mod no cliente E no servidor** para edições maiores que ±8 m e replicação correta.
- `UnlockDeltaLimit` também no servidor, caso contrário o servidor limita o terreno.

### Autoridade do Servidor (Config Sync)
Quando o servidor roda o mod, sua seção `Server` prevalece:
- `AllowTerrainEdits` (padrão `true`) — master switch; se for `false`, os clientes não podem aplicar/desfazer/refazer.
- `AdminOnlyEdits` (padrão `false`) — apenas admins/host podem editar.
- `RespectWards` (padrão `true`) — bloqueia edições dentro de wards alheios.
- `RadiusCap` / `DeltaCap` (0 = off) — limites impostos pelo servidor, visíveis no HUD+preview.

> Servidores **sem** o mod ignoram a seção → o cliente funciona para testes.

### Prevenção de Desync
- Mantenha `MaxUpDelta` / `MaxDownDelta` consistentes no servidor + clientes.
- Raios grandes podem dessincronizar clientes sem o mod.

---

## ⚙️ Configurações

Arquivo: `BepInEx/config/com.terrainforge.valheim.cfg`

### Geral
| Setting | Padrão | Descrição |
|---------|---------|-------------|
| `Enabled` | `true` | Master switch |
| `PlayEffects` | `true` | Poeira + impacto ao aplicar |
| `Language` | `Auto` | Auto / English / Español / Français / Deutsch / Русский / 中文 / 繁體中文 / 日本語 / 한국어 / Polski / Português / Türkçe / Українська / Italiano |
| `ToolTint` | `true` | Tingem a Varinha TerrainForge (clone do Martelo) para diferenciá-la facilmente do Martelo vanilla |
| `ToolTintColor` | `#4FC3F7` | Cor hex da varinha (modelo + ícone). Ex.: `#FFC400` dourado, `#8BC34A` verde |

### Pincel
| Setting | Padrão | Faixa | Descrição |
|---------|---------|-------|-------------|
| `Radius` | 8 | 1–100 | Raio do pincel (m) |
| `RadiusMin` | 2 | — | Raio mínimo ao ajustar |
| `RadiusMax` | 60 | — | Raio máximo ao ajustar |
| `RadiusStep` | 1 | — | Variação de raio por tecla/roda |
| `Softness` | 50 | 0–100 | Suavidade da borda (0 = dura, 100 = muito suave) |
| `FalloffPower` | 3 | 1–5 | Expoente de queda (apenas modo Pontudo) |
| `RaiseLowerMeters` | 2 | 0.5–10 | Metros por aplicação de Levantar/Baixar |
| `PaintStrength` | 1 | 0–1 | Força de pintura |
| `DefaultOperation` | Flatten | — | Operação inicial |
| `Shape` | Circle | — | Forma inicial |
| `Paint` | Dirt | — | Pintura inicial |
| `RampMode` | TwoPoint | — | TwoPoint / FixedAngle / Path |
| `SlopeAngle` | 30 | 5–60 | Ângulo da rampa fixa (graus) |
| `SmoothMode` | Full | — | Full / Bumps / Dips |
| `FalloffMode` | Pointy | — | Pointy / Flat / Smooth / Linear |
| `ShoreOffset` | 0.5 | -2–5 | Margem de costa acima da água (m) |
| `UseFixedHeight` | false | — | Usar altura fixa (estado do conta-gotas) |
| `FixedHeight` | 30 | -10–150 | Altura exata de Aplanar (m) |

### Vanilla / integração
- `BlockVanillaTerrainTools`: false — com a varinha equipada, ignora a edição de terreno da enxada/cultivador vanilla. Também há um interruptor em CONFIGURAÇÕES.

### Entrada
Todos os atalhos são reatribuíveis na aba TECLAS. Destaques:
- `ApplyKey` (Mouse1), `RadiusUp/Down` (=/-), `CycleOp/Shape/Paint` (,/.//)
- `UndoKey` (Ctrl+Z), `RedoKey` (Ctrl+Y), `EyedropKey` (G), `RampModeKey` (H)
- `RadialKey` (Q), `AdjustModeKey` (V), `GiveToolKey` (F8), `RebindMenuKey` (F5)
- **Teclas diretas de op** (9, sem padrão): OpFlatten/Raise/Lower/Smooth/Paint/Restore/Ramp/Shore/Snow
- `CycleProfileKey`, `CycleStampKey`, `CycleStampBackKey`
- `RequiredTool`: Any / Pickaxe / TerrainForgeTool (padrão)
- `AutoGiveTool`: true (entrega a varinha ao entrar)

### Stamps
- `StampToolMode`: Same as Brush / Any / Pickaxe / TerrainForgeTool (padrão Same as Brush)

### Preview
- `ShowPreview`: true, `ShowGuides`: true (guias de alcance, tonalidade de inclinação, anel de nível)

### Histórico
- `UndoDepth`: 50 (0–500, 0 desativa o histórico)

### Servidor (Autoritativo via Config Sync)
- `AllowTerrainEdits`: true (master switch)
- `AdminOnlyEdits`: false (apenas admins)
- `RespectWards`: true (bloqueia wards alheios)
- `RadiusCap` / `DeltaCap`: 0 (off) — limites impostos pelo servidor

### Desempenho
- `TilesPerFrame`: 4 (1–32 tiles/frame)

---

## 🐛 Solução de Problemas

| Problema | Solução |
|-------|----------|
| **O menu não aparece / cursor invisível** | Feche o jogo. Exclua `%APPDATA%\Thunderstore Mod Manager\DataFolder\Valheim\profiles\SeuPerfil\BepInEx\plugins\TerrainForge\` e `%APPDATA%\Thunderstore Mod Manager\PackageCache\`. Reinicie o TMM, verifique as atualizações. |
| **Atualização não aparece no TMM** | TMM → Settings → "Refresh mod list" (Ctrl+R). Feche/reabra o TMM. Verifique o perfil correto. |
| **Conflito de versão no Thunderstore** | Aumente a versão (0.3.3 → 0.3.4) em `.csproj`, `Plugin.cs`, `manifest.json`. Rebuild + re-upload. |
| **Copiar/colar stamp não faz nada** | HUD: `STAMP PRONTO [X/Y]` = pronto. Se disser "ferramenta não equipada", equipe a ferramenta (CONFIGURAÇÕES → Ferramenta de stamp). |
| **Preview invisível debaixo d'água** | Corrigido em 0.3.2 — o preview flutua na superfície. Atualize. |
| **Círculos em Linha/Ret/Poli** | Corrigido em 0.3.2 — apenas o contorno marcado é mostrado. Atualize. |
| **A roda radial não funciona** | Corrigido em 0.3.2 (`ZInput.GetMouseScrollWheel()`) e 0.3.4 (a seleção não é mais apagada com o cursor centralizado). Atualize. |
| **A config não salva** | Feche o menu (F5) para salvar automaticamente. Ou edite `BepInEx/config/com.terrainforge.valheim.cfg` diretamente. |
| **Desync em multijogador** | Mod no cliente+servidor. `UnlockDeltaLimit` em ambos. Mesmos `MaxUpDelta`/`MaxDownDelta`. |
| **O build falha** | Verifique os caminhos `ValheimDir`, `BepInExCoreDir` em `.csproj` ou via CLI. Requer .NET SDK 8+. |
| **Compilar / empacotar** | `dotnet build -c Release` (DLL em `build/`), `-p:Deploy=true` (instala no perfil) ou `-p:Package=true` (zip em `build/`). |

---

## 🔗 Links

- **Página no Thunderstore:** https://thunderstore.io/c/valheim/p/MRAnalytics/TerrainForge/
- **Repositório no GitHub:** https://github.com/marceloanton/TerrainForge
- **Doações:** [cafecito.app/byrka](https://cafecito.app/byrka)
- **Changelog:** [Changelog](Changelog)

---

*TerrainForge — Terraforme o Valheim sem limites.*