using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class RoomManager : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject roomListItem;
    [SerializeField] GameObject playerListItem;

    [SerializeField] TMP_InputField roomName;
    [SerializeField] TMP_Dropdown regionDropdown;
    [SerializeField] TextMeshProUGUI readyButtonText;
    [SerializeField] TextMeshProUGUI nicknameText;
    [SerializeField] TextMeshProUGUI masterText;

    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;

    public LoadingWindow loadingWindow;
    [SerializeField] GameObject listCanvas;
    [SerializeField] GameObject roomCanvas;
    [SerializeField] GameObject startButton;

    public static RoomManager signleton;


    private void Start()
    {
        Debug.Log("start multiplayer");
        signleton = this;
        loadingWindow.ShowLoading("serverConnect", "connecting to server...");

        for (int i = 0; i < regionDropdown.options.Count; i++) {
            if (regionDropdown.options[i].text.Split (", ")[1] == NetworkDataBase.settings.serverRegion) {
                regionDropdown.value = i;
                regionDropdown.RefreshShownValue ();
                break;
            }
        }

        if (NetworkDataBase.settings.nickname.Trim() == "")
        {
            loadingWindow.HideLoading("serverConnect");
            loadingWindow.ShowMessage("wrongNickname", "your nick is empty.\nWrite your nickname in settings", Color.red, LeaveToMenu);
            return;
        }

        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.NickName = NetworkDataBase.settings.nickname;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);

        NetworkDataBase.InitiateLocalPlayerData();

        PhotonNetwork.ConnectUsingSettings (NetworkDataBase.photonServerSettings.AppSettings);
    }
    private void Update()
    {
        nicknameText.text = PhotonNetwork.NickName;
        masterText.text = (PhotonNetwork.IsMasterClient ? "you are a master client" : "");
    }


    public override void OnConnectedToMaster()
    {
        loadingWindow.HideLoading("serverConnect");
        Debug.Log (PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        loadingWindow.HideLoading("serverConnect");
        loadingWindow.ShowMessage("serverDisconnect", "Disconnected from server.\nReason: "+cause.ToString(), Color.red, LeaveToMenu);
    }
    public override void OnCreatedRoom()
    {
        loadingWindow.HideLoading("roomCreation");
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("playerManager", Vector3.zero, Quaternion.identity);
        loadingWindow.HideLoading("joiningRoom");
        SwitchUI("room");
    }
    public override void OnLeftRoom()
    {
        loadingWindow.ShowLoading("serverConnect", "connecting to server...");
        NetworkDataBase.playersManagers.Clear ();
        SwitchUI("list");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        loadingWindow.HideLoading("roomCreation");
        loadingWindow.ShowMessage("roomCreationFail", "room creation failed\n" + message, Color.red);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        loadingWindow.HideLoading("joiningRoom");
        loadingWindow.ShowMessage("roomJoinFail", "failed to join room\n" + message, Color.red);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var info in roomList)
        {
            if (!info.RemovedFromList)
            {
                RoomListItem item = Instantiate(roomListItem, roomListContent).GetComponent<RoomListItem>();
                item.Initialize(info);
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerList();
        UpdateStartButton();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
        UpdateStartButton();
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        NetworkDataBase.playersManagers.Remove (newPlayer.NickName);
        UpdatePlayerList();
        UpdateStartButton();
    }



    #region UI interaction
    public void UIonCreatePressed()
    {
        if (!PhotonNetwork.IsConnected)
        {
            loadingWindow.ShowMessage("roomCreationFail", "room creation failed\nyou are not connected to server", Color.red);
            return;
        }
        if (roomName.text.Trim() == "")
        {
            loadingWindow.ShowMessage("roomCreationFail", "room creation failed\nroom name is empty", Color.red);
            return;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;
        loadingWindow.ShowLoading("roomCreation", "creating room...");
        PhotonNetwork.CreateRoom(roomName.text, options, TypedLobby.Default);
    }
    public void UIonLeaveRoomPressed()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void UIonReadyPressed()
    {
        bool current = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"];
        current = !current;
        NetworkDataBase.SetCustomProperties(PhotonNetwork.LocalPlayer, "isReady", current);

        readyButtonText.text = (current ? "Not ready" : "Ready");
    }
    public void UIonStartPressed()
    {
        if (!PhotonNetwork.IsMasterClient || !NetworkDataBase.CheckReady())
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Map2");
    }
    public void SwitchUI(string type)
    {
        if (type == "room")
        {
            listCanvas.SetActive(false);
            roomCanvas.SetActive(true);

            UpdatePlayerList();
        }
        if (type == "list")
        {
            listCanvas.SetActive(true);
            roomCanvas.SetActive(false);
        }
    }
    public void UpdatePlayerList()
    {
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerListItem item = Instantiate(playerListItem, playerListContent).GetComponent<PlayerListItem>();
            item.Initialize(player);
        }
    }
    public void UpdateStartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            HideStartButton();
            return;
        }
        if (NetworkDataBase.CheckReady() && PhotonNetwork.PlayerList.Length > 1)
        {
            ShowStartButton();
        }
        else
        {
            HideStartButton();
        }
    }
    public void ShowStartButton()
    {
        startButton.SetActive(true);
    }
    public void HideStartButton()
    {
        startButton.SetActive(false);
    }
    #endregion


    public void LeaveToMenu()
    {
        SceneManager.LoadScene("PlayModeSelect");
    }
    public void SwitchRegion () {
        string region = regionDropdown.options[regionDropdown.value].text.Split (", ")[1];
        NetworkDataBase.settings.serverRegion = region;
        NetworkDataBase.SaveSettings ();
        PhotonNetwork.Disconnect ();
    }
    [PunRPC]
    public void Kick(string reason)
    {
        PhotonNetwork.LeaveRoom();
        loadingWindow.ShowMessage("kickReason", reason, Color.red);
    }
}
