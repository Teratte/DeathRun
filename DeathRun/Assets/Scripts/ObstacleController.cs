using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer obstacleRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleRenderer = GetComponent<Renderer>();
    }

    // 장애물 활성화 메서드
    public void Activate()
    {
        if (rb != null)
        {
            // 오른쪽으로 날려버림
            obstacleRenderer.material.color = Color.green; // 색상 변경
            rb.AddForce(Vector3.right * 5f, ForceMode.Impulse);
        }
    }
}
