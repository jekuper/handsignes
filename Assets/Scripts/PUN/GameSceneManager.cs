using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public float initTimerValue = 5f;
    public float currentTimerValue = -1f;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] UIPositionEffector pauseMenuEffector;
    [SerializeField] GameObject GameUI;
    [SerializeField] GameObject descMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject rematchButton;
    [SerializeField] TextMeshProUGUI readyCount;
    [SerializeField] TextMeshProUGUI winMenu_ResultText;
    [SerializeField] UIPositionEffector observeB1, observeB2;

    private PhotonView PV;
    private bool isPauseMenuVisible;
    private bool isWinMenuVisible = false;

    public static GameSceneManager singleton;

    private bool isLeaving = false;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        singleton = this;
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine (InitTimer());
            NetworkDataBase.SetAllNotReady ();
        }
    }
    private void Update()
    {
        HandleSoloKick ();
        HandleTimer();
        HandlePause();
        CheckWinner ();
        HandleWinScreen ();
    }
    private void HandleSoloKick () {
        if (PhotonNetwork.PlayerList.Length == 1 && !isLeaving) {
            PhotonNetwork.LeaveRoom ();
            isLeaving = true;
        }
    }
    private void HandleTimer()
    {
        if (currentTimerValue > 0)
        {
            timerText.text = currentTimerValue.ToString("0.0");
        }
        else
        {
            timerText.text = "";
        }
    }
    private void HandlePause()
    {
        if (isWinMenuVisible)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPauseMenuVisible)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }
    }
    
    
    
    #region Timer
    public IEnumerator InitTimer()
    {
        currentTimerValue = initTimerValue;
        while (currentTimerValue > 0)
        {
            currentTimerValue -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PV.RPC(nameof(UnFreezeLocal), RpcTarget.All);
    }

    [PunRPC]
    private void UnFreezeLocal()
    {
        NetworkDataBase.localPlayerManager.controller.UnFreeze();
    }
    #endregion


    #region PauseMenu
    public void ShowPauseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuEffector.SetFromIndex(1);
        isPauseMenuVisible = true;
    }
    public void HidePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuEffector.SetFromIndex(0);
        descMenu.SetActive (false);
        isPauseMenuVisible = false;
    }
    #endregion
    #region Observe Mode
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
    #endregion
    #region Technics Description
    public void ToggleTechnicsDescMenu () {
        descMenu.SetActive (!descMenu.activeSelf);
    }
    #endregion
    #region Victory screen
    public void CheckWinner () {
        if (!PhotonNetwork.IsMasterClient || isWinMenuVisible)
            return;
        int winnerTeam = NetworkDataBase.SearchWinnerTeam ();
        if (winnerTeam != -1) {
            PV.RPC (nameof (GameSceneManager.ShowWinMenu), RpcTarget.All, winnerTeam);
        }
    }
    public void HandleWinScreen () {
        if (!isWinMenuVisible)
            return;
        readyCount.text = NetworkDataBase.CountReady() + "/" + PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient && !rematchButton.activeSelf) {
            bool ready = NetworkDataBase.CheckReady ();
            if (ready) {
                rematchButton.SetActive (true);
            }
        }
        if (!PhotonNetwork.IsMasterClient) {
            rematchButton.SetActive (false);
        }
    }
    public void OnRematchPressed () {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PV.RPC (nameof (ReloadScene), RpcTarget.All);
    }
    [PunRPC]
    private void ReloadScene () {
        PhotonNetwork.LoadLevel ("Map1");
    }
    public void OnReadyPressed () {
        NetworkDataBase.SetCustomProperties (PhotonNetwork.LocalPlayer, "isReady", true);
    }
    [PunRPC]
    public void ShowWinMenu (int winnerTeam) {
        isWinMenuVisible = true;
        HidePauseMenu ();
        if (NetworkDataBase.localProfile.IsAlive)
            NetworkDataBase.localProfile.Die ();
        NetworkDataBase.ResetLocalProfile ();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (winnerTeam == NetworkDataBase.localProfile.teamIndex) {
            winMenu_ResultText.text = "VICTORY";
            winMenu_ResultText.color = new Color (0.02745098f, 0.8862745f, 0.3294118f);
        } else {
            winMenu_ResultText.text = "DEFEAT";
            winMenu_ResultText.color = new Color (0.8313726f, 0.1333333f, 0f);
        }
        observeB1.SetFromIndex (2);
        observeB2.SetFromIndex (2);

        winScreen.SetActive (true);
    }
    #endregion

    public void OnLeavePressed()
    {
        PhotonNetwork.Disconnect();
    }


    public override void OnLeftRoom()
    {
        NetworkDataBase.playersManagers.Clear();
        SceneManager.LoadScene("BRLobby");
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentTimerValue);
        }
        else
        {
            currentTimerValue = (float)stream.ReceiveNext();
        }
    }
}
