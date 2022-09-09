using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositionEffector : MonoBehaviour
{
    public float animationTime = 0.1f;
    public Vector2[] positions;

    private Vector2 target;
    private RectTransform rect;

    private float speed = 0;

    private void Start () {
        rect = GetComponent<RectTransform> ();
    }

    public void SetTarget (Vector2 newTarget) {
        target = newTarget;
        speed = Vector2.Distance (rect.anchoredPosition, target) / animationTime;
    }
    public void SetFromIndex (int index) {
        SetTarget (positions[index]);
    }
    private void Update () {
        rect.anchoredPosition = Vector2.MoveTowards (rect.anchoredPosition, target, speed * Time.deltaTime);
    }
}
