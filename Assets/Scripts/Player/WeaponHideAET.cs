using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHideAET : MonoBehaviour //AET - Animator Event Transfer
{
    [SerializeField] KatanaManager manager;
    [SerializeField] NetworkIdentity identity;

    public void HideWeapon()
    {
        if (identity.hasAuthority)
            manager.CmdHide();
    }
    public void ShowWeapon()
    {
        if (identity.hasAuthority)
            manager.CmdShow();
    }
}
