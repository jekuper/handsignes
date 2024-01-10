using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void OnStartClient() {
        Game.GamePlayers.Add(this);
        LobbyManager.instance.UpdateLobbyName();
        LobbyManager.instance.UpdateUI();
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
        if (isClient)
            LobbyManager.instance.UpdateUI();
    }
}
