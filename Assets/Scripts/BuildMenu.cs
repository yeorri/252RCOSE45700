using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    private GridTile targetTile; 

    // [수정 1] 인스펙터에서 타워 프리팹을 직접 연결하게 변수 생성
    public GameObject basicTowerPrefab; 

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu(GridTile tile)
    {
        targetTile = tile;
        transform.position = tile.transform.position + Vector3.up * 1.5f;
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

    // [수정 2] 버튼 연결용 함수 (매개변수 없음 -> 무조건 목록에 뜸!)
    public void OnClickBasicTower()
    {
        // 미리 연결해둔 basicTowerPrefab을 사용
        OnClickBuild(basicTowerPrefab);
    }
}