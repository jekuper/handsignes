using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public bool working = true;
    public float sensitivity = 2.0f;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    [Header("Wall run")]
    [SerializeField] private float wallRunFov;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    private float rotX, rotY;
    private float targetTilt = 0;
    private float targetFOV;

    private CinemachineVirtualCamera cinemachineCam;

    private void Start() {
        if (!working)
            return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        targetFOV = defaultFOV;
    }
    private void Update() {
        if (!working)
            return;
        GetMouseDelta();

        RotateCamera();
        RotatePlayer();
        ControlCam();
    }
    private void ControlCam() {
        if (Mathf.Abs(cinemachineCam.m_Lens.Dutch - targetTilt) > 0.5f)
            cinemachineCam.m_Lens.Dutch = Mathf.Lerp(cinemachineCam.m_Lens.Dutch, targetTilt, camTiltTime * Time.deltaTime);


        if (Mathf.Abs(cinemachineCam.m_Lens.FieldOfView - targetFOV) > 0.5f)
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCam.m_Lens.FieldOfView, targetFOV, camTiltTime * Time.deltaTime);
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


    public void StartWallRun(bool wallLeft, bool wallRight) {
        targetFOV = wallRunFov;

        if (wallLeft)
            targetTilt = -camTilt;
        else if (wallRight)
            targetTilt = camTilt;
    }
    public void StopWallRun() {
        targetFOV = defaultFOV;
        targetTilt = 0;
    }
}
