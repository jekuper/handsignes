using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegenManager : MonoBehaviour
{
    [SerializeField] technicsManager techManager;
    [SerializeField] PlayerController controller;
    [SerializeField] ParticleSystem effect;
    [SerializeField] StunManager stun;

    public float manaIncreaseSpeed = 10f;

    private bool lastRegenState = false;
    private Coroutine regenCoroutine;
    private PhotonView PV;
    private void Start () {
        PV = GetComponent<PhotonView> ();
    }

    private void Update()
    {
        if (!PV.AmOwner)
            return;
        if (techManager.isRegeningMana && !lastRegenState)
        {
            stun.Stun(1000000, true, false);
            PV.RPC (nameof (RPC_Effect), RpcTarget.All, true);
            regenCoroutine = StartCoroutine (Regen (0.1f));
        }
        else if (!techManager.isRegeningMana && lastRegenState)
        {
            stun.StunOff(true, false);
            PV.RPC (nameof (RPC_Effect), RpcTarget.All, false);
            StopCoroutine (regenCoroutine);
        }
        lastRegenState = techManager.isRegeningMana;
    }
    [PunRPC]
    private void RPC_Effect (bool state) {
        if (state)
            effect.Play ();
        else
            effect.Stop ();
    }
    private IEnumerator Regen (float syncFrequency) {
        float timer = syncFrequency;

        while (true) {
            if (!PV.AmOwner)
                yield break;
            PlayerProfile dt = NetworkDataBase.GetPlayerProfile (controller.manager.localNickname);
            dt.mana = Mathf.Clamp ((Time.deltaTime * manaIncreaseSpeed) + dt.mana, 0, dt.maxMana);
            yield return new WaitForEndOfFrame ();
        }
    }
}
