using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseStateSwitcher : MonoBehaviour
{
    [SerializeField]private KeyCode switchKey = KeyCode.Q;

    [SerializeField]private Animator armAnim;
    [SerializeField]private technicsManager tm;
    [SerializeField]private KatanaManager wm;

    private void Start () {
        SetState (NetworkDataBase.LocalInternalUserData.mouseState);
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }
        if (Input.GetKeyDown (switchKey)) {
            ToggleState ();
        }
    }
    public void ToggleState () {
        if (NetworkDataBase.LocalInternalUserData.mouseState == mouseState.Weapons) {
            SetState (mouseState.Technics);
        } else {
            SetState (mouseState.Weapons);
        }
    }
    public void SetState (mouseState newState) {
        if (newState == mouseState.Weapons) {
            NetworkDataBase.LocalInternalUserData.mouseState = mouseState.Weapons;
            armAnim.SetInteger ("handsType", 0);
            if (tm != null) 
                tm.TurnOff ();
            if (wm != null)
                wm.TurnOn ();
        } else {
            NetworkDataBase.LocalInternalUserData.mouseState = mouseState.Technics;
            armAnim.SetInteger ("handsType", 1);
            if (tm != null)
                tm.TurnOn ();
            if (wm != null)
                wm.TurnOff ();
        }
    }
}
