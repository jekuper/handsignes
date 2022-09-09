using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowMechanic : NetworkBehaviour
{

    [SerializeField] float shurikenForceValue = 40f;

    [SerializeField] KeyCode ActionCode = KeyCode.F;
    public List<KeyCode> TypesKeyCodes;

    [SerializeField] GameObject kunaiPrefab;
    [SerializeField] GameObject shurikenPrefab;
    [SerializeField] Transform spawnPoint;

    private void Start () {
        GameGUIManager.singleton.UpdateThrowable();
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        for (int i = 0; i < TypesKeyCodes.Count; i++) {
            if (Input.GetKeyDown (TypesKeyCodes[i])) {
                SwitchThrowable ((throwableType)i);
            }
        }

        if (Input.GetKeyDown (ActionCode)) {
            if (NetworkDataBase.LocalUserData.throwableInUse == throwableType.Kunai) {
                ThrowKunai ();
            }
            else if (NetworkDataBase.LocalUserData.throwableInUse == throwableType.Shuriken) {
                ThrowShuriken ();
            }
        }
        GameGUIManager.singleton.UpdateThrowableCount ();
    }

    public void SwitchThrowable (throwableType type) {
        CmdSwitchThrowable (type);
    }


    private void ThrowKunai () {
        if (NetworkDataBase.LocalUserData.kunaiCount <= 0) {
            return;
        }
        CmdThrowKunai (spawnPoint.position, Quaternion.LookRotation (spawnPoint.forward));
    }
    private void ThrowShuriken () {
        if (NetworkDataBase.LocalUserData.shurikenCount <= 0) {
            return;
        }
        CmdThrowShuriken (spawnPoint.position, Quaternion.LookRotation (spawnPoint.forward));
    }


    [Command(requiresAuthority = false)]
    public void CmdSwitchThrowable (throwableType type, NetworkConnectionToClient sender = null) {
        NetworkDataBase.data[sender].throwableInUse = type;
        sender.identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[sender]);
    }

    [Command(requiresAuthority = false)]
    public void CmdThrowKunai (Vector3 spawnPosition, Quaternion spawnRotation, NetworkConnectionToClient sender = null) {
        GameObject kunai = Instantiate (kunaiPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn (kunai);
        kunai.GetComponent<kunai> ().SetOwner (NetworkDataBase.data[sender].nickname);

        NetworkDataBase.data[sender].kunaiCount--;
        sender.identity.GetComponent<LobbyPlayerManager>().TargetUpdateProfileData (NetworkDataBase.data[sender]);

    }

    [Command (requiresAuthority = false)]
    public void CmdThrowShuriken (Vector3 spawnPosition, Quaternion spawnRotation, NetworkConnectionToClient sender = null) {
        GameObject shuriken = Instantiate (shurikenPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn (shuriken);
        shuriken.GetComponent<Rigidbody> ().AddRelativeForce (Vector3.forward * shurikenForceValue, ForceMode.Impulse);
        shuriken.GetComponent<Rigidbody> ().AddRelativeTorque (Vector3.up * 4, ForceMode.Impulse);
        shuriken.GetComponent<shuriken> ().SetOwner (NetworkDataBase.data[sender].nickname);

        NetworkDataBase.data[sender].shurikenCount--;
        sender.identity.GetComponent<LobbyPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[sender]);

    }
}
