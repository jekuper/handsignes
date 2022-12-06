using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthWallManager : MonoBehaviour
{
    [SerializeField] Collider boxCollider;
    [SerializeField] Collider trigger;
    public float lifeTimer = 5;
    public float animationDuration = .5f;
    public float rotationSpeed = 180f;
    public float startYScale = 0.01f;
    public float targetYScale = 2f;
    public float moveImpulse = 10f;

    public KatanaState state;


    public Quaternion to;
    public Quaternion from;
    public Vector3 targetDirection;

    private float animationSpeed;
    //0 - increasing in size, 1 - waiting, 2 - decreasing in size
    private int stage = 0;

    private PhotonView PV;
    private float moveTimer = -1f;
    private Rigidbody rb;


    private void Awake () {
        PV = GetComponent<PhotonView> ();
        rb = GetComponent<Rigidbody> ();
        InitState ();
        rb.isKinematic = true;
        //if (!PV.AmOwner)

        animationSpeed = (targetYScale - startYScale) / animationDuration;
        transform.localScale = new Vector3 (transform.localScale.x, startYScale, transform.localScale.z);
        from = transform.rotation;
        to = transform.rotation;
    }
    private void InitState () {
        if (!PV.AmOwner)
            return;
        return;
        state = NetworkDataBase.localProfile.katanaState;
        if (state == KatanaState.None)
            state = KatanaState.Earth;
        if (state == KatanaState.Earth) {
            boxCollider.enabled = true;
            trigger.enabled = false;
        } else {
            boxCollider.enabled = true;
            trigger.enabled = false;

            switch (state) {
                case KatanaState.Water:
                break;
                case KatanaState.Fire:
                break;
                case KatanaState.Electro:
                break;
            }
        }
    }
    public void AddForce (Transform source) {
        return;
        if (state == KatanaState.Earth)
            return;
        from = transform.rotation;
        if (Vector3.Angle (transform.forward, source.forward) < Vector3.Angle (transform.forward, -source.forward))
            to = source.rotation;
        else
            to = Quaternion.LookRotation (-source.forward);
        rb.AddForce (source.forward * moveImpulse, ForceMode.Impulse);
    }

    private void Update () {
        if (stage == 0) {
            if (transform.localScale.y < targetYScale) {
                transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y + (Time.deltaTime * animationSpeed), transform.localScale.z);
            } else {
                stage = 1;
            }
        }
        if (stage == 1) {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer < 0) {
                stage = 2;
            }
        }
        if (stage == 2) {
            if (transform.localScale.y > startYScale) {
                transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y - (Time.deltaTime * animationSpeed), transform.localScale.z);
            } else {
                Destroy (gameObject);
            }
        }

        HandleRotation ();
//        HandleMovement ();
    }
    private void HandleRotation () {
        transform.rotation = Quaternion.RotateTowards (transform.rotation, to, rotationSpeed * Time.deltaTime);
    }
    private void HandleMovement () {
        if (moveTimer > 0) {
            moveTimer -= Time.fixedDeltaTime;
            transform.Translate (targetDirection * Time.fixedDeltaTime, Space.World);
        }
    }

    public void OnTriggerEnter (Collider other) {
        if (state == KatanaState.Earth)
            return;

    }
}
