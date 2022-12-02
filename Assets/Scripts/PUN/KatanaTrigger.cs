using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaTrigger : MonoBehaviour
{
    [SerializeField] KatanaManager link;
    [SerializeField] private Transform bloodSpawnPoint;

    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            link.TriggerResponce (other);
            GameObject particle = PhotonNetwork.Instantiate ("BloodParticle", bloodSpawnPoint.position, Quaternion.identity);
        }
    }
}
