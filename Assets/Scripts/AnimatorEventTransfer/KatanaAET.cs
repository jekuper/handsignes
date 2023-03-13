using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaAET : MonoBehaviour //AET - Animator Event Transfer
{
    [SerializeField] PhotonView PV;

    public void TurnTriggerOn()
    {
        if (PV.AmOwner)
            PV.RPC(nameof(KatanaManager.RpcTriggerOn), RpcTarget.All);
    }
    public void TurnTriggerOff()
    {
        if (PV.AmOwner)
            PV.RPC(nameof(KatanaManager.RpcTriggerOff), RpcTarget.All);
    }
}
