using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMechanic : NetworkBehaviour
{

    public KeyCode actionCode;

    public const int maxDashCount = 1;
    public float DashReloadTime = 2;

    public float cameraFov = 100f;


    public float StartDashTime = .3f;
    public float DashSpeed = 10f;
    [SyncVar]
    public bool controlsEnabled = true;

    [SerializeField] Transform camHolder;

    private int chargedDashCount;
    private Rigidbody rb;
    private float reloadTimer = -1;

    private bool isDashing = false;

    private void Start () {
        chargedDashCount = maxDashCount;
        rb = GetComponent<Rigidbody> ();
        camHolder = NetworkLevelData.singleton.CamHolder;
    }

    private void Update () {
        if (reloadTimer > 0) {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0) {
                chargedDashCount++;
                if (chargedDashCount < maxDashCount) {
                    reloadTimer = DashReloadTime;
                }
            }
        }
        if (Input.GetKeyDown (actionCode) && controlsEnabled && chargedDashCount > 0 && Cursor.lockState == CursorLockMode.Locked) {
            StartCoroutine (Dash ());
            chargedDashCount--;
            reloadTimer = DashReloadTime;
        }

        if (isDashing) {
            CameraFOVmanager.singleton.AddCommand (cameraFov, FOVchangeSource.Dash);
        }
    }

    private IEnumerator Dash () {
        float dashTimeInternal = StartDashTime;

        Vector3 oldVelocity = rb.velocity;
        isDashing = true;

        while (dashTimeInternal > 0) {
            rb.velocity = camHolder.forward * DashSpeed;
            yield return new WaitForFixedUpdate ();
            dashTimeInternal -= Time.fixedDeltaTime;
        }

        rb.velocity = oldVelocity;
        isDashing = false;
    }
}
