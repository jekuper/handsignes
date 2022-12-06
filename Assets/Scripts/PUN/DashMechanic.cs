using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMechanic : MonoBehaviour, IPunObservable
{

    public KeyCode actionCode;

    public const int maxDashCount = 1;
    public float DashReloadTime = 2;

    public float cameraFov = 100f;


    public float StartDashTime = .3f;
    public float DashSpeed = 10f;

    public bool controlsEnabled = false;

    [SerializeField] Transform camHolder;
    [SerializeField] TrailRenderer[] trails;

    private int chargedDashCount;
    private Rigidbody rb;
    private float reloadTimer = -1;

    private bool isDashing = false;
    private bool lastDashState = false;

    private PhotonView PV;

    private void Start () {
        chargedDashCount = maxDashCount;
        rb = GetComponent<Rigidbody> ();
        PV = GetComponent<PhotonView> ();
        camHolder = NetworkLevelData.singleton.CamHolder;
    }

    private void Update () {
        HandleTrails ();

        if (PV.AmOwner) {
            if (reloadTimer > 0) {
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0) {
                    chargedDashCount++;
                    if (chargedDashCount < maxDashCount) {
                        reloadTimer = DashReloadTime;
                    }
                }
            }
            if (Input.GetKeyDown (actionCode) && controlsEnabled && !NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Earth) 
                && chargedDashCount > 0 && Cursor.lockState == CursorLockMode.Locked) {
                StartCoroutine (Dash ());
                chargedDashCount--;
                reloadTimer = DashReloadTime;
            }

            if (isDashing) {
                CameraFOVmanager.singleton.AddCommand (cameraFov, FOVchangeSource.Dash);
            }
        }
        lastDashState = isDashing;
    }
    private void HandleTrails () {
        if (isDashing && !lastDashState) {
            TurnTrails (true);
        } else if (!isDashing && lastDashState) {
            TurnTrails (false);
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
    public void TurnTrails (bool state) {
        foreach (var item in trails) {
            item.emitting = state;
        }
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext (isDashing);
        } else {
            isDashing = (bool)stream.ReceiveNext ();
        }
    }
}
