using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthWallManager : MonoBehaviour
{
    public float lifeTimer = 5;
    public float animationDuration = .5f;
    public float startYScale = 0.01f;
    public float targetYScale = 2f;

    private float animationSpeed;
    //0 - increasing in size, 1 - waiting, 2 - decreasing in size
    private int stage = 0;

    private void Awake () {
        animationSpeed = (targetYScale - startYScale) / animationDuration;
        transform.localScale = new Vector3 (transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void Update () {
        if (stage == 0) {
            if (transform.localScale.y < targetYScale) {
                transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y + (Time.deltaTime * animationSpeed), transform.localScale.z);
            } else {
                stage = 1;
            }
        }
        if (stage == 1) {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer < 0) {
                stage = 2;
            }
        }
        if (stage == 2) {
            if (transform.localScale.y > startYScale) {
                transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y - (Time.deltaTime * animationSpeed), transform.localScale.z);
            } else {
                Destroy (gameObject);
            }
        }
    }
}
