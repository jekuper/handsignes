using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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
}
public enum KatanaState
{
    None = 0,
    Water = 1,
    Fire = 2,
    Electro = 4,
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
    public TechnicDescription (string _tag, string _name, string _desc){
        tag = _tag;
        name = _name;
        description = _desc;
    }
}

public static class NetworkDataBase
{
    #region ServerOnly
    public static Dictionary<NetworkConnectionToClient, ProfileData> data = new Dictionary<NetworkConnectionToClient, ProfileData>();
    public static bool IsEverybodyReady = false;
    public static int lastTeamIndex = 0;


    public static void UpdateReadyStatus () {
        bool temp = true;
        int readyAmount = 0;
        int total = 0;
        foreach (var item in data) {
            total++;
            if (!item.Value.isReady) {
                temp = false;
                continue;
            }
            readyAmount++;
        }
        IsEverybodyReady = temp;
        if (NetworkServer.active) {
            if (LobbyGUI.singleton != null) {
                LobbyGUI.singleton.room_StartButton.SetActive (IsEverybodyReady);
            } else {
                RematchCounter counter = UnityEngine.Object.FindObjectOfType<RematchCounter> ();
                if (counter != null) {
                    if (total == readyAmount) {
                        counter.text = "Waiting for host...";
                    } else {
                        counter.text = readyAmount.ToString() + "/" + total.ToString();
                    }
                }
                BRGUI.singleton.winMenu_startButton.SetActive (IsEverybodyReady);
            }
        }
    }
    public static int CheckForWinner () {
        int winnerTeam = -1;
        foreach (var playerData in data) {
            if (playerData.Value.IsAlive) {
                if (winnerTeam != -1) {
                    return -1;
                }
                winnerTeam = playerData.Value.teamIndex;
            }
        }
        return winnerTeam;
    }

    public static NetworkConnectionToClient GetConnectionByNickname (string nickName) {
        foreach (var item in data) {
            if (item.Value.nickname == nickName) {
                return item.Key;
            }
        }
        return null;
    }
    public static ProfileData GetDataByNickname (string nickName) {
        foreach (var item in data) {
            if (item.Value.nickname == nickName) {
                return item.Value;
            }
        }
        return null;
    }
    public static bool NicknameExistChecker (string nickname) {
        return (GetConnectionByNickname (nickname) != null);
    }
    #endregion

    public static ProfileData LocalUserData = new ProfileData();
    public static InternalProfileData LocalInternalUserData = new InternalProfileData();

    public static Dictionary<string, TechnicDescription> technicDescription = new Dictionary<string, TechnicDescription> ();
}