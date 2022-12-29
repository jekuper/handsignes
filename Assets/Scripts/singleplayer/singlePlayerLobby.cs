using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singlePlayerLobby : MonoBehaviour
{
    private void Start () {
        NetworkDataBase.gameType = GameType.Singleplayer;
    }
}
