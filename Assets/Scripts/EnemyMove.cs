using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetTransform; // 목표의 위치를 기억할 변수

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = false;
        
        //agent.updateRotation = false;
        
        GameObject targetObj = GameObject.Find("Goal");
        if (targetObj != null)
        {
            targetTransform = targetObj.transform; // 목표 Transform 저장
            agent.SetDestinationImmediate(targetTransform.position, 1.0f);
        }
    }

    void Update()
    {
        // 경로 계산 중이면 대기
        if (agent.pathPending) return;

        if (agent.remainingDistance <= 1f)
        {
            if (targetTransform != null && Vector3.Distance(transform.position, targetTransform.position) <= 2.0f)
            {
                ArrivedAtGoal();
            }
        }
    }

    void ArrivedAtGoal()
    {
        EnemyHP myStats = GetComponent<EnemyHP>();
        int damageToApply = 1;

        if (myStats != null)
        {
            damageToApply = myStats.damageToBase;
        }
        // GameManager가 있다면 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damageToApply);
        }

        Destroy(gameObject);
    }
}