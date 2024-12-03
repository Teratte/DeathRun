using System.Collections;
using UnityEngine;
using Photon.Pun;

public class ColorChangeObstacle : MonoBehaviour, IObstacle
{
    public Color newColor = Color.red;  // ���� ������ ��
    public string newTag = "Tick_Obs";  // ������ �±�
    public float colorChangeDuration = 5f; // ����� �±� ���� ���� �ð�
    private Renderer floorRenderer;     // �ٴ��� Renderer
    private bool isActivated = false;   // ��ֹ��� Ȱ��ȭ�Ǿ����� ����

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        floorRenderer = GetComponent<Renderer>(); // �ٴ��� Renderer ����
    }

    public void Activate()
    {
        if (!isActivated) // �̹� Ȱ��ȭ�Ǿ����� �ٽ� �������� ����
        {
            isActivated = true;

            // �ٴ� ���� ����
            ChangeFloorColor();

            // �±� ����
            ChangeTag();

            // ��Ʈ��ũ ����ȭ
            photonView.RPC("RPC_Activate", RpcTarget.OthersBuffered);

            // 5�� �Ŀ� ������� ���ư��� �ڷ�ƾ ����
            StartCoroutine(RevertChangesAfterDelay(colorChangeDuration));
        }
    }

    [PunRPC]
    public void RPC_Activate()
    {
        if (!isActivated)
        {
            isActivated = true;

            // �ٴ� ���� ����
            ChangeFloorColor();

            // �±� ����
            ChangeTag();

            // 5�� �Ŀ� ������� ���ư��� �ڷ�ƾ ����
            StartCoroutine(RevertChangesAfterDelay(colorChangeDuration));
        }
    }

    private void ChangeFloorColor()
    {
        if (floorRenderer != null)
        {
            floorRenderer.material.color = newColor; // ���� ����
        }
    }

    private void ChangeTag()
    {
        gameObject.tag = newTag; // �±� ����
    }

    private IEnumerator RevertChangesAfterDelay(float delay)
    {
        // 5�� ���
        yield return new WaitForSeconds(delay);

        // �ٴ� ���� ������� ����
        if (floorRenderer != null)
        {
            floorRenderer.material.color = Color.white; // ���� ������ ����
        }

        // �±� ������� ����
        gameObject.tag = "Ground";

        // ��Ʈ��ũ ����ȭ
        photonView.RPC("RPC_Deactivate", RpcTarget.OthersBuffered);

        // ������ �������Ƿ� isActivated�� false�� ����
        isActivated = false;
    }

    [PunRPC]
    public void RPC_Deactivate()
    {
        // �ٴ� ���� ������� ����
        if (floorRenderer != null)
        {
            floorRenderer.material.color = Color.white; // ���� ������ ����
        }

        // �±� ������� ����
        gameObject.tag = "Ground";
    }
}