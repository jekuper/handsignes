using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class singlePlayerLobby : MonoBehaviourPunCallbacks {
    public LoadingWindow loadingWindow;
    public TextMeshProUGUI title;
    public Image[] images;
    public mapListItem[] items;

    private int currentMapIndex = 0;

    private void Start () {
//        loadingWindow.ShowLoading ("serverConnect", "starting single player mode...");
        PhotonNetwork.NickName = NetworkDataBase.settings.nickname;
        PhotonNetwork.CreateRoom ("defaultRoom");
        Show (0);
    }

    public void Show (int mapIndex) {
        currentMapIndex = mapIndex;
        title.text = items[mapIndex].mapName;
        for (int i = 0; i < images.Length; i++) {
            images[i].sprite = items[mapIndex].sprites[i];
        }
    }
    public void Load () {
        SceneManager.LoadScene(items[currentMapIndex].sceneName);
    }


    public override void OnJoinedRoom () {
        PhotonNetwork.Instantiate ("playerManager", Vector3.zero, Quaternion.identity);
        loadingWindow.HideLoading ("serverConnect");
    }
    public void LeaveToMenu () {
        SceneManager.LoadScene ("PlayModeSelect");
    }
}
