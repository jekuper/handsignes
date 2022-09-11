using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition = null;

    void LateUpdate()
    {
        if (cameraPosition != null) {
            transform.position = cameraPosition.position;
            transform.rotation = cameraPosition.rotation;
        }
    }
}
