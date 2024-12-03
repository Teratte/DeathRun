using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotatingObstacle : MonoBehaviour, IObstacle
{
    public Vector3 rotationAxis = Vector3.up; // ȸ�� ��
    public float rotationSpeed = 90f;        // ȸ�� �ӵ�
    private bool isActivated = false;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            // ���ÿ��� �ٷ� ����
            StartCoroutine(RotateObstacle());

            // ��Ʈ��ũ ����ȭ
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
        // ��Ʈ��ũ Ŭ���̾�Ʈ������ ������ �ڷ�ƾ ����
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
