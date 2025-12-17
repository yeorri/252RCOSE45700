using UnityEngine;

public class CrawlTextMover : MonoBehaviour
{
    public float speed = 30f; // 텍스트 올라가는 속도
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 매 프레임 Y축으로 조금씩 이동
        rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
    }
}