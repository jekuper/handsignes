using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHideAET : MonoBehaviour //AET - Animator Event Transfer
{
    [SerializeField] PhotonView PV;

    public void HideWeapon()
    {
        if (PV.AmOwner)
            PV.RPC(nameof(KatanaManager.RpcHide), RpcTarget.All);
    }
    public void ShowWeapon()
    {
        if (PV.AmOwner)
            PV.RPC(nameof(KatanaManager.RpcShow), RpcTarget.All, NetworkDataBase.localProfile.katanaState);
    }
}
