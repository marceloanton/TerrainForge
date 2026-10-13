# TerrainForge Roadmap

This document breaks down the requested features into a testable sequence, pinning them to the current codebase so we know where to touch first.

## Quick orientation (current touch points)

| File | Purpose | Notes |
|---|---|---|
| `TerrainTypes.cs` | Enums, `BrushSpec`, `ToolRequirement` | Add new shapes/ops/fields (undo stack, paths, eyedrop, slope mode, ...) |
| `TerrainEngine.cs` | Core: read/write `TerrainComp` deltas (reflection), `Apply`, `ApplyToComp`, `ApplyRampLine`, `Falloff`, `ClampDelta` | Also needs to expose "original" deltas for undo snapshots, per-heightmap snapshots |
| `TerrainBrush.cs` | Input, HUD, colour, ramp A/B mark, tool detection, `TryApply`, HUD | Central: keys, state (`_rampHasA`, `_rampA`, `_applying`), HUD extension, eyedrop/slope, paths |
| `TerrainPreview.cs` | Live footprint + guides (edge/full/reach/aim + ramp corridor) | New: slope shading, level contours, rect/line/poly previews |
| `Plugin.cs` | BepInEx config | New keys (Ctrl+Z/Ctrl+Y, mode toggles, profiles) |
| `KeybindMenu.cs` | Rebind + HELP tabs, skin | Update help, add radial/profiles help, localisation keys |
| `TerrainTool.cs` | Custom tool (Jotunn) `TerrainForgeTool` (clone Hammer), equip/give | Remove/replace if dropping Jotunn (Harmony path) |
| `Patches.cs` | Harmony patches (clamp ±8m, vanilla tool radius, no-build-cost) | Cleanups when removing Jotunn |
| `manifest.json`, `TerrainForge.csproj` | Jotunn reference/deps | Edit if going no-Jotunn |

## Phase 0 – Baseline (read-only check)
- [ ] Audit `TerrainEngine` field accesses (`m_levelDelta`, `m_smoothDelta`, `m_modifiedHeight`, `m_paintMask`, `m_modifiedPaint`, `m_nview`) are still valid.
- [ ] Check `ZNetView.ClaimOwnership()` + `Save(bool)` calls against `assembly_valheim` (already working).
- [ ] Confirm no new publicizer needed (reflection only).
- [ ] Review `LogOutput.log` quick scan for early errors after deploy.

## Phase 1 – Undo/Redo (★ highest priority) — DONE
Goal: let `Ctrl+Z` undo last brush application(s) and `Ctrl+Y` redo. Track per application a minimal "delta set".

Implemented: sparse per-vertex recording in `TerrainEngine` (one frame per `Apply`
call, ramp included), `History > UndoDepth` (0-500, slider + exact textbox in the
TECLAS tab), HUD undo/redo counts + flash messages, history cleared on world exit.

**Design**
- Keep a history per-heightmap? Or just remember what `TerrainEngine` wrote: for each affected `TerrainComp` (by `nview.GetZDO().m_uid` or the comp instance), snapshot the changed indices of `levelDelta`, `smoothDelta`, `modifiedHeight`, `paintMask`, `modifiedPaint` (and maybe whether created/restored). Also remember centre/search radius used.
- Stack: `List<UndoFrame>` with `maxCount` (configurable, e.g. 50–200).
- On `TerrainEngine.Apply` success → capture a frame before/after? Or capture "diff": store old values at changed indices (copy arrays or byte-diffs). `float[]` and `Color[]` small (max ~65k per comp) but easier: copy the subarrays of touched indices.
- Keys: `Plugin.cs` add `UndoKey = (Ctrl+Z)`, `RedoKey = (Ctrl+Y)` in `Input`.

**Files to edit**
- `TerrainTypes.cs`: add `UndoEntry { compRef, levelCopy, smoothCopy, modHCopy, paintCopy, modPCopy, indices[] }`, `UndoFrame { entries[] }`
- `TerrainEngine.cs`: add `static Stack<UndoFrame> undo = new(), redo = new()`; `Record(...)` inside `ApplyToComp` after computing `any`, save `old` copies for touched indices; `Undo()`/`Redo()` methods that write back + `Save` + `hm.Poke(0)` + `ClutterSystem.ResetGrass`
- `TerrainBrush.cs`: key handling in `Update()` (before menu/blocking as sensible); call engine `Undo/Redo`
- `Plugin.cs`: config for keys + `UndoDepth`

**Acceptance**
- [x] One `Apply` → `Ctrl+Z` reverts terrain to before.
- [x] Ramp creation undoable.
- [x] Paint/Restore undoable.
- [x] Redo works after undo.
- [x] History bounded.
- [ ] Verify in-game (player test + `LogOutput.log` check).

**Effort:** M–L | **Risk:** medium (must write deltas back carefully)

## Phase 2 – HUD with live data (★)
Show: current world height under crosshair (`ZoneSystem.GetGroundHeight(aim)`), target height (what tool will use), slope in degrees, paint strength etc.

**Files**
- `TerrainBrush.cs` (`OnGUI`, `EnsureHudStyle`): extend `status` string; compute slope: `deltaHeight / horizontalDist` over small ray/step or `Vector3.Angle(Vector3.up, normal)` approx via two nearby ground samples? Or sample 4 points around centre to get gradient.
- `TerrainPreview.cs`: optional slope shading later but HUD first.

**Acceptance**
- [ ] HUD shows `Aim Y: … m`, `Target: … m`, `Slope: …°`
- [ ] Updates every frame while equipped
- [ ] No layout break on small widths

**Effort:** S | **Risk:** low

## Phase 3 – Server permissions
Block editing unless allowed: by player role? Check for admin (`ZNet.instance.IsServer() || ZNet.instance.GetPlayerList().Find(p=>p.m_characterID==ZDOID)?` better `Player.m_localPlayer.IsAdmin()` or ward owner). Also `PrivateArea`/`Ward` checks?

Ideas:
- `Plugin.cs`: `RequireAdmin`, `AllowInWards` (already exists concept), `AllowOutsideWardsOnly`, `BypassIfCreator` etc
- `TerrainBrush.cs`: before `TryApply` → permission check: if not admin and in ward and not owner → block + message
- Can reuse `PrivateArea.CheckAccess`? Or `Ward.IsInside` checks.

**Acceptance**
- [ ] Admin can edit anywhere
- [ ] Non-admin blocked in wards unless configured
- [ ] Message shown on blocked attempt
- [ ] Config sync friendly via ConditionalConfigSync if desired

**Effort:** M | **Risk:** low–medium

## Phase 4 – "Smooth" polish + selective smooth (optional next)
Current smooth is 3x3 blur of original neighbours – good. Add "smooth selective": only where `|cur - avg| > threshold` or only up/down? Or `Gaussian` small kernel? Also "smooth along path" later.

But keep minimal: maybe `SmoothMode { Full, UpOnly, DownOnly }` in enum + config + HUD.

**Files:** `TerrainTypes.cs`, `TerrainEngine.cs` (filter), `TerrainBrush.cs` (cycle mode), `KeybindMenu.cs` help.

**Effort:** S | **Risk:** low

## Phase 5 – Eyedropper (copy height)
Key e.g. `Alt+Click` or cycleable? Or new submode.

- In `TerrainBrush.cs`: if `EyedropKey` down → read `GroundHeight(center)` and store `LastHeight`; set `TargetWorld = LastHeight` for Flatten? Or new `SetTarget` action.
- Also allow "level to this object": raycast to `Piece`, `Structure` etc get its base height? More fuzzy.

**Effort:** S | **Risk:** low

## Phase 6 – Slope fixed (angle-based ramp)
Add `SlopeMode { TwoPoint, FixedAngle }` for Ramp op? Or new op `Slope`.

Compute target along direction from A: `deltaZ = dist * tan(angle * deg2rad)` to get hB from hA.

**Files:** `TerrainTypes.cs` (`BrushSpec.Angle`, `SlopeMode`), `TerrainEngine.cs` (`ApplyRampLine` branch), `TerrainBrush.cs` (UI to set angle), `TerrainPreview.cs` draw projected line by angle.

**Effort:** M | **Risk:** low

## Phase 7 – Forms: line, rect (drag), polygon
Biggest UX jump.

- New op `Line`, `Rect`, `Poly`? Or general `Shape` enum grows: `Circle,Square,Line,Rect,Poly`.
- For `Line`: click A, drag to B, apply (like ramp but you pick width). Or two-point apply.
- For `Rect`: drag from corner1 to corner2, axis-aligned or rotated? Start simple axis-aligned in world.
- For `Poly`: click points, close on double-click/apply.
- Preview: draw edges, fill? Use `LineRenderer`s.

**Touch:** `TerrainTypes.cs`, `TerrainBrush.cs` (drag state: `_dragStart`, `_dragging`), `TerrainPreview.cs`, `TerrainEngine.cs` (point-in-shape tests + falloff per shape).

**Effort:** L | **Risk:** medium–high (stateful)

## Phase 8 – Remove Jotunn dependency
Goal: drop `[BepInDependency("com.jotunn.jotunn")]`, `Jotunn.Managers.*`, `TerrainTool.cs` Jotunn path.

Approach (Harmony-only):
1. On `ObjectDB.Awake` postfix: clone Hammer prefab → rename to `TerrainForgeTool`, strip build pieces, set `m_itemType = ItemDrop.ItemData.ItemType.Tool`, `m_weight=2`, `m_maxStackSize=1`, `m_maxQuality=1`, `m_value=0`, `m_teleportable=true`, `m_equipDuration=0.2f`, clear `m_attack.m_attackStamina`. Keep same `m_name`, `m_shared.m_name`.
2. Add to `ObjectDB.m_items` list and ensure `ObjectDB.instance.GetItemPrefab("TerrainForgeTool")` returns it.
3. On `ZNetScene.Awake` postfix: register prefab in `m_prefabs`/`m_namedPrefabs` (reflection) so it's network-safe.
4. `TerrainTool` becomes thin: `Has/Give/Equip/IsEquipped` use `ObjectDB.instance.GetItemPrefab` + `Player.m_localPlayer.m_inventory`.
5. Remove Jotunn refs from `TerrainForge.csproj`, `manifest.json` (or keep optional? but "depend as little as possible").
6. Keep `ToolRequirement.TerrainForgeTool` working.

Need to mirror Jotunn's clone carefully (instantiate, copy components, don't touch vanilla). Also same peer name for networking.

**Acceptance**
- [ ] Loads without Jotunn DLL present (optional dep removed)
- [ ] Tool spawns, equips, works same
- [ ] Multiplayer sync unchanged
- [ ] `AutoGiveTool` still works

**Effort:** M–L | **Risk:** medium (networking/prefab lifecycle)

## Phase 9 – Localisation + radial + profiles
- Localisation: simple CSV/JSON or `LocalizationManager`? But if removing Jotunn maybe own. Or use BepInEx config text + translate keys in `KeybindMenu`.
- Radial: IMGUI radial or Unity UI? Keep IMGUI to avoid extra assets. Draw pie, highlight sector, click to select.
- Profiles: save `Radius`, `Softness`, `Operation`, `Shape`, `Paint`, `FalloffPower` as named sets in config or separate file.

**Effort:** M | **Risk:** low

## Suggested immediate order
1. **Undo/Redo** (Phase 1) – biggest quality-of-life, directly requested.
2. **HUD live data** (Phase 2) – quick win, useful for all ops.
3. **Server permissions** (Phase 3) – multiplayer safety.
4. **Remove Jotunn** (Phase 8) – matches "depend lo menos posible" goal.
5. **Slope fixed + Eyedropper** (6–5) – very practical.
6. **Forms** (7) – most work, highest UX.

## Notes on implementation details
- **Undo snapshots**: only copy touched indices. Track `min/max x,y` per frame to shrink copies.
- **Smooth**: current 3x3 is fine; can add `KernelSize` later. Keep original snapshot (already planned).
- **Ramp preview**: you already hide circular guides when corridor shown – good.
- **Threading**: all Unity/BepInEx on main thread. Reflection cheap here.
- **Sync**: never touch `m_levelDelta` directly except via arrays; always `Save(false)` + `hm.Poke(0)`. Same as now.

Start with **Phase 1 (Undo/Redo)** – shall I implement it now?