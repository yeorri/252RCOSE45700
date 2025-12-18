using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [Header("Enemy 스펙")]
    public float maxHP = 100f;
    private float currentHP;
    public int killReward = 10;
    public int damageToBase = 1;

    [Header("UI")]
    public Slider hpSlider;

    [Header("이펙트")]
    public GameObject floatingTextPrefab;

    void OnEnable()
    {
        currentHP = maxHP;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHealthUI();
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    void UpdateHealthUI()
    {
        if(hpSlider != null)
        {
            hpSlider.value = currentHP / maxHP;
        }
    }

    void Die()
    {
        GameManager.Instance.AddMoney(killReward);
        
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);
            
            GameObject go = Instantiate(floatingTextPrefab, spawnPos, floatingTextPrefab.transform.rotation);

            Text textComp = go.GetComponentInChildren<Text>();

            if (textComp != null)
            {
                textComp.text = $"+{killReward}G"; 
            }
        }
        Destroy(gameObject); 
    }
}