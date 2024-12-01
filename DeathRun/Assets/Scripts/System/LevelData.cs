using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelData : MonoBehaviour
{
    const string timeTextUIName = "TimeTxt";
    const string hpBarUIName = "HpBar";
    const string progressUIName = "Progress";

    [SerializeField] private float LimitTime;

    [SerializeField] private GameObject[] savePoints;
    [SerializeField] private Image savePointImage;

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

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.Instance.SetLimitedTime(LimitTime);

        GameObject _timeText = GameObject.Find(timeTextUIName);
        GameObject _hpBar = GameObject.Find(hpBarUIName);
        GameObject _Progress = GameObject.Find(progressUIName);
        GameManager.Instance.HUDInit(_timeText, _hpBar, _Progress);
        GameManager.Instance.SetPointsHUD(savePointImage, savePoints.Length);
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
