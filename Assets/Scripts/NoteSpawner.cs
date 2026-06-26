using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform laneContainer;

    [Header("Layout")]
    public float spawnY      = 500f;
    public float judgmentY   = -380f;
    public float laneSpacing = 80f;
    public float fallSpeed   = 500f;

    const float ShortNoteThreshold = 0.1f; // seconds — notes shorter than this are short notes

    ComboData combo;
    float sessionStartTime;
    int nextNoteIndex;
    List<Note>[] laneNotes;

    public Action<int> OnNoteMiss; // int = lane index

    void Start()
    {
        laneNotes = new List<Note>[8];
        for (int i = 0; i < 8; i++) laneNotes[i] = new List<Note>();
        CreateLaneLines();
    }

    public void StartSession(ComboData data)
    {
        combo = data;
        sessionStartTime = Time.time;
        nextNoteIndex = 0;
    }

    // Returns the closest unhit note in this lane to currentTime
    public Note GetBestNote(int lane, float currentTime)
    {
        List<Note> notes = laneNotes[lane];
        notes.RemoveAll(n => n == null);

        Note best = null;
        float bestDist = float.MaxValue;
        foreach (Note n in notes)
        {
            if (n.IsHeadHit) continue;
            float dist = Mathf.Abs(n.TargetTime - currentTime);
            if (dist < bestDist) { bestDist = dist; best = n; }
        }
        return best;
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
        float absoluteTailTime   = sessionStartTime + evt.endFrame   / (float)combo.fps;
        float holdSeconds = (evt.endFrame - evt.startFrame) / (float)combo.fps;
        bool isLong = holdSeconds >= ShortNoteThreshold;
        float holdHeight = isLong ? holdSeconds * fallSpeed : 0f;

        float x = (evt.lane - 3.5f) * laneSpacing;
        Color noteColor = GetNoteColor(evt.lane, evt.noteType);

        var obj = new GameObject("Note_L" + evt.lane, typeof(RectTransform), typeof(Note));
        obj.transform.SetParent(laneContainer, false);

        Note note = obj.GetComponent<Note>();
        note.Init(evt.lane, isLong, holdHeight, absoluteTargetTime, absoluteTailTime,
                  judgmentY, fallSpeed, x);
        note.BuildVisual(noteColor);

        int lane = evt.lane;
        note.OnMiss = () =>
        {
            laneNotes[lane].Remove(note);
            OnNoteMiss?.Invoke(lane);
        };

        laneNotes[lane].Add(note);
    }

    void CreateLaneLines()
    {
        if (laneContainer == null) return;

        float lineHeight = spawnY - judgmentY + 100f;
        float centerY = (spawnY + judgmentY) / 2f;

        // Vertical lane separator lines (9 lines for 8 lanes)
        for (int i = 0; i <= 8; i++)
        {
            float x = -320f + i * laneSpacing;
            var line = new GameObject("LaneLine_" + i,
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            line.transform.SetParent(laneContainer, false);
            var rt = line.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, centerY);
            rt.sizeDelta = new Vector2(2f, lineHeight);
            line.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f);
        }

        // Horizontal judgment line
        var jline = new GameObject("JudgmentLine",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        jline.transform.SetParent(laneContainer, false);
        var jrt = jline.GetComponent<RectTransform>();
        jrt.anchorMin = jrt.anchorMax = new Vector2(0.5f, 0.5f);
        jrt.pivot = new Vector2(0.5f, 0.5f);
        jrt.anchoredPosition = new Vector2(0f, judgmentY);
        jrt.sizeDelta = new Vector2(640f, 4f);
        jline.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);
    }

    Color GetNoteColor(int lane, int noteType)
    {
        if (lane <= 3) return Color.white;

        Color[] attackColors = {
            new Color(0.27f, 0.60f, 1.00f),  // Lane 4 Light  — blue
            new Color(1.00f, 0.87f, 0.13f),  // Lane 5 Medium — yellow
            new Color(1.00f, 0.25f, 0.25f),  // Lane 6 Heavy  — red
            new Color(0.73f, 0.40f, 1.00f),  // Lane 7 DParry — purple
        };
        return attackColors[lane - 4];
    }
}
