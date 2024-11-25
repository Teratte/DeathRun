using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E; // 특정 키 지정 (E 키)
    public TrapController trap; // 함정을 연결하기 위한 변수
    private bool isPlayerNearby = false; // 플레이어가 버튼 근처에 있는지 확인
    private bool isButtonUsed = false; // 버튼이 이미 눌렸는지 확인

    Renderer capsuleColor;

    void Start()
    {
        capsuleColor = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Key Press Detected");
        }

        // 버튼이 이미 사용되었으면 더 이상 처리하지 않음
        if (isButtonUsed) return;

        // 플레이어가 버튼 근처에 있을 때 키 입력을 확인
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Button Pressed!");
            trap.ActivateTrap(); // 함정 작동
            isButtonUsed = true; // 버튼 상태를 '사용됨'으로 변경
            capsuleColor.material.color = Color.blue;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 버튼 범위에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player Near Button");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 버튼 범위를 벗어나면 상태 초기화
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player Left Button");
        }
    }
}
