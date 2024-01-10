using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int ConnectionId;
    public bool isPlayerReady;
    public ulong playerSteamId;
    private bool avatarRetrieved;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerReadyText;

    public void UpdateUI() {
        playerNameText.text = playerName;
        if (isPlayerReady) {
            playerReadyText.text = "Ready";
        }
        else {
            playerReadyText.text = "Not Ready";
        }
    }
}
