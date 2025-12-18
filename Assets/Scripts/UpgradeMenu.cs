using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance;
    private Tower selectedTower;

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI damageBtnText;
    public TextMeshProUGUI rangeBtnText;
    public TextMeshProUGUI rateBtnText;
    public TextMeshProUGUI sellBtnText;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SelectTower(Tower tower)
    {
        // 1. 이전에 선택된 타워가 있다면? -> 선택 해제 및 하이라이트 끄기
        if (selectedTower != null)
        {
            selectedTower.HideRange();
            selectedTower.SetSelectionState(false); // 이전 타워에게 "너 이제 선택 풀렸어"라고 알림
        }

        selectedTower = tower;

        // 2. 새로 선택된 타워 -> 하이라이트 켜기
        selectedTower.ShowRange();
        selectedTower.SetSelectionState(true); // 새 타워에게 "너 선택됐어!"라고 알림 (이게 있어야 유지됨!)

        // 메뉴 위치 잡기
        transform.position = tower.transform.position + Vector3.up * 3f + Vector3.right * 2f + Vector3.back * 1f; 
        
        UpdateBtnUI();
        
        // 정보창 갱신 (TowerInfoPanel이 있다면)
        if (TowerInfoPanel.Instance != null)
        {
            TowerInfoPanel.Instance.ShowInfo(tower);
            TowerInfoPanel.Instance.transform.position = tower.transform.position + Vector3.up * 3f + Vector3.left * 2f + Vector3.back * 1f;
            TowerInfoPanel.Instance.transform.rotation = transform.rotation;
        }
            
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        // 메뉴 닫을 때 선택 해제
        if (selectedTower != null)
        {
            selectedTower.HideRange();
            selectedTower.SetSelectionState(false); // [중요] 닫을 때도 하이라이트 꺼줌
        }

        selectedTower = null;
        gameObject.SetActive(false);
        
        if (TowerInfoPanel.Instance != null)
            TowerInfoPanel.Instance.HideInfo();
    }

    void UpdateBtnUI()
    {
        if (selectedTower == null) return;

        // 버튼 텍스트 갱신
        if(damageBtnText) damageBtnText.text = $"Damage Up ({selectedTower.GetDamageUpgradeCost()}G)";
        if(rangeBtnText) rangeBtnText.text = $"Range Up ({selectedTower.GetRangeUpgradeCost()}G)";
        if(rateBtnText) rateBtnText.text = $"Speed Up ({selectedTower.GetRateUpgradeCost()}G)";
        if(sellBtnText) sellBtnText.text = $"Sell (+{selectedTower.GetSellPrice()}G)";
    }

    // --- 버튼 연결 함수 ---
    public void OnClickDamage() 
    { 
        if (selectedTower) 
        { 
            selectedTower.UpgradeDamage(); 
            UpdateBtnUI(); 
            if(TowerInfoPanel.Instance != null) TowerInfoPanel.Instance.ShowInfo(selectedTower);
        } 
    }

    public void OnClickRange() 
    { 
        if (selectedTower) 
        { 
            selectedTower.UpgradeRange(); 
            UpdateBtnUI(); 
            if(TowerInfoPanel.Instance != null) TowerInfoPanel.Instance.ShowInfo(selectedTower);
        } 
    }

    public void OnClickRate() 
    { 
        if (selectedTower) 
        { 
            selectedTower.UpgradeRate(); 
            UpdateBtnUI(); 
            if(TowerInfoPanel.Instance != null) TowerInfoPanel.Instance.ShowInfo(selectedTower);
        } 
    }

    public void OnClickSell() 
    { 
        if (selectedTower) 
        { 
            selectedTower.SellTower(); 
            CloseMenu(); 
        } 
    }
    
    // 닫기 버튼
    public void OnClickClose() 
    { 
        CloseMenu(); 
    }
}