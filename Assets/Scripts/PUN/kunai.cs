using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kunai : MonoBehaviour, IPunObservable
{
    public float kunaiForceValue = 30f;
    public float damage = 30f;
    [SerializeField] GameObject bloodParticle;

    
    private Rigidbody rb;
    private PhotonView PV;
    private Vector3 startPosition;

    //to sync
    public bool isStuck = false;

    private GameObject pin = null;


    private void Start () {
        rb = GetComponent<Rigidbody> ();
        PV = GetComponent<PhotonView> ();
        startPosition = transform.position;
        if (PV.AmOwner)
        {
            rb.AddRelativeForce (Vector3.forward * kunaiForceValue, ForceMode.Impulse);
        }
        else
        {
            Destroy(rb);
        }
    }
    
    private void Update () {
        if (PV.AmOwner) {
            if (pin != null) {
                transform.position = pin.transform.position;
                transform.rotation = pin.transform.rotation;
            }
            if (Vector3.Distance(startPosition, transform.position) > 35) {
                PhotonNetwork.Destroy(PV);
            }
        }
    }

    private void FixedUpdate () {
        if (PV.AmOwner) {
            RaycastHit hitData;
            if (Physics.Raycast (transform.position, transform.forward, out hitData, 0.65f)){
                if (!isStuck) {
                    hit (hitData);
                }
            }
        }
    }
    private void OnTriggerEnter (Collider other) {
        lift (other);
    }
    private void lift (Collider other) {
        if (!isStuck || other.isTrigger)
            return;
        if (other.transform.gameObject.tag == "Player") {
            string hitNickname = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            if (hitNickname == PhotonNetwork.LocalPlayer.NickName &&
                NetworkDataBase.localProfile.kunai < NetworkDataBase.localProfile.kunaiMax) {
                NetworkDataBase.localProfile.kunai++;
                PV.RPC (nameof (Delete), PV.Owner);
            }
        }
    }
    private void hit (RaycastHit hitData) {
        if (!PV.AmOwner || hitData.collider.isTrigger)
            return;
        isStuck = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        pin = new GameObject ();
        pin = Instantiate (pin);
        pin.transform.position = transform.position;
        pin.transform.rotation = transform.rotation;
        pin.transform.parent = hitData.transform;
        Debug.Log (hitData.transform.gameObject.name);
        

        if (hitData.transform.gameObject.tag == "Player") {
            string hitNickname = hitData.rigidbody.GetComponent<PlayerController>().manager.localNickname;
            PlayerProfile hitProfile = NetworkDataBase.GetPlayerProfile (hitNickname);
            if (hitProfile == null)
                return;
            if (NetworkDataBase.localProfile.teamIndex != NetworkDataBase.GetPlayerProfile(hitNickname).teamIndex) {
                NetworkDataBase.GetPlayerManagerPV(hitNickname).RPC(nameof(hitProfile.Damage), NetworkDataBase.GetPlayerByNickname(hitNickname), damage);
                PV.RPC(nameof(playerHitEffect), RpcTarget.All);
                PhotonNetwork.Destroy (gameObject);
            }
        }
    }

    [PunRPC]
    public void playerHitEffect () {
        Instantiate (bloodParticle, transform.position, Quaternion.identity);
    }
    [PunRPC]
    public void Delete () {
        PhotonNetwork.Destroy (gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isStuck);
            stream.SendNext (transform.position);
            stream.SendNext (transform.rotation);
        }
        else
        {
            isStuck = (bool)stream.ReceiveNext();
            transform.position = (Vector3) stream.ReceiveNext ();
            transform.rotation = (Quaternion) stream.ReceiveNext ();
        }
    }
}
