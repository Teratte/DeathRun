using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UpDownObstacle : MonoBehaviour, IObstacle
{
    public float moveDistance = 5f;   // ���� �ö󰡴� �Ÿ�
    public float moveSpeed = 3f;      // �̵� �ӵ�
    private Vector3 initialPosition;  // �ʱ� ��ġ
    private Vector3 targetPosition;   // ��ǥ ��ġ (���� �̵��� ��ġ)
    private bool isMoving = false;    // �̵� ����

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        // �ʱ� ��ġ ����
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * moveDistance; // ��ǥ ��ġ
    }

    public void Activate()
    {
        
        if (photonView.IsMine && !isMoving) // ������ Ŭ���̾�Ʈ������ �̵� ����
        {
            StartCoroutine(MoveObstacle());
            // ��Ʈ��ũ �󿡼� ��� Ŭ���̾�Ʈ���� ��ֹ� Ȱ��ȭ�� ����
            photonView.RPC("RPC_Activate", RpcTarget.OthersBuffered);
        }
        else
        {
            StartCoroutine(MoveObstacle());
        }
    }

    public void Deactivate()
    {
        // ��ֹ��� ��Ȱ��ȭ�Ǹ� �̵��� ���߰� �ʱ� ��ġ�� ���ư����� �� �� �ֽ��ϴ�.
        StopAllCoroutines();
        transform.position = initialPosition;
        isMoving = false;

        // ��Ʈ��ũ �󿡼� ��� Ŭ���̾�Ʈ���� ��ֹ� ��Ȱ��ȭ�� ����
        photonView.RPC("RPC_Deactivate", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    public void RPC_Activate()
    {
        if (!isMoving) // �̹� �̵� ���̸� �ٽ� �������� ����
        {
            StartCoroutine(MoveObstacle());
        }
    }

    [PunRPC]
    public void RPC_Deactivate()
    {
        // ��Ʈ��ũ Ŭ���̾�Ʈ���� ��Ȱ��ȭ
        StopAllCoroutines();
        transform.position = initialPosition;
        isMoving = false;
    }

    private IEnumerator MoveObstacle()
    {
        isMoving = true;

        // 1. �ö󰡴� ����
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime * moveSpeed / moveDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // ��Ȯ�� ��ǥ ��ġ ����

        // 2. 5�� ���
        yield return new WaitForSeconds(5f);

        // 3. �������� ����
        elapsedTime = 0f;
        startPos = transform.position;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPos, initialPosition, elapsedTime * moveSpeed / moveDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition; // ��Ȯ�� �ʱ� ��ġ ����

        // ������ �������Ƿ� isMoving�� false�� ����
        isMoving = false;
    }
}
