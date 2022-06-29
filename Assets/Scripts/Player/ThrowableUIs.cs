using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThrowableUIs : MonoBehaviour
{
    public Image img;
    public List<Sprite> icons;
    public List<throwableObject> iconsType;
    public List<KeyCode> keyCodes;
    public TextMeshProUGUI text;

    private void Start () {
        if (icons.Count != iconsType.Count || icons.Count != keyCodes.Count) {
            Debug.LogError ("not equal sizes");
        }
        SwitchTo (0);
    }


    private void Update () {
        for (int i = 0; i < keyCodes.Count; i++) {
            if (Input.GetKeyDown (keyCodes[i])) {
                SwitchTo (i);
            }
        }
        if (ClonesManager.clones[ClonesManager.activeIndex].throwableInUse == throwableObject.Kunai) {
            if (img.sprite != icons[0])
                img.sprite = icons[0];
            UpdateCounter (ClonesManager.clones[ClonesManager.activeIndex].kunaiCount);
        }
        if (ClonesManager.clones[ClonesManager.activeIndex].throwableInUse == throwableObject.Shuriken) {
            if (img.sprite != icons[1])
                img.sprite = icons[1];
            UpdateCounter (ClonesManager.clones[ClonesManager.activeIndex].shurikenCount);
        }
    }
    public void SwitchTo(int index) {
        img.sprite = icons[index];

        ClonesManager.clones[ClonesManager.activeIndex].throwableInUse = iconsType[index];
    }

    public void UpdateCounter (int num) {
        if (num == 0) {
            img.color = new Color (1, 1, 1, .5f);
        } else {
            img.color = new Color (1, 1, 1, 1f);
        }
        text.text = num.ToString ();
    }
}
