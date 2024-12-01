using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour, IObstacle
{
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 5.0f;
    public float moveSpeed = 2.0f;

    private Vector3 startPosition;
    private bool isActivated = false;

    void Start()
    {
        startPosition = transform.position;
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(MoveObstacle());
        }
    }

    private IEnumerator MoveObstacle()
    {
        float movedDistance = 0f;

        while (movedDistance < moveDistance)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.Translate(moveDirection * step);
            movedDistance += step;
            yield return null;
        }
    }
}
