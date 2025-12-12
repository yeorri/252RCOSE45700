using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed = 20f; 
    private float damage = 20f;

    public void Setup(Transform _target, float _damage)
    {
        target = _target;
        damage = _damage;
    }

    void Update()
    {
        if (target == null) 
        {
            Destroy(gameObject); //target이 hp가 깎이든 도착해서든 없어지면 bullet 소멸
            return;
        }

        Vector3 dir = target.position - transform.position; //총알 방향 벡터 지속적 업데이트
        float distanceThisFrame = speed * Time.deltaTime; // 프레임당 이동할 수 있는 거리

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
        // 최적화된 방식: 적의 체력 스크립트를 가져와서 데미지 줌
        EnemyHP enemyHealth = target.GetComponent<EnemyHP>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        Destroy(gameObject); 
    }
}