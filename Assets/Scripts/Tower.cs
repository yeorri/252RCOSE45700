using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("타워 기본 정보")]
    public string towerName = "Basic Tower";
    public int baseCost = 30;

    [Header("공격 설정")]
    public float range = 5f;       // 사거리
    public float damage = 10f;     // 데미지
    public float fireRate = 1f;    // 초당 발사 속도
    
    [Header("필수 연결")]
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint;     // 총알 나가는 위치 (머리 끝)
    public GameObject rangeIndicator;

    [Header("업그레이드 상승폭")]
    public float damageStep = 500f;
    public float rangeStep = 0.5f;
    public float rateStep = 0.2f;

    // 현재 레벨
    [HideInInspector] public int levelDamage = 1;
    [HideInInspector] public int levelRange = 1;
    [HideInInspector] public int levelRate = 1;

    // [추가] 내가 깔고 앉은 타일 기억하기
    [HideInInspector] public GridTile ownedTile;

    // 내부 변수
    private Transform target;
    private float fireCountdown = 0f;
    public int totalSpentMoney = 0; // public으로 변경 (UpgradeMenu에서 접근하기 편하게)

    void Start()
    {
        totalSpentMoney = baseCost;
        // 0.5초마다 타겟을 다시 찾음 (최적화)
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    // --- [삭제되었던 공격 로직 복구] ---

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

        // 타겟을 바라보게 회전 (선택 사항)
        // Vector3 dir = target.position - transform.position;
        // Quaternion lookRotation = Quaternion.LookRotation(dir);
        // Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        // transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

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

    // --- [업그레이드 및 판매 로직] ---

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
        // 3. [핵심] 타일 상태 초기화! (이제 다시 지을 수 있음)
        if (ownedTile != null)
        {
            ownedTile.isOccupied = false;
        }
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        
        if (UpgradeMenu.Instance != null)
        {
            UpgradeMenu.Instance.SelectTower(this);
        }
    }
}