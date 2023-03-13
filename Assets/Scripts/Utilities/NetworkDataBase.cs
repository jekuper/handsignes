using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

public enum throwableType {
    Kunai,
    Shuriken
}
public enum mouseState {
    Weapons,
    Technics
}
[Flags]
public enum BodyState
{
    None = 0,
    Wet = 1,
    OnFire = 2,
    ElectroShock = 4,
    Metal = 8,
}
public enum KatanaState
{
    None = 0,
    Water = 1,
    Fire = 2,
    Electro = 4,
    Earth = 8,
}

public class InternalProfileData {
    public mouseState mouseState = mouseState.Weapons;
}
public class TechnicDescription
{
    public string tag;
    public string name;
    public string description;
    public int manaCost;
    public bool isCalculatedManaCost;

    public TechnicDescription (string _tag, string _name, string _desc, bool _isCalcMana, int _manaCost){
        tag = _tag;
        name = _name;
        description = _desc;
        isCalculatedManaCost = _isCalcMana;
        manaCost = _manaCost;
    }
}
public enum GameType {
    Multiplayer,
    Singleplayer,
}

public static class NetworkDataBase
{
    public static ServerSettings photonServerSettings;
    public static GameType gameType = GameType.Singleplayer;

    public static void SaveSettings () {
        string json = JsonConvert.SerializeObject (settings);
        FileManager.SaveId ("setttings", json);

        SettingsManager.LoadSettings ();
    }

    public static InternalProfileData LocalInternalUserData = new InternalProfileData();

    public static Dictionary<string, PlayerManager> playersManagers = new Dictionary<string, PlayerManager>();
    public static Dictionary<string, TechnicDescription> technicDescription = new Dictionary<string, TechnicDescription> ();
    public static Dictionary<string, bool> starredTechnics = new Dictionary<string, bool> ();

    public static Settings settings = new Settings();

    public static PlayerProfile localProfile {
        get {
            return GetPlayerProfile(PhotonNetwork.LocalPlayer.NickName);
        }
    }
    public static PlayerManager localPlayerManager
    {
        get
        {
            return playersManagers[PhotonNetwork.LocalPlayer.NickName];
        }
    }

    public static void InitiateLocalPlayerData()
    {
        playersManagers.Clear();
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        SetCustomProperties(PhotonNetwork.LocalPlayer, "isReady", false);
    }
    public static void ResetLocalProfile () {
        localProfile.Reset ();
        LocalInternalUserData.mouseState = mouseState.Weapons;
    }
    public static PlayerProfile GetPlayerProfile(string nickname)
    {
        if (!playersManagers.ContainsKey(nickname))
            return null;
        return playersManagers[nickname].GetComponent<PlayerManager>().playerProfile;
    }
    public static PhotonView GetPlayerManagerPV(string nickname)
    {
        if (!playersManagers.ContainsKey(nickname))
            return null;
        return playersManagers[nickname].GetComponent<PhotonView>();
    }
    public static PhotonView GetPlayerControllerPV (string nickname) {
        return playersManagers[nickname].GetComponent<PlayerManager> ().controller.GetComponent<PhotonView>();
    }
    public static Player GetPlayerByNickname(string nickname)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == nickname)
                return player;
        }
        Debug.LogWarning("player \"" + nickname + "\" not found");
        return null;
    }
    public static void SetCustomProperties(Player player, object key, object value)
    {
        var hash = player.CustomProperties;
        if (!hash.ContainsKey(key))
            hash.Add(key, value);
        else
            hash[key] = value;
        player.SetCustomProperties(hash);
    }
    public static bool CheckReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["isReady"] == false)
            {
                return false;
            }
        }
        return true;
    }
    public static int CountReady () {
        int result = 0;
        foreach (Player player in PhotonNetwork.PlayerList) {
            if ((bool)player.CustomProperties["isReady"]) {
                result++;
            }
        }
        return result;
    }
    public static void SetAllNotReady () {
        foreach (Player player in PhotonNetwork.PlayerList) {
            SetCustomProperties (player, "isReady", false);
        }
    }
    public static int SearchWinnerTeam () {
        if (gameType == GameType.Multiplayer) {
            if (!PhotonNetwork.IsMasterClient)
                return -1;
            int winTeamIndex = -1;
            foreach (Player player in PhotonNetwork.PlayerList) {
                PlayerProfile profile = GetPlayerProfile (player.NickName);
                if (profile.IsAlive) {
                    if (winTeamIndex != -1)
                        return -1;
                    winTeamIndex = profile.teamIndex;
                }
            }
            if (winTeamIndex == -1)
                return 1;
            return winTeamIndex;
        } else {
            if (localProfile == null || localProfile.IsAlive)
                return -1;
            return 1;
        }
    }
}