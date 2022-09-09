using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kunai : NetworkBehaviour
{
    public float kunaiForceValue = 30f;
    public float damage = 30f;
    [SerializeField] GameObject bloodParticle;

    [SyncVar(hook =nameof(StuckUpdateReceived))]
    public bool isStuck = false;
    [SyncVar]
    public string ownerNickname = "";
    
    private Rigidbody rb;

    [SyncVar]
    private Vector3 startPosition;
    [SyncVar]
    private float initTime;


    private void Start () {
        rb = GetComponent<Rigidbody> ();
        startPosition = transform.position;
        initTime = Time.time; 
        rb.AddRelativeForce (Vector3.forward * kunaiForceValue, ForceMode.Impulse);
    }

    [Server]
    public void SetOwner (string nickname) {
        ownerNickname = nickname;
    }

    private void Update () {
        if (NetworkServer.active) {
            if (Vector3.Distance(startPosition, transform.position) > 35) {
                NetworkServer.Destroy (gameObject);
            }
        }
    }

    private void FixedUpdate () {
        if (NetworkServer.active) {
            RaycastHit hitData;
            if (!isStuck && Physics.Raycast (transform.position, transform.forward, out hitData, 0.65f)){
                hitServer (hitData);
            }
        }
    }
    
    private void StuckUpdateReceived (bool oldVal, bool newVal) {
        if (!oldVal && newVal) {
            hitClient ();
        }
    }

    private void hitClient () {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isStuck = true;
    }

    [Server]
    private void hitServer (RaycastHit hitData) {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isStuck = true;

        if (hitData.transform.gameObject.tag == "Player") {
            NetworkConnectionToClient hitConnection = NetworkDataBase.GetConnectionByNickname (hitData.transform.gameObject.GetComponent<GamePlayerManager> ().localNickname);
            if (NetworkDataBase.GetDataByNickname (ownerNickname).teamIndex != NetworkDataBase.data[hitConnection].teamIndex) {
                NetworkDataBase.data[hitConnection].health -= damage;
                hitConnection.identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[hitConnection]);
            }
            playerHitEffect ();
            NetworkServer.Destroy (gameObject);
        }
    }

    [ClientRpc]
    public void playerHitEffect () {
        Debug.Log ("enter");
        Instantiate (bloodParticle, transform.position, Quaternion.identity);
    }
}
