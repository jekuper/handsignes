using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BodyStateManager : NetworkBehaviour
{
    [SerializeField] StunManager stun;
    [SerializeField] PostProcessVolume pp;
    public float fireDamage = 0.5f;
    public float electroTimer = 10f;

    private BodyState lastState = BodyState.None;

    private Coroutine electroCoroutine;

    private void Start()
    {
        pp = NetworkLevelData.singleton.pp;
    }

    private void Update()
    {
        if (!NetworkServer.active)
        {
            return;
        }
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.OnFire) &&
            !lastState.HasFlag(BodyState.OnFire))
        {
            StartCoroutine(OnFireResponce());
        }
        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Wet) &&
            !lastState.HasFlag(BodyState.Wet))
        {
            BlurCamera();
        } else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.Wet) &&
            lastState.HasFlag(BodyState.Wet))
        {
            UnBlurCamera();
        }

        if (NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock) &&
            !lastState.HasFlag(BodyState.ElectroShock))
        {
            electroCoroutine = StartCoroutine(ElectroResponce());
        } else if (!NetworkDataBase.data[connectionToClient].bodyState.HasFlag(BodyState.ElectroShock) &&
            lastState.HasFlag(BodyState.ElectroShock))
        {
            stun.StunOff(true, true);
        }

        lastState = NetworkDataBase.data[connectionToClient].bodyState;
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
    }
}
