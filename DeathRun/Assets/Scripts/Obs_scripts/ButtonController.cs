using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject[] obstacles; // 여러 장애물을 연결할 배열
    public Transform player; // 플레이어 위치
    public KeyCode activationKey = KeyCode.E; // 작동 키
    public float activationRange = 3.0f; // 버튼 작동 범위
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

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= activationRange && Input.GetKeyDown(activationKey) && !isActivated)
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActivated = true;
        buttonRenderer.material.color = Color.green;

        foreach (GameObject obstacle in obstacles)
        {
            IObstacle obstacleScript = obstacle.GetComponent<IObstacle>();
            if (obstacleScript != null)
            {
                obstacleScript.Activate();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}

/*

public class ButtonController : MonoBehaviour
{
    public GameObject obstacle; // 연결할 장애물
    public Transform player; // 플레이어의 Transform
    public KeyCode activationKey = KeyCode.E; // 작동 키 설정
    public float activationRange = 0.1f; // 작동 가능한 최대 거리
    private bool isActivated = false; // 버튼의 작동 상태
    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        buttonRenderer.material.color = Color.red; // 초기 색상

        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < activationRange && Input.GetKeyDown(activationKey) && !isActivated)
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActivated = true;
        buttonRenderer.material.color = Color.blue;

        if (obstacle != null)
        {
            ObstacleController controller = obstacle.GetComponent<ObstacleController>();
            if (controller != null)
            {
                controller.Activate();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
*/