using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

[RequireComponent(typeof(NetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    public ulong current_lobbyID;
    public List<CSteamID> lobbyIDS = new List<CSteamID>();

    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyCreated_t> lobbyCreated;

    private NetworkManager networkManager;



    private void Start() {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        MakeInstance();

        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
    }

    private void OnDestroy() {
        // Unregister callbacks to avoid duplicate calls
        if (gameLobbyJoinRequested != null) {
            gameLobbyJoinRequested.Dispose();
        }

        if (Callback_lobbyList != null) {
            Callback_lobbyList.Dispose();
        }

        if (Callback_lobbyInfo != null) {
            Callback_lobbyInfo.Dispose();
        }

        if (lobbyEntered != null) {
            lobbyEntered.Dispose();
        }

        if (lobbyCreated != null) {
            lobbyCreated.Dispose();
        }
    }

    void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void InviteFriend() {
        SteamFriends.ActivateGameOverlayInviteDialog((CSteamID)current_lobbyID);
    }

    public void CreateLobby (ELobbyType type, string lobbyName) {
        SteamMatchmaking.CreateLobby(type, networkManager.maxConnections);
    }

    public void GetLobbiesList() {

        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        SteamMatchmaking.AddRequestLobbyListStringFilter("GameName", "Ronikara", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);

        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    public void JoinLobby(CSteamID id) {
        SteamMatchmaking.JoinLobby(id);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        Debug.Log("OnGameLobbyJoinRequested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }


    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) {
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString()+"Ronikara"+Application.version);
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            LobbySelectionManager.instance.lobbyName);
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "GameName",
            "Ronikara");

    }

    private static bool validateHostAddress(string hostAddress, ref string result) {
        string pattern = "Ronikara" + Application.version;
        if (pattern.Length > hostAddress.Length)
            return false;

        for (int i = pattern.Length - 1; i >= 0; i--) {
            if (hostAddress[hostAddress.Length - (pattern.Length - i)] != pattern[i]) {
                return false;
            }
        }
        result = hostAddress.Substring(0, hostAddress.Length - pattern.Length);
        return true;
    }
    public static bool ValidLobbyAddress(CSteamID id) {
        string address = SteamMatchmaking.GetLobbyData(id, "HostAddress");

        return (validateHostAddress(address, ref address));
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {

        current_lobbyID = callback.m_ulSteamIDLobby;
        Debug.Log("OnLobbyEntered for lobby with id: " + current_lobbyID.ToString());
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        if (!validateHostAddress(hostAddress, ref hostAddress)) {
            Debug.LogWarning("NOT Ronikara lobby!");
            SteamMatchmaking.LeaveLobby(new CSteamID(callback.m_ulSteamIDLobby));
            return;
        }

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        lobbyIDS.Clear();
        if (GameObject.Find("LobbySelectionManager")) {
            LobbySelectionManager.instance.DestroyOldLobbyListItems();
        }
    }

    void OnGetLobbiesList (LobbyMatchList_t result) {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies! " + name);

        LobbySelectionManager.instance.DestroyOldLobbyListItems();

        for (int i = 0; i < result.m_nLobbiesMatching; i++) {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);

        }
    }
    void OnGetLobbyInfo(LobbyDataUpdate_t result) {
        LobbySelectionManager.instance.DisplayLobbies(lobbyIDS, result);
    }
}
