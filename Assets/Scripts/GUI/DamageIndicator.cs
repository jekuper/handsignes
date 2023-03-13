using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public float alphaDecreaseSpeed = 5f;
    [Range(0f, 1f)]
    public float hitIncrement = 0.2f;

    private Image image;

    private void Start () {
        image = GetComponent<Image> ();
    }

    private void Update () {
        float newAlpha = Mathf.Clamp01 ((image.color.a - (alphaDecreaseSpeed * Time.deltaTime)));
        image.color = new Color (image.color.r, image.color.g, image.color.b, newAlpha);
    }
    public void Hitted () {
        float newAlpha = Mathf.Clamp01 ((image.color.a + hitIncrement));
        image.color = new Color (image.color.r, image.color.g, image.color.b, newAlpha);
    }
}
