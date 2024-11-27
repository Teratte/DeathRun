using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject obstacle; // ������ ��ֹ�
    public Transform player; // �÷��̾��� Transform
    public KeyCode activationKey = KeyCode.E; // �۵� Ű ����
    public float activationRange = 1.0f; // �۵� ������ �ִ� �Ÿ�
    private bool isActivated = false; // ��ư�� �۵� ����
    private Renderer buttonRenderer; // ���� ����
    private Rigidbody rb; // RigidBody

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        buttonRenderer = GetComponent<Renderer>();
        buttonRenderer.material.color = Color.red; // �ʱ� ����

        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // �÷��̾�� ��ư ������ �Ÿ� ���
        float distance = Vector3.Distance(player.position, transform.position);

        // Ű �Է� �� �Ÿ� ���� Ȯ��
        if (distance < activationRange && Input.GetKeyDown(activationKey) && !isActivated)
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActivated = true; // ��ư Ȱ��ȭ
        buttonRenderer.material.color = Color.blue; // ���� ����

        if (obstacle != null)
        {
            ObstacleController controller = obstacle.GetComponent<ObstacleController>();
            if (controller != null)
            {
                controller.Activate(); // ��ֹ� Ȱ��ȭ
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
