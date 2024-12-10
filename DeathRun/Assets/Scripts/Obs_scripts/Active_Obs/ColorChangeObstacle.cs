using System.Collections;
using UnityEngine;
using Photon.Pun;

public class ColorChangeObstacle : MonoBehaviour, IObstacle
{
    public Color newColor = Color.red;  // 색상 변경할 색
    public string newTag = "Tick_Obs";  // 변경할 태그
    public float colorChangeDuration = 5f; // 색상과 태그 변경 지속 시간
    private Renderer floorRenderer;     // 바닥의 Renderer
    private bool isActivated = false;   // 장애물이 활성화되었는지 여부
    [SerializeField]
    private GameObject Tick_Obs;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        floorRenderer = GetComponent<Renderer>(); // 바닥의 Renderer 참조
        Tick_Obs.SetActive(false);
    }

    public void Activate()
    {
        if (!isActivated) // 이미 활성화되었으면 다시 변경하지 않음
        {
            isActivated = true;

            // 바닥 색상 변경
            ChangeFloorColor();

            // 태그 변경
            ChangeTag();

            Chage_Tick_Obs_State();

            // 네트워크 동기화
            photonView.RPC("RPC_Activate", RpcTarget.OthersBuffered);

            // 5초 후에 원래대로 돌아가는 코루틴 시작
            StartCoroutine(RevertChangesAfterDelay(colorChangeDuration));
        }
    }

    [PunRPC]
    public void RPC_Activate()
    {
        if (!isActivated)
        {
            isActivated = true;

            // 바닥 색상 변경
            ChangeFloorColor();

            // 태그 변경
            ChangeTag();

            Chage_Tick_Obs_State();

            // 5초 후에 원래대로 돌아가는 코루틴 시작
            StartCoroutine(RevertChangesAfterDelay(colorChangeDuration));
        }
    }

    private void ChangeFloorColor()
    {
        if (floorRenderer != null)
        {
            floorRenderer.material.color = newColor; // 색상 변경
        }
    }

    private void ChangeTag()
    {
        gameObject.tag = newTag; // 태그 변경
    }
    private void Chage_Tick_Obs_State()
    {
        if(Tick_Obs.activeSelf)
        {
            Tick_Obs.SetActive(false);
        }
        else
        {
            Tick_Obs.SetActive(true);
        }
    }

    private IEnumerator RevertChangesAfterDelay(float delay)
    {
        // 5초 대기
        yield return new WaitForSeconds(delay);

        // 바닥 색상 원래대로 복구
        if (floorRenderer != null)
        {
            floorRenderer.material.color = Color.white; // 원래 색으로 복구
        }

        // 태그 원래대로 복구
        gameObject.tag = "Ground";

        Chage_Tick_Obs_State();

        // 네트워크 동기화
        photonView.RPC("RPC_Deactivate", RpcTarget.OthersBuffered);

        // 동작이 끝났으므로 isActivated를 false로 설정
        isActivated = false;
    }

    [PunRPC]
    public void RPC_Deactivate()
    {
        // 바닥 색상 원래대로 복구
        if (floorRenderer != null)
        {
            floorRenderer.material.color = Color.white; // 원래 색으로 복구
        }

        // 태그 원래대로 복구
        gameObject.tag = "Ground";
    }
}
