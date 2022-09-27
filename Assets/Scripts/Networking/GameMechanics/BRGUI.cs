using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class BRGUI : NetworkBehaviour
{
    private enum BRMenu {
        GameGUI,
        Pause,
        Observe,
    }
    public GameObject winMenu_startButton;
    [SerializeField] GameObject GameUI;
    [SerializeField] GameObject descMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] TextMeshProUGUI winMenu_ResultText;
    [SerializeField] UIPositionEffector pauseMenuEffector;

    [SerializeField] UIPositionEffector observeB1, observeB2;

    public static BRGUI singleton;

    bool isPauseMenuVisible = false;
    bool isWinMenuVisible = false;

    private void Awake () {
        singleton = this;
    }

    public void ShowPauseMenu () {
        if (isWinMenuVisible)
            return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuEffector.SetFromIndex (1);
        isPauseMenuVisible = true;
    }
    public void HidePauseMenu () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuEffector.SetFromIndex (0);
        descMenu.SetActive (false);
        isPauseMenuVisible = false;
    }
    public void ToggleTechnicsDescMenu () {
        descMenu.SetActive (!descMenu.activeSelf);
    }
    public void ShowObserveMenu () {
        if (isWinMenuVisible)
            return;
        GameUI.SetActive (false);
        observeB1.SetFromIndex (1);
        observeB2.SetFromIndex (1);
    }
    public void HideObserveMenu () {
        observeB1.SetFromIndex (0);
        observeB2.SetFromIndex (0);
    }
    public void ShowWinMenu (int winnerTeam) {
        isWinMenuVisible = true;
        HidePauseMenu ();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (winnerTeam == NetworkDataBase.LocalUserData.teamIndex) {
            winMenu_ResultText.text = "VICTORY";
            winMenu_ResultText.color = new Color(0.02745098f, 0.8862745f, 0.3294118f);
        } else {
            winMenu_ResultText.text = "DEFEAT";
            winMenu_ResultText.color = new Color(0.8313726f, 0.1333333f, 0f);
        }
        //        observeB1.MoveToCenter();
        //        observeB2.MoveToCenter();
        observeB1.SetFromIndex(2);
        observeB2.SetFromIndex(2);

        winScreen.SetActive (true);
    }

    public void OnDisconnectPressed () {
        NetworkManager.singleton.StopHost ();
    }
    public void OnRematchRequestPressed () {
        NetworkDataBase.LocalUserData.isReady = true;
        if (NetworkClient.active) {
            NetworkClient.connection.Send (new UpdateReadyStatusRequest { newState = NetworkDataBase.LocalUserData.isReady });
        }
    }
    [Server]
    public void OnRematchPressed () {
        NetworkDataBase.technics.Clear();
        NetworkDataBase.LocalInternalUserData = new InternalProfileData();
        NetworkManager.singleton.ServerChangeScene ("Map1");
    }
    [ClientRpc]
    public void RpcShowWinMenu (int winnerTeam) {
        ShowWinMenu (winnerTeam);
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (isPauseMenuVisible)
                HidePauseMenu ();
            else
                ShowPauseMenu ();
        }
    }
}
