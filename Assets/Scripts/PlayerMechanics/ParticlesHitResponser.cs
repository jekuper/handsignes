using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    Fire = 0,
    Water = 1,
    Ice,
}

public class ParticlesHitResponser : MonoBehaviour
{
    private readonly int[] maxHits = { 3, 3, 5 };
    private readonly int[] hits = { 0, 0, 0 };
    private PhotonView PV;

    private void Start () {
        PV = GetComponent<PhotonView> ();
    }

    [PunRPC]
    public void Hit(ParticleType type)
    {
        hits[(int)type]++;
        CheckLimit();
    }
    public void Response(ParticleType type)
    {
        if (!PV.AmOwner)
            return;
        if (type == ParticleType.Fire)
        {
            NetworkDataBase.localProfile.SetBodyState(BodyState.OnFire);
        }
        if (type == ParticleType.Water) {
            NetworkDataBase.localProfile.SetBodyState (BodyState.Wet);
        }
    }

    private void CheckLimit()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] >= maxHits[i])
            {
                hits[i] %= maxHits[i];
                Response((ParticleType)i);
            }
        }
    }
}
