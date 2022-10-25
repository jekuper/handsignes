using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Settings {
    public string nickname = "";
    public bool effectsEnabled = true;
    public Dictionary<string, KeyCode> inputSettings = new Dictionary<string, KeyCode>();
    public Resolution savedResolution = new Resolution();

    public Settings()
    {
        savedResolution.width = 1920;
        savedResolution.height = 1080;
    }
}

public class SettingsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameField;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] CustomToggle effects;

    Resolution[] resolutions;

    private void Awake () {
        resolutions = Screen.resolutions;
        LoadSettingsInternal ();
    }

    public void OnBackPressed () {
        SaveSettings ();
    }
    public void SaveSettings () {
        Settings set = new Settings ();

        set.nickname = nicknameField.text;
        set.effectsEnabled = effects.isOn;
        set.savedResolution = resolutions[resolutionDropdown.value];

        string json = JsonConvert.SerializeObject (set);
        FileManager.SaveId ("setttings", json);

        LoadSettings ();
    }
    public void LoadSettingsInternal () {
        Settings set = LoadSettings ();
        nicknameField.text = set.nickname;
        effects.isOn = set.effectsEnabled;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == set.savedResolution.width &&
                resolutions[i].height == set.savedResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, false);
        
    }
    public static Settings LoadSettings () {
        string json = FileManager.ConnectLines (FileManager.LoadId ("setttings"), "");

        Settings set = JsonConvert.DeserializeObject<Settings> (json);

        NetworkDataBase.LocalUserData.nickname = set.nickname;
        NetworkDataBase.ppProfile.GetSetting<Bloom>().enabled.value = set.effectsEnabled;
        NetworkDataBase.ppProfile.GetSetting<MotionBlur>().enabled.value = set.effectsEnabled;

        int currentResolutionIndex = 0;
        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {

            if (resolutions[i].width == set.savedResolution.width &&
                resolutions[i].height == set.savedResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, false);

        return set;
    }
}
