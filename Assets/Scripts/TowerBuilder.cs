using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    public GameObject towerPrefab;
    // 타워 부모 폴더
    public Transform towerContainer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Tower towerScript = towerPrefab.GetComponent<Tower>();
                    if (GameManager.Instance.SpendMoney(towerScript.cost))
                    {
                        // 2. 지불 성공 시 타워 건설 진행
                        BuildTower(hit);
                    }
                }
            }
        }
    }
    void BuildTower(RaycastHit hit)
    {
        // 격자(Grid)에 딱 맞춰서 좌표 계산
        float x = Mathf.Floor(hit.point.x) + 0.5f;
        float z = Mathf.Floor(hit.point.z) + 0.5f;
        Vector3 spawnPos = new Vector3(x, 0.5f, z);

        // 타워 생성
        Instantiate(towerPrefab, spawnPos, Quaternion.identity, towerContainer);
        
        Debug.Log("타워 건설 완료! 남은 돈: " + GameManager.Instance.money);
    }
}