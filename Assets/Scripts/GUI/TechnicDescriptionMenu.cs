using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TechnicDescriptionMenu : MonoBehaviour
{
    public static TechnicDescriptionMenu singleton;
    
    [SerializeField] Transform list;
    [SerializeField] GameObject listElement;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI manaCostText;
    [SerializeField] Image[] mouseIcons;
    [SerializeField] Sprite[] signIcons;
    [SerializeField] Sprite blankSprite;

    private bool isListLoaded = false;   

    public void Show (string tag) {
        if (!isListLoaded)
            LoadList ();
        descriptionText.text = NetworkDataBase.technicDescription[tag].description;
        nameText.text = NetworkDataBase.technicDescription[tag].name;
        if (NetworkDataBase.technicDescription[tag].isCalculatedManaCost)
        {
            manaCostText.text = "Mana Cost: Not fixed";
        }
        else
        {
            manaCostText.text = "Mana Cost: "+NetworkDataBase.technicDescription[tag].manaCost.ToString();
        }

        LoadTag (tag);
    }
    public void LoadTag (string tag) {
        for(int i = 0; i < tag.Length; i++) {
            mouseIcons[i].sprite = signIcons[tag[i] - '0'];
        }
        for (int i = tag.Length; i < mouseIcons.Length; i++) {
            mouseIcons[i].sprite = blankSprite;
        }
    }
    public void LoadList () {
        bool isFirst = true;
        isListLoaded = true;
        foreach (var item in NetworkDataBase.technicDescription) {
            GameObject elem = Instantiate (listElement, list);
            elem.GetComponent<TechnicDescriptionBlock> ().Load (item.Value);
            if (isFirst) {
                Show (item.Value.tag);
            }
            isFirst = false;
        }
    }
    private void Awake () {
        singleton = this;
    }
    private void Start () {
        if (!isListLoaded)
            LoadList ();
    }
}
