using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticles : NetworkBehaviour
{
    public float damage;

    [ServerCallback]
    private void OnParticleCollision (GameObject other) {
        if (other.tag == "Player") {
            string nick1 = NetworkDataBase.data[connectionToClient].nickname;
            string nick2 = other.GetComponent<GamePlayerManager> ().localNickname;

            ProfileData hit1Data = NetworkDataBase.GetDataByNickname (nick1);
            ProfileData hit2Data = NetworkDataBase.GetDataByNickname (nick2);

            if (hit1Data.teamIndex != hit2Data.teamIndex) {
                hit2Data.health -= damage;
                NetworkDataBase.GetConnectionByNickname (nick2).identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (hit2Data);
            }
        }
    }
}