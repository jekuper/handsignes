using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaManager : MonoBehaviour
{
    public float damage = 15;
    public bool isOff = false;

    private bool isTriggerOff = false;
    [SerializeField] private Animator armAnim;
    [SerializeField] private GamePlayerManager gamePlayerManagerLink;
    [SerializeField] private KatanaTrigger triggerManager;

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
        if (!gamePlayerManagerLink.hasAuthority) {
            return;
        }
        if (!isOff && Input.GetKeyDown (KeyCode.Mouse0)) {
            armAnim.SetTrigger("strike");
            armAnim.SetFloat ("randFloat", (1f / 3f) * Random.Range (0, 3));
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
                NetworkBRManager.brSingleton.ApplyDamage (nick2, damage);
            }
        }
    }
}
