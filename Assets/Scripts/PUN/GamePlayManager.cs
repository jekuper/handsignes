using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] PhotonView photonView;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int index = 0;
            foreach (Player pl in PhotonNetwork.PlayerList)
            {
                photonView.RPC(nameof(InstantiatePlayer), pl, pl, index);

                index++;
            }
        }
    }
    [PunRPC]
    public void InstantiatePlayer(Player player, int index)
    {
        NetworkDataBase.localProfile.teamIndex = index;
        GameObject playerInst = PhotonNetwork.Instantiate ("PlayerPrefab", spawnPoints[index % spawnPoints.Length].position, Quaternion.identity);
    }
}
