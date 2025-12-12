using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GameObject target = GameObject.Find("Goal");
        if (target != null)
        {
            agent.destination = target.transform.position;
        }
    }

    void Update()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= 0.2f)
        {
            ArrivedAtGoal();
        }
    }

    void ArrivedAtGoal()
    {
        Debug.Log("적이 목표지점에 도착했습니다! (생명력 깎임)");

        Destroy(gameObject);
    }
}