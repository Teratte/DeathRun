using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject[] obstacles; // ���� ��ֹ��� ������ �迭
    public KeyCode activationKey = KeyCode.E; // �۵� Ű
    public float activationRange = 3.0f; // ��ư �۵� ����
    private bool isActivated = false;
    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        buttonRenderer.material.color = Color.red;
    }

    void Update()
    {
        if (!isActivated && Input.GetKeyDown(activationKey))
        {
            // ��ư �ֺ����� Tracer �±׸� ���� ��ü Ž��
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, activationRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Tracer"))
                {
                    ActivateButton();
                    break;
                }
            }
        }
    }

    void ActivateButton()
    {
        isActivated = true;
        buttonRenderer.material.color = Color.green;

        // ���ÿ��� ����
        ActivateObstacles();

        // RPC ȣ��� ��Ʈ��ũ ����ȭ
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.RPC("RPC_ActivateButton", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    void RPC_ActivateButton()
    {
        if (!isActivated)
        {
            isActivated = true;
            buttonRenderer.material.color = Color.green;
            ActivateObstacles();
        }
    }

    void ActivateObstacles()
    {
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
