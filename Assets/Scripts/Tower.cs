using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [Header("타워 기본 정보")]
    public string towerName;
    public int baseCost = 30;

    [Header("공격 설정")]
    public float range = 5f;       
    public float damage = 10f;     
    public float fireRate = 1f;    
    
    [Header("필수 연결")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     
    public GameObject rangeIndicator; 
    public Outline towerOutline;      
    public GameObject floatingTextPrefab;

    [Header("업그레이드 상승폭")]
    public float damageStep = 5f;   
    public float rangeStep = 0.5f;
    public float rateStep = 0.2f;

    // 현재 레벨
    [HideInInspector] public int levelDamage = 1;
    [HideInInspector] public int levelRange = 1;
    [HideInInspector] public int levelRate = 1;

    [HideInInspector] public GridTile ownedTile;

    // 내부 변수
    private Transform target;
    private float fireCountdown = 0f;
    public int totalSpentMoney = 0; 
    private bool isSelected = false;

    // [추가] 최적화를 위한 타겟 검색 쿨타임 변수
    private float searchCountdown = 0f;
    public LayerMask enemyLayer;
    public Transform partToRotate;
    public float turnSpeed = 10;

    void Start()
    {
        towerName = gameObject.name.Replace("(Clone)", "").Trim();
        totalSpentMoney = baseCost;
        
        // [변경] InvokeRepeating 제거함.
        // 이제 Update에서 직접 로직을 제어합니다.
    }

    // --- 공격 및 타겟팅 로직 (핵심 변경 부분) ---

    void Update()
    {
        // 1. 현재 타겟이 유효한지 검사 (거리 & 생존 여부)
        if (target != null)
        {
            // 타겟이 비활성화(죽음) 되었거나, 사거리를 벗어났는지 매 프레임 체크
            if (!target.gameObject.activeInHierarchy || Vector3.Distance(transform.position, target.position) > range)
            {
                target = null; // 타겟 놓아줌
            }
        }

        // 2. 타겟이 없다면? -> 새로운 타겟 검색 (0.2초마다 시도 - 최적화)
        if (target == null)
        {
            searchCountdown -= Time.deltaTime;
            if (searchCountdown <= 0f)
            {
                FindNearestTarget();
                searchCountdown = 0.2f; // 검색 주기는 0.2초로 설정
            }
        }

        // 3. 타겟이 있다면? -> 공격
        if (target != null)
        {
            LockOnTarget();
            // 공격 쿨타임 계산
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }
    void LockOnTarget()
    {
        // 1. 적이 있는 방향 벡터 구하기
        Vector3 dir = target.position - transform.position;

        // [최적화] 적과 나의 각도 차이가 5도 미만이면 회전 계산 건너뜀
        if (Vector3.Angle(partToRotate.forward, dir) < 5f) return;

        // 2. 그 방향을 바라보는 회전값(Quaternion) 계산
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        // 3. 부드럽게 회전시키기 (Lerp 사용)
        // partToRotate가 없다면 transform(자기자신)을 회전
        Transform rotationTarget = partToRotate != null ? partToRotate : transform;
        
        Vector3 rotation = Quaternion.Lerp(rotationTarget.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;

        // 4. Y축(좌우)으로만 돌리고, X/Z축(위아래/기울기)은 고정
        // (안 그러면 적이 높거나 낮을 때 타워가 이상하게 기울어집니다)
        rotationTarget.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    // [변경] 이름 변경: UpdateTarget -> FindNearestTarget
    // 가장 가까운 적을 찾아서 target 변수에 할당하는 역할만 함
    // [중요] Inspector에서 이 변수에 'Enemy' 레이어를 꼭 선택해줘야 합니다! 
    void FindNearestTarget()
    {
        // 1. 수정됨: enemies inRange -> enemiesInRange (띄어쓰기 제거)
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, enemyLayer);
        
        float shortestDistanceSqr = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider enemyCollider in enemiesInRange)
        {
            if (!enemyCollider.CompareTag("Enemy")) continue;
            
            float distanceSqr = (transform.position - enemyCollider.transform.position).sqrMagnitude;

            if (distanceSqr < shortestDistanceSqr)
            {
                shortestDistanceSqr = distanceSqr;
                nearestEnemy = enemyCollider.gameObject;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

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

    public void SetSelectionState(bool selected)
    {
        isSelected = selected;
        ToggleHighlight(selected);
    }

    private void ToggleHighlight(bool isOn)
    {
        if (towerOutline != null)
        {
            towerOutline.enabled = isOn;
        }
    }

    // --- 마우스 상호작용 (Hover & Click) ---

    void OnMouseEnter()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        Debug.Log("마우스 인식 성공! " + gameObject.name);
        ToggleHighlight(true);
    }

    void OnMouseExit()
    {
        if (!isSelected)
        {
            ToggleHighlight(false);
        }
    }

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
            range += rangeStep;
            totalSpentMoney += cost;
            levelRange++;
            
            if (rangeIndicator != null && rangeIndicator.activeInHierarchy)
            {
                ShowRange(); 
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
            Vector3 spawnPos = transform.position + Vector3.up * 2.5f; 
            GameObject go = Instantiate(floatingTextPrefab, spawnPos, floatingTextPrefab.transform.rotation);
            Text textComp = go.GetComponentInChildren<Text>();
            if (textComp != null)
            {
                textComp.text = message;
                textComp.color = color; 
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
        MazeSolver.Instance.UnblockNode(transform.position); 
        
        if (ownedTile != null)
        {
            ownedTile.isOccupied = false;
        }
        Destroy(gameObject);
    }
}