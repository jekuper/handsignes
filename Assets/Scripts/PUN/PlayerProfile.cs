using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LivingCreature : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public float mana = 500;
    public float maxMana = 500;

    public BodyState bodyState = BodyState.None;

    public bool IsAlive
    {
        get { return health > 0; }
    }
    public virtual void Die() { }
    [PunRPC]
    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    public void Heal(float healValue)
    {
        health += healValue;
    }
    public bool TrySpendMana(float spent)
    {
        if (mana < spent)
        {
            return false;
        }
        mana -= spent;
        return true;
    }
    public void RecoverMana(float recoverValue)
    {
        mana += recoverValue;
        mana = Mathf.Clamp(mana, 0, recoverValue);
    }
    [PunRPC]
    public void SetBodyState(BodyState state)
    {
        bodyState |= state;
        ApplyBodyState();
    }
    [PunRPC]
    public void UnSetBodyState(BodyState state)
    {
        bodyState &= ~state;
        ApplyBodyState();
    }
    private void ApplyBodyState() //reactions go here
    {
        if (bodyState.HasFlag(BodyState.Wet) && bodyState.HasFlag(BodyState.OnFire))
        {
            bodyState &= ~(BodyState.Wet | BodyState.OnFire);
        }
        if (bodyState.HasFlag(BodyState.ElectroShock) && bodyState.HasFlag(BodyState.OnFire))
        {
            bodyState &= ~(BodyState.ElectroShock | BodyState.OnFire);
        }
    }
}
public class PlayerProfile : LivingCreature, IPunObservable
{
    public int kunai = 10;
    public int kunaiMax = 10;

    public throwableType throwableInUse = throwableType.Kunai;
    public KatanaState katanaState = KatanaState.None;

    public int teamIndex = -1;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(maxHealth);
            stream.SendNext(mana);
            stream.SendNext(maxMana);
            stream.SendNext(bodyState);

            stream.SendNext(kunai);
            stream.SendNext(kunaiMax);
            stream.SendNext(throwableInUse);
            stream.SendNext(katanaState);

            stream.SendNext(teamIndex);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            maxHealth = (float)stream.ReceiveNext();
            mana = (float)stream.ReceiveNext();
            maxMana = (float)stream.ReceiveNext();
            bodyState = (BodyState)stream.ReceiveNext();

            kunai = (int)stream.ReceiveNext();
            kunaiMax = (int)stream.ReceiveNext();
            throwableInUse = (throwableType)stream.ReceiveNext();
            katanaState = (KatanaState)stream.ReceiveNext();

            teamIndex = (int)stream.ReceiveNext();
        }
    }
    public override void Die () {
        if (!GetComponent<PhotonView> ().AmOwner)
            return;
        if (GetComponent<PlayerManager> ().controller != null)
            PhotonNetwork.Destroy(GetComponent<PlayerManager> ().controller.gameObject);
        NetworkLevelData.singleton.CamHolder.GetComponent<CameraController> ().enabled = true;
        GameSceneManager.singleton.ShowObserveMenu ();
    }
}
