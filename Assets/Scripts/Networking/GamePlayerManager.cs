using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GamePlayerManager : NetworkBehaviour
{
    [SyncVar(hook =nameof(NicknameRecieved))] 
    public string localNickname;
    [SyncVar]
    public LobbyPlayerManager mainNetworkPlayer;

    [Range(0.01f, 1f)]
    public float manaCountSync = 0.5f;
    public float manaIncreaseSpeed = 10f;

    [SerializeField] private Transform cameraPos;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private RectTransform healthBarBackground;
    [SerializeField] private RectTransform healthBarForeground;
    [SerializeField] private GameObject worldCanvas;
    [SerializeField] private SkinnedMeshRenderer headMesh;


    private void Start () {
        GetComponent<technicsManager> ().enabled = true;
        if (GetComponent<NetworkIdentity> ().hasAuthority) {
            GetComponent<PlayerMovement> ().enabled = true;
            GetComponent<PlayerLook> ().enabled = true;
            GetComponent<WallRun> ().enabled = true;
            GetComponent<DashMechanic> ().enabled = true;
            GetComponent<mouseStateSwitcher> ().enabled = true;
            GetComponentInChildren<katanaManager> ().enabled = true;
            headMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            NetworkLevelData.singleton.CameraMovement.cameraPosition = cameraPos;
            nicknameText.transform.parent.gameObject.SetActive (false);
            worldCanvas.SetActive (false);
        } else {
            SkinnedMeshRenderer[] mr = GetComponentsInChildren<SkinnedMeshRenderer> (true);
            foreach (SkinnedMeshRenderer item in mr) {
                item.gameObject.layer = 0;
            }
        }

        if (NetworkServer.active) {
            StartCoroutine (PassiveManaSync());
        }
    }

    private IEnumerator PassiveManaSync () {
        while (true) {
            yield return new WaitForSeconds (manaCountSync);
            mainNetworkPlayer.TargetUpdateProfileData (NetworkDataBase.GetDataByNickname(mainNetworkPlayer.localNickname));
        }
    }
    private void Update () {
        if (NetworkServer.active) {
            ProfileData dt = NetworkDataBase.GetDataByNickname (mainNetworkPlayer.localNickname);
            NetworkDataBase.GetDataByNickname (mainNetworkPlayer.localNickname).mana = Mathf.Clamp((Time.deltaTime * manaIncreaseSpeed) + dt.mana, 0, dt.manaMax);
        }
    }

    public void NicknameRecieved(string nicknameOld, string nicknameNew) {
        if (nicknameText.gameObject.activeSelf)
            nicknameText.text = nicknameNew;
    }
    [Command]
    public void CmdUpdateHealth () {
        ProfileData data = NetworkDataBase.GetDataByNickname (localNickname);
        RpcHealthBarUpdate (data.health, data.healthMax);
    }
    [ClientRpc]
    public void RpcHealthBarUpdate (float health, float healthMax) {
        healthBarForeground.sizeDelta = new Vector2 ((healthBarBackground.rect.width) * (health / healthMax), healthBarForeground.sizeDelta.y);
    }
}
