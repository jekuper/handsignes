using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public struct UpdateReadyStatusRequest : NetworkMessage {
    public bool newState;
}

public class NetworkBRManager : NetworkManager
{
    public static NetworkBRManager brSingleton;

    public GameObject LobbyPlayer;
    public GameObject GamePlayer;

    public override void Awake () {
        base.Awake ();
        brSingleton = this;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void OnServerAddPlayer (NetworkConnectionToClient conn) {
        if (LobbyGUI.singleton == null) {
            base.OnServerAddPlayer (conn);
            return;
        }
        GameObject player = Instantiate (playerPrefab);
        player.name = $"{LobbyPlayer.name} [{NetworkDataBase.data[conn].nickname}]";
        NetworkServer.AddPlayerForConnection (conn, player);

        foreach (var item in NetworkDataBase.data) {
            item.Key.identity.GetComponent<LobbyPlayerManager> ().RpcInitiate (item.Value);
        }
    }

    public override void OnServerConnect (NetworkConnectionToClient conn) {
        NetworkDataBase.UpdateReadyStatus ();
        base.OnServerConnect (conn);
    }

    public override void OnServerSceneChanged (string sceneName) {
        foreach (var item in NetworkDataBase.data) {
            GameObject GamePlayerInst = Instantiate (GamePlayer, new Vector3(Random.Range(-5, 5), 2, 0), Quaternion.identity);
            NetworkServer.Spawn (GamePlayerInst, item.Key);
            GamePlayerInst.GetComponent<GamePlayerManager> ().localNickname = item.Value.nickname;
            GamePlayerInst.GetComponent<GamePlayerManager> ().mainNetworkPlayer = item.Key.identity.GetComponent<LobbyPlayerManager> ();
            item.Key.identity.GetComponent<LobbyPlayerManager> ().gamePlayerManager = GamePlayerInst.GetComponent<GamePlayerManager> ();
        }
        base.OnServerSceneChanged (sceneName);
    }

    public override void OnServerDisconnect (NetworkConnectionToClient conn) {
        NetworkDataBase.data.Remove (conn);
        NetworkDataBase.UpdateReadyStatus ();
        base.OnServerDisconnect (conn);
    }

    public override void OnStopClient () {
        base.OnStopClient ();
        NetworkDataBase.LocalUserData.isReady = false;
        if (LobbyGUI.singleton != null) {
            LobbyGUI.singleton.ShowMessage ("connection aborted, check ip address");
        } else {
            Debug.Log ("unable to locate LobbyGUI");
            SceneManager.LoadSceneAsync ("PlayModeSelect");
            Destroy(gameObject);
        }
    }


    #region SERVER
    public override void OnStartServer () {
        NetworkServer.RegisterHandler<UpdateReadyStatusRequest> (UpdateProfileDataHandler, false);
        base.OnStartServer ();
    }

    public override void OnStopServer () {
        NetworkServer.UnregisterHandler<UpdateReadyStatusRequest> ();
        base.OnStopServer ();
    }

    private void UpdateProfileDataHandler (NetworkConnectionToClient conn, UpdateReadyStatusRequest request) {
        NetworkDataBase.data[conn].isReady = request.newState;
        conn.identity.GetComponent<LobbyPlayerManager> ().RpcInitiate (NetworkDataBase.data[conn]);
        NetworkDataBase.UpdateReadyStatus ();
    }


    #endregion

    #region Public Funcitons
    public void ApplyDamage (string nickname, float damage) {
        ApplyDamage (NetworkDataBase.GetConnectionByNickname(nickname), damage);
    }
    public void ApplyDamage (NetworkConnectionToClient connection, float damage) {
        ProfileData data = NetworkDataBase.data[connection];
        data.health -= damage;
        if (data.health <= 0) {
            Die (connection);
        }
        connection.identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (data);
    }
    public void Die (NetworkConnectionToClient playerConn) {

    }
    #endregion
}
