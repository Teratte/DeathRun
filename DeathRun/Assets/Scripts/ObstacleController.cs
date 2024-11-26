using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 장애물 활성화 메서드
    public void Activate()
    {
        if (rb != null)
        {
            // 위쪽으로 힘을 가하는 예제
            rb.AddForce(Vector3.up * 500f, ForceMode.Impulse);
        }
    }
}
