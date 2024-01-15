using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNumberChanged))] public int playerNumber = -1;

    private void OnPlayerNumberChanged(int old, int newV) {
        Debug.Log("changing playerNumber from " + old.ToString() + " to " + newV.ToString());
        if (playerNumber == NetworkClient.localPlayer.GetComponent<GamePlayer>().playerNumber)
            name = "LocalPlayer";
        else
            name = "player [" + newV + "]";
    }
}
