using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // 메인 카메라의 위치 정보를 가져옵니다.
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // LateUpdate는 모든 이동/회전 계산이 끝난 직후에 실행됩니다.
    // (적이 몸을 다 돌린 후에 UI가 "잠깐! 넌 저기 봐" 하고 고쳐주는 타이밍)
    void LateUpdate()
    {
        if (camTransform == null) return;

        // 내 회전값을 카메라의 회전값과 똑같이 맞춥니다.
        // 이러면 항상 카메라와 평행하게 서 있게 됩니다.
        transform.rotation = camTransform.rotation;
    }
}