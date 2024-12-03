using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotatingObstacle : MonoBehaviour, IObstacle
{
    public Vector3 rotationAxis = Vector3.up; // 회전 축
    public float rotationSpeed = 90f;        // 회전 속도
    private bool isActivated = false;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            // 로컬에서 바로 시작
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
        // 네트워크 클라이언트에서도 동일한 코루틴 실행
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(RotateObstacle());
        }
    }

    private IEnumerator RotateObstacle()
    {
        float rotationAmount = 0f;
        while (rotationAmount < 360f)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(rotationAxis * step);
            rotationAmount += step;
            yield return null;
        }
    }
}
