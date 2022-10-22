using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Rendering.PostProcessing;

public class Settings {
    public string nickname = "";
    public bool effectsEnabled = true;
    public Dictionary<string, KeyCode> inputSettings = new Dictionary<string, KeyCode>();
}

public class SettingsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameField;
    [SerializeField] CustomToggle effects;

    private void Awake () {
        LoadSettingsInternal ();
    }

    public void OnBackPressed () {
        SaveSettings ();
    }
    public void SaveSettings () {
        Settings set = new Settings ();

        set.nickname = nicknameField.text;
        set.effectsEnabled = effects.isOn;

        string json = JsonConvert.SerializeObject (set);
        FileManager.SaveId ("setttings", json);

        LoadSettings ();
    }
    public void LoadSettingsInternal () {
        Settings set = LoadSettings ();
        nicknameField.text = set.nickname;
        effects.isOn = set.effectsEnabled;
    }
    public static Settings LoadSettings () {
        string json = FileManager.ConnectLines (FileManager.LoadId ("setttings"), "");

        Settings set = JsonConvert.DeserializeObject<Settings> (json);

        NetworkDataBase.LocalUserData.nickname = set.nickname;
        NetworkDataBase.ppProfile.GetSetting<Bloom>().enabled.value = set.effectsEnabled;
        NetworkDataBase.ppProfile.GetSetting<MotionBlur>().enabled.value = set.effectsEnabled;

        return set;
    }
}
