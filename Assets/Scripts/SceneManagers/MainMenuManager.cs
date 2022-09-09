using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI version;
    private void Start () {
        version.text = "v"+ Application.version;
        SettingsManager.LoadSettings ();
    }
}
