using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject[] obstacles; // 여러 장애물을 연결할 배열
    public KeyCode activationKey = KeyCode.E; // 작동 키
    public float activationRange = 3.0f; // 버튼 작동 범위
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
            // 버튼 주변에서 Tracer 태그를 가진 객체 탐색
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

        // 로컬에서 동작
        ActivateObstacles();

        // RPC 호출로 네트워크 동기화
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
