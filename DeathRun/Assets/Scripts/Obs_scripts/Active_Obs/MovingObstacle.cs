using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right; // 이동 방향
    public float moveDistance = 5.0f; // 이동 거리
    public float moveSpeed = 2.0f; // 이동 속도
    private Vector3 startPosition; // 시작 위치

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장

        // 로컬에서 코루틴 시작
        StartCoroutine(MoveObstacle());

        // 네트워크 동기화
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
            // Mathf.PingPong으로 이동 계산
            float pingPong = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

            // 클라이언트 간 시간이 어긋나는 경우를 방지하고 싶을 때
            // float pingPong = Mathf.PingPong((float)PhotonNetwork.Time * moveSpeed, moveDistance);

            transform.position = startPosition + moveDirection.normalized * pingPong;

            yield return null; // 다음 프레임까지 대기
        }
    }
}
