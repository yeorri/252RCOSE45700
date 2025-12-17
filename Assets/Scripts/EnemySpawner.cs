using UnityEngine;
using UnityEngine.UI; // 1. 일반 UI 시스템 사용
using System.Collections;

[System.Serializable] 
public class Wave
{
    public string waveName = "Wave 1"; 
    public GameObject enemyPrefab;    
    public int count = 10;            
    public float spawnRate = 1.0f;    
}

public class EnemySpawner : MonoBehaviour
{
    [Header("UI 연결")]
    public Text waveText;
    public Text countdownText; // 2. 카운트다운 전용 Text 추가!

    [Header("설정")]
    public Wave[] waves;               
    public float timeBetweenWaves = 10f;
    public Transform enemyContainer;   

    [Header("상태 확인용")]
    public float countdown = 10f;       
    public int currentWaveIndex = 0;   
    private bool isSpawning = false;   

    void Awake()
    {
        UnityEngine.AI.NavMesh.pathfindingIterationsPerFrame = 1000;
    }

    void Start()
    {
        UpdateWaveUI();
    }

    void Update()
    {
        // 3. UI 텍스트 업데이트 로직
        UpdateCountdownUI();

        if (isSpawning) return;
        if (enemyContainer.childCount > 0) return;

        if (currentWaveIndex >= waves.Length)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
            }
            this.enabled = false; 
            return;
        }

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves; 
            return;
        }

        countdown -= Time.deltaTime;
    }

    // 4. 카운트다운 UI를 상황에 맞게 업데이트하는 함수
    void UpdateCountdownUI()
    {
        if (countdownText == null) return;

        if (isSpawning)
        {
            // 적 소환 중에는 메시지 숨김
            countdownText.text = "";
        }
        else if (enemyContainer.childCount > 0)
        {
            // 전투 중에는 전투 중 메시지 표시
            countdownText.text = "";
        }
        else if (currentWaveIndex < waves.Length)
        {
            // 다음 웨이브 대기 중일 때만 시간 표시
            // Mathf.Ceil을 사용해 4.1초 -> 5초로 깔끔하게 정수로 보여줍니다.
            countdownText.text = "Next Wave in " + Mathf.Ceil(countdown).ToString() + " sec...";
        }
        else
        {
            // 모든 웨이브 클리어 시
            countdownText.text = "All Waves Clear!";
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        UpdateWaveUI();

        Wave wave = waves[currentWaveIndex];
        Debug.Log("웨이브 시작: " + wave.waveName);

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        isSpawning = false; 
        currentWaveIndex++; 
    }

    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "WAVE: " + (currentWaveIndex + 1).ToString();
        }
    }

    void SpawnEnemy(GameObject _enemyPrefab)
    {
        Instantiate(_enemyPrefab, transform.position, Quaternion.identity, enemyContainer);
    }
}