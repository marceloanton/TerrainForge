# TerrainForge Roadmap – Cycle 2

Follow-up to `ROADMAP.md` (all gameplay/visual phases there are DONE; Jotunn
stays as a dependency by decision). This cycle adds measurement, numeric input,
path ramps, terrain snapshots, deeper server authority, chunked application and
the Thunderstore release.

## Quick orientation (same touch points)

| File | Role in this cycle |
|---|---|
| `TerrainTypes.cs` | `Loc` keys, maybe `RampMode.Path`, snapshot structs |
| `TerrainEngine.cs` | Path-ramp applier, snapshot read/write, chunk pump, ward/cap gates |
| `TerrainBrush.cs` | Live measurements, typed-height state, path marking, snapshot calls, `EffectiveRadius`, radial/preset leftovers untouched |
| `TerrainPreview.cs` | Path corridor preview (reuse `_poly`), measurement readouts stay in HUD |
| `Plugin.cs` | `UseFixedHeight`/`FixedHeight`, `RespectWards`, `RadiusCap`/`DeltaCap`, chunk tuning, version bump |
| `KeybindMenu.cs` | AJUSTES rows (typed height, snapshots UI), `TerrainPresets`-style snapshot list, help updates |
| `manifest.json`, `TerrainForge.csproj` | Version + description for release |
| `README.md`, `CHANGELOG.md` | Per-phase docs |

General rules for the cycle: one phase per commit, one build per phase,
`Loc` keys for every new user-visible string (EN+ES), undo support for every
new write path, `LogOutput.log` check after each deploy.

## Phase 10 – Live measurements (★ quick win)
Show numbers while marking: line/ramp length + rise + angle, rect W×D + area,
poly perimeter + area.

**Design**
- Compute in `TerrainBrush` from already-known points (`_lineA`, `_rectA`,
  `_polyPoints`, `_rampA` + cursor): horizontal dist, `Δh`, angle, shoelace area
  for poly, W×D rect. No engine changes.
- Display inside the existing shape-state HUD segment (replaces/augments it),
  e.g. `LÍNEA: 24.5 m, +3.2 m (7°)`; keep the "done" `Message()` texts with final
  numbers for ramps (fixed-angle already does; add for two-point).
- New `Loc` keys: units + formats (`hud.m`, `hud.m2`, measurement formats).

**Files to edit**
- `TerrainBrush.cs`: measurement helpers + HUD/`TargetText`/done-messages.
- `TerrainTypes.cs`: `Loc` keys.
- `KeybindMenu.cs`: one help bullet.

**Acceptance**
- [ ] Line/ramp marking shows live length, rise and angle updating with cursor
- [ ] Rect shows W×D + area; poly shows perimeter + area + point count
- [ ] Both languages

**Effort:** S | **Risk:** low

## Phase 11 – Typed target height (★ completes eyedropper)
Type an exact Flatten height instead of only capturing it with `G`.

**Design**
- Replace instance fields `_hasPicked`/`_pickedHeight` with configs
  `UseFixedHeight` (bool) + `FixedHeight` (float, range e.g. 0–100): `G` toggles
  `UseFixedHeight` and captures `FixedHeight = aim`; release clears it; AJUSTES
  gets a textbox + "Fijar" row writing both. `BaseSpec`, `TargetText`, `_levelY`
  read the configs (same `fija X m` HUD). State persists by design (it's config).
- Migration: delete the two fields; behaviour identical otherwise.

**Files to edit**
- `Plugin.cs`: 2 configs. `TerrainBrush.cs`: spec/text/level/eyedrop.
- `KeybindMenu.cs`: AJUSTES row + help line. `TerrainTypes.cs`: `Loc` keys.

**Acceptance**
- [ ] Typing 32.5 levels Flatten to 32.5 m; `G` still captures/releases
- [ ] HUD + level ring follow the typed value; survives menu reopen

**Effort:** S | **Risk:** low

## Phase 12 – Path ramps / trails (★ biggest feature)
`RampMode.Path`: mark A→B→C…, double-press closes, smooth ramp follows the curve.

**Design**
- Marking mirrors poly (min spacing, double-press with 3+ points closes).
- Each marked point stores ground height at mark time → `BrushSpec` gains
  `HasRampPath`, `Vector3[] RampPath`, `float[] RampPathH`.
- Engine `ApplyRampPath`: densify Catmull-Rom (≈1 m samples) in vertex space,
  per-vertex project onto dense segments (height lerps between sample heights),
  same flat-core + side-fade profile as ramps. One-shot (discrete apply, not held).
- Preview: reuse `_poly` center line through points + cursor; `SuppressCircles`.
- HUD `SENDA: N puntos`; done message with length + total rise.

**Files to edit**
- `TerrainTypes.cs`: spec fields + `Loc` keys. `TerrainEngine.cs`: densify + applier.
- `TerrainBrush.cs`: marking state, dispatch, HUD. `TerrainPreview.cs`: nothing
  new (reuse). `KeybindMenu.cs`: help.

**Acceptance**
- [ ] 3+ point path builds a smooth continuous ramp, width = radius
- [ ] Works across heightmap tiles, undoable, preview matches
- [ ] Both languages

**Effort:** M–L | **Risk:** medium (geometry)

## Phase 13 – Terrain snapshots (★ killer feature)
Save terrain to disk and restore it later (backups, sharing designs).

**Design**
- Snapshot = full copies of the 5 delta arrays for every comp referenced by the
  current undo history (bounded, meaningful set) into one binary file:
  header (magic + version) + per comp (tile world XZ + width + arrays).
- Import resolves comps via `FindHeightmap(tilePos)`, claims ownership, writes
  through the normal recorder path (so **import itself is undoable**), saves,
  pokes, resets grass. Missing comps (far/unloaded) → message listing them.
- Files: `BepInEx/config/TerrainForge/snapshots/<name>.dat`. UI mirrors presets
  (AJUSTES: name box + Save, list with Load/Delete, max ~12).
- Format versioned from day one; mismatched version → clean error, no partial write.

**Files to edit**
- New logic inside `TerrainEngine.cs` (`ExportSnapshot`/`ImportSnapshot`) or a
  small static class in the same file (no new files policy).
- `TerrainBrush.cs`: thin call wrappers + messages. `KeybindMenu.cs`: UI + help.
- `TerrainTypes.cs`: `Loc` keys.

**Acceptance**
- [ ] Snapshot → flatten the area → import → terrain back, then undo the import
- [ ] Missing-tile import reports cleanly; corrupt file never half-applies
- [ ] 12-file cap, both languages

**Effort:** M–L | **Risk:** medium (file IO, format)

## Phase 14 – Wards + server caps (completes authority)
- `RespectWards` (default true): `PrivateArea.CheckAccess(center, radius, flash:false)`
  gates brush apply, ramp/shape marking and undo/redo (consistent deny + message).
- `RadiusCap` / `DeltaCap` (0 = off, server-synced): `EffectiveRadius()` /
  effective deltas used by every spec builder AND the preview, so caps are
  visible before applying.
- New `Loc` keys, help bullets, README server section.

**Acceptance**
- [ ] Editing inside someone else's ward blocked with reason (when enabled)
- [ ] Server-set radius/delta caps visibly clamp brush + preview on clients

**Effort:** S–M | **Risk:** low–medium

## Phase 15 – Chunked application (performance)
Split huge applications across frames instead of one long hitch.

**Design**
- Chunk by `TerrainComp` (the existing per-comp loop is the natural boundary):
  `Apply` processes up to `ChunkCompsPerFrame` tiles per call; remainder +
  accumulated undo frame persist in engine static state; `TerrainBrush.Update`
  pumps until done (auto-continues even if the key is released).
- Undo frame pushed once, at completion. Within-tile loops stay single-pass
  (bounded by tile size). Keep the 0.1 s throttle + add an adaptive guard later
  only if profiling demands it.
- No HUD progress bar v1 (flash `Aplicando…` while pending is enough).

**Acceptance**
- [ ] Radius 100 stroke completes without multi-second freeze
- [ ] Result identical to unchunked; single undo step reverts the whole stroke
- [ ] Release-then-complete works (no lost writes)

**Effort:** M | **Risk:** medium (state machine)

## Phase 16 – Thunderstore release
- Bump `0.1.0 → 0.2.0` in `TerrainForge.csproj` (`Version`), `Plugin.cs`
  (`BepInPlugin`), `manifest.json` (`version_number`); refresh manifest
  description (8 ops, 5 shapes, undo, radial, presets, EN/ES).
- `CHANGELOG.md`: move Unreleased → `0.2.0` with date. `README.md` final pass.
- `icon.png`: player-provided (cannot generate images here) — release waits for it.
- Release build + smoke checklist: clean load, tool auto-give, one apply per op,
  undo/redo, all three menu tabs, radial, preset round-trip, server join.
- Package `TerrainForge.dll + README + CHANGELOG (+ icon)` → manual upload
  (uploading can't be done from here).

**Effort:** S | **Risk:** none

## Suggested order
1. **Measurements** (10) – visible utility, trivial risk.
2. **Typed height** (11) – finishes eyedropper.
3. **Path ramps** (12) – the remaining headline feature.
4. **Snapshots** (13) – high value, isolated.
5. **Wards + caps** (14) – closes the authority loop.
6. **Chunks** (15) – needs real-world profiling input first.
7. **Release** (16) – last, with icon.

## Notes
- `Smooth-along-path` and selective smooth are already covered (Line + Smooth,
  `SmoothMode`); no further work planned there.
- Jotunn stays (decision recorded in Cycle 1); if it ever blocks a game update,
  the removal plan in `ROADMAP.md` Phase 8 still applies (Harmony prefab clone
  + one-RPC config sync).
- Anything that writes terrain must record undo and carry `Loc` keys — no
  exceptions this cycle.
