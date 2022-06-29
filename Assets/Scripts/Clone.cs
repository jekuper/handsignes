using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Clone : MonoBehaviour {
    public int cloneIndex = 0;
    private void Awake () {
        if (cloneIndex == 0) {
            ClonesManager.AddBasePlayer (gameObject);
        }
    }
}
