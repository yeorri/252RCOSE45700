using UnityEngine;

public class GameManager : MonoBehaviour
{
    // [싱글톤 패턴] 어디서든 GameManager.Instance로 접근 가능하게 함
    public static GameManager Instance;

    [Header("게임 설정")]
    public int startLives = 20;   // 시작 체력
    public int startMoney = 100;  // 시작 돈

    [Header("현재 상태 (확인용)")]
    public int lives;
    public int money;

    public bool isGameOver = false;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance != null)
        {
            Debug.LogError("GameManager가 두 개 이상입니다! 하나를 파괴합니다.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        lives = startLives; //20
        money = startMoney; //100
        isGameOver = false;
    }

    // 체력 감소 함수 (적이 목표에 도착했을 때 호출)
    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        lives -= amount;
        Debug.Log("아군 기지 피해 입음! 남은 체력: " + lives);

        if (lives <= 0)
        {
            EndGame();
        }
    }

    // 돈 획득 함수 (적이 죽었을 때 호출)
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("돈 획득! 현재 돈: " + money);
    }

    // 돈 사용 함수 (타워 지을 때 호출, true면 성공, false면 실패)
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true; // 구매 성공
        }
        
        Debug.Log("돈이 부족합니다!");
        return false; // 구매 실패
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("게임 오버!");
        // 여기에 나중에 게임 오버 UI 띄우는 코드 추가 예정
    }
}