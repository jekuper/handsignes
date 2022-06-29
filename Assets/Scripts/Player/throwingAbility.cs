using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class throwingAbility : MonoBehaviour
{
    public float kunaiForceValue = 30f;
    public float shurikenForceValue = 40f;
    public float getDistance = 1f;

    [SerializeField] ThrowableUIs uiManager;

    [SerializeField] GameObject kunaiPrefab;
    [SerializeField] GameObject shurikenPrefab;
    [SerializeField] Transform spawnPoint;

    private List<kunai> throwedKunais = new List<kunai> ();


    private void Update () {

        searchForKunais ();

        if (Input.GetKeyUp (KeyCode.F)) {
            if (ClonesManager.clones[ClonesManager.activeIndex].throwableInUse == throwableObject.Kunai) {
                throwKunai ();
            }
            if (ClonesManager.clones[ClonesManager.activeIndex].throwableInUse == throwableObject.Shuriken) {
                throwShuriken ();
            }
        }
    }
    private void searchForKunais () {
        if (ClonesManager.clones[ClonesManager.activeIndex].kunaiCount == ClonesManager.clones[ClonesManager.activeIndex].kunaiCountMax)
            return;
        bool found = false;
        for(int i = 0; i < throwedKunais.Count; i++) {
            kunai kunai = throwedKunais[i];
            if (kunai.isStuck && Vector3.Distance (kunai.transform.position, transform.position) <= getDistance) {
                found = true;
                ClonesManager.clones[ClonesManager.activeIndex].kunaiCount++;
                Destroy (kunai.gameObject);
                throwedKunais.RemoveAt (i);
                i--;
            }
        }
        if (found)
            uiManager.UpdateCounter (ClonesManager.clones[ClonesManager.activeIndex].kunaiCount);
    }
    private void throwKunai () {
        if (ClonesManager.clones[ClonesManager.activeIndex].kunaiCount <= 0) {
            return;
        }

        GameObject kunai = Instantiate (kunaiPrefab, spawnPoint.position, Quaternion.LookRotation (spawnPoint.forward));
        throwedKunais.Add (kunai.GetComponent<kunai>());
        kunai.GetComponent<Rigidbody> ().AddRelativeForce (Vector3.forward * kunaiForceValue, ForceMode.Impulse);
        ClonesManager.clones[ClonesManager.activeIndex].kunaiCount--;

        uiManager.UpdateCounter (ClonesManager.clones[ClonesManager.activeIndex].kunaiCount);
    }
    private void throwShuriken () {
        if (ClonesManager.clones[ClonesManager.activeIndex].shurikenCount <= 0) {
            return;
        }

        GameObject shuriken = Instantiate (shurikenPrefab, spawnPoint.position, Quaternion.LookRotation (spawnPoint.forward));

        shuriken.GetComponent<Rigidbody> ().AddRelativeForce (Vector3.forward * shurikenForceValue, ForceMode.Impulse);
        shuriken.GetComponent<Rigidbody> ().AddRelativeTorque (Vector3.up * 4, ForceMode.Impulse);
        ClonesManager.clones[ClonesManager.activeIndex].shurikenCount--;

        uiManager.UpdateCounter (ClonesManager.clones[ClonesManager.activeIndex].shurikenCount);
    }
}
