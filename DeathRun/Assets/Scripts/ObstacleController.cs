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

    // ��ֹ� Ȱ��ȭ �޼���
    public void Activate()
    {
        if (rb != null)
        {
            // ���������� ��������
            obstacleRenderer.material.color = Color.green; // ���� ����
            rb.AddForce(Vector3.right * 5f, ForceMode.Impulse);
        }
    }
}
