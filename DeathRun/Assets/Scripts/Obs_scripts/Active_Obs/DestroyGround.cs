using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyGround : MonoBehaviour
{
    private PhotonView photonView; // PhotonView 컴포넌트
    private bool isDestroyed = false; // 함정이 파괴되었는지 여부

    void Start()
    {
        photonView = GetComponent<PhotonView>(); // PhotonView 컴포넌트 가져오기
    }

    // 플레이어와 충돌할 때 호출되는 메서드 (콜리전 방식 사용)
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Player" 태그를 가진 경우
        if (collision.gameObject.CompareTag("Player"))
        {
            // 이미 파괴되었으면 추가적인 처리 하지 않음
            if (!isDestroyed && PhotonNetwork.IsMasterClient) // 마스터 클라이언트에서만 처리
            {
                photonView.RPC("RPC_DestroyTrap", RpcTarget.All); // 네트워크 모든 클라이언트에서 파괴 동기화
            }
            else
            {
                DestroyTrap();
            }
        }
    }

    private void DestroyTrap()
    {
        Destroy(gameObject); // 게임 오브젝트 파괴
    }

    // 트랩 파괴 RPC 메서드
    [PunRPC]
    void RPC_DestroyTrap()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject); // 게임 오브젝트 파괴
        }
    }
}
