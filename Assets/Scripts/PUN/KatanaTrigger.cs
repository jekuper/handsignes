using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaTrigger : MonoBehaviour
{
    [SerializeField] KatanaManager link;

    private void OnTriggerEnter (Collider other) {
         link.TriggerResponce (other);
    }
}
