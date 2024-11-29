using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviourPunCallbacks
{
    const string timeTextUIName = "TimeTxt";
    const string hpBarUIName = "HpBar";
    const string progressUIName = "Progress";

    private Text timeText;
    private Slider hpBarSlider;
    private GridLayoutGroup progressGridLayoutGroup;
    private Image[] savePointImages;

    private float limitedTime;
    private bool isGameStart = false;

    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<GameManager>();

            if (instance == null)
            {
                GameObject singletonObject = new GameObject("GameManager");
                instance = singletonObject.AddComponent<GameManager>();
            }

            return instance;
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

        isGameStart = false;
    }

    private void Update()
    {
        DecreaseTime();
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

    public void SetLimitedTime(float time)
    {
        limitedTime = time;

        isGameStart = true;
    }

    private void DecreaseTime()
    {
        if (isGameStart == false)
        {
            return;
        }

        limitedTime -= Time.deltaTime;

        int minute = (int)(limitedTime / 60f);
        int second = (int)(limitedTime % 60f);

        string minuteText = minute.ToString();
        string secondText = second.ToString();

        if(minute < 10)
        {
            minuteText = $"0{minuteText}";
        }

        if (second < 10)
        {
            secondText = $"0{secondText}";
        }

        timeText.text = $"{minuteText} : {secondText}";

        if(limitedTime <= 0.0f)
        {
            timeText.text = "00 : 00";
            isGameStart = false;
        }
    }

    public void UpdateHpBar(float MaxHp, float CurrentHp)
    {
        float hpRatio = CurrentHp / MaxHp;

        hpBarSlider.value = hpRatio;
    }

    public void SetPointsHUD(Image savePointImage, int num)
    {
        for(int i = 1; i < num; i++)
        {
            Image image = GameObject.Instantiate(savePointImage, progressGridLayoutGroup.transform);
            image.name = $"SavePoint{i}";
        }

        savePointImages = progressGridLayoutGroup.GetComponentsInChildren<Image>();
    }

    public void SetSavePointHUD(int pointNum)
    {
        for(int i = 0; i <= pointNum; i++)
        {
            savePointImages[i].color = Color.blue;
        }
    }

    public void HUDInit(GameObject _timeText, GameObject _hpBar, GameObject _Progress)
    {
        
        if(_timeText != null) timeText = _timeText.GetComponent<Text>();

        
        if( _hpBar != null ) hpBarSlider = _hpBar.GetComponent<Slider>();

        
        if (_Progress != null) progressGridLayoutGroup = _Progress.GetComponent<GridLayoutGroup>();

        isGameStart = true;
        LockCursor();
    }
}
