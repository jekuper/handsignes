using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

[RequireComponent(typeof(NetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;
    public List<CSteamID> lobbyIDS = new List<CSteamID>();

    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    private NetworkManager networkManager;



    private void Start() {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        MakeInstance();

        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
    }

    void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void GetLobbiesList() {

        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);

        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
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
