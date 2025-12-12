using UnityEngine;
using System.Collections;

[System.Serializable] 
public class Wave
{
    public string waveName = "Wave 1"; // 웨이브 이름 (식별용)
    public GameObject enemyPrefab;     // 소환할 적 종류 
    public int count = 10;             // 소환할 마릿수
    public float spawnRate = 1.0f;     // 소환 간격
}

public class EnemySpawner : MonoBehaviour
{
    [Header("설정")]
    public Wave[] waves;               // 웨이브 목록 (배열)
    public float timeBetweenWaves = 5f;// 웨이브 사이 휴식 시간
    public Transform enemyContainer;   // 적들을 담을 폴더

    [Header("상태 확인용")]
    public float countdown = 2f;       // 남은 시간 표시
    public int currentWaveIndex = 0;   // 현재 몇 번째 웨이브인지
    private bool isSpawning = false;   // 지금 소환 중인가?
    void Awake()
    {
        UnityEngine.AI.NavMesh.pathfindingIterationsPerFrame = 1000;
    }
    void Update()
    {
        // 1. 소환 중이면 아무것도 안 하고 대기
        if (isSpawning) return;

        // 2. 맵에 적이 한 마리라도 살아있으면 다음 웨이브로 안 넘어감
        // (EnemyContainer의 자식 개수를 세면 현재 살아있는 적 수를 알 수 있음!)
        if (enemyContainer.childCount > 0) return;

        // 3. 모든 웨이브가 끝났다면? (게임 클리어)
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("모든 웨이브 클리어! 승리!");
            this.enabled = false; // 스크립트 끄기
            return;
        }

        // 4. 휴식 시간 카운트다운
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves; // 카운트다운 초기화
            return;
        }

        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        Wave wave = waves[currentWaveIndex];

        Debug.Log("웨이브 시작: " + wave.waveName);

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        isSpawning = false; // 소환 끝 (이제 적들이 다 죽기를 기다리는 상태가 됨)
        currentWaveIndex++; // 다음 웨이브 번호 미리 올려둠
    }

    void SpawnEnemy(GameObject _enemyPrefab)
    {
        Instantiate(_enemyPrefab, transform.position, Quaternion.identity, enemyContainer);
    }
}