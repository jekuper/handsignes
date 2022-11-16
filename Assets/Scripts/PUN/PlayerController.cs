using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public string localNickname;
    public bool isLocalPlayer = false;

    [SerializeField] Transform cameraPosition;
    [SerializeField] GameObject canvas;
    [SerializeField] SkinnedMeshRenderer[] renderers;
    PhotonView PV;

    [SerializeField] SkinnedMeshRenderer head;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.AmOwner)
            PV.RPC(nameof(Initialize), RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void Initialize(string nickname)
    {
        //Debug.Log("initializing with nickname: " + nickname + "; Local nickname: " + PhotonNetwork.LocalPlayer.NickName);

        NetworkDataBase.playersControllers.Add(nickname, gameObject);
        localNickname = nickname;
        if (PV == null)
            PV = GetComponent<PhotonView>();
        if (PV.AmOwner)
        {
            isLocalPlayer = true;
            NetworkLevelData.singleton.CameraMovement.cameraPosition = cameraPosition;

            head.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            GetComponent<PlayerLook>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<WallRun>().enabled = true;
            GetComponent<DashMechanic>().enabled = true;
            GetComponent<ThrowMechanic>().enabled = true;
            canvas.SetActive(false);
        }
        else
        {
            Destroy(GetComponent<Rigidbody>());
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.gameObject.layer = 0;
            }
        }
    }
}
