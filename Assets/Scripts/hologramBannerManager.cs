using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hologramBannerManager : MonoBehaviour
{
    public Texture2D[] textures;

    [ColorUsage (true, true)]
    public Color[] hologramColors;
    public float transitionDuration = 1f;

    [SerializeField] Renderer rd;

    float timer = 0;
    int currentTexture = 0;
    MaterialPropertyBlock _propBlock;

    private void Start () {
        SelectRandomTexture ();
    }
    private void SelectRandomTexture () {
        currentTexture = Random.Range (0, textures.Length);
        timer = Random.Range (7, 12);

        StopAllCoroutines ();
        //Debug.Log ("coroutine stopped");
        StartCoroutine (Transition(textures[currentTexture], hologramColors[currentTexture]));
    }
    private void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            SelectRandomTexture ();
        }
    }
    private IEnumerator Transition (Texture2D to, Color newColor) {
        Color currentColor = rd.material.GetColor("_hologram_Color");

        float currentBlendValue = rd.material.GetFloat ("_blendValue");
        float newBlendValue = (currentBlendValue < 0.5f ? 1f : 0f);
        float step = (newBlendValue < 0.5f ? -1f : 1f);

        //Debug.Log (newBlendValue + " " + step);

        if (newBlendValue < 0.5f) {
            rd.material.SetTexture ("_texture1", to);
        } else {
            rd.material.SetTexture ("_texture2", to);
        }

        float timeLeft = transitionDuration;
        
        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;

            //Debug.Log (timeLeft + " current: " + currentBlendValue);

            currentBlendValue += step * (1f / transitionDuration) * Time.deltaTime;
            currentBlendValue = Mathf.Clamp01 (currentBlendValue);

            rd.material.SetFloat ("_blendValue", currentBlendValue);
            currentColor = Color.Lerp (currentColor, newColor, Time.deltaTime / transitionDuration);
            rd.material.SetColor ("_hologram_Color", currentColor);

            yield return new WaitForEndOfFrame ();
        }
        rd.material.SetFloat ("_blendValue", newBlendValue);
    }
}
