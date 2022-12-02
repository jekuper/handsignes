using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLaser : MonoBehaviour
{
    public float damage = 9999999;

    private void OnTriggerEnter (Collider other) {
        if (PhotonNetwork.IsMasterClient) {
            return;
        }
        if (other.tag == "Player") {
            string hitNickname = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            NetworkDataBase.GetPlayerManagerPV (hitNickname).RPC (nameof (PlayerProfile.Damage), NetworkDataBase.GetPlayerByNickname (hitNickname), damage);
        }
    }
}
