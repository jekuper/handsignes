using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLaser : MonoBehaviour
{
    public float damage = 9999999;

    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            if (!PhotonNetwork.IsMasterClient) {
                if (!other.attachedRigidbody.GetComponent<PlayerController> ().manager.isLocalPlayer) {
                    return;
                }
            }
            string hitNickname = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            NetworkDataBase.GetPlayerManagerPV (hitNickname).RPC (nameof (PlayerProfile.Damage), NetworkDataBase.GetPlayerByNickname (hitNickname), damage);
        }
    }
}
