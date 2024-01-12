using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI LobbyNameText;
    [SerializeField] private GameObject PlayerListItemPrefab;
    [SerializeField] private Transform ContentPanel;

    public GamePlayer localGamePlayer;
    public ulong currentLobbyId;

    private List<PlayerListItem> playerListitems;
    private RonikaraNetworkManager game;
    private RonikaraNetworkManager Game {
        get {
            if (game != null) {
                return game;
            }
            return game = RonikaraNetworkManager.singleton as RonikaraNetworkManager;
        }
    }

    private void Awake() {
        MakeInstance();
        playerListitems = new List<PlayerListItem>();
    }
    void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void UpdateLocalGamePlayer(GamePlayer newLocal) {
        localGamePlayer = newLocal;
    }

    public void UpdateUI() {
        UpdatePlayerList();
    }

    public void UpdatePlayerList() {
        for (int i = 0; i < Game.GamePlayers.Count; i++) {
            if (i >= playerListitems.Count) {
                GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab, ContentPanel);
                PlayerListItem newPlayerListItemScript = newPlayerListItem.GetComponent<PlayerListItem>();

                playerListitems.Add(newPlayerListItemScript);
            }

            playerListitems[i].ConnectionId = Game.GamePlayers[i].ConnectionId;
            playerListitems[i].playerName = Game.GamePlayers[i].playerName;
            playerListitems[i].playerSteamId = Game.GamePlayers[i].playerSteamId;
            playerListitems[i].UpdateUI();
        }
        while(playerListitems.Count > Game.GamePlayers.Count) {
            Destroy(playerListitems[playerListitems.Count - 1].gameObject);
            playerListitems.RemoveAt(playerListitems.Count - 1);
        }
    }
    public void UpdateLobbyName() {
        currentLobbyId = Game.GetComponent<SteamLobby>().current_lobbyID;
        string lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)currentLobbyId, "name");
        LobbyNameText.text = lobbyName;
    }

    public void PlayerQuitLobby() {
        localGamePlayer.QuitLobby();
    }
}
