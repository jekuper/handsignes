using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour, IPunObservable
{
    [Header("References")]
    [SerializeField] WallRun wallRun;

    [SerializeField] Transform orientation = null;

    [SerializeField] Texture cursorTexture;
    [SerializeField] Transform camPosition;
    [SerializeField] Transform headMeshBone;

    [SerializeField] Transform shoulderR;
    [SerializeField] Transform shoulderL;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    Quaternion shoulderRRotation;
    Quaternion shoulderLRotation;
    PhotonView PV;

    Vector3 shoulderRDefault;
    Vector3 shoulderLDefault;

    private void OnGUI () {
        if (Cursor.lockState != CursorLockMode.Locked || !PV.AmOwner) {
            return;
        }
        GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 10, 10), cursorTexture);
    }

    private void Start()
    {
        PV = GetComponent<PhotonView> ();
        shoulderRDefault = shoulderR.localEulerAngles;
        shoulderLDefault = shoulderL.localEulerAngles;
        if (PV.AmOwner) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }



    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked || !PV.AmOwner) {
            return;
        }

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
         
        yRotation += mouseX * NetworkDataBase.settings.sensX * multiplier;
        xRotation -= mouseY * NetworkDataBase.settings.sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90, 90f);

        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        camPosition.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        headMeshBone.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        shoulderRRotation = Quaternion.Euler (xRotation, shoulderRDefault.y, shoulderRDefault.z);
        shoulderLRotation = Quaternion.Euler (xRotation, shoulderLDefault.y, shoulderLDefault.z);
        //camPosition.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //headMeshBone.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    private void LateUpdate()
    {
        if (NetworkDataBase.LocalInternalUserData.mouseState == mouseState.Weapons) {
        shoulderR.localRotation = shoulderRRotation;
            //shoulderL.localRotation = shoulderLRotation;
        }
        //shoulderL.rotation = Quaternion.Euler(xRotation, shoulderL.eulerAngles.y, shoulderL.eulerAngles.z);
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext (shoulderRRotation);
            stream.SendNext (shoulderLRotation);
        } else {
            shoulderRRotation = (Quaternion)stream.ReceiveNext ();
            shoulderLRotation = (Quaternion)stream.ReceiveNext ();
        }
    }
}
