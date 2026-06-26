using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputJudge : MonoBehaviour
{
    public NoteSpawner noteSpawner;

    public Action<Judgment> OnJudgment;

    public const float PerfectWindow = 0.05f; // ±50ms

    static readonly Dictionary<Key, int> KeyToLane = new()
    {
        { Key.A,          0 }, { Key.LeftArrow,  0 },
        { Key.W,          1 }, { Key.UpArrow,    1 },
        { Key.S,          2 }, { Key.DownArrow,  2 },
        { Key.D,          3 }, { Key.RightArrow, 3 },
        { Key.U,          4 }, { Key.J,          4 },
        { Key.I,          5 }, { Key.K,          5 },
        { Key.O,          6 }, { Key.L,          6 },
        { Key.Space,      7 },
    };

    // Tracks which long note each key is currently holding
    readonly Dictionary<Key, Note> heldNotes = new();

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        foreach (var kv in KeyToLane)
        {
            var key = kb[kv.Key];
            int lane = kv.Value;

            if (key.wasPressedThisFrame)
            {
                Note note = noteSpawner.GetBestNote(lane, Time.time);
                if (note == null) continue;

                float error = Mathf.Abs(Time.time - note.TargetTime);
                if (error > PerfectWindow) continue;

                if (note.IsLong)
                {
                    note.MarkHeadHit();
                    heldNotes[kv.Key] = note;
                }
                else
                {
                    note.MarkHit();
                    OnJudgment?.Invoke(Judgment.Perfect);
                }
            }

            if (key.wasReleasedThisFrame && heldNotes.TryGetValue(kv.Key, out Note held))
            {
                heldNotes.Remove(kv.Key);
                if (held == null) continue; // note already fell off screen

                float tailError = Mathf.Abs(Time.time - held.TailTargetTime);
                if (tailError <= PerfectWindow)
                {
                    held.MarkTailHit();
                    OnJudgment?.Invoke(Judgment.Perfect);
                }
                // Outside window: no judgment shown, note continues falling off
            }
        }
    }
}

public enum Judgment { Perfect, Miss }
