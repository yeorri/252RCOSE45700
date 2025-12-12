using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    void OnEnable()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); 
    }
}