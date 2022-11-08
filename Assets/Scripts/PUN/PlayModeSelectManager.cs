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
}
