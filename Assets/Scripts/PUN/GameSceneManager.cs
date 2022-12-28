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
    [SerializeField] EventChatManager eventChat;
    [SerializeField] DamageIndicator damageIndicator;
    [SerializeField] GameObject GameUI;
    [SerializeField] GameObject descMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject rematchButton;
    [SerializeField] TextMeshProUGUI readyCount;
    [SerializeField] TextMeshProUGUI winMenu_ResultText;
    [SerializeField] UIPositionEffector observeB1, observeB2;
    [SerializeField] Transform starredTechnicsHolder;
    [SerializeField] GameObject technicDescShort;

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
        UpdateStarredTechnics ();
    }
    private void HandleSoloKick () {
        if (PhotonNetwork.PlayerList.Length == 1 && !isLeaving) {
            PhotonNetwork.Disconnect ();
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
        PV.RPC(nameof(UnFreezeLocal), RpcTarget.AllViaServer);
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
        damageIndicator.gameObject.SetActive (false);
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
        ResetLocalProfile ();
        PhotonNetwork.LoadLevel ("Map2");
    }
    [PunRPC]
    public void ResetLocalProfile () {
        NetworkDataBase.ResetLocalProfile ();
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

        if (PhotonNetwork.IsMasterClient) {
            PV.RPC (nameof (ResetLocalProfile), RpcTarget.All);
        }

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
    #region DamageIndication
    public void IndicateDamage () {
        damageIndicator.Hitted ();
    }
    #endregion
    #region Event Chat
    public void SendDeathMessage (PhotonMessageInfo info) {
        if (info.Sender != null)
            PV.RPC (nameof (RPC_DeathMessage), RpcTarget.All, info.Sender.NickName, PhotonNetwork.LocalPlayer.NickName);
        else
            PV.RPC (nameof (RPC_DeathMessage), RpcTarget.All, null, PhotonNetwork.LocalPlayer.NickName);
    }
    public void SendLeaveMessage (Player leftPlayer) {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PV.RPC (nameof (RPC_LeaveMessage), RpcTarget.All, leftPlayer.NickName);
    }
    [PunRPC]
    public void RPC_DeathMessage (string killer, string victim) {
        string message = killer + "<color=\"red\"> killed </color>" + victim;
        eventChat.DisplayMessage(message);
    }
    [PunRPC]
    public void RPC_LeaveMessage (string leftPlayer) {
        string message = leftPlayer + "<color=\"yellow\"> left </color>";
        eventChat.DisplayMessage (message);
    }
    #endregion
    #region starred technics
    public void UpdateStarredTechnics () {
        foreach (Transform child in starredTechnicsHolder) {
            Destroy (child.gameObject);
        }
        int count = 0;
        foreach (var item in NetworkDataBase.starredTechnics) {
            if (count >= 4)
                break;
            if (item.Value == false)
                continue;
            count++;
            Instantiate (technicDescShort, starredTechnicsHolder).GetComponent<starredTechnicBlock>().Load(item.Key);
        }
    }
    #endregion

    public void OnLeavePressed()
    {
        PhotonNetwork.Disconnect();
    }


    public override void OnLeftRoom()
    {
        NetworkDataBase.playersManagers.Clear();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("BRLobby");
    }
    public override void OnPlayerLeftRoom (Player otherPlayer) {
        SendLeaveMessage (otherPlayer);
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
