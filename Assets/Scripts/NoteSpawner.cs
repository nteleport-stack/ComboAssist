using UnityEngine;
using UnityEngine.UI;

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject notePrefab;
    public RectTransform laneContainer;

    [Header("Layout")]
    public float spawnY = 500f;
    public float judgmentY = -320f;
    public float laneSpacing = 80f;
    public float fallSpeed = 500f;

    ComboData combo;
    float sessionStartTime;
    int nextNoteIndex;

    void Start()
    {
        CreateLaneLines();
    }

    public void StartSession(ComboData data)
    {
        combo = data;
        sessionStartTime = Time.time;
        nextNoteIndex = 0;
    }

    void Update()
    {
        if (combo == null || nextNoteIndex >= combo.events.Count) return;

        float travelTime = (spawnY - judgmentY) / fallSpeed;

        while (nextNoteIndex < combo.events.Count)
        {
            InputEvent evt = combo.events[nextNoteIndex];
            float noteTargetTime = evt.startFrame / (float)combo.fps;
            float spawnAt = noteTargetTime - travelTime;

            if (Time.time - sessionStartTime >= spawnAt)
            {
                SpawnNote(evt);
                nextNoteIndex++;
            }
            else break;
        }
    }

    void SpawnNote(InputEvent evt)
    {
        float absoluteTargetTime = sessionStartTime + evt.startFrame / (float)combo.fps;
        float x = (evt.lane - 3.5f) * laneSpacing;

        GameObject obj = Instantiate(notePrefab, laneContainer);
        RectTransform rt = obj.GetComponent<RectTransform>();

        float holdSeconds = (evt.endFrame - evt.startFrame) / (float)combo.fps;
        float noteHeight = Mathf.Max(30f, holdSeconds * fallSpeed);
        rt.sizeDelta = new Vector2(60f, noteHeight);
        rt.pivot = new Vector2(0.5f, 0f); // bottom-center: bottom edge reaches judgment line at targetTime

        Note note = obj.GetComponent<Note>();
        note.Init(absoluteTargetTime, judgmentY, fallSpeed, x, noteHeight);

        obj.GetComponent<Image>().color = GetNoteColor(evt.lane, evt.noteType);
    }

    void CreateLaneLines()
    {
        if (laneContainer == null) return;

        float lineHeight = spawnY - judgmentY + 100f;
        float centerY = (spawnY + judgmentY) / 2f;

        // 9 lines: left border, 7 separators between lanes, right border
        for (int i = 0; i <= 8; i++)
        {
            float x = -320f + i * laneSpacing;

            GameObject line = new GameObject("LaneLine_" + i,
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            line.transform.SetParent(laneContainer, false);

            RectTransform rt = line.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, centerY);
            rt.sizeDelta = new Vector2(2f, lineHeight);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

            line.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f);
        }
    }

    Color GetNoteColor(int lane, int noteType)
    {
        if (lane <= 3) return Color.white;  // directional lanes

        Color[] attackColors = {
            new Color(0.27f, 0.60f, 1.00f),  // Lane 4 Light  — blue
            new Color(1.00f, 0.87f, 0.13f),  // Lane 5 Medium — yellow
            new Color(1.00f, 0.25f, 0.25f),  // Lane 6 Heavy  — red
            new Color(0.73f, 0.40f, 1.00f),  // Lane 7 DParry — purple
        };
        return attackColors[lane - 4];
    }
}
