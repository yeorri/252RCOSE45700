using UnityEngine;
using UnityEngine.SceneManagement; // [필수] 씬(Scene)을 다시 불러오려면 이게 필요합니다!
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("게임 설정")]
    public int startLives = 20;
    public int startMoney = 100;

    [Header("UI 연결")]
    public GameObject gameOverUI;
    public GameObject victoryUI; 
    private bool isVictory = false;

    [Header("현재 상태")]
    public int lives;
    public int money;
    public bool isGameOver = false;

    [Header("BGM 설정")]
    public AudioSource bgmSource;      // 현재 배경음악 스피커
    public AudioClip victoryMusic;  
    public AudioClip gameOverMusic;   // 승리 시 재생할 음악 파일

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        lives = startLives;
        money = startMoney;
        isGameOver = false;
        
        // 시작할 때 게임 오버 창은 꺼둡니다.
        if(gameOverUI != null) gameOverUI.SetActive(false);
    }
    public void UpdateAllEnemiesPath()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject goal = GameObject.Find("Goal"); // Goal 오브젝트 이름 확인 필요

        if (goal == null) return;

        foreach (GameObject enemy in enemies)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // SetDestinationImmediate를 사용하여 즉시 갱신
                agent.SetDestinationImmediate(goal.transform.position, 1.0f);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        lives -= amount;

        if (lives <= 0)
        {
            lives = 0; // 음수가 안 되게 0으로 고정
            EndGame();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }
    public void WinGame()
    {
        if (isGameOver) return;
        bgmSource.Stop();
        bgmSource.clip = victoryMusic;
        bgmSource.loop = true;
        bgmSource.Play();

        if (isVictory) return; // 이미 끝났으면 무시
        isVictory = true;

        Debug.Log("모든 웨이브 클리어! 승리!");
        
        if (victoryUI != null)
        {
            victoryUI.SetActive(true); // 승리 패널 켜기!
            Time.timeScale = 0f; // 게임 정지 
        }
    }

    void EndGame()
    {
        bgmSource.Stop();

        bgmSource.clip = gameOverMusic;
        bgmSource.loop = true;
        bgmSource.Play();
        isGameOver = true;
        Debug.Log("게임 오버!");

        // 1. 게임 오버 UI 켜기
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. 게임 시간 멈추기 (모든 움직임 정지)
        Time.timeScale = 0f; 
    }

    // "다시 하기" 버튼이 누를 함수
    public void Retry()
    {
        // 1. 멈췄던 시간을 다시 흐르게 함 
        Time.timeScale = 1f;

        // 2. 현재 보고 있는 씬을 처음부터 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}