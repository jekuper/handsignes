using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float shiftSpeed = 8f;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;
    private void Update () {
        HandleRotation ();
        HandleMovement ();
    }
    private void HandleRotation () {
        if (Cursor.lockState != CursorLockMode.Locked) {
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
        float horizontalMovement = Input.GetAxisRaw ("Horizontal");
        float verticalMovement = Input.GetAxisRaw ("Vertical");

        Vector3 direction = transform.forward * verticalMovement + transform.right * horizontalMovement;

        transform.position += direction * Time.deltaTime * (Input.GetKey (KeyCode.LeftShift) ? shiftSpeed : speed);
    }
}
