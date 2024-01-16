using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayer : NetworkBehaviour
{
    [Header("GamePlayer Info")]
    [SyncVar(hook = nameof(HandlePlayerNameUpdate))] public string playerName;
    [SyncVar] public int ConnectionId;
    [SyncVar] public int playerNumber;
    [Header("Game Info")]
    [SyncVar] public bool IsGameLeader = false;
    [SyncVar(hook = nameof(HandlePlayerReadyStatusChange))] public bool isPlayerReady;
    [SyncVar] public ulong playerSteamId;

    private RonikaraNetworkManager game;
    private RonikaraNetworkManager Game {
        get {
            if (game != null) {
                return game;
            }
            return game = RonikaraNetworkManager.singleton as RonikaraNetworkManager;
        }
    }


    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnStartAuthority() {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyManager.instance.UpdateLocalGamePlayer(this);
        LobbyManager.instance.UpdateLobbyName();
    }

    [Command]
    private void CmdSetPlayerName (string newName) {
        this.HandlePlayerNameUpdate(this.playerName, newName);
    }
    public void ChangeReadyStatus() {
        Debug.Log("Executing ChangeReadyStatus for player: " + this.playerName);
        if (isOwned)
            CmdChangePlayerReadyStatus(!this.isPlayerReady);
    }
    public void ChangeReadyStatus(bool newVal) {
        Debug.Log("Executing ChangeReadyStatus for player: " + this.playerName);
        if (isOwned)
            CmdChangePlayerReadyStatus(newVal);
    }
    [Command]
    void CmdChangePlayerReadyStatus(bool newVal) {
        Debug.Log("Executing CmdChangePlayerReadyStatus on the server for player: " + this.playerName);
        this.HandlePlayerReadyStatusChange(this.isPlayerReady, newVal);
    }

    public void CanLobbyStartGame() {
        if (isOwned)
            CmdCanLobbyStartGame();
    }
    [Command]
    void CmdCanLobbyStartGame() {
        Game.StartGame();
    }


    public override void OnStartClient() {
        Game.GamePlayers.Add(this);
        LobbyManager.instance.UpdateLobbyName();
        LobbyManager.instance.UpdateUI();
    }

    public void QuitLobby() {
        if (isLocalPlayer) {
            if (IsGameLeader) {
                Game.StopHost();
            }
            else {
                Game.StopClient();
            }
        }
    }
    private void OnDestroy() {
        if (isLocalPlayer) {
            Debug.Log("LobbyPlayer destroyed. Returning to main menu.");
            SteamMatchmaking.LeaveLobby((CSteamID)LobbyManager.instance.currentLobbyId);
        }
    }

    public void HandlePlayerNameUpdate(string oldValue, string newValue) {
        Debug.Log("Player name has been updated for: " + oldValue + " to new value: " + newValue);
        LobbyManager.instance.UpdateUI();
        if (isServer)
            this.playerName = newValue;
    }

    void HandlePlayerReadyStatusChange(bool oldValue, bool newValue) {
        if (isServer)
            this.isPlayerReady = newValue;
        if (isClient && LobbyManager.instance != null)
            LobbyManager.instance.UpdateUI();
    }

    public override void OnStopClient() {
        Debug.Log(playerName + " is quiting the game.");
        Game.GamePlayers.Remove(this);
        Debug.Log("Removed player from the GamePlayer list: " + this.playerName);

        if (LobbyManager.instance != null)
            LobbyManager.instance.UpdateUI();
    }
}
