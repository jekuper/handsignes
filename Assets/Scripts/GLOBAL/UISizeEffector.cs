using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISizeEffector : MonoBehaviour
{
    public Vector2 delta;
    public float animationTime = 0.1f;

    private Vector2 startSize;
    private Vector2 targetSize;
    private RectTransform rect;
    private float speed;

    private void Start () {
        rect = GetComponent<RectTransform> ();
        startSize = rect.sizeDelta;
        targetSize = startSize;
        speed = Vector2.Distance(Vector2.zero, delta) / animationTime;
    }
    private void Update () {
        rect.sizeDelta = Vector2.MoveTowards (rect.sizeDelta, targetSize, speed * Time.deltaTime);
    }
    public void Expand () {
        targetSize = startSize + delta;
    }
    public void Shrink () {
        targetSize = startSize;
    }
}
