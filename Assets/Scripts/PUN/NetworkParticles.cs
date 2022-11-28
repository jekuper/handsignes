using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkParticles : MonoBehaviour
{
    public ParticleType type;
    public float damage;

    private PhotonView PV;
    private void Start () {
        PV = GetComponent<PhotonView> ();
    }
    private void OnParticleCollision (GameObject other) {
        if (other.tag == "Player" && PV.AmOwner) {
            string nick1 = PhotonNetwork.LocalPlayer.NickName;
            string nick2 = other.GetComponent<PlayerController> ().manager.localNickname;

            PlayerProfile hit1Data = NetworkDataBase.GetPlayerProfile (nick1);
            PlayerProfile hit2Data = NetworkDataBase.GetPlayerProfile (nick2);

            if (hit1Data.teamIndex != hit2Data.teamIndex) {
                NetworkDataBase.GetPlayerManagerPV (nick2).RPC (nameof (hit2Data.Damage), NetworkDataBase.GetPlayerByNickname (nick2), damage);
                NetworkDataBase.GetPlayerControllerPV (nick2).RPC (nameof (ParticlesHitResponser.Hit), NetworkDataBase.GetPlayerByNickname (nick2), type);
            }
        }
    }
}
