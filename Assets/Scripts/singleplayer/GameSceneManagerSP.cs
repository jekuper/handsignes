using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManagerSP : MonoBehaviourPunCallbacks {
    public float currentTimerValue = -1f;
    [SerializeField] UIPositionEffector pauseMenuEffector;
    [SerializeField] DamageIndicator damageIndicator;
    [SerializeField] GameObject descMenu;
    [SerializeField] Transform starredTechnicsHolder;
    [SerializeField] GameObject technicDescShort;

    private PhotonView PV;
    private bool isPauseMenuVisible;
    private bool isWinMenuVisible = false;

    public static GameSceneManagerSP singleton;

    private bool isLeaving = false;

    private void Start () {
        PV = GetComponent<PhotonView> ();
        singleton = this;
        NetworkDataBase.SetAllNotReady ();

        StartCoroutine (unfreezeAfterTime ());
    }
    private void Update () {
        HandlePause ();
        UpdateStarredTechnics ();
    }
    private void HandlePause () {
        if (isWinMenuVisible)
            return;
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (isPauseMenuVisible)
                HidePauseMenu ();
            else
                ShowPauseMenu ();
        }
    }



    #region Freeze

    [PunRPC]
    private void UnFreezeLocal () {
        NetworkDataBase.localPlayerManager.controller.UnFreeze ();
    }
    private IEnumerator unfreezeAfterTime () {
        yield return new WaitForSeconds (1);

        PV.RPC (nameof (UnFreezeLocal), RpcTarget.AllViaServer);
    }
    #endregion


    #region PauseMenu
    public void ShowPauseMenu () {
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
    #endregion
    #region Technics Description
    public void ToggleTechnicsDescMenu () {
        descMenu.SetActive (!descMenu.activeSelf);
    }
    #endregion

    #region DamageIndication
    public void IndicateDamage () {
        damageIndicator.Hitted ();
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
            Instantiate (technicDescShort, starredTechnicsHolder).GetComponent<starredTechnicBlock> ().Load (item.Key);
        }
    }
    #endregion

    public void OnLeavePressed () {
        PhotonNetwork.LeaveRoom ();
    }
    public override void OnLeftRoom () {
        NetworkDataBase.playersManagers.Clear ();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneChanger.LoadScene ("singlePlayerLobby");
    }
}
