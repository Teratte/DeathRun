using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right; // �̵� ����
    public float moveDistance = 5.0f; // �̵� �Ÿ�
    public float moveSpeed = 2.0f; // �̵� �ӵ�
    private Vector3 startPosition; // ���� ��ġ

    void Start()
    {
        startPosition = transform.position; // ���� ��ġ ����

        // ���ÿ��� �ڷ�ƾ ����
        StartCoroutine(MoveObstacle());

        // ��Ʈ��ũ ����ȭ
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView != null && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_StartMovingObstacle", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    void RPC_StartMovingObstacle()
    {
        StartCoroutine(MoveObstacle());
    }

    private IEnumerator MoveObstacle()
    {
        while (true)
        {
            // Mathf.PingPong���� �̵� ���
            float pingPong = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

            // Ŭ���̾�Ʈ �� �ð��� ��߳��� ��츦 �����ϰ� ���� ��
            // float pingPong = Mathf.PingPong((float)PhotonNetwork.Time * moveSpeed, moveDistance);

            transform.position = startPosition + moveDirection.normalized * pingPong;

            yield return null; // ���� �����ӱ��� ���
        }
    }
}
