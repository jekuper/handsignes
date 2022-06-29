using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthPrison : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public float lifeTimer = 10f;

    private void Update () {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0) {
            anim.SetTrigger ("dead");
        }
    }
}
