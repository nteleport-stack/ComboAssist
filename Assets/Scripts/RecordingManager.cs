using UnityEngine;

[RequireComponent(typeof(InputRecorder))]
public class RecordingManager : MonoBehaviour
{
    private InputRecorder _recorder;
    private string _comboName = "MyCombo";
    private string _status = "Ready. Enter a name and press Start.";

    void Start()
    {
        _recorder = GetComponent<InputRecorder>();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 320, 180));

        GUILayout.Label("=== ComboAssist — Recording ===");
        GUILayout.Space(8);

        GUILayout.Label("Combo name:");
        if (!_recorder.IsRecording)
            _comboName = GUILayout.TextField(_comboName, 40);
        else
            GUILayout.Label(_comboName);

        GUILayout.Space(8);

        if (!_recorder.IsRecording)
        {
            if (GUILayout.Button("Start Recording"))
            {
                _recorder.StartRecording(_comboName);
                _status = "Recording... press your combo keys, then Stop.";
            }
        }
        else
        {
            if (GUILayout.Button("Stop & Save"))
            {
                _recorder.StopAndSave(_comboName);
                _status = $"Saved! Check Documents/ComboAssist/{_comboName}.combo.json";
            }
        }

        GUILayout.Space(8);
        GUILayout.Label(_status);

        GUILayout.EndArea();
    }
}
