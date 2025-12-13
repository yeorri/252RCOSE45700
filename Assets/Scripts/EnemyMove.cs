using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetTransform; // 목표의 위치를 기억할 변수 추가

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 이걸 끄면 Agent는 이동은 하되, 몸통 방향(Rotation)은 바꾸지 않습니다. 체력바 고정을 위한 장치
        agent.updateRotation = false;
        
        GameObject targetObj = GameObject.Find("Goal");
        if (targetObj != null)
        {
            targetTransform = targetObj.transform; // 목표 Transform 저장
            agent.destination = targetTransform.position;
        }
    }

    void Update()
    {
        // 경로 계산 중이면 대기
        if (agent.pathPending) return;

        // 1차 체크: NavMeshAgent가 멈췄는가?
        if (agent.remainingDistance <= 0.2f)
        {
            // [중요 수정!] 2차 체크: 진짜 목표 지점 근처에 있는가?
            // "내 위치"와 "목표 위치" 사이의 거리가 1.0f보다 작을 때만 도착으로 인정
            if (targetTransform != null && Vector3.Distance(transform.position, targetTransform.position) <= 2.0f)
            {
                ArrivedAtGoal();
            }
        }
    }

    void ArrivedAtGoal()
    {
        // GameManager가 있다면 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(1);
        }

        Destroy(gameObject);
    }
}