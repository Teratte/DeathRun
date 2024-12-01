using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject[] obstacles; // ���� ��ֹ��� ������ �迭
    public Transform player; // �÷��̾� ��ġ
    public KeyCode activationKey = KeyCode.E; // �۵� Ű
    public float activationRange = 3.0f; // ��ư �۵� ����
    private bool isActivated = false;
    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        buttonRenderer.material.color = Color.red;

        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(pl    ayer.position, transform.position);
        if (distance <= activationRange && Input.GetKeyDown(activationKey) && !isActivated)
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActivated = true;
        buttonRenderer.material.color = Color.blue;

        foreach (GameObject obstacle in obstacles)
        {
            IObstacle obstacleScript = obstacle.GetComponent<IObstacle>();
            if (obstacleScript != null)
            {
                obstacleScript.Activate();
            }
        }
    }
}
