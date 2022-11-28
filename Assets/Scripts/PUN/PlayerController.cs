using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public PlayerManager manager;
    public Transform cameraPosition;

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
    public void UnFreeze()
    {
        if (!PV.AmOwner)
        {
            return;
        }
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<WallRun>().enabled = true;
        GetComponent<DashMechanic>().enabled = true;
        GetComponent<ThrowMechanic>().enabled = true;
        GetComponent<KatanaManager>().enabled = true;
        GetComponent<StunManager>().enabled = true;
        GetComponent<mouseStateSwitcher>().enabled = true;
        GetComponent<technicsManager>().enabled = true;
        GetComponent<ParticlesHitResponser>().enabled = true;
        GetComponent<ManaRegenManager>().enabled = true;
        GetComponent<BodyStateManager>().enabled = true;
    }

    [PunRPC]
    public void Initialize(string nickname)
    {
        //Debug.Log("initializing with nickname: " + nickname + "; Local nickname: " + PhotonNetwork.LocalPlayer.NickName);

        manager = NetworkDataBase.playersManagers[nickname];
        manager.controller = this;

        if (PV == null)
            PV = GetComponent<PhotonView>();
        if (PV.AmOwner)
        {
            NetworkLevelData.singleton.CameraMovement.cameraPosition = cameraPosition;

            head.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            GetComponent<PlayerLook>().enabled = true;
            canvas.SetActive(false);
        }
        else
        {
            GetComponent<Rigidbody> ().isKinematic = true;
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.gameObject.layer = 0;
            }
        }
    }
}
