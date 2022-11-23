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
    [SerializeField] GameObject descMenu;

    private PhotonView PV;
    private bool isPauseMenuVisible;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine (InitTimer());
        }
    }
    private void Update()
    {
        HandleTimer();
        HandlePause();        
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
        isPauseMenuVisible = false;
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
