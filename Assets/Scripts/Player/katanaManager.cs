using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaManager : NetworkBehaviour
{
    public float damage = 15;
    public bool isOff = false;
    public Texture noneTexture, waterTexture, fireTexture, electroTexture;

    private bool isTriggerOff = false;
    [SerializeField] private Animator armAnim;
    [SerializeField] private GamePlayerManager gamePlayerManagerLink;
    [SerializeField] private KatanaTrigger triggerManager;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private Renderer katanaRenderer;

    public void TurnOn () {
        isOff = false;
        triggerManager.GetComponent<Collider> ().enabled = true;
    }
    public void TurnOff () {
        isOff = true;
        triggerManager.GetComponent<Collider> ().enabled = false;
    }
    #region Trigger On/Off
    [ClientRpc]
    public void RpcTriggerOn()
    {
//        Debug.Log("trigger turn on");
        isTriggerOff = false;
        triggerManager.GetComponent<Collider>().enabled = true;
    }
    [ClientRpc]
    public void RpcTriggerOff()
    {
//        Debug.Log("trigger turn off");
        isTriggerOff = true;
        triggerManager.GetComponent<Collider>().enabled = false;
    }
    [Command]
    public void CmdTurnTriggerOn () {
        RpcTriggerOn();
    }
    [Command]
    public void CmdTurnTriggerOff ()
    {
        RpcTriggerOff();
    }
    #endregion
    #region Hide/Show katana
    [ClientRpc]
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
        weaponHolder.SetActive(true);
    }
    [ClientRpc]
    public void RpcHide()
    {
        weaponHolder.SetActive(false);
    }
    [Command]
    public void CmdShow()
    {
        RpcShow(NetworkDataBase.data[connectionToClient].katanaState);
    }
    [Command]
    public void CmdHide()
    {
        RpcHide();
    }
    #endregion

    private void Start()
    {
        if (hasAuthority)
        {
            CmdTurnTriggerOff();
        }
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }
        if (!gamePlayerManagerLink.hasAuthority) {
            return;
        }
        if (!isOff && Input.GetKeyDown (KeyCode.Mouse0)) {
            armAnim.SetTrigger("strike");
            armAnim.SetFloat ("randFloat", (1f / 3f) * Random.Range (0, 3));
        }
    }
    

    public void TriggerResponce (Collider other) {
        if (other.tag == "Player" && !isTriggerOff && hasAuthority) {
            CmdTriggerResponce(other.transform.parent.parent.parent.GetComponent<GamePlayerManager>().localNickname);
        }
    }
    [Command]
    public void CmdTriggerResponce(string nick2)
    {
        string nick1 = gamePlayerManagerLink.localNickname;

        ProfileData hit1Data = NetworkDataBase.GetDataByNickname(nick1);
        ProfileData hit2Data = NetworkDataBase.GetDataByNickname(nick2);


        if (hit1Data.teamIndex != hit2Data.teamIndex)
        {
            float damageMultiplier = 1f;
            if (hit1Data.katanaState.HasFlag(KatanaState.Water))
            {
                NetworkBRManager.brSingleton.SetBodyState(nick2, BodyState.Wet);
            }
            if (hit1Data.katanaState.HasFlag(KatanaState.Electro))
            {
                NetworkBRManager.brSingleton.SetBodyState(nick2, BodyState.ElectroShock);
                if (hit2Data.bodyState.HasFlag(BodyState.Wet))
                {
                    damageMultiplier = 2f;
                }
            }
            if (hit1Data.katanaState.HasFlag(KatanaState.Fire))
            {
                NetworkBRManager.brSingleton.SetBodyState(nick2, BodyState.OnFire);
            }
            NetworkBRManager.brSingleton.ApplyDamage(nick2, damage * damageMultiplier);
        }
    }
}
