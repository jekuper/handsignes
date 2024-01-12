using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySelectionManager : MonoBehaviour
{
    public static LobbySelectionManager instance;

    public List<GameObject> listOfLobbyListItems = new List<GameObject>();

    [SerializeField] GameObject offlineCanvas;

    [Header("Lobby List UI")]
    [SerializeField] GameObject lobbyListCanvas;
    [SerializeField] private GameObject LobbyListItemPrefab;
    [SerializeField] private GameObject ContentPanel;
    [SerializeField] private TMP_InputField searchBox;

    [Header("Lobby Create UI")]
    [SerializeField] GameObject lobbyCreateCanvas;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Toggle friendsOnlyToggle;

    public string lobbyName;

    private void Awake() {
        MakeInstance();
    }
    private void Start() {
        lobbyNameInputField.text = SteamFriends.GetPersonaName().ToString() + "'s lobby";
    }
    void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void CreateLobbyScene() {
        offlineCanvas.SetActive(false);
        lobbyListCanvas.SetActive(false);
        lobbyCreateCanvas.SetActive(true);
    }

    public void CreateLobby() {
        ELobbyType newLobbyType;

        if (lobbyNameInputField.text.Trim() == "") {
            return;
        }
        lobbyName = lobbyNameInputField.text;

        if (friendsOnlyToggle.isOn) {
            Debug.Log("CreateNewLobby: friendsOnlyToggle is on. Making lobby friends only.");
            newLobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        }
        else {
            Debug.Log("CreateNewLobby: friendsOnlyToggle is OFF. Making lobby public.");
            newLobbyType = ELobbyType.k_ELobbyTypePublic;
        }

        SteamLobby.instance.CreateLobby(newLobbyType, lobbyName);
    }

    public void GetLobbiesList () {
        offlineCanvas.SetActive(false);
        lobbyCreateCanvas.SetActive(false);
        lobbyListCanvas.SetActive(true);

        SteamLobby.instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result) {
        for (int i = 0; i < lobbyIDS.Count; i++) {
            if (/*SteamLobby.ValidLobbyAddress((CSteamID)lobbyIDS[i].m_SteamID) &&*/
                lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby) {
                Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name") + " number of players: " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID).ToString() + " max players: " + SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID).ToString());

                if (SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name").ToLower().Contains(searchBox.text.ToLower())) {
                    InstantiateLobbyItem((CSteamID)lobbyIDS[i].m_SteamID);
                }

                return;
            }
        }
    }

    private void InstantiateLobbyItem(CSteamID id) {
        GameObject newLobbyListItem = Instantiate(LobbyListItemPrefab) as GameObject;
        LobbyListItem newLobbyListItemScript = newLobbyListItem.GetComponent<LobbyListItem>();

        newLobbyListItemScript.lobbySteamId = id;
        newLobbyListItemScript.lobbyName = SteamMatchmaking.GetLobbyData(id, "name");
        newLobbyListItemScript.numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers(id);
        newLobbyListItemScript.maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit(id);
        newLobbyListItemScript.SetLobbyItemValues();


        newLobbyListItem.transform.SetParent(ContentPanel.transform);
        newLobbyListItem.transform.localScale = Vector3.one;

        listOfLobbyListItems.Add(newLobbyListItem);
    }

    public void DestroyOldLobbyListItems() {
        if (listOfLobbyListItems.Count <= 0)
            return;

        Debug.Log("DestroyOldLobbyListItems");
        foreach (GameObject lobbyListItem in listOfLobbyListItems) {
            GameObject lobbyListItemToDestroy = lobbyListItem;
            Destroy(lobbyListItemToDestroy);
            lobbyListItemToDestroy = null;
        }
        listOfLobbyListItems.Clear();
    }

    public void BackToMainMenu() {
        lobbyCreateCanvas.SetActive(false);
        lobbyListCanvas.SetActive(false);
        offlineCanvas.SetActive(true);

        DestroyOldLobbyListItems();
        searchBox.text = "";
    }
}
