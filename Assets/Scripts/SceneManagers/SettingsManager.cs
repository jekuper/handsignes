using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class Settings {
    public string nickname = "";
    
}

public class SettingsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameField;

    private void Start () {
        LoadSettingsInternal ();
    }

    public void OnBackPressed () {
        SaveSettings ();
    }
    public void SaveSettings () {
        Settings set = new Settings ();

        set.nickname = nicknameField.text;

        string json = JsonConvert.SerializeObject (set);
        FileManager.SaveId ("setttings", json);

        LoadSettings ();
    }
    public void LoadSettingsInternal () {
        Settings set = LoadSettings ();
        nicknameField.text = set.nickname;
    }
    public static Settings LoadSettings () {
        string json = FileManager.ConnectLines (FileManager.LoadId ("setttings"), "");

        Settings set = JsonConvert.DeserializeObject<Settings> (json);

        NetworkDataBase.LocalUserData.nickname = set.nickname;

        return set;
    }
}
