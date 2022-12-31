using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeSelectManager : MonoBehaviour
{
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }
    public void SetGameType (int type) {
        NetworkDataBase.gameType = (GameType)type;
        if (NetworkDataBase.gameType == GameType.Singleplayer) {
            PhotonNetwork.OfflineMode = true;
        } else {
            PhotonNetwork.OfflineMode = false;
        }
    }
}
