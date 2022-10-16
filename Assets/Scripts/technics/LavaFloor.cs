using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : NetworkBehaviour
{
    public float damageTimeSpan = 0.5f;
    public float damage = 20;
    [SyncVar]
    public int teamIndex;

    private Dictionary<string, bool> players = new Dictionary<string, bool>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TriggerResponce (other.attachedRigidbody.GetComponent<GamePlayerManager>().localNickname);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            string nick = other.attachedRigidbody.GetComponent<GamePlayerManager>().localNickname;
            if (players.ContainsKey(nick))
            {
                players.Remove(nick);
            }
        }
    }
    [ServerCallback]
    public void TriggerResponce(string nick2)
    {
        ProfileData hit2Data = NetworkDataBase.GetDataByNickname(nick2);


        if (teamIndex != hit2Data.teamIndex)
        {
            players.Add(nick2, true);
            StartCoroutine(ConstantDamage(nick2));
        }
    }
    [Server]
    private IEnumerator ConstantDamage(string nick)
    {
        NetworkConnectionToClient connection = NetworkDataBase.GetConnectionByNickname(nick);
        float timer = damageTimeSpan;
        while (players.ContainsKey(nick))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                NetworkBRManager.brSingleton.ApplyDamage(connection, damage);
                timer = damageTimeSpan;
            }
            if (!NetworkDataBase.data[connection].bodyState.HasFlag(BodyState.OnFire))
            {
                NetworkBRManager.brSingleton.SetBodyState(connection, BodyState.OnFire);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
