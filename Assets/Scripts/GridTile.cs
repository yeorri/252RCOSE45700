using UnityEngine;

public class GridTile : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    
    [Header("색상 설정")]
    public Color hoverColor = Color.green; // 건설 가능 (초록)
    public Color errorColor = Color.red;   // 건설 불가 (빨강)

    [HideInInspector] public Vector3 myPosition; 
    public bool isOccupied = false; 

    // 이번에 마우스 올렸을 때 여기가 안전했는지 기억하는 변수
    private bool isBuildSafe = false;
    private bool isMenuOpen = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void SetSelectionState(bool isOpen)
    {
        isMenuOpen = isOpen;

        if (isMenuOpen)
        {
            // 메뉴가 열리면 강제로 초록색 유지
            rend.material.color = hoverColor;
        }
        else
        {
            // 메뉴가 닫히면 원래 색으로 복구
            rend.material.color = originalColor;
        }
    }

    // 마우스가 들어왔을 때 (미리 검사!)
    void OnMouseEnter()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        if (isOccupied) return; // 이미 타워 있으면 무시
        if (isMenuOpen) return;

        // 1. TowerBuilder에게 물어봅니다. "여기 길 막히나요?"
        // (마우스 올리는 순간 즉시 계산)
        isBuildSafe = TowerBuilder.Instance.CheckPathSafety(myPosition);

        // 2. 결과에 따라 색상 변경
        if (isBuildSafe)
        {
            rend.material.color = hoverColor; // 안전 -> 초록색
        }
        else
        {
            rend.material.color = errorColor; // 막힘 -> 빨간색
        }
    }

    void OnMouseExit()
    {
        if (!isMenuOpen)
        {
            rend.material.color = originalColor; // 원래 색 복구
        }
    }

    // 클릭했을 때
    void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        if (isOccupied) return;
        
        isBuildSafe = TowerBuilder.Instance.CheckPathSafety(myPosition);
        // 3. 아까 검사한 결과(isBuildSafe)를 보고 행동 결정
        if (isBuildSafe)
        {
            // 안전하면 -> 메뉴 오픈!
            TowerBuilder.Instance.ShowBuildMenu(this);
        }
        else
        {
            // 위험하면 -> 메뉴 안 열고 경고 메시지!
            Debug.Log("여기에는 지을 수 없습니다! (길이 막힘)");
            
            // (선택) 여기서 "띠-딕!" 하는 경고음 효과음을 넣어도 좋습니다.
        }
    }
}