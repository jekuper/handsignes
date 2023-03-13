using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float shiftSpeed = 8f;
    [SerializeField] private GameObject uiGroup;
    [SerializeField] private TextMeshProUGUI observableNicknameUI;

    Transform target;
    Transform targetRotation;

    float mouseX;
    float mouseY;

    int observablePlayer = 0;
    bool isLockedToPlayer = false;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;
    private void Update () {
        HandlePlayerView ();
        HandleRotation ();
        HandleMovement ();
    }
    private void HandlePlayerView () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            isLockedToPlayer = !isLockedToPlayer;
            if (isLockedToPlayer && (target == null || !NetworkDataBase.GetPlayerProfile (PhotonNetwork.PlayerList[observablePlayer].NickName).IsAlive)) {
                TrackLeft ();
            }
        }
        uiGroup.SetActive (isLockedToPlayer);
        if (!isLockedToPlayer)
            return;
        observablePlayer = Mathf.Clamp (observablePlayer, 0, PhotonNetwork.PlayerList.Length - 1);

        transform.position = target.position;
        transform.rotation = targetRotation.rotation;

        if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
            TrackLeft ();
        }
        if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
            TrackRight ();
        }
    }
    private void TrackLeft () {
        if (!isLockedToPlayer)
            return;
        bool found = false;
        for (int i = observablePlayer - 1; i >= 0; i--) {
            if (NetworkDataBase.GetPlayerProfile (PhotonNetwork.PlayerList[i].NickName).IsAlive) {
                observablePlayer = i;
                found = true;
                break;
            }
        }
        if (!found) {
            for (int i = PhotonNetwork.PlayerList.Length - 1; i >= observablePlayer; i--) {
                if (NetworkDataBase.GetPlayerProfile (PhotonNetwork.PlayerList[i].NickName).IsAlive) {
                    observablePlayer = i;
                    found = true;
                    break;
                }
            }
        }
        if (!found) {
            isLockedToPlayer = false;
        } else {
            target = NetworkDataBase.playersManagers[PhotonNetwork.PlayerList[observablePlayer].NickName].controller.cameraPosition;
            targetRotation = NetworkDataBase.playersManagers[PhotonNetwork.PlayerList[observablePlayer].NickName].controller.GetComponent<PlayerLook> ().headMeshBone;
            observableNicknameUI.text = PhotonNetwork.PlayerList[observablePlayer].NickName;
        }
    }
    private void TrackRight () {
        if (!isLockedToPlayer)
            return;
        bool found = false;
        for (int i = observablePlayer + 1; i < PhotonNetwork.PlayerList.Length; i++) {
            if (NetworkDataBase.GetPlayerProfile (PhotonNetwork.PlayerList[i].NickName).IsAlive) {
                observablePlayer = i;
                found = true;
                break;
            }
        }
        if (!found) {
            for (int i = 0; i <= observablePlayer; i++) {
                if (NetworkDataBase.GetPlayerProfile (PhotonNetwork.PlayerList[i].NickName).IsAlive) {
                    observablePlayer = i;
                    found = true;
                    break;
                }
            }
        }
        if (!found) {
            isLockedToPlayer = false;
        } else {
            target = NetworkDataBase.playersManagers[PhotonNetwork.PlayerList[observablePlayer].NickName].controller.cameraPosition;
            targetRotation = NetworkDataBase.playersManagers[PhotonNetwork.PlayerList[observablePlayer].NickName].controller.GetComponent<PlayerLook> ().headMeshBone;
            observableNicknameUI.text = PhotonNetwork.PlayerList[observablePlayer].NickName;
        }
    }
    private void HandleRotation () {
        if (Cursor.lockState != CursorLockMode.Locked || isLockedToPlayer) {
            return;
        }

        mouseX = Input.GetAxisRaw ("Mouse X");
        mouseY = Input.GetAxisRaw ("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp (xRotation, -90, 90f);

        transform.rotation = Quaternion.Euler (xRotation, yRotation, 0);
    }
    private void HandleMovement () {
        if (isLockedToPlayer)
            return;
        float horizontalMovement = Input.GetAxisRaw ("Horizontal");
        float verticalMovement = Input.GetAxisRaw ("Vertical");

        Vector3 direction = transform.forward * verticalMovement + transform.right * horizontalMovement;

        transform.position += direction * Time.deltaTime * (Input.GetKey (KeyCode.LeftShift) ? shiftSpeed : speed);
    }
}
