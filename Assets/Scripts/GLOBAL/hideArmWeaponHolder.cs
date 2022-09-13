using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideArmWeaponHolder : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    public void HideWeapon () {
        Debug.Log ("weapon hidden");
        weaponHolder.SetActive (false);
    }
    public void ShowWeapon () {
        weaponHolder.SetActive (true);
    }
}
