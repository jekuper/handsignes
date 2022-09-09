using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class katanaTrigger : MonoBehaviour
{
    [SerializeField] katanaManager link;
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private Transform bloodSpawnPoint;

    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            link.TriggerResponce (other);
            GameObject particle = Instantiate (bloodParticle, bloodSpawnPoint.position, Quaternion.identity);
        }
    }
}
