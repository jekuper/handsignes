using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunManager : NetworkBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] WallRun wallrun;
    [SerializeField] DashMechanic dashManager;
    [SerializeField] technicsManager technicManager;
    [SyncVar]
    public bool IsTechnicStunned = false;

    private float timerMovement = -1;
    private float timerTechnic = -1;

    [Server]
    public void Stun(float duration, bool includeMovements = false, bool includeTechnics = false)
    {
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
    [ServerCallback]
    private void Update()
    {
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
    [Server]
    public void StunOn(bool includeMovements, bool includeTechnics)
    {
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
    [Server]
    public void StunOff(bool includeMovements, bool includeTechnics)
    {
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
}
