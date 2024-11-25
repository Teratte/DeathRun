using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E; // Ư�� Ű ���� (E Ű)
    public TrapController trap; // ������ �����ϱ� ���� ����
    private bool isPlayerNearby = false; // �÷��̾ ��ư ��ó�� �ִ��� Ȯ��
    private bool isButtonUsed = false; // ��ư�� �̹� ���ȴ��� Ȯ��

    Renderer capsuleColor;

    void Start()
    {
        capsuleColor = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Key Press Detected");
        }

        // ��ư�� �̹� ���Ǿ����� �� �̻� ó������ ����
        if (isButtonUsed) return;

        // �÷��̾ ��ư ��ó�� ���� �� Ű �Է��� Ȯ��
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Button Pressed!");
            trap.ActivateTrap(); // ���� �۵�
            isButtonUsed = true; // ��ư ���¸� '����'���� ����
            capsuleColor.material.color = Color.blue;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ��ư ������ ���Դ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player Near Button");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ ��ư ������ ����� ���� �ʱ�ȭ
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player Left Button");
        }
    }
}
