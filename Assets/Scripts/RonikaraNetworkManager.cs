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
}
