using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject obstacle; // ������ ��ֹ�
    public Transform player; // �÷��̾��� Transform
    public KeyCode activationKey = KeyCode.E; // �۵� Ű ����
    public float activationRange = 0.1f; // �۵� ������ �ִ� �Ÿ�
    private bool isActivated = false; // ��ư�� �۵� ����
    private Renderer buttonRenderer;

    void Start()
    {
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
