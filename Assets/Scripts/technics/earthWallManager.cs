using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthWallManager : MonoBehaviour {
    [SerializeField] Collider boxCollider1;
    [SerializeField] Collider boxCollider2;
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

    public Color EarthColor;
    public Color ElectroColor;
    public Color WaterColor;
    public Color FireColor;

    private float animationSpeed;
    //0 - increasing in size, 1 - waiting, 2 - decreasing in size
    private int stage = 0;

    private float moveTimer = -1f;
    private PhotonView PV;
    private Renderer rd;
    private Rigidbody rb;


    private void Awake () {
        PV = GetComponent<PhotonView> ();
        rb = GetComponent<Rigidbody> ();
        rd = GetComponent<MeshRenderer> ();
        if (PV.AmOwner)
            PV.RPC(nameof(InitState), RpcTarget.All, NetworkDataBase.localProfile.katanaState);

        animationSpeed = (targetYScale - startYScale) / animationDuration;
        transform.localScale = new Vector3 (transform.localScale.x, startYScale, transform.localScale.z);
        from = transform.rotation;
        to = transform.rotation;
    }
    [PunRPC]
    private void InitState (KatanaState newState) {
        state = newState;
        if (state == KatanaState.None)
            state = KatanaState.Earth;
        if (state == KatanaState.Earth) {
            boxCollider1.enabled = true;
            boxCollider2.enabled = false;
            rb.isKinematic = true;
            trigger.enabled = false;
            rd.material.SetColor ("_Color", EarthColor);
        } else {
            boxCollider1.enabled = false;
            boxCollider2.enabled = true;
            trigger.enabled = true;

            switch (state) {
                case KatanaState.Water:
                rd.material.SetColor ("_Color", WaterColor);
                break;
                case KatanaState.Fire:
                rd.material.SetColor ("_Color", FireColor);
                break;
                case KatanaState.Electro:
                rd.material.SetColor ("_Color", ElectroColor);
                break;
            }
        }
    }
    public void AddForce (Transform source) {
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
        if (state == KatanaState.Earth || !PV.AmOwner)
            return;
        if (other.gameObject.tag == "Player") {
            string hitNickname = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            PlayerProfile hitProfile = NetworkDataBase.GetPlayerProfile (hitNickname);
            if (hitProfile == null)
                return;
            if (NetworkDataBase.localProfile.teamIndex != NetworkDataBase.GetPlayerProfile (hitNickname).teamIndex) {
                BodyState bodyState = BodyState.Earth;
                switch (state) {
                    case KatanaState.Water:
                    bodyState = BodyState.Wet;
                    break;
                    case KatanaState.Fire:
                    bodyState = BodyState.OnFire;
                    break;
                    case KatanaState.Electro:
                    bodyState = BodyState.ElectroShock;
                    break;
                    case KatanaState.Earth:
                    bodyState = BodyState.Earth;
                    break;
                }
                NetworkDataBase.GetPlayerManagerPV (hitNickname).RPC (nameof (PlayerProfile.SetBodyState), NetworkDataBase.GetPlayerByNickname (hitNickname), bodyState);
            }
        }
    }
}
