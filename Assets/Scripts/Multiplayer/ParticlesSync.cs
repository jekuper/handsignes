using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesSync : MonoBehaviour
{
    private PhotonView PV;
    private void Start () {
        PV = GetComponent<PhotonView> ();
    }
    private void Update () {
        if (PV.AmOwner) {
            transform.position = NetworkLevelData.singleton.ParticlesSpawnPoint.position;
            transform.rotation = NetworkLevelData.singleton.ParticlesSpawnPoint.rotation;
        }
    }
}
