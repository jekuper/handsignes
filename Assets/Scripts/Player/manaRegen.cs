using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaRegen : NetworkBehaviour
{
    [SerializeField] technicsManager techManager;
    [SerializeField] DashMechanic dashManager;
    [SerializeField] GamePlayerManager manager;
    [SerializeField] PlayerMovement movement;

    public float manaIncreaseSpeed = 10f;

    [ServerCallback]
    private void Update()
    {
        if (techManager.isRegeningMana)
        {
            movement.controlsEnabled = false;
            dashManager.controlsEnabled = false;

            ProfileData dt = NetworkDataBase.GetDataByNickname(manager.localNickname);
            NetworkDataBase.GetDataByNickname(manager.localNickname).mana = Mathf.Clamp((Time.deltaTime * manaIncreaseSpeed) + dt.mana, 0, dt.manaMax);
            //TODO: change from calling every frame to calling every 0.1 seconds.
            NetworkDataBase.GetConnectionByNickname(manager.localNickname).identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.GetDataByNickname(manager.localNickname));
        }
        else
        {
            movement.controlsEnabled = true;
            dashManager.controlsEnabled = true;
        }
    }
}
