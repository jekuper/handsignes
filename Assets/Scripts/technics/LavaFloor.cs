using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : MonoBehaviour, IPunObservable
{
    public float damageTimeSpan = 0.5f;
    public float damage = 20;

    public int teamIndex;

    private Dictionary<string, bool> players = new Dictionary<string, bool>();
    private PhotonView PV;

    private void Awake () {
        PV = GetComponent<PhotonView> ();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && PV.AmOwner)
        {
            TriggerResponce (other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            string nick = other.attachedRigidbody.GetComponent<PlayerController> ().manager.localNickname;
            if (players.ContainsKey (nick)) {
                players.Remove (nick);
            }
        }
    }
    public void TriggerResponce(string nick2)
    {
        PlayerProfile hit2Data = NetworkDataBase.GetPlayerProfile(nick2);

        if (teamIndex != hit2Data.teamIndex)
        {
            players.Add(nick2, true);
            StartCoroutine(ConstantDamage(nick2));
        }
    }
    private IEnumerator ConstantDamage(string nick)
    {
        float timer = damageTimeSpan;
        while (players.ContainsKey(nick))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                NetworkDataBase.GetPlayerManagerPV (nick).RPC (nameof (PlayerProfile.Damage), NetworkDataBase.GetPlayerByNickname (nick), damage);
                timer = damageTimeSpan;
            }
            if (!NetworkDataBase.GetPlayerProfile(nick).bodyState.HasFlag(BodyState.OnFire))
            {
                NetworkDataBase.GetPlayerManagerPV (nick).RPC (nameof (PlayerProfile.SetBodyState), NetworkDataBase.GetPlayerByNickname (nick), BodyState.OnFire);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext (teamIndex);
        } else {
            teamIndex = (int)stream.ReceiveNext ();
        }
    }
}
