using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BodyStateManager : MonoBehaviour
{
    [SerializeField] StunManager stun;
    [SerializeField] Sprite[] bodyStatesSprites;
    [SerializeField] SkinnedMeshRenderer body_rd, head_rd;
    [SerializeField] Material metalMaterial0, metalMaterial1;
    [SerializeField] ParticleSystem waterEffect;
    [SerializeField] ParticleSystem fireEffect;
    [SerializeField] ParticleSystem electroEffect;
    [SerializeField] GameObject[] bodyStatesImagesIngameCanvas;
    public float fireDamage = 0.5f;
    public float electroTimer = 10f;

    UIPositionEffector[] bodyStatesEffectors;
    Image[] bodyStatesImages;

    private BodyState lastState = BodyState.None;

    private Coroutine electroCoroutine;
    private Dictionary<int, BodyState> cellsData = new Dictionary<int, BodyState>();

    private PhotonView PV;

    private void Start()
    {
        bodyStatesEffectors = NetworkLevelData.singleton.bodyStatesEffectors;
        bodyStatesImages = NetworkLevelData.singleton.bodyStatesImages;
        PV = GetComponent<PhotonView> ();
    }

    private void Update()
    {
        if (!PV.AmOwner)
        {
            return;
        }
        HandleWater();
        HandleFire();
        HandleElectro();
        HandleMetal();

        lastState = NetworkDataBase.localProfile.bodyState;
    }
    #region Water
    private void HandleWater()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Wet) &&
           !lastState.HasFlag(BodyState.Wet))
        {
            BlurCamera();
            PV.RPC (nameof (RPC_WaterEffect), RpcTarget.All, true);
            AddCell (BodyState.Wet);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Wet) &&
          lastState.HasFlag(BodyState.Wet))
        {
            UnBlurCamera();
            PV.RPC (nameof (RPC_WaterEffect), RpcTarget.All, false);
            DeleteCell (BodyState.Wet);
        }
    }
    [PunRPC]
    private void RPC_WaterEffect (bool state) {
        if (state) {
            bodyStatesImagesIngameCanvas[0].SetActive (true);
            waterEffect.Play ();
        } else {
            bodyStatesImagesIngameCanvas[0].SetActive (false);
            waterEffect.Stop ();
        }
    }
    #endregion
    #region Fire
    private void HandleFire()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.OnFire) &&
            !lastState.HasFlag(BodyState.OnFire))
        {
            StartCoroutine(OnFireResponce());
            PV.RPC (nameof (RPC_FireEffect), RpcTarget.All, true);
            AddCell (BodyState.OnFire);
        } else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.OnFire) &&
            lastState.HasFlag(BodyState.OnFire))
        {
            PV.RPC (nameof (RPC_FireEffect), RpcTarget.All, false);
            DeleteCell(BodyState.OnFire);
        }
    }

    [PunRPC]
    private void RPC_FireEffect (bool state) {
        if (state) {
            bodyStatesImagesIngameCanvas[1].SetActive (true);
            fireEffect.Play ();
        } else {
            bodyStatesImagesIngameCanvas[1].SetActive (false);
            fireEffect.Stop ();
        }
    }
    #endregion
    #region Electro
    private void HandleElectro()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock) &&
            !lastState.HasFlag(BodyState.ElectroShock))
        {
            electroCoroutine = StartCoroutine(ElectroResponce());
            PV.RPC (nameof (RPC_ElectroEffect), RpcTarget.All, true);
            AddCell (BodyState.ElectroShock);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock) &&
            lastState.HasFlag(BodyState.ElectroShock))
        {
            PV.RPC (nameof (RPC_ElectroEffect), RpcTarget.All, false);
            DeleteCell(BodyState.ElectroShock);
        }
    }
    [PunRPC]
    private void RPC_ElectroEffect (bool state) {
        if (state) {
            bodyStatesImagesIngameCanvas[2].SetActive (true);
            electroEffect.Play ();
        } else {
            bodyStatesImagesIngameCanvas[2].SetActive (false);
            electroEffect.Stop ();
        }
    }
    #endregion
    #region Metal
    private void HandleMetal()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Metal) &&
            !lastState.HasFlag(BodyState.Metal))
        {
            AddCell(BodyState.Metal);
            PV.RPC (nameof (RPC_MetalEffect), RpcTarget.All, true);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Metal) &&
          lastState.HasFlag(BodyState.Metal))
        {
            DeleteCell(BodyState.Metal);
            PV.RPC (nameof (RPC_MetalEffect), RpcTarget.All, false);
        }
    }
    [PunRPC]
    private void RPC_MetalEffect (bool state) {
        if (!state) {
            bodyStatesImagesIngameCanvas[3].SetActive (true);
            body_rd.materials[0].CopyPropertiesFromMaterial (metalMaterial0);
            head_rd.materials[0].CopyPropertiesFromMaterial (metalMaterial0);
        } else {
            bodyStatesImagesIngameCanvas[3].SetActive (false);
            body_rd.materials[0].CopyPropertiesFromMaterial (metalMaterial1);
            head_rd.materials[0].CopyPropertiesFromMaterial (metalMaterial1);
        }
    }
    #endregion


    private void AddCell(BodyState state)
    {
        int index = GetFirstFreeUI();
        cellsData.Add (index, state);
        bodyStatesEffectors[index].SetFromIndex(1);
        if (state == BodyState.Wet)
            bodyStatesImages[index].sprite = bodyStatesSprites[0];
        if (state == BodyState.OnFire)
            bodyStatesImages[index].sprite = bodyStatesSprites[1];
        if (state == BodyState.ElectroShock)
            bodyStatesImages[index].sprite = bodyStatesSprites[2];
        if (state == BodyState.Metal)
            bodyStatesImages[index].sprite = bodyStatesSprites[3];
    }
    private void DeleteCell(BodyState state)
    {
        int index = GetCellIndex(state);
        bodyStatesEffectors[index].SetFromIndex(0);
        cellsData.Remove(index);
    }
    private int GetFirstFreeUI()
    {
        for (int i = 0; i < bodyStatesEffectors.Length; i++)
        {
            if (!cellsData.ContainsKey(i))
            {
                return i;
            }
        }
        Debug.LogError("not enought body state cells");
        return 0;
    }
    private int GetCellIndex(BodyState state)
    {
        for (int i = 0; i < bodyStatesEffectors.Length; i++)
        {
            if (cellsData.ContainsKey(i) && cellsData[i] == state)
            {
                return i;
            }
        }
        return -1;
    }

    private void BlurCamera()
    {
        Volume volume = Camera.main.GetComponent<Volume> ();
        DepthOfField DOF;

        volume.sharedProfile.TryGet (out DOF);

        DOF.active = true;
    }
    private void UnBlurCamera() {
        Volume volume = Camera.main.GetComponent<Volume> ();
        DepthOfField DOF;

        volume.sharedProfile.TryGet (out DOF);

        DOF.active = false;
    }
    private IEnumerator OnFireResponce()
    {
        while (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.OnFire))
        {
            NetworkDataBase.localProfile.Damage(fireDamage * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator ElectroResponce()
    {
        float timer = electroTimer;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock))
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock))
        {
            yield break;
        }
        stun.Stun (3, true, true);
        yield return new WaitForSeconds(3);
        stun.StunOff(true, true);
        NetworkDataBase.localProfile.UnSetBodyState(BodyState.ElectroShock);
    }
}
