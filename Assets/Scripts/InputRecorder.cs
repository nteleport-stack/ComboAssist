using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRecorder : MonoBehaviour
{
    public bool IsRecording { get; private set; }

    private ComboData _combo;
    private readonly Dictionary<Key, int> _heldKeys = new();

    // Maps each keyboard key to (lane, noteType)
    private static readonly Dictionary<Key, (int lane, int noteType)> KeyMap = new()
    {
        { Key.A,         (0, 0) }, { Key.LeftArrow,  (0, 0) },  // Left
        { Key.W,         (1, 0) }, { Key.UpArrow,    (1, 0) },  // Up
        { Key.S,         (2, 0) }, { Key.DownArrow,  (2, 0) },  // Down
        { Key.D,         (3, 0) }, { Key.RightArrow, (3, 0) },  // Right
        { Key.U,         (4, 0) },                               // Light Punch
        { Key.J,         (4, 1) },                               // Light Kick
        { Key.I,         (5, 0) },                               // Medium Punch
        { Key.K,         (5, 1) },                               // Medium Kick
        { Key.O,         (6, 0) },                               // Heavy Punch
        { Key.L,         (6, 1) },                               // Heavy Kick
        { Key.Space,     (7, 0) },                               // Drive Parry
    };

    public void StartRecording(string comboName)
    {
        _combo = new ComboData { comboName = comboName, fps = 60 };
        _heldKeys.Clear();
        IsRecording = true;
        Debug.Log("Recording started.");
    }

    public void StopAndSave(string fileName)
    {
        if (!IsRecording) return;
        IsRecording = false;

        // Close any keys still held when recording stops
        foreach (var kv in _heldKeys)
        {
            var (lane, noteType) = KeyMap[kv.Key];
            _combo.events.Add(new InputEvent
            {
                lane = lane, noteType = noteType,
                startFrame = kv.Value, endFrame = Time.frameCount
            });
        }
        _heldKeys.Clear();

        FileManager.SaveCombo(_combo, fileName);
        Debug.Log($"Saved {_combo.events.Count} events.");
    }

    void Update()
    {
        if (!IsRecording) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        foreach (var kv in KeyMap)
        {
            var key = kb[kv.Key];

            if (key.wasPressedThisFrame && !_heldKeys.ContainsKey(kv.Key))
                _heldKeys[kv.Key] = Time.frameCount;

            if (key.wasReleasedThisFrame && _heldKeys.TryGetValue(kv.Key, out int startFrame))
            {
                var (lane, noteType) = kv.Value;
                _combo.events.Add(new InputEvent
                {
                    lane = lane, noteType = noteType,
                    startFrame = startFrame, endFrame = Time.frameCount
                });
                _heldKeys.Remove(kv.Key);
            }
        }
    }
}
