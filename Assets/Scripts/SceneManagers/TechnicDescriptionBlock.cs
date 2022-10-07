using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TechnicDescriptionBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI technicName;
    public string technicTag = "";

    public void Load (TechnicDescription technic) {
        technicTag = technic.tag;
        technicName.text = technic.name;
    }
    public void OnPressed () {
        TechnicDescriptionMenu.singleton.Show (technicTag);
    }
}
