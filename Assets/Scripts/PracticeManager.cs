using System.IO;
using UnityEngine;

public class PracticeManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;

    void Start()
    {
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
