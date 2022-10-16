using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    public float lifeTimer = 10f;

    private void Update () {
        if (!NetworkServer.active)
            return;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0) {
            anim.SetTrigger ("dead");
        }
    }
}
