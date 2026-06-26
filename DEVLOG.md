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

## Session 4 — 2026-06-24 — M2 Bug Fixes & Lane Redesign

**Achieved:**
- Fixed `InputRecorder.cs` noteType bug — Punch/Kick were swapped on attack lanes (Punch was 1, Kick was 2; corrected to Punch=0, Kick=1)
- Redesigned lane structure to final 8-lane layout:
  - Lane 0: Left (A) | Lane 1: Up (W) | Lane 2: Down (S) | Lane 3: Right (D)
  - Lane 4: Light (U=Punch, J=Kick) | Lane 5: Medium (I=Punch, K=Kick) | Lane 6: Heavy (O=Punch, L=Kick)
  - Lane 7: Drive Parry (Space)
  - Up and Down kept as separate lanes to support combos that hold both simultaneously
- Updated `ComboData.cs` noteType comment to reflect correct mapping
- Fixed `FileManager.cs` SaveCombo — now sorts events chronologically by startFrame and normalizes frames so the first input always lands at frame 180 (3-second lead-in at 60 fps)
- Verified with re-recorded 626LP Shoryuken `.combo.json`

**Up next:** Milestone 3 — Rhythm Game Core (load combo → falling notes → hit detection)

---

## Session 5 — 2026-06-25 — M3 Part 1: Falling Notes ✅

**Achieved:**
- Created `Practice` scene with Canvas (Screen Space - Overlay, 1920×1080 reference resolution)
- `Note.cs` — falling note component; position driven by wall-clock time so speed is frame-rate independent; destroys only after the top edge clears the judgment line (fixes hold-note early disappearance)
- `NoteSpawner.cs` — loads ComboData, pre-calculates spawn time per note, instantiates colored note prefabs; draws 9 semi-transparent lane separator lines on Start; color scheme: directionals = white, Light = blue, Medium = yellow, Heavy = red, Drive Parry = purple
- `PracticeManager.cs` — auto-loads the most recently modified `.combo.json` from `~/Documents/ComboAssist/` on scene start
- **Bug fixed:** `InputRecorder.cs` was using `Time.frameCount` (engine frames) instead of wall-clock time, causing playback speed to scale with monitor refresh rate; fixed to `Mathf.RoundToInt(Time.time * fps)` — existing `.combo.json` files must be re-recorded

**M3 Part 1 deliverable met:** Load a recorded combo → colored notes fall at correct speed → hold notes stay visible for full duration → lanes separated by visible lines.

**Up next:** M3 Part 2 — InputJudge (hit detection, Perfect/Good/Miss judgment)

---

## Session 6 — 2026-06-26 — M3 Part 2: Input Judgment (in progress)

**Achieved:**
- Removed "Good" window — only Perfect (±50ms) and Miss, matching fighting game conventions
- Short note vs long note classification (threshold: 0.1s / ~6 frames at 60fps):
  - **Short note**: always 30px standardized height; perfect on press-down only; disappears on hit
  - **Long note**: height based on hold duration; press-down turns note grey; note disappears only when key-up also lands within ±50ms of tail target time; missed head = note falls through; missed tail = note stays and scrolls off
- `Note.cs` — rebuilt with compound visual: short = single circle (center is judgment point); long = darker rectangle body flanked by two circles (bottom = head judgment, top = tail judgment); pivot adapts per type so the `y = judgmentY + fallSpeed * (targetTime - Time.time)` formula works correctly for both
- `InputJudge.cs` — key press hits short notes immediately; long note head hit tracked per key in `heldNotes` dict; key release checks tail window
- `JudgmentDisplay.cs` — switched to OnGUI (no TMP scene setup required); gold "PERFECT" / red "MISS" with fade-out; positioned to the right of lanes
- `NoteSpawner.cs` — notes now built entirely in code (no prefab Image needed); added visible horizontal judgment line; `judgmentY` exposed as public Inspector field
- `PracticeManager.cs` — added `FindObjectOfType` fallbacks so nothing breaks if Inspector slots are empty
- Circle sprite fixed: `Resources.GetBuiltinResource` was returning rectangles in Unity 6; replaced with a programmatically generated 128×128 circle texture (cached static)

**Still outstanding (next session):**
- Judgment line position in fullscreen is still off — user ran out of time before resolving; `judgmentY` value needs tuning for the player's monitor/resolution

**Up next:** Finish M3 Part 2 — fix fullscreen judgment line position, then move to M4 (speed control)

---
