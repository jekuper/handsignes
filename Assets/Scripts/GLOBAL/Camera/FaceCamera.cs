using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform target;

    private void Start () {
        target = Camera.main.transform;
    }

    private void Update () {
        transform.LookAt (target);
    }
}
