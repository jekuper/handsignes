using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BRMenuesManager : MonoBehaviour
{
    private enum BRMenu {
        GameGUI,
        Pause,
        Observe,
    }
    [SerializeField] GameObject GameUI;
    [SerializeField] UIPositionEffector pauseMenuEffector;

    [SerializeField] UIPositionEffector observeB1, observeB2;

    public static BRMenuesManager singleton;

    bool isPauseMenuVisible = false;

    private void Awake () {
        singleton = this;
    }

    public void ShowPauseMenu () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuEffector.SetFromIndex (1);
        isPauseMenuVisible = true;
    }
    public void HidePauseMenu () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuEffector.SetFromIndex (0);
        isPauseMenuVisible = false;
    }
    public void ShowObserveMenu () {
        GameUI.SetActive (false);
        observeB1.SetFromIndex (1);
        observeB2.SetFromIndex (1);
    }
    public void HideObserveMenu () {
        observeB1.SetFromIndex (0);
        observeB2.SetFromIndex (0);
    }

    public void OnDisconnectPressed () {
        NetworkManager.singleton.StopHost ();
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (isPauseMenuVisible)
                HidePauseMenu ();
            else
                ShowPauseMenu ();
        }
    }
}
