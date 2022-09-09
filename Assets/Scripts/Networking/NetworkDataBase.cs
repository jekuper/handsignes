using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum throwableType {
    Kunai,
    Shuriken
}
public enum mouseState {
    Weapons,
    Technics
}

public class ProfileData {
    public string nickname;
    public bool isReady = false;

    public float healthMax = 100;
    public float health = 100;

    public float manaMax = 500;
    public float mana = 500;
    public float manaRecoverSpeed = 10;


    public int kunaiCountMax = 5;
    public int kunaiCount = 5;

    public int shurikenCountMax = 10;
    public int shurikenCount = 10;

    public throwableType throwableInUse = throwableType.Kunai;

    public int teamIndex = 0;
}
public class InternalProfileData {
    public mouseState mouseState = mouseState.Weapons;
}

public static class NetworkDataBase
{
    #region ServerOnly
    public static Dictionary<NetworkConnectionToClient, ProfileData> data = new Dictionary<NetworkConnectionToClient, ProfileData>();
    public static bool IsEverybodyReady = false;
    public static int lastTeamIndex = 0;


    public static void UpdateReadyStatus () {
        bool temp = true;
        foreach (var item in data) {
            if (!item.Value.isReady) {
                temp = false;
                break;
            }
        }
        IsEverybodyReady = temp;

        if (LobbyGUI.singleton != null && NetworkServer.active) {
            LobbyGUI.singleton.room_StartButton.SetActive (IsEverybodyReady);
        }
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
}
