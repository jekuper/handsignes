using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaAET : MonoBehaviour //AET - Animator Event Transfer
{
    [SerializeField] KatanaManager manager;
    [SerializeField] NetworkIdentity identity;

    public void TurnTriggerOn()
    {
        if (identity.hasAuthority)
            manager.CmdTurnTriggerOn();
    }
    public void TurnTriggerOff()
    {
        if (identity.hasAuthority)
            manager.CmdTurnTriggerOff();
    }
}
