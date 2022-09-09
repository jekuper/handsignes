using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class LobbyPlayerGUI : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public Image readyImage;
    public Image deleteImage;

    public float fadeTime;

    public Color readyColor;
    public Color notReadyColor;

    private LobbyPlayerManager manager;

    public void Initiate (LobbyPlayerManager _manager, ProfileData data) {
        nickname.text = data.nickname;
        manager = _manager;
        if (data.isReady) {
            readyImage.color = readyColor;
        } else {
            readyImage.color = notReadyColor;
        }
    }

    public void OnRemovePressed () {
        if (!isDeleteable()) {
            return;
        }
        manager.RemovePlayer ();
    }
    private bool isDeleteable () {
        return (NetworkServer.active && manager.localNickname != NetworkDataBase.LocalUserData.nickname);
    }

    public void Hover () {
        if (!isDeleteable ()) {
            return;
        }
        StartCoroutine (Fade(readyImage, 0));
        StartCoroutine (Fade(deleteImage, 1));
    }
    public void UnHover () {
        if (!isDeleteable ()) {
            return;
        }
        StartCoroutine (Fade (readyImage, 1));
        StartCoroutine (Fade (deleteImage, 0));
    }
    private IEnumerator Fade (Image image, float target) {
        float speed = 1 / fadeTime;

        while(image.color.a < target) {
            float newAlpha = image.color.a + speed * Time.deltaTime;
            image.color = new Color (image.color.r, 
                                     image.color.g, 
                                     image.color.b, 
                                     newAlpha);
            if (image.color.a >= target) {
                image.color = new Color (image.color.r, 
                                         image.color.g, 
                                         image.color.b, 
                                         target);
                break;
            }
            yield return new WaitForEndOfFrame ();
        }
        while(image.color.a > target) {
            float newAlpha = image.color.a - speed * Time.deltaTime;
            image.color = new Color (image.color.r,
                                     image.color.g,
                                     image.color.b,
                                     newAlpha);
            if (image.color.a <= target) {
                image.color = new Color (image.color.r,
                                         image.color.g,
                                         image.color.b,
                                         target);
                break;
            }
            yield return new WaitForEndOfFrame ();
        }
    }
}
