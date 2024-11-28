using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer obstacleRenderer;

    void Start()
    {
        obstacleRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        obstacleRenderer.material.color = Color.black;
    }

    // ��ֹ� Ȱ��ȭ �޼���
    public void Activate()
    {
        if (rb != null)
        {
            // �������� ���� ���ϴ� ����
            rb.AddForce(Vector3.right * 50f, ForceMode.Impulse);
        }
    }
}
