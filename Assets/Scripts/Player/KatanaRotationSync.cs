using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaRotationSync : NetworkBehaviour
{
    [SerializeField] Transform shoulderR;
    [SerializeField] Transform camHolder;
    [SerializeField] KatanaManager katanaM;
    [SerializeField] Transform networkTransfer;

    private void Start () {
        camHolder = NetworkLevelData.singleton.CamHolder;
    }

    private void LateUpdate () {
        if (!katanaM.isOff) {
            shoulderR.rotation = Quaternion.Euler (networkTransfer.eulerAngles.x, shoulderR.eulerAngles.y, shoulderR.eulerAngles.z);
            if (hasAuthority) {
                networkTransfer.rotation = Quaternion.Euler (camHolder.eulerAngles.x, shoulderR.eulerAngles.y, shoulderR.eulerAngles.z);
            }
        }
    }
}
