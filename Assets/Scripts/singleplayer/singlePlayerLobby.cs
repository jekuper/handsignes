using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class singlePlayerLobby : MonoBehaviourPunCallbacks, IConnectionCallbacks {
    public LoadingWindow loadingWindow;
    public TextMeshProUGUI title;
    public TextMeshProUGUI storyText;
    public Image[] images;
    public mapListItem[] items;

    private int currentMapIndex = 0;

    private void Start () {
        //        loadingWindow.ShowLoading ("serverConnect", "starting single player mode...");
//        PhotonNetwork.ConnectUsingSettings (NetworkDataBase.photonServerSettings.AppSettings, true);
        PhotonNetwork.NickName = NetworkDataBase.settings.nickname;
        PhotonNetwork.CreateRoom ("defaultRoom");
        Show (0);
    }
    public override void OnConnectedToMaster () {
        Debug.Log ("connected to master 1");
        PhotonNetwork.CreateRoom ("defaultRoom");
    }

    public void Show (int mapIndex) {
        currentMapIndex = mapIndex;
        title.text = items[mapIndex].mapName;
        storyText.text = items[mapIndex].mapStory;
        for (int i = 0; i < images.Length; i++) {
            images[i].sprite = items[mapIndex].sprites[i];
        }
    }
    public void Load () {
        SceneChanger.LoadScene(items[currentMapIndex].sceneName);
    }


    public override void OnJoinedRoom () {
        PhotonNetwork.Instantiate ("playerManager", Vector3.zero, Quaternion.identity);
        loadingWindow.HideLoading ("serverConnect");
    }
    public void LeaveToMenu () {
        SceneChanger.LoadScene ("PlayModeSelect");
    }
}
