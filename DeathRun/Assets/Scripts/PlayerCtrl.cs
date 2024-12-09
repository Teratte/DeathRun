using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour, IPunObservable
{
    [SerializeField] TextMesh playerName;
    private PhotonView pv; // PhotonView
    private Vector3 currPos;
    private Quaternion currRot;
    private Quaternion currArmRot;


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
    //public Transform[] spine;
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

        pv = GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this;
        if(pv.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;
            playerName.gameObject.SetActive(false);
        }
        else
        {
            playerName.text = pv.Owner.NickName;
        }
        gameObject.tag = pv.IsMine ? (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerTag"] : (string)pv.Owner.CustomProperties["PlayerTag"];

        pv = GetComponent<PhotonView>(); // PhotonView ��������
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth; // ü�� �ʱ�ȭ

        if (hitbox != null)
        {
            hitbox.SetActive(false); // ��Ʈ�ڽ� ��Ȱ��ȭ
        }

        if (!pv.IsMine)
        {
            // ���� �÷��̾ �ƴϸ� ī�޶� ��Ȱ��ȭ
            gameObject.GetComponentInChildren<Camera>().enabled = false;
            
        }
        else
        {
            SetLayerRecursively(playerBody, LayerMask.NameToLayer("LocalPlayerBody"));
            SetLayerRecursively(playerArm, LayerMask.NameToLayer("Default"));
        }
    }

    void Update()
    {
        if (isDead) return; // 플레이어가 죽었으면 동작 멈춤

        if (pv.IsMine)
        {
            // 로컬 플레이어의 입력 처리
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            smoothH = Mathf.SmoothDamp(smoothH, h, ref hVelocity, 0.1f);
            smoothV = Mathf.SmoothDamp(smoothV, v, ref vVelocity, 0.1f);

            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * Time.deltaTime * speed);

            RotatePlayer();

            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            cameraArm.Rotate(Vector3.left * mouseY);

            Vector3 clampedRotation = cameraArm.localEulerAngles;
            if (clampedRotation.x > 180f) clampedRotation.x -= 360f;
            clampedRotation.x = Mathf.Clamp(clampedRotation.x, -80f, 80f);
            cameraArm.localEulerAngles = new Vector3(clampedRotation.x, 0f, 0f);

            isGrounded = CheckGrounded();

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                pv.RPC("SyncTrigger", RpcTarget.Others, "Jump");
            }

            if (!isGrounded && rb.velocity.y < 0)
            {
                animator.SetBool("IsFalling", true);
                pv.RPC("SyncBool", RpcTarget.Others, "IsFalling", true);
            }
            else
            {
                animator.SetBool("IsFalling", false);
                pv.RPC("SyncBool", RpcTarget.Others, "IsFalling", false);
            }

            if (moveDir.magnitude > 0)
            {
                animator.SetFloat("Speed", 1.0f);
                pv.RPC("SyncFloat", RpcTarget.Others, "Speed", 1.0f);
                animator.SetFloat("Horizontal", smoothH);
                pv.RPC("SyncFloat", RpcTarget.Others, "Horizontal", smoothH);
                animator.SetFloat("Vertical", smoothV);
                pv.RPC("SyncFloat", RpcTarget.Others, "Vertical", smoothV);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
                pv.RPC("SyncFloat", RpcTarget.Others, "Speed", 0.0f);
            }
        }
        else if (!pv.IsMine)
        {
            if (Vector3.Distance(tr.position, currPos) > 0.03f)
            {
                animator.SetFloat("Speed", 1.0f);
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }

            if (tr.rotation != currRot)
            {
                tr.rotation = Quaternion.Lerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
            }

            if (cameraArm.localRotation != currArmRot)
            {
                cameraArm.localRotation = Quaternion.Lerp(cameraArm.localRotation, currArmRot, Time.deltaTime * 10.0f);
            }
        }

        playerName.transform.LookAt(playerName.transform.position + Camera.main.transform.forward);
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

    private void Attack()
    {
        if (isAttackOnCooldown) return; // ��Ÿ�� �߿��� ���� ���� �Ұ�

        isAttackOnCooldown = true; // ���� ��
        animator.SetTrigger("Attack");
        armAnimator.SetTrigger("Attack");

        StartCoroutine(EnableHitbox());
        StartCoroutine(AttackCooldown());

        // ��Ʈ�ڽ� ����ȭ
        pv.RPC("EnableHitboxRPC", RpcTarget.Others);
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

    void SetLayerRecursively(GameObject obj, int bodyLayer)
    {
        obj.layer = bodyLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, bodyLayer);
        }
    }

    [PunRPC]
    private void EnableHitboxRPC()
    {
        StartCoroutine(EnableHitbox());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
            stream.SendNext(cameraArm.localRotation);
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("IsFalling"));
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            currArmRot = (Quaternion)stream.ReceiveNext(); 
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("IsFalling", (bool)stream.ReceiveNext());
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

    private void OnTriggerEnter(Collider other)
    {
        //if (hitbox.activeSelf && other.CompareTag("Tracer"))
        //{
        //    Debug.Log("Hit");
        //    PlayerCtrl targetPlayer = other.GetComponent<PlayerCtrl>();
        //    if (targetPlayer != null)
        //    {
        //        Debug.Log("TakeDamage");
        //        targetPlayer.TakeDamage(10); // �ٸ� �÷��̾�� 10 ������
        //    }
        //}

        //if(other.CompareTag("Dead_Obs"))
        //{
        //    TakeDamage(100);
        //}
    }

    [PunRPC]
    private void SyncTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void SyncBool(string boolName, bool value)
    {
        animator.SetBool(boolName, value);
    }

    [PunRPC]
    private void SyncFloat(string floatName, float value)
    {
        animator.SetFloat(floatName, value);
    }




}
