using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [Header("타워 기본 정보")]
    public string towerName;
    public int baseCost = 30;

    [Header("공격 설정")]
    public float range = 5f;       // 사거리
    public float damage = 10f;     // 데미지
    public float fireRate = 1f;    // 초당 발사 속도
    
    [Header("필수 연결")]
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint;     // 총알 나가는 위치 (머리 끝)
    public GameObject rangeIndicator; // 사거리 표시용 구체
    public Outline towerOutline;      // [추가] 아웃라인 컴포넌트 (Quick Outline 에셋)

    public GameObject floatingTextPrefab;

    [Header("업그레이드 상승폭")]
    public float damageStep = 5f;   // (테스트용 500f -> 5f로 정상화)
    public float rangeStep = 0.5f;
    public float rateStep = 0.2f;

    // 현재 레벨
    [HideInInspector] public int levelDamage = 1;
    [HideInInspector] public int levelRange = 1;
    [HideInInspector] public int levelRate = 1;

    // 내가 깔고 앉은 타일 기억하기
    [HideInInspector] public GridTile ownedTile;

    // 내부 변수
    private Transform target;
    private float fireCountdown = 0f;
    public int totalSpentMoney = 0; 

    // [추가] 현재 선택된 상태인지 기억하는 변수
    private bool isSelected = false;

    void Start()
    {
        towerName = gameObject.name.Replace("(Clone)", "").Trim();
        totalSpentMoney = baseCost;
        // 0.5초마다 타겟을 다시 찾음 (최적화)
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    // --- 공격 로직 ---

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 공격 쿨타임 계산
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.Setup(target, damage); // 데미지 정보 전달
            }
        }
    }

    // --- 시각화 (사거리 & 아웃라인) ---

    // 사거리 표시 (디버그용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // 사거리 표시 (인게임용)
    public void ShowRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
            rangeIndicator.transform.localScale = Vector3.one * range * 2f;
        }
    }

    public void HideRange()
    {   
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    // [추가] 외부(UpgradeMenu)에서 선택 상태를 설정할 때 호출
    public void SetSelectionState(bool selected)
    {
        isSelected = selected;
        // 선택되면 켜고, 해제되면 끔
        ToggleHighlight(selected);
    }

    // [추가] 내부적으로 아웃라인을 껐다 켜는 함수
    private void ToggleHighlight(bool isOn)
    {
        if (towerOutline != null)
        {
            towerOutline.enabled = isOn;
        }
    }

    // --- 마우스 상호작용 (Hover & Click) ---

    // 마우스가 타워 위에 올라왔을 때 (Hover)
    void OnMouseEnter()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        
        // 마우스 올리면 무조건 하이라이트 ON
        ToggleHighlight(true);
    }

    // 마우스가 타워에서 나갔을 때
    void OnMouseExit()
    {
        // "선택된 상태(isSelected)"가 아닐 때만 하이라이트를 끔
        // 선택되어 있다면 마우스가 나가도 하이라이트 유지
        if (!isSelected)
        {
            ToggleHighlight(false);
        }
    }

    // 타워 클릭 시
    void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        
        if (UpgradeMenu.Instance != null)
        {
            UpgradeMenu.Instance.SelectTower(this);
        }
    }

    // --- 업그레이드 및 판매 로직 ---

    public int GetDamageUpgradeCost() { return levelDamage * 10; }
    public int GetRangeUpgradeCost() { return levelRange * 10; }
    public int GetRateUpgradeCost() { return levelRate * 10; }

    public void UpgradeDamage()
    {
        int cost = GetDamageUpgradeCost();
        if (GameManager.Instance.SpendMoney(cost))
        {
            damage += damageStep;
            totalSpentMoney += cost;
            levelDamage++;
            ShowFloatingText("Dmg Up", Color.red);
        }
    }

    public void UpgradeRange()
    {
        int cost = GetRangeUpgradeCost();
        if (GameManager.Instance.SpendMoney(cost))
        {
            // 1. 수치 증가
            range += rangeStep;
            totalSpentMoney += cost;
            levelRange++;
            
            // 2. [추가] 사거리 표시가 켜져 있다면, 즉시 크기 갱신!
            if (rangeIndicator != null && rangeIndicator.activeInHierarchy)
            {
                ShowRange(); // 이 함수가 변경된 range 값으로 크기를 다시 맞춥니다.
            }
            ShowFloatingText("Range Up", Color.cyan);
        }
    }

    public void UpgradeRate()
    {
        int cost = GetRateUpgradeCost();
        if (GameManager.Instance.SpendMoney(cost))
        {
            fireRate += rateStep;
            totalSpentMoney += cost;
            levelRate++;
            ShowFloatingText("Spd Up", Color.green);
        }
    }
    void ShowFloatingText(string message, Color color)
    {
        if (floatingTextPrefab != null)
        {
            // 타워 머리 위(높이 2.5f 정도)에 생성
            Vector3 spawnPos = transform.position + Vector3.up * 2.5f; 
            
            // 1. 생성
            GameObject go = Instantiate(floatingTextPrefab, spawnPos, floatingTextPrefab.transform.rotation);
            
            // 2. 텍스트 변경
            Text textComp = go.GetComponentInChildren<Text>();
            if (textComp != null)
            {
                textComp.text = message;
                textComp.color = color; // 색상도 바꿔줍니다!
            }
        }
    }
    public int GetSellPrice()
    {
        return Mathf.RoundToInt(totalSpentMoney * 0.7f);
    }

    public void SellTower()
    {
        GameManager.Instance.AddMoney(GetSellPrice());
        MazeSolver.Instance.UnblockNode(transform.position); // 길 뚫기
        
        // 타일 상태 초기화
        if (ownedTile != null)
        {
            ownedTile.isOccupied = false;
        }
        Destroy(gameObject);
    }
}