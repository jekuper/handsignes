using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrapManager : MonoBehaviour, IPunObservable
{
    [SerializeField] MeshRenderer rd;
    [SerializeField] Animator animator;
    [SerializeField] MeshCollider mainCollider;
    public int teamIndex = -1;
    public float timer = 5f;

    private bool activated = false;
    private PhotonView PV;

    private void Start () {
        PV = GetComponent<PhotonView> ();
    }
    private void Update () {
        if (teamIndex == NetworkDataBase.localProfile.teamIndex || activated)
            rd.enabled = true;
        else
            rd.enabled = false;
        if (activated && PV.AmOwner) {
            timer -= Time.deltaTime;
            if (timer <= 0)
                PhotonNetwork.Destroy (gameObject);
        }
    }
    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            string nick = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            PlayerProfile profile = NetworkDataBase.GetPlayerProfile (nick);
            if (profile.teamIndex != teamIndex) {
                PV.RPC (nameof (Activate), RpcTarget.All);
            }
        }
    }
    [PunRPC]
    private void Activate () {
        activated = true;
        mainCollider.enabled = true;
        animator.Play ("wallTrapActivate");
        Debug.Log ("activated");
        rd.enabled = true;
    }
    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext (teamIndex);
        } else {
            teamIndex = (int)stream.ReceiveNext ();
        }
    }
}
