using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kunai : MonoBehaviour
{
    public bool isStuck = false;
    
    private Rigidbody rb;
    private Vector3 startPosition;
    private float initTime;


    private void Start () {
        rb = GetComponent<Rigidbody> ();
        startPosition = transform.position;
        initTime = Time.time;
    }

    private void Update () {
//        if (Time.time - initTime > 20) {
//            Destroy (gameObject);
//        }
        if (Vector3.Distance(startPosition, transform.position) > 35) {
            Destroy (gameObject);
        }
    }

    private void FixedUpdate () {
//        Debug.DrawRay (transform.position, transform.forward, Color.red, 1f);
        if (!isStuck && Physics.Raycast (transform.position, transform.forward, 0.65f)){
            hit ();
        }
    }
    /*    private void FixedUpdate () {
            if (rb.velocity != Vector3.zero) {
                rb.rotation = Quaternion.LookRotation (rb.velocity);
            }
        }
    */
    private void hit () {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isStuck = true;
    }
}
