using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class katanaManager : MonoBehaviour
{
    public float damage = 30;

    private bool isOff = false;
    private bool isTriggerOff = false;
    [SerializeField] private Animator armAnim;
    [SerializeField] private GamePlayerManager gamePlayerManagerLink;
    [SerializeField] private katanaTrigger triggerManager;

    public void TurnOn () {
        isOff = false;
        triggerManager.GetComponent<Collider> ().enabled = true;
    }
    public void TurnOff () {
        isOff = true;
        triggerManager.GetComponent<Collider> ().enabled = false;
    }

    public void TurnTriggerOn () {
        isTriggerOff = false;
        triggerManager.GetComponent<Collider> ().enabled = true;
    }
    public void TurnTriggerOff () {
        isTriggerOff = true;
        triggerManager.GetComponent<Collider> ().enabled = false;
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }
        if (!gamePlayerManagerLink.GetComponent<NetworkIdentity> ().hasAuthority) {
            return;
        }
        if (Input.GetKeyDown (KeyCode.Mouse0) && !isOff) {
            armAnim.SetTrigger("strike");
        }
    }

    [ServerCallback]
    public void TriggerResponce (Collider other) {
        if (other.tag == "Player" && !isTriggerOff) {
            string nick1 = gamePlayerManagerLink.localNickname;
            string nick2 = other.transform.parent.parent.parent.GetComponent<GamePlayerManager> ().localNickname;

            ProfileData hit1Data = NetworkDataBase.GetDataByNickname (nick1);
            ProfileData hit2Data = NetworkDataBase.GetDataByNickname (nick2);


            if (hit1Data.teamIndex != hit2Data.teamIndex) {
                hit2Data.health -= damage;
                NetworkDataBase.GetConnectionByNickname (nick2).identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (hit2Data);
            }
        }
    }
}
