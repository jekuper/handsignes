using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Clone : MonoBehaviour {
    public int cloneIndex = -1;
    [SerializeField] private Material[] materials;
    [SerializeField] private MeshRenderer renderer;
    private void Start () {
        if (cloneIndex == 0) {
            ClonesManager.AddBasePlayer (gameObject);
        }
    }
    private void Update () {
        renderer.material = materials[(int)ClonesManager.clones[cloneIndex].cloneType];
    }
}
