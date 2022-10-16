using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    Fire = 0,
    Water = 1,
    Ice,
}

public class ParticlesHitResponser : NetworkBehaviour
{
    private readonly SyncList<int> hits = new SyncList<int>(){0, 0, 0};
    private readonly int[] maxHits = { 3, 3, 5 };

    [Server]
    public void Hit(ParticleType type)
    {
        hits[(int)type]++;
        CheckLimit();
    }
    [Server]
    public void Response(ParticleType type)
    {
        if (type == ParticleType.Fire)
        {
            NetworkBRManager.brSingleton.SetBodyState(connectionToClient, BodyState.OnFire);
        }
        if (type == ParticleType.Water)
        {
            NetworkBRManager.brSingleton.SetBodyState(connectionToClient, BodyState.Wet);
        }
    }
    [Server]
    private void CheckLimit()
    {
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i] >= maxHits[i])
            {
                hits[i] %= maxHits[i];
                Response((ParticleType)i);
            }
        }
    }
}
