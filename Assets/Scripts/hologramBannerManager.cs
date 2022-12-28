using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hologramBannerManager : MonoBehaviour
{
    public Texture2D[] textures;
    [SerializeField] Renderer rd;

    float timer = 0;
    int currentTexture = 0;

    private void Start () {
        SelectRandomTexture ();
    }
    private void SelectRandomTexture () {
        currentTexture = Random.Range (0, textures.Length);
        rd.material.SetTexture ("_MainTex", textures[currentTexture]);
        rd.material.SetTexture ("_EmissionMap", textures[currentTexture]);
        timer = Random.Range (7, 12);
    }
    private void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            SelectRandomTexture ();
        }
    }
}
