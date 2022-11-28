using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public float lifeTimer = 10f;

    private PhotonView PV;

    private void Start () {
        PV = GetComponent<PhotonView> ();
    }
    private void Update () {
        if (!PV.AmOwner)
            return;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0) {
            anim.SetTrigger ("dead");
        }
    }
}
