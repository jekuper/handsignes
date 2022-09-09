using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class shuriken : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar]
    private Vector3 startPosition;
    [SyncVar]
    private float initTime;
    [SyncVar]
    public string ownerNickname;


    private void Start () {
        rb = GetComponent<Rigidbody> ();
        startPosition = transform.position;
        initTime = Time.time;
    }

    [Server]
    public void SetOwner (string nickname) {
        ownerNickname = nickname;
    }

    private void Update () {
        if (NetworkServer.active) {
            if (Time.time - initTime > 20) {
                NetworkServer.Destroy (gameObject);
            }
            if (Vector3.Distance (startPosition, transform.position) > 35) {
                NetworkServer.Destroy (gameObject);
            }
        }
    }

    [ServerCallback]
    private void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "Player") {

        }
        NetworkServer.Destroy (gameObject);
    }
}
