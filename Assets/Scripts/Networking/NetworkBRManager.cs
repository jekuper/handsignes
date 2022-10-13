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
    public GameObject deathParticle;

    public override void Awake () {
        base.Awake ();
        brSingleton = this;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region Client
    public override void OnStopClient () {
        base.OnStopClient ();
        NetworkDataBase.LocalUserData.isReady = false;
        if (LobbyGUI.singleton != null) {
            LobbyGUI.singleton.ShowMessage ("connection aborted, check ip address");
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadSceneAsync ("PlayModeSelect");
            Destroy(gameObject);
        }
    }
    #endregion
    #region SERVER
    public override void OnServerAddPlayer (NetworkConnectionToClient conn) {
        if (LobbyGUI.singleton == null) {
            base.OnServerAddPlayer (conn);
            return;
        }
        GameObject player = Instantiate (playerPrefab);
        player.name = $"{LobbyPlayer.name} [{NetworkDataBase.data[conn].nickname}]";
        NetworkServer.AddPlayerForConnection (conn, player);

        foreach (var item in NetworkDataBase.data) {
            item.Key.identity.GetComponent<NetworkPlayerManager> ().RpcInitiate (item.Value);
        }
    }

    public override void OnServerConnect (NetworkConnectionToClient conn) {
        NetworkDataBase.UpdateReadyStatus ();
        base.OnServerConnect (conn);
    }
    public override void OnServerDisconnect (NetworkConnectionToClient conn) {
        NetworkDataBase.data.Remove (conn);
        NetworkDataBase.UpdateReadyStatus ();
        if (LobbyGUI.singleton == null) {
            CheckForWinner ();
        }
        base.OnServerDisconnect (conn);
    }

    public override void OnServerSceneChanged (string sceneName) {
        foreach (var item in NetworkDataBase.data) {
            GameObject GamePlayerInst = Instantiate (GamePlayer, new Vector3 (Random.Range (-5, 5), 2, 0), Quaternion.identity);
            NetworkServer.Spawn (GamePlayerInst, item.Key);
            GamePlayerInst.GetComponent<GamePlayerManager> ().localNickname = item.Value.nickname;
            GamePlayerInst.GetComponent<GamePlayerManager> ().mainNetworkPlayer = item.Key.identity.GetComponent<NetworkPlayerManager> ();
            item.Key.identity.GetComponent<NetworkPlayerManager> ().gamePlayerManager = GamePlayerInst.GetComponent<GamePlayerManager> ();
        }
        base.OnServerSceneChanged (sceneName);
    }

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
        conn.identity.GetComponent<NetworkPlayerManager> ().RpcInitiate (NetworkDataBase.data[conn]);
        NetworkDataBase.UpdateReadyStatus ();
    }


    #endregion

    #region Public Funcitons
    [Server]
    public void ApplyDamage (string nickname, float damage) {
        ApplyDamage (NetworkDataBase.GetConnectionByNickname(nickname), damage);
    }
    [Server]
    public void ApplyDamage (NetworkConnectionToClient connection, float damage) {
        ProfileData data = NetworkDataBase.data[connection];
        data.health -= damage;
        connection.identity.GetComponent<NetworkPlayerManager> ().TargetUpdateProfileData (data);
        if (data.health <= 0) {
            Die (connection);
        }
    }
    [Server]
    public void SetBodyState(string nickname, BodyState state)
    {
        SetBodyState(NetworkDataBase.GetConnectionByNickname(nickname), state);
    }
    [Server]
    public void SetBodyState(NetworkConnectionToClient connection, BodyState state)
    {
        ProfileData data = NetworkDataBase.data[connection];
        data.bodyState |= state;
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(data);
    }
    [Server]
    public void UnSetBodyState(string nickname, BodyState state)
    {
        UnSetBodyState(NetworkDataBase.GetConnectionByNickname(nickname), state);
    }
    [Server]
    public void UnSetBodyState(NetworkConnectionToClient connection, BodyState state)
    {
        ProfileData data = NetworkDataBase.data[connection];
        data.bodyState &= ~state;
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(data);
    }

    public void Die (NetworkConnectionToClient playerConn) {
        NetworkPlayerManager player = playerConn.identity.GetComponent<NetworkPlayerManager> ();
        GameObject deathParticleInst = Instantiate (deathParticle, player.transform.position, Quaternion.identity);
        NetworkServer.Spawn (deathParticleInst);
        CheckForWinner ();
        player.RpcDie ();
        NetworkServer.Destroy (player.gamePlayerManager.gameObject);
    }
    public void CheckForWinner () {
        int winnerTeam = NetworkDataBase.CheckForWinner ();
        if (winnerTeam == -1)
            return;
        List<NetworkConnectionToClient> keys = new List<NetworkConnectionToClient> (NetworkDataBase.data.Keys);
        foreach (var item in keys) {
            ProfileData Value = NetworkDataBase.data[item];
            NetworkDataBase.data[item] = new ProfileData (Value.nickname, false, Value.teamIndex);
            item.identity.GetComponent<NetworkPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[item]);
        }
        BRGUI.singleton.RpcShowWinMenu (winnerTeam);
    }
    [Client]
    public void ResetLocalData()
    {
       NetworkDataBase.LocalInternalUserData = new InternalProfileData();
    }
    #endregion
}
