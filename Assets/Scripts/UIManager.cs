using UnityEngine;
using UnityEngine.UI; // UI를 다루려면 이게 필수!

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI 연결")]
    public Text moneyText;
    public Text livesText;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // GameManager의 정보를 계속 가져와서 갱신
        if (GameManager.Instance != null)
        {
            moneyText.text = "GOLD: " + GameManager.Instance.money;
            livesText.text = "LIVES: " + GameManager.Instance.lives;
        }
    }
}