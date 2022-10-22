using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowMechanic : NetworkBehaviour
{

    [SerializeField] KeyCode ActionCode = KeyCode.F;
    public List<KeyCode> TypesKeyCodes;

    [SerializeField] GameObject kunaiPrefab;
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


    [Command(requiresAuthority = false)]
    public void CmdSwitchThrowable (throwableType type, NetworkConnectionToClient sender = null) {
        NetworkDataBase.data[sender].throwableInUse = type;
        sender.identity.GetComponent<NetworkPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[sender]);
    }

    [Command(requiresAuthority = false)]
    public void CmdThrowKunai (Vector3 spawnPosition, Quaternion spawnRotation, NetworkConnectionToClient sender = null) {
        if (!NetworkDataBase.data[sender].IsAlive)
            return;

        GameObject kunai = Instantiate (kunaiPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn (kunai);
        kunai.GetComponent<kunai> ().SetOwner (NetworkDataBase.data[sender].nickname);

        NetworkDataBase.data[sender].kunaiCount--;
        sender.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData (NetworkDataBase.data[sender]);

    }
}
