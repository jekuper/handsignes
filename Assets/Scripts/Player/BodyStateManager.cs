using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BodyStateManager : NetworkBehaviour
{
    [SerializeField] StunManager stun;
    [SerializeField] PostProcessVolume pp;
    [SerializeField] UIPositionEffector[] bodyStatesEffectors;
    [SerializeField] Image[] bodyStatesImages;
    [SerializeField] Sprite[] bodyStatesSprites;
    public float fireDamage = 0.5f;
    public float electroTimer = 10f;

    private BodyState lastState = BodyState.None;

    private Coroutine electroCoroutine;
    private Dictionary<int, BodyState> cellsData = new Dictionary<int, BodyState>();

    private void Start()
    {
        pp = NetworkLevelData.singleton.pp;
        bodyStatesEffectors = NetworkLevelData.singleton.bodyStatesEffectors;
        bodyStatesImages = NetworkLevelData.singleton.bodyStatesImages;
    }

    private void Update()
    {
        if (!NetworkServer.active)
        {
            return;
        }
        HandleWater();
        HandleFire();
        HandleElectro();
        HandleEarth();

        lastState = NetworkDataBase.data[connectionToClient].bodyState;
    }
    private void HandleWater()
    {
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Wet) &&
           !lastState.HasFlag(BodyState.Wet))
        {
            BlurCamera();
            AddCell (BodyState.Wet);
        }
        else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Wet) &&
          lastState.HasFlag(BodyState.Wet))
        {
            UnBlurCamera();
            DeleteCell(BodyState.Wet);
        }
    }
    private void HandleFire()
    {
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.OnFire) &&
            !lastState.HasFlag(BodyState.OnFire))
        {
            StartCoroutine(OnFireResponce());
            AddCell(BodyState.OnFire);
        } else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.OnFire) &&
            lastState.HasFlag(BodyState.OnFire))
        {
            DeleteCell(BodyState.OnFire);
        }
    }
    private void HandleElectro()
    {
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock) &&
            !lastState.HasFlag(BodyState.ElectroShock))
        {
            electroCoroutine = StartCoroutine(ElectroResponce());
            AddCell(BodyState.ElectroShock);
        }
        else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock) &&
            lastState.HasFlag(BodyState.ElectroShock))
        {
            DeleteCell(BodyState.ElectroShock);
        }
    }
    private void HandleEarth()
    {
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Earth) &&
            !lastState.HasFlag(BodyState.Earth))
        {
            AddCell(BodyState.Earth);
        }
        else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Earth) &&
          lastState.HasFlag(BodyState.Earth))
        {
            DeleteCell(BodyState.Earth);
        }
    }
    [TargetRpc]
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
    [TargetRpc]
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

    [TargetRpc]
    private void BlurCamera()
    {
        DepthOfField blur = pp.profile.GetSetting<DepthOfField> ();
        blur.enabled.value = true;
    }
    [TargetRpc]
    private void UnBlurCamera()
    {
        DepthOfField blur = pp.profile.GetSetting<DepthOfField>();
        blur.enabled.value = false;
    }
    [Server]
    private IEnumerator OnFireResponce()
    {
        while (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.OnFire))
        {
            NetworkBRManager.brSingleton.ApplyDamage(connectionToClient, fireDamage * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    [Server]
    private IEnumerator ElectroResponce()
    {
        float timer = electroTimer;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock))
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock))
        {
            yield break;
        }
        stun.Stun (3, true, true);
        yield return new WaitForSeconds(3);
        stun.StunOff(true, true);
        NetworkBRManager.brSingleton.UnSetBodyState(connectionToClient, BodyState.ElectroShock);
    }
}
