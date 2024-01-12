using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int ConnectionId;
    public bool isPlayerReady;
    public ulong playerSteamId;
    private bool avatarRetrieved;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerReadyText;
    [SerializeField] private RawImage playerAvatar;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    private void Start() {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    public void UpdateUI() {
        GetPlayerAvatar();
        
        playerNameText.text = playerName;
        if (isPlayerReady) {
            playerReadyText.text = "Ready";
            playerReadyText.color = Color.green;
        }
        else {
            playerReadyText.text = "Not Ready";
            playerReadyText.color = Color.red;
        }
    }
    void GetPlayerAvatar() {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamId);

        if (imageId == -1) {
            return;
        }

        playerAvatar.texture = GetSteamImageAsTexture(imageId);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback) {
        if (callback.m_steamID.m_SteamID == playerSteamId) {
            playerAvatar.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else {
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage) {
        Debug.Log("Executing GetSteamImageAsTexture for player: " + this.playerName);
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid) {
            Debug.Log("GetSteamImageAsTexture: Image size is valid?");
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid) {
                Debug.Log("GetSteamImageAsTexture: Image size is valid for GetImageRBGA?");
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarRetrieved = true;
        return texture;
    }
}
