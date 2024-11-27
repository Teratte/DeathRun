using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject obstacle; // 연결할 장애물
    public Transform player; // 플레이어의 Transform
    public KeyCode activationKey = KeyCode.E; // 작동 키 설정
    public float activationRange = 1.0f; // 작동 가능한 최대 거리
    private bool isActivated = false; // 버튼의 작동 상태
    private Renderer buttonRenderer; // 색상 설정
    private Rigidbody rb; // RigidBody

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

        // 플레이어와 버튼 사이의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 키 입력 및 거리 조건 확인
        if (distance < activationRange && Input.GetKeyDown(activationKey) && !isActivated)
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActivated = true; // 버튼 활성화
        buttonRenderer.material.color = Color.blue; // 색상 변경

        if (obstacle != null)
        {
            ObstacleController controller = obstacle.GetComponent<ObstacleController>();
            if (controller != null)
            {
                controller.Activate(); // 장애물 활성화
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
