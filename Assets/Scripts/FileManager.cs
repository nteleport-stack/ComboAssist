using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class FileManager
{
    public static string SaveDirectory => Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
        "ComboAssist");

    private const int LeadInFrames = 180; // 3 seconds at 60 fps

    public static void SaveCombo(ComboData combo, string fileName)
    {
        combo.events.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));

        int offset = combo.events[0].startFrame - LeadInFrames;
        foreach (var e in combo.events)
        {
            e.startFrame -= offset;
            e.endFrame   -= offset;
        }

        Directory.CreateDirectory(SaveDirectory);
        string path = Path.Combine(SaveDirectory, fileName + ".combo.json");
        string json = JsonConvert.SerializeObject(combo, Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public static ComboData LoadCombo(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
        return JsonConvert.DeserializeObject<ComboData>(File.ReadAllText(filePath));
    }
}
