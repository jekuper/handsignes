using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CloneType {
    Player,         //can do all
    Simple,         //can not do technics, mana - 100
    Advanced,       //can do technics, mana - 150
    Shadow,         //can do technics, mana - initMana / 2
}
public static class ClonesManager {

    public static Dictionary<int, PlayerData> clones = new Dictionary<int, PlayerData>();
    public static int activeIndex = 0;

    private static int nextCloneIndex = 0;

    public static void AddBasePlayer (GameObject obj) {
        PlayerData dt = new PlayerData ();
        dt.instance = obj;
        clones.Add (nextCloneIndex, dt);
        nextCloneIndex++;
    }
    public static void AddClone (GameObject obj, CloneType cloneType) {
        obj.GetComponent<Clone> ().cloneIndex = nextCloneIndex;
        clones.Add (nextCloneIndex, new PlayerData (obj, cloneType, clones[activeIndex], clones.Count));
        nextCloneIndex++;
    }
    public static void SwitchToClone (GameObject obj) {
        SwitchToClone (obj.GetComponent<Clone>().cloneIndex);
    }
    public static void SwitchToClone (int index) {
        if (!clones.ContainsKey (index)) {
            Debug.Log ("switch to wrond index");
        }
        GameObject oldObj = clones[activeIndex].instance;

        Vector3 pos = oldObj.transform.position;
        oldObj.transform.position = clones[index].instance.transform.position;
        clones[index].instance.transform.position = pos;

        Vector3 velocity = oldObj.GetComponent<Rigidbody>().velocity;
        oldObj.GetComponent<Rigidbody> ().velocity = clones[index].instance.GetComponent<Rigidbody> ().velocity;
        clones[index].instance.GetComponent<Rigidbody> ().velocity = velocity;

        Vector3 angVelocity = oldObj.GetComponent<Rigidbody> ().angularVelocity;
        oldObj.GetComponent<Rigidbody> ().angularVelocity = clones[index].instance.GetComponent<Rigidbody> ().angularVelocity;
        clones[index].instance.GetComponent<Rigidbody> ().angularVelocity = angVelocity;


        clones[activeIndex].instance.GetComponent<Clone> ().cloneIndex = index;
        clones[index].instance.GetComponent<Clone> ().cloneIndex = activeIndex;


        clones[activeIndex].instance = clones[index].instance;
        clones[index].instance = oldObj;

        activeIndex = index;
    }
    public static void DeleteClone (int index) {
        if (!clones.ContainsKey (index)) {
            Debug.LogError ("no key found");
        }
        Object.Destroy (clones[index].instance);
        clones.Remove (index);
    }
}
