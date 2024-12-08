using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // 초기에는 중력을 비활성화
    }

    // 버튼 컨트롤러에서 호출하는 Activate 메서드
    public void Activate()
    {
        if (rb != null)
        {
            rb.useGravity = true; // 중력 활성화
        }
        else
        {
            Debug.LogWarning("Rigidbody가 없습니다!");
        }
    }

    // 충돌 처리
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그를 확인
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 장애물 삭제
        }
    }
}

