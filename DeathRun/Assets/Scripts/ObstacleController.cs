using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // ��ֹ� Ȱ��ȭ �޼���
    public void Activate()
    {
        if (rb != null)
        {
            // �������� ���� ���ϴ� ����
            rb.AddForce(Vector3.up * 500f, ForceMode.Impulse);
        }
    }
}
