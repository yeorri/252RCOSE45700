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
                    float x = Mathf.Floor(hit.point.x) + 0.5f;
                    float z = Mathf.Floor(hit.point.z) + 0.5f;
                    Vector3 spawnPos = new Vector3(x, 0.5f, z);

                    Instantiate(towerPrefab, spawnPos, Quaternion.identity, towerContainer);
                }
            }
        }
    }
}