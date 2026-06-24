# ComboAssist — Dev Log

A rhythm-game combo trainer for Street Fighter 6. Keyboard/Hitbox players practice combos via falling notes, with adjustable speed.

**Stack:** Unity 6 LTS (6000.3.18f1), C#, JSON, Windows x64

---

## Session 2 — 2026-06-24 — Milestone 1: Project Setup ✅

**Achieved:**
- Unity 6.3 LTS (6000.3.18f1) installed and verified
- 2D URP project created
- Input System (`1.19.0`) confirmed present via template
- Newtonsoft JSON (`3.2.1`) added to package manifest
- Asset folder structure created: `Scripts/`, `Prefabs/`, `UI/`, `Data/`
- `GameManager.cs` written — singleton pattern, `DontDestroyOnLoad`
- GameManager GameObject added to SampleScene and confirmed running in Play mode
- Product name set to `ComboAssist`

**Milestone 1 deliverable met:** Unity project opens and `GameManager` runs without errors.

**Up next:** Milestone 2 — Input Recording (keyboard → `.combo.json` file)

---

## Session 3 — 2026-06-24 — Milestone 2: Input Recording ✅

**Achieved:**
- `ComboData.cs` — data model (`InputEvent`, `ComboData`) with 8 lanes, Punch/Kick/Directional note types
- `FileManager.cs` — saves/loads `.combo.json` via Newtonsoft JSON to `~/Documents/ComboAssist/`
- `InputRecorder.cs` — frame-accurate key recording using Unity New Input System; maps full Hitbox layout (WASD + arrows + UIOJKL + Space)
- `RecordingManager.cs` — in-game UI (OnGUI) with combo name field, Start/Stop buttons, status display
- Verified with a real 626LP Shoryuken input (→↓→ + Light Punch) — frame data, hold durations, and lane assignments all correct

**Milestone 2 deliverable met:** Press keys → Stop & Save → `.combo.json` appears with correct frame data.

**Up next:** Milestone 3 — Rhythm Game Core (load combo → falling notes → hit detection)

---
