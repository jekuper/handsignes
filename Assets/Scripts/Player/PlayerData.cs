using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum throwableObject {
    Kunai,
    Shuriken
}
public enum mouseState {
    Weapons,
    Technics
}

public class PlayerData
{
    public GameObject instance;

    public int cloneIndex = 0;
    public int parentCloneIndex = -1;

    public CloneType cloneType = CloneType.Player;

    public float healthMax = 100;
    public float health = 100;
    public float healthRecoverSpeed = 3;

    public float manaMax = 500;
    public float mana = 500;
    public float manaRecoverSpeed = 10;


    public int kunaiCountMax = 5;
    public int kunaiCount = 5;

    public int shurikenCountMax = 10;
    public int shurikenCount = 10;

    public throwableObject throwableInUse = throwableObject.Shuriken;
    public mouseState mouseState = mouseState.Weapons;

    public PlayerData () { }
    public PlayerData (GameObject obj, CloneType _cloneType, PlayerData parentData, int _cloneIndex) {
        instance = obj;
        cloneIndex = _cloneIndex;
        cloneType = _cloneType;
        parentCloneIndex = parentData.cloneIndex;

        kunaiCountMax = parentData.kunaiCountMax;
        kunaiCount = parentData.kunaiCount;

        shurikenCountMax = parentData.shurikenCountMax;
        shurikenCount = parentData.shurikenCount;

        throwableInUse = parentData.throwableInUse;
        mouseState = parentData.mouseState;

        if (_cloneType == CloneType.Simple) {
            healthMax = 30;
            manaMax = 100;
            manaRecoverSpeed = 5;
        }
        if (_cloneType == CloneType.Advanced) {
            healthMax = 50;
            manaMax = 150;
            manaRecoverSpeed = 7;
        }
        if (_cloneType == CloneType.Shadow) {
            healthMax = 70;
            manaMax = Mathf.Floor(parentData.mana / 2);
            manaRecoverSpeed = 10;
        }
        mana = manaMax;
        health = healthMax;
    }
}
