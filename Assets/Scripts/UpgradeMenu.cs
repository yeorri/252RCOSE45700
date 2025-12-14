using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance;
    private Tower selectedTower;

    [Header("UI 텍스트 연결")]
    public Text damageBtnText;
    public Text rangeBtnText;
    public Text rateBtnText;
    public Text sellBtnText;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SelectTower(Tower tower)
    {
        // 이전 타워의 정보를 끄고 새로운 타워를 선택하기 전에 호출
        if (selectedTower != null)
        {
            selectedTower.HideRange(); // <--- 이전 타워의 사거리 끄기
        }
        selectedTower = tower;
        tower.ShowRange();
        // 메뉴 위치: 타워 머리 위
        transform.position = tower.transform.position + Vector3.up * 2f + Vector3.right * 3f + Vector3.forward * 1.7f;
        UpdateBtnUI();
        TowerInfoPanel.Instance.ShowInfo(tower);
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange(); 
        }
        selectedTower = null;
        gameObject.SetActive(false);
        if (TowerInfoPanel.Instance != null)
            TowerInfoPanel.Instance.HideInfo();
    }

    void UpdateBtnUI()
    {
        if (selectedTower == null) return;

        // 버튼 텍스트 (연결 안됐을 경우를 대비해 null 체크)
        if(damageBtnText) damageBtnText.text = $"Dmg Up ({selectedTower.GetDamageUpgradeCost()}G)";
        if(rangeBtnText) rangeBtnText.text = $"Rng Up ({selectedTower.GetRangeUpgradeCost()}G)";
        if(rateBtnText) rateBtnText.text = $"Spd Up ({selectedTower.GetRateUpgradeCost()}G)";
        
        // 판매 버튼
        if(sellBtnText) sellBtnText.text = $"Sell (+{selectedTower.GetSellPrice()}G)";
        TowerInfoPanel.Instance.ShowInfo(selectedTower);
    }

    // --- 버튼 연결 함수 ---
    public void OnClickDamage() { if (selectedTower) { selectedTower.UpgradeDamage(); UpdateBtnUI(); } }
    public void OnClickRange() { if (selectedTower) { selectedTower.UpgradeRange(); UpdateBtnUI();  } }
    public void OnClickRate() { if (selectedTower) { selectedTower.UpgradeRate(); UpdateBtnUI(); } }
    public void OnClickSell() { if (selectedTower) { selectedTower.SellTower(); CloseMenu(); } }
    public void OnClickClose() { if (selectedTower) { CloseMenu(); }}
}