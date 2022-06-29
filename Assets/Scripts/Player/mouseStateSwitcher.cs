using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseStateSwitcher : MonoBehaviour
{
    [SerializeField]private KeyCode switchKey = KeyCode.CapsLock;

    [SerializeField]private GameObject weaponHolder;

    [SerializeField]private Animator armAnim;
    [SerializeField]private technicsManager tm;
    [SerializeField]private strikeManager sm;

    private void Start () {
        SetState (ClonesManager.clones[ClonesManager.activeIndex].mouseState);
    }

    private void Update () {
        if (Input.GetKeyDown (switchKey)) {
            ToggleState ();
        }
    }
    public void ToggleState () {
        if (ClonesManager.clones[ClonesManager.activeIndex].mouseState == mouseState.Weapons) {
            SetState (mouseState.Technics);
        } else {
            SetState (mouseState.Weapons);
        }
    }
    public void SetState (mouseState newState) {
        if (newState == mouseState.Weapons) {
            ClonesManager.clones[ClonesManager.activeIndex].mouseState = mouseState.Weapons;
            armAnim.SetInteger ("handsType", 0);
//            weaponHolder.SetActive (true);
            tm.TurnOff ();
            sm.TurnOn ();
        } else {
            ClonesManager.clones[ClonesManager.activeIndex].mouseState = mouseState.Technics;
            armAnim.SetInteger ("handsType", 1);
//            weaponHolder.SetActive (false);
            tm.TurnOn ();
            sm.TurnOff ();
        }
    }
}
