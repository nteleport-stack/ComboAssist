using UnityEngine;

public class Note : MonoBehaviour
{
    float targetTime;   // absolute Time.time when this note's bottom should reach the judgment line
    float judgmentY;
    float fallSpeed;
    float laneX;
    float noteHeight;
    RectTransform rt;

    public void Init(float targetTime, float judgmentY, float fallSpeed, float laneX, float noteHeight)
    {
        this.targetTime = targetTime;
        this.judgmentY = judgmentY;
        this.fallSpeed = fallSpeed;
        this.laneX = laneX;
        this.noteHeight = noteHeight;
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        float y = judgmentY + fallSpeed * (targetTime - Time.time);
        rt.anchoredPosition = new Vector2(laneX, y);

        // destroy only after the top edge has cleared the judgment line
        if (y + noteHeight < judgmentY - 50f)
            Destroy(gameObject);
    }
}
