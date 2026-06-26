using System.IO;
using UnityEngine;

public class PracticeManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    public InputJudge inputJudge;
    public JudgmentDisplay judgmentDisplay;

    void Start()
    {
        // Auto-find if not wired in Inspector
        if (noteSpawner == null)     noteSpawner = FindObjectOfType<NoteSpawner>();
        if (inputJudge == null)      inputJudge = FindObjectOfType<InputJudge>();
        if (judgmentDisplay == null) judgmentDisplay = FindObjectOfType<JudgmentDisplay>();

        // Create JudgmentDisplay on this object if it doesn't exist anywhere in the scene
        if (judgmentDisplay == null) judgmentDisplay = gameObject.AddComponent<JudgmentDisplay>();

        // Wire InputJudge to the NoteSpawner in case it wasn't set in Inspector
        if (inputJudge != null) inputJudge.noteSpawner = noteSpawner;

        inputJudge.OnJudgment  += judgmentDisplay.Show;
        noteSpawner.OnNoteMiss += _ => judgmentDisplay.Show(Judgment.Miss);

        ComboData combo = LoadMostRecent();
        if (combo != null)
            noteSpawner.StartSession(combo);
        else
            Debug.LogWarning("No combo files found in " + FileManager.SaveDirectory);
    }

    ComboData LoadMostRecent()
    {
        if (!Directory.Exists(FileManager.SaveDirectory)) return null;

        string[] files = Directory.GetFiles(FileManager.SaveDirectory, "*.combo.json");
        if (files.Length == 0) return null;

        string newest = files[0];
        foreach (string f in files)
            if (File.GetLastWriteTime(f) > File.GetLastWriteTime(newest))
                newest = f;

        Debug.Log("Loading combo: " + newest);
        return FileManager.LoadCombo(newest);
    }
}
