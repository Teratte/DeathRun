using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RotateSaw : MonoBehaviour, IObstacle
{
    public Vector3 rotationAxis = Vector3.up; // 회전 축
    public float rotationSpeed = 90f;        // 회전 속도
    private bool isActivated = false;

    private void Start()
    {
        // 로컬에서 코루틴 시작
        StartCoroutine(RotateObstacle());

        // 마스터 클라이언트만 회전 시작
        if (PhotonNetwork.IsMasterClient)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;

            // 로컬에서 회전 시작
            StartCoroutine(RotateObstacle());

            // 네트워크 동기화
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("RPC_Activate", RpcTarget.OthersBuffered);
            }
        }
    }

    [PunRPC]
    public void RPC_Activate()
    {
        // 다른 클라이언트에서도 회전 시작
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(RotateObstacle());
        }
    }

    private IEnumerator RotateObstacle()
    {
        while (true)
        {
            // 로컬 축 기준으로 회전
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(rotationAxis * step, Space.Self);
            yield return null; // 다음 프레임 대기
        }
    }
}
