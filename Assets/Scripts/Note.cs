using System;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public int Lane { get; private set; }
    public float TargetTime { get; private set; }
    public float TailTargetTime { get; private set; }
    public bool IsLong { get; private set; }
    public bool IsHeadHit { get; private set; }

    public Action OnMiss;

    float judgmentY;
    float fallSpeed;
    float laneX;
    float holdHeight;
    float topOffset;  // distance from pivot to top visual edge, used for off-screen destroy
    bool missFired;
    RectTransform rt;

    Image headImage;
    Image bodyImage;   // null for short notes
    Image tailImage;   // null for short notes

    public const float NoteWidth = 60f;
    const float MissLatency = 0.05f;

    public void Init(int lane, bool isLong, float holdHeight, float targetTime, float tailTargetTime,
                     float judgmentY, float fallSpeed, float laneX)
    {
        Lane = lane;
        IsLong = isLong;
        this.holdHeight = holdHeight;
        TargetTime = targetTime;
        TailTargetTime = tailTargetTime;
        this.judgmentY = judgmentY;
        this.fallSpeed = fallSpeed;
        this.laneX = laneX;

        rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        if (isLong)
        {
            // Pivot at bottom-center so anchoredPosition = head circle center
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(NoteWidth, holdHeight + NoteWidth);
            topOffset = holdHeight + NoteWidth * 0.5f;  // top of tail circle relative to pivot
        }
        else
        {
            // Pivot at center so anchoredPosition = circle center
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(NoteWidth, NoteWidth);
            topOffset = NoteWidth * 0.5f;
        }
    }

    // Called by NoteSpawner after Init to build child visual objects
    public void BuildVisual(Color noteColor)
    {
        Sprite circle = GetCircleSprite();

        if (IsLong)
        {
            // Body created first so circles render on top of it
            bodyImage = CreateChild("Body", 0f, new Vector2(0f, holdHeight * 0.5f),
                                    new Vector2(NoteWidth, holdHeight), null, Darken(noteColor));
            headImage = CreateChild("Head", 0f, Vector2.zero,
                                    new Vector2(NoteWidth, NoteWidth), circle, noteColor);
            tailImage = CreateChild("Tail", 0f, new Vector2(0f, holdHeight),
                                    new Vector2(NoteWidth, NoteWidth), circle, noteColor);
        }
        else
        {
            headImage = CreateChild("Circle", 0.5f, Vector2.zero,
                                    new Vector2(NoteWidth, NoteWidth), circle, noteColor);
        }
    }

    // Short note: press matched — destroy immediately
    public void MarkHit() => Destroy(gameObject);

    // Long note: head press matched — turn grey, keep falling
    public void MarkHeadHit()
    {
        IsHeadHit = true;
        if (headImage != null) headImage.color = new Color(0.55f, 0.55f, 0.55f);
        if (bodyImage != null) bodyImage.color = new Color(0.38f, 0.38f, 0.38f);
        if (tailImage != null) tailImage.color = new Color(0.55f, 0.55f, 0.55f);
    }

    // Long note: tail release matched — destroy
    public void MarkTailHit() => Destroy(gameObject);

    void Update()
    {
        float y = judgmentY + fallSpeed * (TargetTime - Time.time);
        rt.anchoredPosition = new Vector2(laneX, y);

        if (!IsHeadHit && !missFired && Time.time > TargetTime + MissLatency)
        {
            missFired = true;
            OnMiss?.Invoke();
        }

        // Destroy when the top visual edge has scrolled 50px below the judgment line
        if (y + topOffset < judgmentY - 50f)
            Destroy(gameObject);
    }

    // anchorY: 0 = anchor to parent bottom-center (long notes), 0.5 = anchor to parent center (short notes)
    Image CreateChild(string childName, float anchorY, Vector2 offset, Vector2 size, Sprite sprite, Color color)
    {
        var go = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(transform, false);
        var crt = go.GetComponent<RectTransform>();
        crt.anchorMin = crt.anchorMax = new Vector2(0.5f, anchorY);
        crt.pivot = new Vector2(0.5f, 0.5f);
        crt.anchoredPosition = offset;
        crt.sizeDelta = size;
        var img = go.GetComponent<Image>();
        if (sprite != null) img.sprite = sprite;
        img.color = color;
        return img;
    }

    static Color Darken(Color c) => new Color(c.r * 0.65f, c.g * 0.65f, c.b * 0.65f, c.a);

    // --- Circle sprite (generated once, reused by all notes) ---

    static Sprite _circleSprite;

    static Sprite GetCircleSprite()
    {
        if (_circleSprite != null) return _circleSprite;

        const int size = 128;
        float center = size * 0.5f;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - center + 0.5f;
            float dy = y - center + 0.5f;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            // 1px soft edge for anti-aliasing
            float alpha = Mathf.Clamp01(center - dist);
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
        tex.Apply();
        _circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return _circleSprite;
    }
}
