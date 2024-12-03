using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RotateSaw : MonoBehaviour, IObstacle
{
    public Vector3 rotationAxis = Vector3.up; // ȸ�� ��
    public float rotationSpeed = 90f;        // ȸ�� �ӵ�
    private bool isActivated = false;

    private void Start()
    {
        // ���ÿ��� �ڷ�ƾ ����
        StartCoroutine(RotateObstacle());

        // ������ Ŭ���̾�Ʈ�� ȸ�� ����
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

            // ���ÿ��� ȸ�� ����
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
        // �ٸ� Ŭ���̾�Ʈ������ ȸ�� ����
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
            // ���� �� �������� ȸ��
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(rotationAxis * step, Space.Self);
            yield return null; // ���� ������ ���
        }
    }
}
