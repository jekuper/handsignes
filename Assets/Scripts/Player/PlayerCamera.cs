using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public bool working = true;
    public float sensitivity = 2.0f;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    private float rotX, rotY;

    private void Start() {
        if (!working)
            return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update() {
        if (!working)
            return;
        GetMouseDelta();

        RotateCamera();
        RotatePlayer();
    }
    private void GetMouseDelta() {
        rotX += Input.GetAxis("Mouse X") * sensitivity;

        rotY += Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, -90f, 90f);
    }
    private void RotateCamera() {
        cam.localRotation = Quaternion.Euler(-rotY, cam.localEulerAngles.y, cam.localEulerAngles.z);
    }
    private void RotatePlayer() {
        orientation.localRotation = Quaternion.Euler(orientation.localEulerAngles.x, rotX, orientation.localEulerAngles.z);
    }
}
