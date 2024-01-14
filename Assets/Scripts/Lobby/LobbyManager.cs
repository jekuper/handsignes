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
    [SerializeField] private TextMeshProUGUI ReadyUpButtonText;
    [SerializeField] private GameObject StartGameButton;

    [SerializeField] private GameObject playerListCanvas;
    [SerializeField] private GameObject levelsListCanvas;

    public GamePlayer localGamePlayer;
    public ulong currentLobbyId;
    public string selectedScene = "Level_1";

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

    public void ShowPlayerList() {
        playerListCanvas.SetActive(true);
        levelsListCanvas.SetActive(false);
    }
    public void ShowLevelsList() {
        playerListCanvas.SetActive(false);
        levelsListCanvas.SetActive(true);
    }
    public void SelectLevel(string level) {
        selectedScene = level;
    }

    public void UpdateLocalGamePlayer(GamePlayer newLocal) {
        localGamePlayer = newLocal;
        UpdateReadyUpButtonText();
    }

    public void UpdateUI() {
        UpdatePlayerList();
        UpdateReadyUpButtonText();
        CheckIfAllPlayersAreReady();
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
            playerListitems[i].isPlayerReady = Game.GamePlayers[i].isPlayerReady;
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
    void UpdateReadyUpButtonText() {
        if (localGamePlayer == null)
            return;
        if (localGamePlayer.isPlayerReady)
            ReadyUpButtonText.text = "Unready";
        else
            ReadyUpButtonText.text = "Ready Up";
    }

    void CheckIfAllPlayersAreReady() {
        Debug.Log("Executing CheckIfAllPlayersAreReady");
        bool areAllPlayersReady = false;
        foreach (GamePlayer player in Game.GamePlayers) {
            if (player.isPlayerReady) {
                areAllPlayersReady = true;
            }
            else {
                Debug.Log("CheckIfAllPlayersAreReady: Not all players are ready. Waiting for: " + player.playerName);
                areAllPlayersReady = false;
                break;
            }
        }
        if (areAllPlayersReady) {
            Debug.Log("CheckIfAllPlayersAreReady: All players are ready!");
            if (localGamePlayer != null && localGamePlayer.IsGameLeader) {
                Debug.Log("CheckIfAllPlayersAreReady: Local player is the game leader. They can start the game now.");
                StartGameButton.gameObject.SetActive(true);
            }
        }
        else {
            if (StartGameButton.gameObject.activeInHierarchy)
                StartGameButton.gameObject.SetActive(false);
        }
    }
    public void StartGame() {
        localGamePlayer.CanLobbyStartGame();
    }


    public void PlayerReadyUp() {
        Debug.Log("Executing PlayerReadyUp");
        localGamePlayer.ChangeReadyStatus();
    }
    public void PlayerInviteFriend () {
        Debug.Log("Executing PlayerInviteFriend");
        SteamLobby.instance.InviteFriend();
    }

    public void PlayerQuitLobby() {
        localGamePlayer.QuitLobby();
    }
}
