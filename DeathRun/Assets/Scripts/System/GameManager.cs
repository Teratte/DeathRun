using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isGameStart;

    const string timeTextUIName = "TimeTxt";
    const string hpBarUIName = "HpBar";
    const string progressUIName = "Progress";
    const string winLosePanelName = "WinLosePanel";

    private Text timeText;
    private Slider hpBarSlider;
    private GridLayoutGroup progressGridLayoutGroup;
    private Image[] savePointImages;
    private GameObject winLosePanel = null;
    private Text winLoseText = null;

    private int overTime;
    private int LastSavePointIndex;

    private PhotonView pv;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            if (instance == null && PhotonNetwork.IsMasterClient)
            {
                GameObject singletonObject = PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
                return singletonObject.GetComponent<GameManager>();
            }
            else
            {
                instance = FindObjectOfType<GameManager>();
                return instance;
            }
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        pv = gameObject.GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this;

    }

    private void Update()
    {
        if(winLosePanel == null) winLosePanel = GameObject.Find(winLosePanelName);
        if (winLoseText == null)
        {
            winLoseText = winLosePanel.GetComponentInChildren<Text>();
            winLosePanel.SetActive(false);
        }
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameStart(int limitTime)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SetStart", RpcTarget.All, limitTime);
        }
    }

    [PunRPC]
    private void SetStart(int limitTime)
    {
        timeText = GameObject.Find(timeTextUIName).GetComponent<Text>();
        hpBarSlider = GameObject.Find(hpBarUIName).GetComponent<Slider>();
        progressGridLayoutGroup = GameObject.Find(progressUIName).GetComponent<GridLayoutGroup>();

        for (int i = 1; i < LevelData.Instance.savePoints.Length; i++)
        {
            Image image = GameObject.Instantiate(LevelData.Instance.savePointImage, progressGridLayoutGroup.transform);
            image.name = $"SavePoint{i}";
        }
        savePointImages = progressGridLayoutGroup.GetComponentsInChildren<Image>();

        overTime = limitTime;
        isGameStart = true;
        StartCoroutine("DecreaseTime");
    }

    private IEnumerator DecreaseTime()
    {
        while (true)
        {
            if (isGameStart == false) yield break;

            int minute = (int)(overTime / 60f);
            int second = (int)(overTime % 60f);

            string minuteText = minute.ToString();
            string secondText = second.ToString();

            if (minute < 10)
            {
                minuteText = $"0{minuteText}";
            }

            if (second < 10)
            {
                secondText = $"0{secondText}";
            }

            if ((overTime % 60) == 0)
            {
                timeText.color = Color.red;
            }
            else
            {
                timeText.color = Color.black;
            }

            timeText.text = $"{minuteText} : {secondText}";

            if (overTime <= 0)
            {
                timeText.text = "00 : 00";
                winLosePanel.SetActive(true);

                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerTag"] == "Player")
                {
                    winLoseText.text = "LOSE...";
                }
                else
                {
                    winLoseText.text = "WIN!!!";
                }

                yield break;
            }

            yield return new WaitForSeconds(1.0f);
            --overTime;
        }
    }

    public void UpdateHpBar(float MaxHp, float CurrentHp)
    {
        float hpRatio = CurrentHp / MaxHp;

        hpBarSlider.value = hpRatio;
    }

    public void SetSavePoint(int pointNum)
    {
        for (int i = 0; i <= pointNum; i++)
        {
            savePointImages[i].color = Color.blue;
        }

        LastSavePointIndex = pointNum;
    }

    public void RespawnAtLastSavePoint(GameObject playerObject)
    {
        playerObject.transform.position = LevelData.Instance.savePoints[LastSavePointIndex].gameObject.transform.position;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    public void TracerDead()
    {
        pv.RPC("PlayerWin", RpcTarget.All);
    }

    [PunRPC]
    private void PlayerWin()
    {
        isGameStart = false;

        winLosePanel.SetActive(true);

        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerTag"] == "Player")
        {
            winLoseText.text = "WIN!!!";
        }
        else
        {
            winLoseText.text = "LOSE...";
        }
    }
}
