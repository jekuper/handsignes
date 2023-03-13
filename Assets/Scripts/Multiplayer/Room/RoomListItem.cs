using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] TextMeshProUGUI playerCountText;

    private RoomInfo _info;

    public void Initialize(RoomInfo info)
    {
        _info = info;

        roomName.text = info.Name;
        playerCountText.text = info.PlayerCount.ToString() + "/" + info.MaxPlayers.ToString();
    }
    public void Join()
    {
        PhotonNetwork.JoinRoom (_info.Name);
        RoomManager.signleton.loadingWindow.ShowLoading("joiningRoom", "connecting to room...");
    }
}
