using UnityEngine;
using System.Collections.Generic;

public class MazeSolver : MonoBehaviour
{
    public static MazeSolver Instance;

    [Header("맵 설정 (Plane Scale X:2, Z:2 기준)")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float offset = 10f; // 좌표 보정값 (-10 ~ 10을 0 ~ 20으로 변환)

    // 타워가 어디에 있는지 기억하는 가상 지도 (true면 막힘)
    private bool[,] blockedGrid;

    void Awake()
    {
        Instance = this;
        // 맵 크기만큼 가상 지도 생성 (기본값은 다 false/뚫림)
        blockedGrid = new bool[gridWidth, gridHeight];
    }

    // 좌표를 그리드 인덱스로 변환 (예: -9.5 -> 0)
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x + offset);
        int y = Mathf.FloorToInt(worldPos.z + offset);
        return new Vector2Int(x, y);
    }

    // 타워 건설 확정 시 기록
    public void BlockNode(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        if (IsValid(gridPos)) blockedGrid[gridPos.x, gridPos.y] = true;
    }

    // 범위 체크
    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    // [핵심] 가상 경로 탐색 (BFS 알고리즘)
    // "내가 저기에 타워를 지으면(testBlock), 시작점에서 끝점까지 갈 수 있어?"
    public bool IsPathPossible(Vector3 startWorld, Vector3 endWorld, Vector3 testBlockWorld)
    {
        Vector2Int start = WorldToGrid(startWorld);
        Vector2Int end = WorldToGrid(endWorld);
        Vector2Int testBlock = WorldToGrid(testBlockWorld);

        // BFS를 위한 준비물
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[gridWidth, gridHeight]; // 방문 기록

        // 시작점 등록
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        // 길찾기 루프
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 도착했다면 성공!
            if (current == end) return true;

            // 상하좌우 탐색
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                // 1. 맵 범위 안이고
                // 2. 방문한 적 없고
                // 3. 기존 타워가 없고
                // 4. [중요] 지금 지으려는 타워 위치가 아니라면
                if (IsValid(next) && !visited[next.x, next.y] 
                    && !blockedGrid[next.x, next.y] 
                    && next != testBlock)
                {
                    visited[next.x, next.y] = true;
                    queue.Enqueue(next);
                }
            }
        }
        

        // 큐가 빌 때까지 도착 못했으면 길이 막힌 것
        return false;
    }
    public void UnblockNode(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        // 범위 체크 후 false(안 막힘)로 변경
        if (IsValid(gridPos)) blockedGrid[gridPos.x, gridPos.y] = false;
    }
}