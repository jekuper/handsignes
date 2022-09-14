using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLaser : NetworkBehaviour
{
    public float damage = 9999999;

    [ServerCallback]
    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            NetworkConnectionToClient conn = NetworkDataBase.GetConnectionByNickname (other.attachedRigidbody.GetComponent<GamePlayerManager> ().localNickname);
            NetworkBRManager.brSingleton.ApplyDamage (conn, damage);
        }
    }
}
