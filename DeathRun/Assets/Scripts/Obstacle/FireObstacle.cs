using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : MonoBehaviour, IObstacle
{
    private Rigidbody rb;
    private Renderer obstacleRenderer;

    void Start()
    {
        obstacleRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        obstacleRenderer.material.color = Color.black;
    }

    // 장애물 활성화 메서드
    public void Activate()
    {
        if (rb != null)
        {
            // 위쪽으로 힘을 가하는 예제
            rb.AddForce(Vector3.right * 50f, ForceMode.Impulse);
        }
    }
}
