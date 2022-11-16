using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaRegen : NetworkBehaviour
{
    [SerializeField] technicsManager techManager;
    [SerializeField] DashMechanic dashManager;
    [SerializeField] PlayerController manager;
    [SerializeField] PlayerMovement movement;
    [SerializeField] StunManager stun;

    public float manaIncreaseSpeed = 10f;

    private bool lastRegenState = false;
    private Coroutine regenCoroutine;

    [ServerCallback]
    private void Update()
    {
        if (techManager.isRegeningMana && !lastRegenState)
        {
            stun.Stun(1000000, true, false);
            regenCoroutine = StartCoroutine (Regen(0.1f));
        }
        else if (!techManager.isRegeningMana && lastRegenState)
        {
            stun.StunOff(true, false);
            StopCoroutine(regenCoroutine);
        }
        lastRegenState = techManager.isRegeningMana;
    }
    private IEnumerator Regen(float syncFrequency)
    {
        float timer = syncFrequency;

        while (true)
        {
            timer -= Time.deltaTime;
            ProfileData dt = NetworkDataBase.GetDataByNickname(manager.localNickname);
            NetworkDataBase.GetDataByNickname(manager.localNickname).mana = Mathf.Clamp((Time.deltaTime * manaIncreaseSpeed) + dt.mana, 0, dt.manaMax);
            if (timer <= 0)
            {
                NetworkDataBase.GetConnectionByNickname(manager.localNickname).identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.GetDataByNickname(manager.localNickname));
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
