using UnityEngine;
using UnityEngine.SceneManagement; // 필수!

public class OpeningManager : MonoBehaviour
{
    // 버튼 클릭 시 호출될 함수
    public void GameStart()
    {
        // "GameScene" 부분에 실제 게임 씬 이름을 정확히 적으세요!
        SceneManager.LoadScene("GameScene"); 
    }
}