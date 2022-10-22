using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI version;
    [SerializeField] PostProcessProfile ppProfile;
    private void Start () {
        version.text = "v"+ Application.version;
        NetworkDataBase.ppProfile = ppProfile;
        SettingsManager.LoadSettings ();
    }
}
