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
    public NetworkPlayerManager mainNetworkPlayer;

    public Transform cameraPos;

    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private RectTransform healthBarBackground;
    [SerializeField] private RectTransform healthBarForeground;
    [SerializeField] private GameObject worldCanvas;
    [SerializeField] private SkinnedMeshRenderer headMesh;


    private void Start () {
        GetComponent<technicsManager> ().enabled = true;
        GetComponent<manaRegen> ().enabled = true;
        if (hasAuthority) {
            GetComponent<PlayerMovement> ().enabled = true;
            GetComponent<PlayerLook> ().enabled = true;
            GetComponent<WallRun> ().enabled = true;
            GetComponent<DashMechanic> ().enabled = true;
            GetComponent<mouseStateSwitcher> ().enabled = true;
            GetComponentInChildren<KatanaManager> ().enabled = true;
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
