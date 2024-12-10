using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public static PhotonInit instance;

    bool isGameStart = false;
    bool isLoggIn = false;
    bool isReady = false;
    string playerName = "";
    public string chatMessage;
    PhotonView pv;

    [SerializeField] private string playerPrefabName = "Player";
    [SerializeField] private Define.Scenes woodLandScene = Define.Scenes.WoodLand;
    [Header("LobbyCanvas")] public GameObject LobbyCanvas;
    public GameObject TitlePanel;
    public GameObject CreateRoomPanel;
    public GameObject RoomPanel;
    public InputField NewRoomNameIF;
    public InputField NewRoomPwInput;
    public Toggle PwToggle;
    public GameObject PwPanel;
    public GameObject PwErrorLog;
    public GameObject PwConfirmBtn;
    public GameObject PwPanelCloseBtn;
    public InputField PwCheckIF;
    public bool LockState = false;
    public string privateroom;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public Text NickName;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple, roomnumber;

    private const string TracerExistence = "TracerExistence";
    private const int TracerNum = 0;

    private void Awake()
    {
        PhotonNetwork.GameVersion = "MyFps";
        PhotonNetwork.ConnectUsingSettings();

        DontDestroyOnLoad(gameObject);
    }

    public static PhotonInit Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(PhotonInit)) as PhotonInit;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }

            return instance;
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        //PhotonNetwork.CreateRoom("MyRoom");
        //PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRoom("MyRoom");
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToServer");
        isReady = true;
    }

    public void Connect()
    {

        if (PhotonNetwork.IsConnected && isReady)
        {
            PhotonNetwork.JoinLobby();
            NickName.text = playerName;
            RoomPanel.SetActive(true);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No Room");
        //����� �κе� �ּ� ó��
        //PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Finish make a room");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed:" + returnCode + "-" + message);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        isLoggIn = true;
        PlayerPrefs.SetInt("LogIn", 1);

        PhotonNetwork.LoadLevel(woodLandScene.ToString());
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LogIn", 0);
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("LogIn") == 1)
            isLoggIn = true;

        if (isGameStart == false && SceneManager.GetActiveScene().name == woodLandScene.ToString() && isLoggIn == true)
        {
            Debug.Log("Update :" + isGameStart + ", " + isLoggIn);
            isGameStart = true;

            StartCoroutine(CreatePlayer());
        }
    }

    IEnumerator CreatePlayer()
    {
        while (!isGameStart)
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject tempPlayer = PhotonNetwork.Instantiate(playerPrefabName,
                                    new Vector3(-12, 9, -28),
                                    Quaternion.identity,
                                    0);
        pv = GetComponent<PhotonView>();

        yield return null;
    }

    public void SetPlayerName(string _playerName)
    {
        if (isGameStart == false && isLoggIn == false)
        {
            PhotonNetwork.LocalPlayer.NickName = _playerName;
            Connect();

        }
    }

    public void CreateRoomBtnOnClick()
    {
        CreateRoomPanel.SetActive(true);
    }

    public void OKBtnOnClick()
    {
        CreateRoomPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(NewRoomNameIF.text == "" ? "Game" + Random.Range(0, 20) : NewRoomNameIF.text,
               new RoomOptions { MaxPlayers = 4 });
        TitlePanel.SetActive(false);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        RoomPanel.SetActive(false);
        TitlePanel.SetActive(true);
        isGameStart = false;
        isLoggIn = false;
        isReady = false;
        PlayerPrefs.SetInt("LogIn", 0);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.CustomRoomProperties = new Hashtable()
        {
            {"password", NewRoomPwInput.text},
            {TracerExistence, false},
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password", TracerExistence };

        if (Random.Range(0, 3) == TracerNum)
        {
            Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Tracer" } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            roomOptions.CustomRoomProperties[TracerExistence] = true;
        }
        else
        {
            Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Player" } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
        }

        if (PwToggle.isOn)
        { 
            PhotonNetwork.CreateRoom(NewRoomNameIF.text == "" ? "Game" + Random.Range(0, 20) : "*" + NewRoomNameIF.text,
                roomOptions);
        }
        else
        {
            roomOptions.CustomRoomProperties.Remove("password");
            PhotonNetwork.CreateRoom(NewRoomNameIF.text == "" ? "Game" + Random.Range(0, 20) : NewRoomNameIF.text,
                roomOptions);
        }

        CreateRoomPanel.SetActive(false);
        //LobbyCanvas.SetActive(false);
    }

    public void MyListClick(int num)
    {

        if (num == -2)
        {
            --currentPage;
            MyListRenewal();
        }
        else if (num == -1)
        {
            ++currentPage;
            MyListRenewal();
        }
        else if (myList[multiple + num].CustomProperties["password"] != null)
        {
            PwPanel.SetActive(true);
        }
        else
        {
            Hashtable customProperties = myList[multiple + num].CustomProperties;

            if((bool)customProperties[TracerExistence] == false)
            {
                if (myList[multiple + num].PlayerCount == myList[multiple + num].MaxPlayers - 1)
                {
                    Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Tracer" } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                    myList[multiple + num].CustomProperties[TracerExistence] = true;
                }
                else
                {
                    if (Random.Range(0, 3) == TracerNum)
                    {
                        Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Tracer" } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                        myList[multiple + num].CustomProperties[TracerExistence] = true;
                    }
                    else
                    {
                        Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Player" } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                    }
                }
            }
            else
            {
                Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Player" } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            }

            PhotonNetwork.JoinRoom(myList[multiple + num].Name);

            MyListRenewal();

        }

    }

    public void RoomPw(int number)
    {
        switch (number)
        {
            case 0:
                roomnumber = 0;
                break;
            case 1:
                roomnumber = 1;
                break;
            case 2:
                roomnumber = 2;
                break;
            case 3:
                roomnumber = 3;
                break;

            default:
                break;
        }
    }

    public void EnterRoomWithPW()
    {
        if ((string)myList[multiple + roomnumber].CustomProperties["password"] == PwCheckIF.text)
        {
            Hashtable customProperties = myList[multiple + roomnumber].CustomProperties;

            if ((bool)customProperties[TracerExistence] == false)
            {
                if (myList[multiple + roomnumber].PlayerCount == myList[multiple + roomnumber].MaxPlayers - 1)
                {
                    Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Tracer" } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                    myList[multiple + roomnumber].CustomProperties[TracerExistence] = true;
                }
                else
                {
                    if (Random.Range(0, 3) == TracerNum)
                    {
                        Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Tracer" } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                        myList[multiple + roomnumber].CustomProperties[TracerExistence] = true;
                    }
                    else
                    {
                        Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Player" } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                    }
                }
            }
            else
            {
                Hashtable playerCustomProperties = new Hashtable { { "PlayerTag", "Player" } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            }

            PhotonNetwork.JoinRoom(myList[multiple + roomnumber].Name);
            MyListRenewal();
            PwPanel.SetActive(false);
        }
        else
        {
            StartCoroutine("ShowPwWrongMsg");
        }
    }

    IEnumerator ShowPwWrongMsg()
    {
        if (!PwErrorLog.activeSelf)
        {
            PwErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            PwErrorLog.SetActive(false);
        }
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0)
            ? myList.Count / CellBtn.Length
            : myList.Count / CellBtn.Length + 1;

        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].Name : "";

        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate:" + roomList.Count);
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }

        MyListRenewal();

    }
}
