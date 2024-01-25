using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNumberChanged))] public int playerNumber = -1;

    public Transform CameraPosition;


    private void OnPlayerNumberChanged(int old, int newV) {
        Debug.Log("changing playerNumber from " + old.ToString() + " to " + newV.ToString());
        if (playerNumber == NetworkClient.localPlayer.GetComponent<GamePlayer>().playerNumber) {
            GetComponent<PlayerMovement>().working = true;
            name = "LocalPlayer";
        }
        else {
            GetComponent<PlayerMovement>().working = false;
            name = "player [" + newV + "]";
        }
    }

    public override void OnStopClient() {
        base.OnStopClient();
        Debug.Log("Removing player#" + playerNumber.ToString() + " from LevelManager.players");
        LevelManager.instance.players.Remove(this);
    }
}
