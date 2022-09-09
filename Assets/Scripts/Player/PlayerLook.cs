using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WallRun wallRun;

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform orientation = null;

    [SerializeField] Texture cursorTexture;
    [SerializeField] Transform camPosition;
    [SerializeField] Transform headMeshBone;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    private void OnGUI () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }
        GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 10, 10), cursorTexture);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
         
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90, 90f);

        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        camPosition.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        headMeshBone.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
    }
}
