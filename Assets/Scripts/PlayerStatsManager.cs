using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsManager : MonoBehaviour
{
    [SerializeField]RectTransform manaBarBackground;
    [SerializeField]RectTransform manaBarForeground;

    [SerializeField]TextMeshProUGUI manaText;


    void Update()
    {
        ClonesManager.clones[ClonesManager.activeIndex].mana = Mathf.Min (ClonesManager.clones[ClonesManager.activeIndex].mana + Time.deltaTime * ClonesManager.clones[ClonesManager.activeIndex].manaRecoverSpeed, ClonesManager.clones[ClonesManager.activeIndex].manaMax);

        manaText.text = ClonesManager.clones[ClonesManager.activeIndex].mana.ToString ("0");
        manaBarForeground.sizeDelta = new Vector2 ((manaBarBackground.rect.width - 3) * (ClonesManager.clones[ClonesManager.activeIndex].mana / ClonesManager.clones[ClonesManager.activeIndex].manaMax), manaBarForeground.sizeDelta.y);
    }
}
