using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunManager : MonoBehaviour, IPunObservable
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] WallRun wallrun;
    [SerializeField] DashMechanic dashManager;
    [SerializeField] technicsManager technicManager;
    [SerializeField] PhotonView PV;
    
    public bool IsTechnicStunned = false;

    private float timerMovement = -1;
    private float timerTechnic = -1;


    [PunRPC]
    public void Stun(float duration, bool includeMovements = false, bool includeTechnics = false)
    {
        if (!PV.AmOwner)
            return;
        StunOn(includeMovements, includeTechnics);
        if (includeMovements)
        {
            timerMovement = Mathf.Max(timerMovement, duration);
        }
        if (includeTechnics)
        {
            timerTechnic = Mathf.Max(timerTechnic, duration);
        }
    }

    private void Update()
    {
        if (!PV.AmOwner)
            return;

        if (timerMovement > 0)
        {
            timerMovement -= Time.deltaTime;
            if (timerMovement <= 0)
            {
                StunOff(true, false);
            }
        }
        if (timerTechnic > 0)
        {
            timerTechnic -= Time.deltaTime;
            if (timerTechnic <= 0)
            {
                StunOff(false, true);
            }
        }
    }

    [PunRPC]
    public void StunOn(bool includeMovements, bool includeTechnics)
    {
        if (!PV.AmOwner)
            return;
        if (includeMovements)
        {
            movement.controlsEnabled = false;
            wallrun.controlsEnabled = false;
            dashManager.controlsEnabled = false;
        }
        if (includeTechnics)
        {
            IsTechnicStunned = true;
            technicManager.TurnOff();
        }
    }

    [PunRPC]
    public void StunOff(bool includeMovements, bool includeTechnics)
    {
        if (!PV.AmOwner)
            return;
        if (includeMovements)
        {
            timerMovement = -1;
            movement.controlsEnabled = true;
            wallrun.controlsEnabled = true;
            dashManager.controlsEnabled = true;
        }
        if (includeTechnics)
        {
            timerTechnic = -1;
            IsTechnicStunned = false;
            technicManager.TurnOn();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext (IsTechnicStunned);
        }
        else
        {
            IsTechnicStunned = (bool)stream.ReceiveNext();
        }
    }
}
