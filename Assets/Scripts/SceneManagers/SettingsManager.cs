using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Settings {
    public string nickname = "player";
    public bool effectsEnabled = true;
    public bool isFullsreen = true;
    public Dictionary<string, KeyCode> inputSettings = new Dictionary<string, KeyCode>();
    public Resolution savedResolution = new Resolution();

    public Settings()
    {
        savedResolution.width = 1920;
        savedResolution.height = 1080;
        savedResolution.refreshRate = 60;
    }
}

public class SettingsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameField;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] CustomToggle effects;
    [SerializeField] CustomToggle fullscreenToogle;

    List<string> dropdownOptions = new List<string>();

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
        set.isFullsreen = fullscreenToogle.isOn;
        set.savedResolution = StringToResolution(dropdownOptions[resolutionDropdown.value]);

        string json = JsonConvert.SerializeObject (set);
        FileManager.SaveId ("setttings", json);

        LoadSettings ();
    }
    public void LoadSettingsInternal () {
        Settings set = LoadSettings ();
        nicknameField.text = set.nickname;
        effects.isOn = set.effectsEnabled;
        fullscreenToogle.isOn = set.isFullsreen;

        resolutionDropdown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (i > 0 &&
                resolutions[i].width == resolutions[i - 1].width &&
                resolutions[i].height == resolutions[i - 1].height)
                continue;
            string option = resolutions[i].width + " x " + resolutions[i].height;
            dropdownOptions.Add(option);

            if (resolutions[i].width == set.savedResolution.width &&
                resolutions[i].height == set.savedResolution.height)
            {
                currentResolutionIndex = dropdownOptions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(dropdownOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = StringToResolution(dropdownOptions[resolutionIndex]);
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToogle.isOn);  
    }
    public void FullscreenToogleResponce(BaseEventData data)
    {
        SetResolution(resolutionDropdown.value);
    }
    public Resolution StringToResolution(string resolution)
    {
        Resolution result = new Resolution();
        result.width = int.Parse(resolution.Split("x")[0]);
        result.height = int.Parse(resolution.Split("x")[1]);
        result.refreshRate = Screen.resolutions[0].refreshRate;
        return result;
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
            if (i > 0 &&
                resolutions[i].width == resolutions[i - 1].width &&
                resolutions[i].height == resolutions[i - 1].height)
                continue;
            if (resolutions[i].width == set.savedResolution.width &&
                resolutions[i].height == set.savedResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, set.isFullsreen);

        NetworkDataBase.settings = set;
        return set;
    }
}
