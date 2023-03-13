using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI version;
    private void Start () {
        version.text = "v"+ Application.version;
        NetworkDataBase.photonServerSettings = PhotonNetwork.PhotonServerSettings;
        SettingsManager.LoadSettings ();
    }
}
