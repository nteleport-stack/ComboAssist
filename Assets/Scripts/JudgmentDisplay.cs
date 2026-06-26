using UnityEngine;

// OnGUI-based display — no scene setup required, attach to any active GameObject
public class JudgmentDisplay : MonoBehaviour
{
    Judgment current;
    float showUntil;

    static readonly Color ColorPerfect = new Color(1f, 0.95f, 0.3f);
    static readonly Color ColorMiss    = new Color(1f, 0.3f,  0.3f);

    const float ShowDuration = 0.7f;
    const float FadeDuration = 0.3f;

    public void Show(Judgment j)
    {
        current = j;
        showUntil = Time.time + ShowDuration;
    }

    void OnGUI()
    {
        float remaining = showUntil - Time.time;
        if (remaining <= 0f) return;

        float alpha = Mathf.Clamp01(remaining / FadeDuration);
        Color c = current == Judgment.Perfect ? ColorPerfect : ColorMiss;
        c.a = alpha;

        string text = current == Judgment.Perfect ? "PERFECT" : "MISS";

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 45,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };

        // Positioned to the right of the lane area, vertically near the judgment line (~85% down)
        var rect = new Rect(Screen.width * 0.73f, Screen.height * 0.82f - 40f, 280f, 80f);

        // Shadow
        style.normal.textColor = new Color(0f, 0f, 0f, c.a * 0.5f);
        GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), text, style);

        // Main text
        style.normal.textColor = c;
        GUI.Label(rect, text, style);
    }
}
