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
            Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);
            
            // 1. 생성된 오브젝트를 변수(go)에 담습니다.
            GameObject go = Instantiate(floatingTextPrefab, spawnPos, floatingTextPrefab.transform.rotation);

            // 2. [핵심] 자식들 중에 숨어있는 Text 컴포넌트를 찾습니다.
            Text textComp = go.GetComponentInChildren<Text>();

            // 3. 찾았다면 내용을 killReward로 바꿔치기 합니다.
            if (textComp != null)
            {
                textComp.text = $"+{killReward}G"; 
            }
        }
        Destroy(gameObject); 
    }
}