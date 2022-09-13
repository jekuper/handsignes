using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesSync : NetworkBehaviour
{

    [ClientCallback]
    private void Update () {
        if (hasAuthority) {
            transform.position = NetworkLevelData.singleton.ParticlesSpawnPoint.position;
            transform.rotation = NetworkLevelData.singleton.ParticlesSpawnPoint.rotation;
            cmdSyncPlanePosition (transform.position, transform.rotation);
        }
    }
    [Server]
    public void Stop () {
        RpcStop ();
    }
    [ClientRpc]
    private void RpcStop () {
        GetComponent<ParticleSystem> ().Stop ();
    }
    [Command]
    private void cmdSyncPlanePosition (Vector3 currentPosition, Quaternion currentRotation) {
        ServerSyncPosition (currentPosition, currentRotation);
    }
    [ClientRpc]
    private void ServerSyncPosition (Vector3 currentPosition, Quaternion currentRotation) {
        if (!hasAuthority) {
            transform.position = currentPosition;
            transform.rotation = currentRotation;
        }
    }
}
