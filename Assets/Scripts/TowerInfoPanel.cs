using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    // 어디서든 접근 쉽게 싱글톤 처리
    public static TowerInfoPanel Instance;

    [Header("UI 연결")]
    public GameObject panelObj; // 패널 전체 (껏다 켰다 하기 위함)
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statText; // 데미지, 사거리 등을 한 번에 보여주거나 따로 만드셔도 됩니다.

    void Awake()
    {
        Instance = this;
        panelObj.SetActive(false); // 처음엔 꺼둠
    }

    // 타워 정보를 받아서 화면에 갱신
    public void ShowInfo(Tower tower)
    {
        if (tower == null) return;

        panelObj.SetActive(true);

        nameText.text = tower.towerName;

        // 보기 좋게 정보 표시
        statText.text = $"Damage : {tower.damage} (Lv.{tower.levelDamage})\n" +
                        $"Range  : {tower.range} (Lv.{tower.levelRange})\n" +
                        $"Speed  : {tower.fireRate:F2} (Lv.{tower.levelRate})";
    }

    // 메뉴 닫을 때 정보창도 같이 끄기
    public void HideInfo()
    {
        panelObj.SetActive(false);
    }
}