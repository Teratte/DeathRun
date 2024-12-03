using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UpDownObstacle : MonoBehaviour, IObstacle
{
    public float moveDistance = 5f;   // 위로 올라가는 거리
    public float moveSpeed = 3f;      // 이동 속도
    private Vector3 initialPosition;  // 초기 위치
    private Vector3 targetPosition;   // 목표 위치 (위로 이동한 위치)
    private bool isMoving = false;    // 이동 여부

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        // 초기 위치 설정
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * moveDistance; // 목표 위치
    }

    public void Activate()
    {
        
        if (photonView.IsMine && !isMoving) // 마스터 클라이언트에서만 이동 시작
        {
            StartCoroutine(MoveObstacle());
            // 네트워크 상에서 모든 클라이언트에게 장애물 활성화를 전송
            photonView.RPC("RPC_Activate", RpcTarget.OthersBuffered);
        }
        else
        {
            StartCoroutine(MoveObstacle());
        }
    }

    public void Deactivate()
    {
        // 장애물이 비활성화되면 이동을 멈추고 초기 위치로 돌아가도록 할 수 있습니다.
        StopAllCoroutines();
        transform.position = initialPosition;
        isMoving = false;

        // 네트워크 상에서 모든 클라이언트에게 장애물 비활성화를 전송
        photonView.RPC("RPC_Deactivate", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    public void RPC_Activate()
    {
        if (!isMoving) // 이미 이동 중이면 다시 시작하지 않음
        {
            StartCoroutine(MoveObstacle());
        }
    }

    [PunRPC]
    public void RPC_Deactivate()
    {
        // 네트워크 클라이언트에서 비활성화
        StopAllCoroutines();
        transform.position = initialPosition;
        isMoving = false;
    }

    private IEnumerator MoveObstacle()
    {
        isMoving = true;

        // 1. 올라가는 동작
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime * moveSpeed / moveDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // 정확한 목표 위치 설정

        // 2. 5초 대기
        yield return new WaitForSeconds(5f);

        // 3. 내려가는 동작
        elapsedTime = 0f;
        startPos = transform.position;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPos, initialPosition, elapsedTime * moveSpeed / moveDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition; // 정확한 초기 위치 설정

        // 동작이 끝났으므로 isMoving을 false로 설정
        isMoving = false;
    }
}
