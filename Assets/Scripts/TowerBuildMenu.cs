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
        // 1. 만약 이전에 열려있던 다른 타일이 있다면? -> 걔는 꺼준다.
        if (targetTile != null && targetTile != tile)
        {
            targetTile.SetSelectionState(false);
        }
// 2. 새로운 타겟 설정
        targetTile = tile;
        
        // [추가] 타일에게 "너 지금 메뉴 열렸으니까 초록색 유지해!" 라고 명령
        targetTile.SetSelectionState(true);

        transform.position = tile.transform.position + Vector3.up * 3f + Vector3.right * 2.2f + Vector3.back * 0.7f;
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        //  메뉴 닫을 때 타일 색상 원상복구
        if (targetTile != null)
        {
            targetTile.SetSelectionState(false);
            targetTile = null; // 타겟 초기화
        }
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