using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSync : NetworkBehaviour
{
    [SerializeField] NetworkIdentity idenity;

    [ClientCallback]
    private void Update () {
        if (idenity.hasAuthority) {
            transform.position = NetworkLevelData.singleton.ParticlesSpawnPoint.position;
            transform.rotation = NetworkLevelData.singleton.ParticlesSpawnPoint.rotation;
            cmdSyncPlanePosition (transform.position, transform.rotation);
        }
    }
    [Command]
    private void cmdSyncPlanePosition (Vector3 currentPosition, Quaternion currentRotation) {
        ServerSyncPosition (currentPosition, currentRotation);
    }
    [ClientRpc]
    private void ServerSyncPosition (Vector3 currentPosition, Quaternion currentRotation) {
        if (!idenity.hasAuthority) {
            transform.position = currentPosition;
            transform.rotation = currentRotation;
        }
    }
}
