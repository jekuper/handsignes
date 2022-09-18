using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TechnicDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI technicName;
    public string technicTag = "";

    public void Load (Technic technic) {
        technicTag = technic.tag;
        technicName.text = technic.name;
    }
    public void OnPressed () {
        TechnicDescriptionMenu.singleton.Show (technicTag);
    }
}
