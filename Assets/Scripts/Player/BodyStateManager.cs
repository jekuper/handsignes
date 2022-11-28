using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BodyStateManager : MonoBehaviour
{
    [SerializeField] StunManager stun;
    [SerializeField] Sprite[] bodyStatesSprites;
    public float fireDamage = 0.5f;
    public float electroTimer = 10f;

    PostProcessVolume pp;
    UIPositionEffector[] bodyStatesEffectors;
    Image[] bodyStatesImages;

    private BodyState lastState = BodyState.None;

    private Coroutine electroCoroutine;
    private Dictionary<int, BodyState> cellsData = new Dictionary<int, BodyState>();

    private PhotonView PV;

    private void Start()
    {
        pp = NetworkLevelData.singleton.pp;
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
        HandleEarth();

        lastState = NetworkDataBase.localProfile.bodyState;
    }
    private void HandleWater()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Wet) &&
           !lastState.HasFlag(BodyState.Wet))
        {
            BlurCamera();
            AddCell (BodyState.Wet);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Wet) &&
          lastState.HasFlag(BodyState.Wet))
        {
            UnBlurCamera();
            DeleteCell(BodyState.Wet);
        }
    }
    private void HandleFire()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.OnFire) &&
            !lastState.HasFlag(BodyState.OnFire))
        {
            StartCoroutine(OnFireResponce());
            AddCell(BodyState.OnFire);
        } else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.OnFire) &&
            lastState.HasFlag(BodyState.OnFire))
        {
            DeleteCell(BodyState.OnFire);
        }
    }
    private void HandleElectro()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock) &&
            !lastState.HasFlag(BodyState.ElectroShock))
        {
            electroCoroutine = StartCoroutine(ElectroResponce());
            AddCell(BodyState.ElectroShock);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.ElectroShock) &&
            lastState.HasFlag(BodyState.ElectroShock))
        {
            DeleteCell(BodyState.ElectroShock);
        }
    }
    private void HandleEarth()
    {
        if (NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Earth) &&
            !lastState.HasFlag(BodyState.Earth))
        {
            AddCell(BodyState.Earth);
        }
        else if (!NetworkDataBase.localProfile.bodyState.HasFlag(BodyState.Earth) &&
          lastState.HasFlag(BodyState.Earth))
        {
            DeleteCell(BodyState.Earth);
        }
    }
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
        if (state == BodyState.Earth)
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
        DepthOfField blur = pp.profile.GetSetting<DepthOfField> ();
        blur.enabled.value = true;
    }
    private void UnBlurCamera()
    {
        DepthOfField blur = pp.profile.GetSetting<DepthOfField>();
        blur.enabled.value = false;
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
