using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class FileManager
{
    public static string SaveDirectory => Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
        "ComboAssist");

    public static void SaveCombo(ComboData combo, string fileName)
    {
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
