using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;
    public int killReward = 10;
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
        //Debug.Log("현재 체력: " + currentHP + " / 슬라이더 값: " + (currentHP / maxHP));
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
            // 적의 현재 위치에 생성하되, 높이(Y)를 조금 올려서(1.0f) 잘 보이게 함
            Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);
            
            // 생성! (Quaternion.identity 대신 프리팹 자체의 회전값 사용)
            Instantiate(floatingTextPrefab, spawnPos, floatingTextPrefab.transform.rotation);
        }
        Destroy(gameObject); 
    }
}