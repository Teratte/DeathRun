using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelData : MonoBehaviour
{
    const string timeTextUIName = "TimeTxt";
    const string hpBarUIName = "HpBar";
    const string progressUIName = "Progress";
    private bool isGameStart = false;

    [SerializeField] private int startPlayerCount;
    [SerializeField] private float LimitTime;

    public GameObject[] savePoints;
    public Image savePointImage;

    private static LevelData instance;
    
    public static LevelData Instance
    {
        get
        {
            if (instance != null) return instance;
            else
            {
                Debug.Log("LevelData is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        instance = this;

        isGameStart = false;
    }

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient) GameManager.Instance.enabled = true;
    }

    private void Update()
    {
        if (FindObjectsOfType<PlayerCtrl>().Length >= startPlayerCount && isGameStart == false && PhotonNetwork.IsMasterClient)
        {
            isGameStart = true;

            //GameManager.Instance.SetPointsHUD(savePointImage, savePoints.Length);
            GameManager.Instance.GameStart(LimitTime);
        }
    }

    public void SetSavePoint(GameObject savePoint)
    {
        for (int i = 0; i < savePoints.Length; i++)
        {
            if (savePoints[i] == savePoint)
            {
                GameManager.Instance.SetSavePointHUD(i);
                return;
            }
        }

        Debug.Log("SavePoint is null");
    }
}
