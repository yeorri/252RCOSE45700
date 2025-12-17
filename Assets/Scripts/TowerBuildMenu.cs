using UnityEngine;

public class TowerBuildMenu : MonoBehaviour
{
    private GridTile targetTile; 

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu(GridTile tile)
    {
        targetTile = tile;
        transform.position = tile.transform.position + Vector3.up * 1.5f + Vector3.right * 2f;
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    // 내부 로직용 (건설 실행)
    public void OnClickBuild(GameObject towerPrefab)
    {
        if (targetTile != null)
        {
            TowerBuilder.Instance.TryBuildTower(targetTile, towerPrefab);
        }
        CloseMenu();
    }
}