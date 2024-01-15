using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RonikaraNetworkManager : NetworkManager {
    [SerializeField] private GamePlayer gamePlayerPrefab;
    [SerializeField] public int minPlayers = 1;
    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

    private int spawnpointIndex = 0;

    public override void OnStartServer() {
        Debug.Log("Mirror Server has started");
//        ServerChangeScene("Lobby");
    }
    public override void OnStopServer() {
        GamePlayers.Clear();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        if (SceneManager.GetActiveScene().name == "Lobby") {
            bool isGameLeader = GamePlayers.Count == 0;

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.current_lobbyID, GamePlayers.Count);

            GamePlayerInstance.gameObject.name = "GamePlayer [conn="+conn.connectionId.ToString()+"]";

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn) {
        base.OnServerReady(conn);
        if (SceneManager.GetActiveScene().name.Split("_")[0] == "Level") {
            conn.identity.GetComponent<GamePlayer>().ChangeReadyStatus(true);
            LevelManager.instance.ServerSpawnPlayer(conn, spawnpointIndex++);
        }
    }

    public void StartGame() {
        if (CanStartGame() && SceneManager.GetActiveScene().name == "Lobby") {
            foreach (GamePlayer item in GamePlayers) {
                item.isPlayerReady = false;
            }
            ServerChangeScene("Level_1");
        }
    }
    private bool CanStartGame() {
        if (numPlayers < minPlayers)
            return false;
        foreach (GamePlayer player in GamePlayers) {
            if (!player.isPlayerReady)
                return false;
        }
        return true;
    }
}
