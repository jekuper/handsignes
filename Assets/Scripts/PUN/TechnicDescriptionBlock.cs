using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TechnicDescriptionBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI technicName;
    [SerializeField] TextMeshProUGUI technicCost;
    public string technicTag = "";

    public void Load (TechnicDescription technic) {
        technicTag = technic.tag;
        technicName.text = technic.name;
        if (technic.isCalculatedManaCost)
        {
            technicCost.text = "Not fixed";
        }
        else
        {
            technicCost.text = technic.manaCost.ToString();
        }
    }
    public void OnPressed () {
        TechnicDescriptionMenu.singleton.Show (technicTag);
    }
}
