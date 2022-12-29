using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string localNickname;
    public bool isLocalPlayer = false;
    public PlayerProfile playerProfile;
    public PlayerController controller;

    private PhotonView PV;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PV = GetComponent<PhotonView>();
        playerProfile = GetComponent<PlayerProfile>();

        if (PV.AmOwner)
            PV.RPC(nameof(Initialize), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        if (NetworkDataBase.gameType == GameType.Singleplayer) {
            Initialize (NetworkDataBase.settings.nickname);
        }
    }

    [PunRPC]
    public void Initialize(string nickname)
    {
        localNickname = nickname;
        NetworkDataBase.playersManagers.Add(nickname, this);

        if (PV == null)
            PV = GetComponent<PhotonView>();
        if (PV.AmOwner || NetworkDataBase.gameType == GameType.Singleplayer)
        {
            isLocalPlayer = true;
        }
    }
}
