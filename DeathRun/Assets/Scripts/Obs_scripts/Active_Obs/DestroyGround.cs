using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyGround : MonoBehaviour
{
    private PhotonView photonView; // PhotonView ������Ʈ
    private bool isDestroyed = false; // ������ �ı��Ǿ����� ����

    void Start()
    {
        photonView = GetComponent<PhotonView>(); // PhotonView ������Ʈ ��������
    }

    // �÷��̾�� �浹�� �� ȣ��Ǵ� �޼��� (�ݸ��� ��� ���)
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� "Player" �±׸� ���� ���
        if (collision.gameObject.CompareTag("Player"))
        {
            // �̹� �ı��Ǿ����� �߰����� ó�� ���� ����
            if (!isDestroyed && PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ������ ó��
            {
                photonView.RPC("RPC_DestroyTrap", RpcTarget.All); // ��Ʈ��ũ ��� Ŭ���̾�Ʈ���� �ı� ����ȭ
            }
            else
            {
                DestroyTrap();
            }
        }
    }

    private void DestroyTrap()
    {
        Destroy(gameObject); // ���� ������Ʈ �ı�
    }

    // Ʈ�� �ı� RPC �޼���
    [PunRPC]
    void RPC_DestroyTrap()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }
}
