using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffect : MonoBehaviour
{
    public Vector2 delta;
    public float animationTime = 0.1f;

    private Vector2 startSize;
    private Vector2 targetSize;
    private RectTransform rect;
    private float timer = -1;
    private Vector2 speed;

    private void Start () {
        rect = GetComponent<RectTransform> ();
        startSize = rect.sizeDelta;
        speed = delta / animationTime;
        speed = new Vector2 (Mathf.Abs (speed.x), Mathf.Abs (speed.y));
    }
    private void Update () {
        if (timer > 0) {
            timer -= Time.deltaTime;
            float newX, newY;
            newX = rect.sizeDelta.x + ((targetSize.x > rect.sizeDelta.x ? 1 : -1) * speed.x * Time.deltaTime);
            newY = rect.sizeDelta.y + ((targetSize.y > rect.sizeDelta.y ? 1 : -1) * speed.y * Time.deltaTime);
            rect.sizeDelta = new Vector2 (newX, newY);

            if (timer <= 0) {
                rect.sizeDelta = targetSize;    
            }
        }
    }
    public void Expand () {
        targetSize = startSize + delta;
        timer = animationTime;
    }
    public void Shrink () {
        targetSize = startSize;
        timer = animationTime;
    }
}
