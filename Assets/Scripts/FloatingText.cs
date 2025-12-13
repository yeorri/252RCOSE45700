using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;   // 떠오르는 속도
    public float destroyTime = 0.7f; 

    void Start()
    {
        // 태어나자마자 자신의 죽을 날짜를 정합니다. 
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 매 프레임 위쪽(Y축)으로 이동합니다.
        // 텍스트가 누워있다면(Rot 90), 이 코드는 화면상 '위쪽(Z축)'으로 이동하게 보여서 자연스럽습니다.
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}