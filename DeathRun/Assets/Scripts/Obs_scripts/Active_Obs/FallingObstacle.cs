using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // �ʱ⿡�� �߷��� ��Ȱ��ȭ
    }

    // ��ư ��Ʈ�ѷ����� ȣ���ϴ� Activate �޼���
    public void Activate()
    {
        if (rb != null)
        {
            rb.useGravity = true; // �߷� Ȱ��ȭ
        }
        else
        {
            Debug.LogWarning("Rigidbody�� �����ϴ�!");
        }
    }

    // �浹 ó��
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �±׸� Ȯ��
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // ��ֹ� ����
        }
    }
}

