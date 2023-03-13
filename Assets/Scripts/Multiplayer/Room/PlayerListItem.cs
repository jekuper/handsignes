using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListItem : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public Image readyImage;
    public Image deleteImage;

    public float fadeTime;

    public Color readyColor;
    public Color notReadyColor;
    
    public void Initialize(Player data)
    {
        nickname.text = data.NickName;
        if ((bool)data.CustomProperties["isReady"] == true)
        {
            readyImage.color = readyColor;
        }
        else
        {
            readyImage.color = notReadyColor;
        }
    }

    public void OnRemovePressed()
    {
        if (!isDeleteable())
        {
            return;
        }
        RoomManager.signleton.GetComponent<PhotonView>().RPC(nameof(RoomManager.signleton.Kick), NetworkDataBase.GetPlayerByNickname(nickname.text), "kicked by master client");
    }
    private bool isDeleteable()
    {
        return (PhotonNetwork.IsMasterClient && nickname.text != PhotonNetwork.LocalPlayer.NickName);
    }

    public void Hover()
    {
        if (!isDeleteable())
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(Fade(readyImage, 0));
        StartCoroutine(Fade(deleteImage, 1));
    }
    public void UnHover()
    {
        if (!isDeleteable())
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(Fade(readyImage, 1));
        StartCoroutine(Fade(deleteImage, 0));
    }
    private IEnumerator Fade(Image image, float target)
    {
        float speed = 1 / fadeTime;

        while (image.color.a < target)
        {
            float newAlpha = image.color.a + speed * Time.deltaTime;
            image.color = new Color(image.color.r,
                                     image.color.g,
                                     image.color.b,
                                     newAlpha);
            if (image.color.a >= target)
            {
                image.color = new Color(image.color.r,
                                         image.color.g,
                                         image.color.b,
                                         target);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        while (image.color.a > target)
        {
            float newAlpha = image.color.a - speed * Time.deltaTime;
            image.color = new Color(image.color.r,
                                     image.color.g,
                                     image.color.b,
                                     newAlpha);
            if (image.color.a <= target)
            {
                image.color = new Color(image.color.r,
                                         image.color.g,
                                         image.color.b,
                                         target);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
