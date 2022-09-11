using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System.Linq;

public class LobbyPlayerManager : NetworkBehaviour
{
    public GameObject GUIprefab;
    public string localNickname = "";

    [SyncVar]
    public GamePlayerManager gamePlayerManager;

    public LobbyPlayerGUI spawnedGUI;

    public void Awake () {
        DontDestroyOnLoad (gameObject);
    }

    [ClientRpc]
    public void RpcInitiate (ProfileData _data) {
        localNickname = _data.nickname;
        if (spawnedGUI != null) {
            UpdateGUI (_data);
            return;
        }
        if (LobbyGUI.singleton != null) {
            spawnedGUI = Instantiate (GUIprefab, LobbyGUI.singleton.room_ScrollViewContent).GetComponent<LobbyPlayerGUI>();
            spawnedGUI.Initiate (this, _data);
        }
    }
    public void UpdateGUI (ProfileData newData) {
        if (spawnedGUI != null)
            spawnedGUI.Initiate (this, newData);
    }

    [Server]
    public void RemovePlayer () {
        GetComponent<NetworkIdentity> ().connectionToClient.Disconnect();
    }

    [TargetRpc]
    public void TargetUpdateProfileData (ProfileData dataNew) {
        NetworkDataBase.LocalUserData = dataNew;
        GameGUIManager.singleton.UpdateThrowable ();
        gamePlayerManager.CmdUpdateHealth ();
    }

    private void OnDestroy () {
        if (spawnedGUI != null)
            Destroy (spawnedGUI.gameObject);
    }
}
