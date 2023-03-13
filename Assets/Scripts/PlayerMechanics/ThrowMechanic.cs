using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowMechanic : MonoBehaviour
{

    [SerializeField] KeyCode ActionCode = KeyCode.F;

    Transform spawnPoint;

    private PhotonView PV;

    private void Start () {
        spawnPoint = NetworkLevelData.singleton.SpawnPoint;
        PV = GetComponent<PhotonView>();
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        if (Input.GetKeyDown (ActionCode)) {
            if (NetworkDataBase.localProfile.throwableInUse == throwableType.Kunai) {
                ThrowKunai ();
            }
        }
    }


    private void ThrowKunai () {
        if (NetworkDataBase.localProfile.kunai <= 0 ||
            !NetworkDataBase.localProfile.IsAlive) {
            return;
        }
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(spawnPoint.forward);
        GameObject kunai = PhotonNetwork.Instantiate("kunaiPrefab", spawnPosition, spawnRotation);
        NetworkDataBase.GetPlayerProfile(PhotonNetwork.LocalPlayer.NickName).kunai--;
    }
}
