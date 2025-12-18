using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtensions
{
    // agent.SetDestination 대신 사용할 새로운 함수입니다.
    public static bool SetDestinationImmediate(this NavMeshAgent agent, Vector3 targetLocation, float positionLeniency = 0)
    {
        // 1. 경로를 담을 그릇 생성
        NavMeshPath path = new NavMeshPath();
        
        // 2. 필터 설정 (기본 설정 가져오기)
        NavMeshQueryFilter queryFilter = new NavMeshQueryFilter()
        {
            agentTypeID = agent.agentTypeID,
            areaMask = agent.areaMask
        };

        // 3. (옵션) 목표 지점이 NavMesh 위에 딱 붙어있지 않을 경우를 대비해 보정
        if (positionLeniency > 0)
        {
            if (NavMesh.SamplePosition(targetLocation, out NavMeshHit hit, positionLeniency, queryFilter))
            {
                targetLocation = hit.position;
            }
            else
            {
                return false; // 유효한 위치를 못 찾으면 실패
            }
        }

        // 4. [핵심] 경로를 즉시 계산합니다 (동기 방식)
        bool canSetPath = NavMesh.CalculatePath(
            agent.transform.position, 
            targetLocation, 
            queryFilter, 
            path
        );

        // 5. 계산된 경로가 유효하면 에이전트에게 바로 주입!
        if (canSetPath && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
            return true;
        }

        return false;
    }
}