using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaManager : MonoBehaviour
{
    public float damage = 15;
    public bool isOff = false;
    public Texture noneTexture, waterTexture, fireTexture, electroTexture, earthTexture;

    private bool isTriggerOff = false;
    [SerializeField] private Animator armAnim;
    [SerializeField] private KatanaTrigger triggerManager;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private Renderer katanaRenderer;

    [SerializeField]  private PhotonView PV;

    public void TurnOn () {
        isOff = false;
        triggerManager.GetComponent<Collider> ().enabled = true;
    }
    public void TurnOff () {
        isOff = true;
        triggerManager.GetComponent<Collider> ().enabled = false;
    }
    #region Trigger On/Off
    [PunRPC]
    public void RpcTriggerOn()
    {
//        Debug.Log("trigger turn on");
        isTriggerOff = false;
        triggerManager.GetComponent<Collider>().enabled = true;
    }
    [PunRPC]
    public void RpcTriggerOff()
    {
//        Debug.Log("trigger turn off");
        isTriggerOff = true;
        triggerManager.GetComponent<Collider>().enabled = false;
    }
    #endregion
    #region Hide/Show katana
    [PunRPC]
    public void RpcShow(KatanaState state)
    {
        if (state == KatanaState.None)
            katanaRenderer.material.SetTexture("_MainTex", noneTexture);
        if (state == KatanaState.Water)
            katanaRenderer.material.SetTexture("_MainTex", waterTexture);
        if (state == KatanaState.Fire)
            katanaRenderer.material.SetTexture("_MainTex", fireTexture);
        if (state == KatanaState.Electro)
            katanaRenderer.material.SetTexture("_MainTex", electroTexture);
        if (state == KatanaState.Earth)
            katanaRenderer.material.SetTexture("_MainTex", earthTexture);
        weaponHolder.SetActive(true);
    }
    [PunRPC]
    public void RpcHide()
    {
        weaponHolder.SetActive(false);
    }
    #endregion

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        if (PV.AmOwner)
        {
            PV.RPC(nameof(RpcTriggerOff), RpcTarget.AllBuffered);
        }
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked || 
            !PV.AmOwner) {
            return;
        }
        if (!isOff && Input.GetKeyDown (KeyCode.Mouse0)) {
            armAnim.SetTrigger("strike");
            armAnim.SetFloat ("randFloat", (1f / 3f) * Random.Range (0, 3));
        }
    }
    

    public void TriggerResponce (Collider other) {
        if (other.tag == "Player" && !isTriggerOff && PV.AmOwner) {
            string hitNickname = other.transform.parent.parent.parent.GetComponent<PhotonView>().Owner.NickName;
            CmdTriggerResponce(hitNickname);
        }
    }

    public void CmdTriggerResponce(string nick2)
    {
        string nick1 = PV.Owner.NickName;

        PlayerProfile hit1Profile = NetworkDataBase.GetPlayerProfile(nick1);
        PlayerProfile hit2Profile = NetworkDataBase.GetPlayerProfile(nick2);


        if (hit1Profile.teamIndex != hit2Profile.teamIndex)
        {
            float damageMultiplier = 1f;
            if (hit1Profile.katanaState.HasFlag(KatanaState.Water))
            {
                hit2Profile.SetBodyState(BodyState.Wet);
            }
            if (hit1Profile.katanaState.HasFlag(KatanaState.Electro))
            {
                hit2Profile.SetBodyState(BodyState.ElectroShock);
                if (hit2Profile.bodyState.HasFlag(BodyState.Wet))
                {
                    damageMultiplier = 2f;
                }
            }
            if (hit1Profile.katanaState.HasFlag(KatanaState.Fire))
            {
                hit2Profile.SetBodyState(BodyState.OnFire);
            }
            if (hit1Profile.katanaState.HasFlag(KatanaState.Earth))
            {
                hit2Profile.SetBodyState(BodyState.Earth);
            }
            NetworkDataBase.GetPlayerManagerPV(nick2).RPC(nameof(PlayerProfile.Damage), NetworkDataBase.GetPlayerByNickname(nick2), damage * damageMultiplier);
        }
    }
}