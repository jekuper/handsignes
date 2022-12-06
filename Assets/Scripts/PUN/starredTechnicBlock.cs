using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class starredTechnicBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI technicName;
    [SerializeField] Image[] mouseIcons;
    [SerializeField] Sprite[] signIcons;
    [SerializeField] Sprite blankSprite;

    public void Load (string tag) {
        technicName.text = NetworkDataBase.technicDescription[tag].name + " - ";
        for (int i = 0; i < tag.Length; i++) {
            mouseIcons[i].sprite = signIcons[tag[i] - '0'];
        }
        for (int i = tag.Length; i < mouseIcons.Length; i++) {
            mouseIcons[i].sprite = blankSprite;
        }
    }
}
