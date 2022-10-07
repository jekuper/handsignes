using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaManager : NetworkBehaviour
{
    public float damage = 15;
    public bool isOff = false;

    private bool isTriggerOff = false;
    [SerializeField] private Animator armAnim;
    [SerializeField] private GamePlayerManager gamePlayerManagerLink;
    [SerializeField] private KatanaTrigger triggerManager;
    [SerializeField] private GameObject weaponHolder;

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
    public void RpcShow()
    {
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
        RpcShow();
    }
    [Command]
    public void CmdHide()
    {
        RpcHide();
    }
    #endregion

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
            NetworkBRManager.brSingleton.ApplyDamage(nick2, damage);
        }
    }
}
