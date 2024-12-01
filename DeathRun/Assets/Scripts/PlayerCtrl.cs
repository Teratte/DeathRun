using UnityEngine.Animations.Rigging;
using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0f;
    private float v = 0f;

    private float smoothH = 0f;
    private float smoothV = 0f;

    private float hVelocity = 0f; // Horizontal ������ �ӵ�
    private float vVelocity = 0f; // Vertical ������ �ӵ�

    private Transform tr;
    private Rigidbody rb;
    private Animator animator;

    public Transform cameraArm;
    public Transform[] spine;
    public Transform mainCamera;
    public GameObject playerBody;
    public GameObject playerArm;
    public Animator armAnimator;

    public GameObject hitbox; // ��Ʈ�ڽ� ������Ʈ ����

    public float speed = 10f;
    public float rotSpeed = 800f;
    public float jumpForce = 5f;

    public float groundCheckDistance = 1.1f;

    private bool isGrounded = false;
    public float mouseSensitivity = 2f;

    public int maxHealth = 100; // �ִ� ü��
    private int currentHealth; // ���� ü��
    private bool isDead = false; // ��� ���� Ȯ��

    private bool isAttackOnCooldown = false; // ���� ��Ÿ�� ����
    public float attackCooldown = 0.5f; // ��Ÿ�� �ð�

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        SetLayerRecursively(playerBody, LayerMask.NameToLayer("LocalPlayerBody"));
        SetLayerRecursively(playerArm, LayerMask.NameToLayer("Default"));

        currentHealth = maxHealth; // ü�� �ʱ�ȭ

        if (hitbox != null)
        {
            hitbox.SetActive(false); // ��Ʈ�ڽ� ��Ȱ��ȭ
        }

        RigBuilder rigBuilder = GetComponent<RigBuilder>();
        if (rigBuilder != null)
        {
            rigBuilder.Build();
        }
    }

    void Update()
    {
        if (isDead) return; // ��� ���¸� ������Ʈ ����

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        smoothH = Mathf.SmoothDamp(smoothH, h, ref hVelocity, 0.1f);
        smoothV = Mathf.SmoothDamp(smoothV, v, ref vVelocity, 0.1f);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * Time.deltaTime * speed);

        RotatePlayer();

        // ī�޶� Y��
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraArm.Rotate(Vector3.left * mouseY);

        Vector3 clampedRotation = cameraArm.localEulerAngles;
        if (clampedRotation.x > 180f) clampedRotation.x -= 360f;
        clampedRotation.x = Mathf.Clamp(clampedRotation.x, -80f, 80f);
        cameraArm.localEulerAngles = new Vector3(clampedRotation.x, 0f, 0f);

        isGrounded = CheckGrounded();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump"); // ���� �ִϸ��̼� ����
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack(); // ���� ����
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("IsFalling", true); // �������� �ִϸ��̼� ����
        }
        else
        {
            animator.SetBool("IsFalling", false); // ���� ���·� ��ȯ
        }

        if (moveDir.magnitude > 0)
        {
            animator.SetFloat("Speed", 1.0f);
            animator.SetFloat("Vertical", smoothV);
            animator.SetFloat("Horizontal", smoothH);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        tr.Rotate(Vector3.up * mouseX);
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(tr.position, Vector3.down, out hit, groundCheckDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    void SetLayerRecursively(GameObject obj, int bodyLayer)
    {
        obj.layer = bodyLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, bodyLayer);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log($"Player Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        Debug.Log("Player is dead!");

        this.enabled = false;
        rb.velocity = Vector3.zero;
        Invoke("Respawn", 3f);
    }

    private void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        this.enabled = true;
        transform.position = Vector3.zero;
        Debug.Log("Player respawned!");
    }

    private void Attack()
    {
        if (isAttackOnCooldown) return; // ��Ÿ�� �߿��� ���� ���� �Ұ�

        isAttackOnCooldown = true; // ���� ��
        animator.SetTrigger("Attack");
        armAnimator.SetTrigger("Attack");

        StartCoroutine(EnableHitbox());
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator EnableHitbox()
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.5f); // ��Ʈ�ڽ� Ȱ��ȭ �ð�
        hitbox.SetActive(false);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown); // ��Ÿ�� ���
        isAttackOnCooldown = false; // ��Ÿ�� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitbox.activeSelf && other.CompareTag("Killer"))
        {
            Debug.Log("Hit");
            PlayerCtrl targetPlayer = other.GetComponent<PlayerCtrl>();
            if (targetPlayer != null)
            {
                Debug.Log("TakeDamage");
                targetPlayer.TakeDamage(10); // �ٸ� �÷��̾�� 10 ������
            }
        }
    }
}
