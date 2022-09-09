using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    [SerializeField] UIPositionEffector pauseMenuEffector;
    [SerializeField] GameObject InGameUI;

    bool isVisible = false;

    public void ShowMenu () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        InGameUI.SetActive (false);
        pauseMenuEffector.SetFromIndex (1);
        isVisible = true;
    }
    public void HideMenu () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InGameUI.SetActive (true);
        pauseMenuEffector.SetFromIndex (0);
        isVisible = false;
    }

    public void OnDisconnectPressed () {
        NetworkManager.singleton.StopHost ();
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (isVisible)
                HideMenu ();
            else
                ShowMenu ();
        }
    }
}
