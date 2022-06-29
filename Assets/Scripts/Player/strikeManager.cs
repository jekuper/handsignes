using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strikeManager : MonoBehaviour
{
    private bool isOff = false;
    [SerializeField] private Animator armAnim;

    public void TurnOn () {
        isOff = false;
    }
    public void TurnOff () {
        isOff = true;
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Mouse0) && !isOff) {
            armAnim.SetTrigger("strike");
        }
    }
}
