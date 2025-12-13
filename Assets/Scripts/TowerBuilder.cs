using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [Header("설정")]
    public GameObject towerPrefab;
    
    [Header("참조")]
    public Transform towerContainer;
    public Transform spawnPoint;
    public Transform goalPoint;

    // 코루틴 없이 바로 짓습니다!
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    // 1. 돈 확인
                    Tower towerScript = towerPrefab.GetComponent<Tower>();
                    if (GameManager.Instance.money < towerScript.cost)
                    {
                        Debug.Log("돈이 부족합니다!");
                        return;
                    }

                    // 2. 좌표 계산
                    float x = Mathf.Floor(hit.point.x) + 0.5f;
                    float z = Mathf.Floor(hit.point.z) + 0.5f;
                    Vector3 buildPos = new Vector3(x, 0.5f, z);

                    // 3. [핵심] 가상 시뮬레이션! (MazeSolver에게 물어보기)
                    // "여기(buildPos)에 지어도 길 있어?"
                    bool isPathSafe = MazeSolver.Instance.IsPathPossible(
                        spawnPoint.position, 
                        goalPoint.position, 
                        buildPos
                    );

                    if (isPathSafe)
                    {
                        // 4. 안전하다면 진짜 건설 진행!
                        GameManager.Instance.SpendMoney(towerScript.cost);
                        BuildTower(buildPos);
                    }
                    else
                    {
                        Debug.Log("길을 막지 마세요! (적들에게 영향 없음)");
                    }
                }
            }
        }
    }

    void BuildTower(Vector3 pos)
    {
        Instantiate(towerPrefab, pos, Quaternion.identity, towerContainer);
        
        // 5. 건설했으니 가상 지도에도 "여기 막힘"이라고 기록
        MazeSolver.Instance.BlockNode(pos);
    }
}