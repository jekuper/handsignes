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
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyCreated_t> lobbyCreated;

    private NetworkManager networkManager;



    private void Start() {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        MakeInstance();

        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
    }

    void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void CreateLobby (ELobbyType type, string lobbyName) {
        SteamMatchmaking.CreateLobby(type, networkManager.maxConnections);
    }

    public void GetLobbiesList() {

        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);

        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    public void JoinLobby(CSteamID id) {
        SteamMatchmaking.JoinLobby(id);
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

    }

    private bool validateHostAddress(string hostAddress, ref string result) {
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
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");

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
