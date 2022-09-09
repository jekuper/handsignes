using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using UnityEngine.SceneManagement;

enum LobbyMode {
    Host,
    Client,
    Room
}

public class LobbyGUI : NetworkBehaviour
{
    public static LobbyGUI singleton;

    public TextMeshProUGUI switchButtonText;
    public TextMeshProUGUI backButtonText;
    public GameObject switchButton;

    [Header ("Windows")]
    public GameObject clientWindow;
    public GameObject hostWindow;
    public GameObject roomWindow;
    public GameObject loadWindow;

    [Header ("HostWindow")]

    [Header("ClientWindow")]
    public TMP_InputField client_IpAddress;

    [Header("RoomWindow")]
    public GameObject room_ReadyButton;
    public GameObject room_StartButton;
    public Transform room_ScrollViewContent;

    [Header ("LoadWindow")]
    public TextMeshProUGUI load_message;
    public GameObject load_gif;
    public GameObject load_okButton;

    private LobbyMode mode = LobbyMode.Host;


    public void Awake () {
        singleton = this;
    }

    public void swithcerPressed () {
        NetworkManager.singleton.StopHost ();
        if (mode == LobbyMode.Client) {
            mode = LobbyMode.Host;
        } else if (mode == LobbyMode.Host) {
            mode = LobbyMode.Client;
        } else if (mode == LobbyMode.Room) {
            mode = LobbyMode.Host;
        }
        UpdateGUI ();
    }

    public void UpdateGUI () {
        if (mode == LobbyMode.Client) {
            switchButtonText.text = "Host";
            backButtonText.text = "Back";

            hostWindow.SetActive (false);
            clientWindow.SetActive (true);
            roomWindow.SetActive (false);
            switchButton.SetActive (true);
        } else if (mode == LobbyMode.Host) {
            switchButtonText.text = "Client";
            backButtonText.text = "Back";

            hostWindow.SetActive (true);
            clientWindow.SetActive (false);
            roomWindow.SetActive (false);
            switchButton.SetActive (true);
        } else if (mode == LobbyMode.Room) {
            backButtonText.text = "Stop";

            hostWindow.SetActive (false);
            clientWindow.SetActive (false);
            roomWindow.SetActive (true);
            switchButton.SetActive (false);
        }
    }
    public void ShowLoading () {
        if (load_message == null || loadWindow == null || load_gif == null || load_okButton == null)
            return;
        load_message.text = "Loading";
        loadWindow.SetActive (true);
        load_gif.SetActive (true);
        load_okButton.SetActive (false);
    }
    public void StopLoading (string message = "") {
        if (load_message == null || load_gif == null || load_okButton == null)
            return;
        load_message.text = message;
        load_gif.SetActive (false);
        load_okButton.SetActive (true);
    }
    public void ShowMessage (string message) {
        ShowLoading ();
        StopLoading (message);
    }
    public void HideLoading () {
        if (load_message == null || loadWindow == null)
            return;
        loadWindow.SetActive (false);
        load_message.text = "";
    }

    public void OnHostPressed () {
        if (!string.IsNullOrWhiteSpace (NetworkDataBase.LocalUserData.nickname)) {
            ShowLoading ();
            NetworkManager.singleton.StartHost ();
        } else {
            ShowMessage ("nickname must have at least one character\ntry changing it in settings");
        }
    }
    public void OnConnectPressed () {
        if (!string.IsNullOrWhiteSpace (NetworkDataBase.LocalUserData.nickname) &&
            !string.IsNullOrWhiteSpace (client_IpAddress.text)) {
            ShowLoading ();
            NetworkManager.singleton.networkAddress = client_IpAddress.text;
            NetworkManager.singleton.StartClient ();
        } else {
            if (string.IsNullOrWhiteSpace (NetworkDataBase.LocalUserData.nickname)) {
                ShowMessage ("nickname must have at least one character\ntry changing it in settings");
            } else if (string.IsNullOrWhiteSpace (client_IpAddress.text)) {
                ShowMessage ("ip adress is incorrect");
            }
        }
    }
    public void OnReadyPressed () {
        if (NetworkDataBase.LocalUserData.isReady) {
            room_ReadyButton.GetComponentInChildren<TextMeshProUGUI> ().text = "Ready";
            NetworkDataBase.LocalUserData.isReady = false;
        } else {
            room_ReadyButton.GetComponentInChildren<TextMeshProUGUI> ().text = "Not ready";
            NetworkDataBase.LocalUserData.isReady = true;
        }
        if (NetworkClient.active) {
            NetworkClient.connection.Send (new UpdateReadyStatusRequest { newState = NetworkDataBase.LocalUserData.isReady });
        }
    }
    [Server]
    public void OnStartPressed () {
        NetworkManager.singleton.ServerChangeScene ("Map1");
    }
    public void OnBackPresed () {
        if (mode == LobbyMode.Room) {
            NetworkManager.singleton.StopHost ();
            mode = LobbyMode.Host;
            UpdateGUI ();
        } else {
            NetworkManager.singleton.StopHost ();
            Destroy (NetworkManager.singleton.gameObject);
            SceneManager.LoadScene ("PlayModeSelect");
        }
    }

    public void OnStopPressed () {
        NetworkManager.singleton.StopHost ();
    }

    public override void OnStartClient () {
        base.OnStartClient ();
        mode = LobbyMode.Room;
        HideLoading ();
        UpdateGUI ();
    }
    public override void OnStartServer () {
        base.OnStartServer ();
        HideLoading ();
    }
    public override void OnStopServer () {
        base.OnStopServer ();
        StopLoading ("host failed.");
    }

    public void OnNickNameChanged () {
//        NetworkDataBase.LocalUserData.nickname = NicknameInputField.text;
    }
}
