using UnityEngine.Animations.Rigging;
using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0f;
    private float v = 0f;

    private float smoothH = 0f;
    private float smoothV = 0f;

    private float hVelocity = 0f; // Horizontal 보간용 속도
    private float vVelocity = 0f; // Vertical 보간용 속도

    private Transform tr;
    private Rigidbody rb;
    private Animator animator;

    public Transform cameraArm;
    public Transform[] spine;
    public Transform mainCamera;
    public GameObject playerBody;
    public GameObject playerArm;
    public Animator armAnimator;

    public GameObject hitbox; // 히트박스 오브젝트 연결

    public float speed = 10f;
    public float rotSpeed = 800f;
    public float jumpForce = 5f;

    public float groundCheckDistance = 1.1f;

    private bool isGrounded = false;
    public float mouseSensitivity = 2f;

    public int maxHealth = 100; // 최대 체력
    private int currentHealth; // 현재 체력
    private bool isDead = false; // 사망 상태 확인

    private bool isAttackOnCooldown = false; // 공격 쿨타임 관리
    public float attackCooldown = 0.5f; // 쿨타임 시간

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        SetLayerRecursively(playerBody, LayerMask.NameToLayer("LocalPlayerBody"));
        SetLayerRecursively(playerArm, LayerMask.NameToLayer("Default"));

        currentHealth = maxHealth; // 체력 초기화

        if (hitbox != null)
        {
            hitbox.SetActive(false); // 히트박스 비활성화
        }

        RigBuilder rigBuilder = GetComponent<RigBuilder>();
        if (rigBuilder != null)
        {
            rigBuilder.Build();
        }
    }

    void Update()
    {
        if (isDead) return; // 사망 상태면 업데이트 종료

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        smoothH = Mathf.SmoothDamp(smoothH, h, ref hVelocity, 0.1f);
        smoothV = Mathf.SmoothDamp(smoothV, v, ref vVelocity, 0.1f);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * Time.deltaTime * speed);

        RotatePlayer();

        // 카메라 Y축
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraArm.Rotate(Vector3.left * mouseY);

        Vector3 clampedRotation = cameraArm.localEulerAngles;
        if (clampedRotation.x > 180f) clampedRotation.x -= 360f;
        clampedRotation.x = Mathf.Clamp(clampedRotation.x, -80f, 80f);
        cameraArm.localEulerAngles = new Vector3(clampedRotation.x, 0f, 0f);

        isGrounded = CheckGrounded();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump"); // 점프 애니메이션 실행
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack(); // 공격 실행
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("IsFalling", true); // 떨어지는 애니메이션 실행
        }
        else
        {
            animator.SetBool("IsFalling", false); // 착지 상태로 전환
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
        if (isAttackOnCooldown) return; // 쿨타임 중에는 공격 실행 불가

        isAttackOnCooldown = true; // 공격 중
        animator.SetTrigger("Attack");
        armAnimator.SetTrigger("Attack");

        StartCoroutine(EnableHitbox());
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator EnableHitbox()
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.5f); // 히트박스 활성화 시간
        hitbox.SetActive(false);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown); // 쿨타임 대기
        isAttackOnCooldown = false; // 쿨타임 해제
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
                targetPlayer.TakeDamage(10); // 다른 플레이어에게 10 데미지
            }
        }
    }
}
