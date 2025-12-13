using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("설정")]
    public int cost = 10;
    public float range = 5f;
    public float fireRate = 1f;
    public LayerMask enemyLayer;

    [Header("참조")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform target;
    private float fireCountdown = 0f;
    
    // 총알을 담을 부모 폴더
    private Transform bulletContainer; 

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.2f); // UpdateTarget 를 1초에 5번 실행해

        // 게임 세상에 있는 "Bullets"라는 이름의 오브젝트를 찾는다.
        GameObject containerObj = GameObject.Find("Bullets");
        if (containerObj != null)
        {
            bulletContainer = containerObj.transform;
        }
    }

    // ... (UpdateTarget과 Update 함수는 그대로 둠) ...
    void UpdateTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        if (nearestEnemy != null) target = nearestEnemy.transform; //neareast enemy를 target에 등록
        else target = null;
    }

    void Update()
    {
        if (target == null) return;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

        GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity, bulletContainer);
        
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Setup(target, 20f);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}