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
            if (Vector3.Distance(startPosition, transform.position) > 35) {
                PhotonNetwork.Destroy(PV);
            }
        }
    }

    private void FixedUpdate () {
        if (PV.AmOwner) {
            RaycastHit hitData;
            if (!isStuck && Physics.Raycast (transform.position, transform.forward, out hitData, 0.65f)){
                hit (hitData);
            }
        }
    }

    private void hit (RaycastHit hitData) {
        if (!PV.AmOwner || hitData.collider.isTrigger)
            return;
        isStuck = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        transform.parent = hitData.transform;
        

        if (hitData.transform.gameObject.tag == "Player") {
            string hitNickname = hitData.rigidbody.GetComponent<PlayerController>().manager.localNickname;
            PlayerProfile hitProfile = NetworkDataBase.GetPlayerProfile (hitNickname);
            if (hitProfile == null)
                return;
            if (NetworkDataBase.localProfile.teamIndex != NetworkDataBase.GetPlayerProfile(hitNickname).teamIndex) {
                NetworkDataBase.GetPlayerManagerPV(hitNickname).RPC(nameof(hitProfile.Damage), NetworkDataBase.GetPlayerByNickname(hitNickname), damage);
                PV.RPC(nameof(playerHitEffect), RpcTarget.All);
                PhotonNetwork.Destroy (PV);
            }
        }
    }

    [PunRPC]
    public void playerHitEffect () {
        Instantiate (bloodParticle, transform.position, Quaternion.identity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isStuck);
        }
        else
        {
            isStuck = (bool)stream.ReceiveNext();
        }
    }
}
