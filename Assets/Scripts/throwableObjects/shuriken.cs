using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shuriken : MonoBehaviour {
    private Rigidbody rb;
    private Vector3 startPosition;
    private float initTime;
    private bool isStuck = false;


    private void Start () {
        rb = GetComponent<Rigidbody> ();
        startPosition = transform.position;
        initTime = Time.time;
    }

    private void Update () {
        if (Time.time - initTime > 20) {
            Destroy (gameObject);
        }
        if (Vector3.Distance (startPosition, transform.position) > 35) {
            Destroy (gameObject);
        }
    }
    private void OnCollisionEnter (Collision collision) {
        Destroy (gameObject);
    }
}
