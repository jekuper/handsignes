using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;
using Photon.Realtime;

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
    Earth = 8,
}
public enum KatanaState
{
    None = 0,
    Water = 1,
    Fire = 2,
    Electro = 4,
    Earth = 8,
}

public class ProfileData {
    public string nickname;
    public bool isReady = false;

    public bool IsAlive {
        get { return health > 0; }
    }

    public float healthMax = 100;
    public float health = 100;

    public float manaMax = 500;
    public float mana = 500;
    public float manaRecoverSpeed = 10;


    public int kunaiCountMax = 10;
    public int kunaiCount = 10;

    public int shurikenCountMax = 10;
    public int shurikenCount = 10;

    public throwableType throwableInUse = throwableType.Kunai;
    public BodyState bodyState = BodyState.None;
    public KatanaState katanaState = KatanaState.None;

    public int teamIndex = 0;

    public ProfileData () { }

    public ProfileData (string _nickname, bool _isReady, int _teamIndex) {
        nickname = _nickname;
        isReady = _isReady;
        teamIndex = _teamIndex;
    }
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

public static class NetworkDataBase
{

    public static ProfileData LocalUserData = new ProfileData();
    public static InternalProfileData LocalInternalUserData = new InternalProfileData();

    public static Dictionary<string, PlayerManager> playersManagers = new Dictionary<string, PlayerManager>();
    public static Dictionary<string, TechnicDescription> technicDescription = new Dictionary<string, TechnicDescription> ();

    public static Settings settings = new Settings();
    public static PostProcessProfile ppProfile;

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
            return 0;
        return winTeamIndex;
    }
}