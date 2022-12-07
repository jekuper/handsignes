using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TechnicDescriptionBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI technicName;
    [SerializeField] TextMeshProUGUI technicCost;
    [SerializeField] Image star;
    [SerializeField] Sprite starBorderSprite;
    [SerializeField] Sprite starSprite;
    public string technicTag = "";

    public void Load (TechnicDescription technic) {
        technicTag = technic.tag;
        technicName.text = technic.name;
        if (NetworkDataBase.starredTechnics[technic.tag]) {
            star.sprite = starSprite;
        }
        if (technic.isCalculatedManaCost)
        {
            technicCost.text = "Not fixed";
        }
        else
        {
            technicCost.text = technic.manaCost.ToString();
        }
    }

    public void ToggleStar () {
        if (star.sprite == starBorderSprite) {
            int count = 0;
            foreach (var item in NetworkDataBase.starredTechnics) {
                if (item.Value)
                    count++;
            }
            if (count < 4) {
                star.sprite = starSprite;
                NetworkDataBase.starredTechnics[technicTag] = true;
                GameSceneManager.singleton.UpdateStarredTechnics ();
            }
        } else {
            star.sprite = starBorderSprite;
            NetworkDataBase.starredTechnics[technicTag] = false;
            GameSceneManager.singleton.UpdateStarredTechnics ();
        }
    }
    public void OnPressed () {
        TechnicDescriptionMenu.singleton.Show (technicTag);
    }
}
