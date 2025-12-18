using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed = 20f; 
    private float damage = 20f;

    // [추가] 총알이 타겟을 못 만나고 무한히 날아가는 것을 방지하기 위한 수명
    private float lifeTime = 2f;
    private float timer;

    public void Setup(Transform _target, float _damage)
    {
        target = _target;
        damage = _damage;
        timer = 0f; // [추가] 활성화될 때마다 타이머 초기화
    }

    // [추가] 오브젝트 풀링에서 꺼내질 때 실행되는 함수
    void OnEnable()
    {
        timer = 0f; // 타이머 리셋
    }

    void Update()
    {
        // 1. 수명 체크 (타겟을 못 찾아도 일정 시간 뒤엔 풀로 반납)
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Deactivate();
            return;
        }

        // 2. 타겟이 없어졌다면 풀로 반납
        if (target == null || !target.gameObject.activeInHierarchy) 
        {
            Deactivate();
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target); 
    }

    void HitTarget()
    {
        EnemyHP enemyHealth = target.GetComponent<EnemyHP>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        
        // [변경] Destroy 대신 비활성화(풀로 반납)
        Deactivate(); 
    }

    // [추가] 공통 비활성화 함수
    void Deactivate()
    {
        target = null; // 타겟 정보 초기화
        gameObject.SetActive(false); // 오브젝트 풀러로 반납
    }
}