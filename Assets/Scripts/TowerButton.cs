using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [Header("이 버튼이 지을 타워 연결")]
    public GameObject myTowerPrefab;

    [Header("메뉴 연결")]
    public TowerBuildMenu buildMenu; // 부모인 TowerBuildMenu를 연결합니다.

    // 버튼을 클릭하면 실행될 함수
    public void OrderTower()
    {
        if (buildMenu != null && myTowerPrefab != null)
        {
            // 주방장(BuildMenu)에게 "내 프리팹(myTowerPrefab)으로 요리해줘!" 라고 전달
            buildMenu.OnClickBuild(myTowerPrefab);
        }
        else
        {
            Debug.LogError("타워 프리팹이나 빌드메뉴가 연결되지 않았습니다!");
        }
    }
}