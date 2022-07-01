using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class technicsManager : MonoBehaviour {
    public static readonly int interactableMask = ((1 << 8) | (1 << 7));
    public static readonly int cloneMask = (1 << 8);


    public float timerInitValue = .5f;
    public float shiftForce = 5;

    [SerializeField]private TextMeshProUGUI technicsTimer;
    [SerializeField]private Sprite[] iconSprites;
    [SerializeField]private Image[] icons;
    [SerializeField] private Transform particlesSpawnPoint;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform camHolder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator armAnim;

    [SerializeField] private GameObject firePariticle, waterParticle, earthWall, earthPrison, clonePrefab;

    private Rigidbody rb;
    private Dictionary<string, Technic> technics = new Dictionary<string, Technic> ();
    private float timer = -1;
    private string buffer = "";
    private bool isOff = false;


    public void TurnOff () {
        buffer = "";
        technicsTimer.color = new Color (technicsTimer.color.r, technicsTimer.color.g, technicsTimer.color.b, 0);
        timer = 0.001f; 
        UpdateIcons ();
        armAnim.SetInteger ("signType", -1);
        isOff = true;
    }
    public void TurnOn () {
        buffer = "";
        technicsTimer.color = new Color (technicsTimer.color.r, technicsTimer.color.g, technicsTimer.color.b, 1);
        timer = 0.001f;
        UpdateIcons ();
        armAnim.SetInteger ("signType", -1);
        isOff = false;
    }
    private void Start () {
        rb = GetComponent<Rigidbody> ();


        AddTechnic (BlowFireParticle, "01210", 200);
        AddTechnic (BlowWaterParticle, "12010", 200);
        AddTechnic (EarthWall, "0210", 60);
        AddTechnic (EarthPrison, "01012");
        
        AddTechnic (CreateSimpleClone, "120", 100);
        AddTechnic (CreateAdvancedClone, "2102", 150);
        AddTechnic (CreateShadowClone, "102101");

        AddTechnic (SwitchToClone, "0", 0);


        //        timer = timerInitValue;
        technicsTimer.text = timerInitValue.ToString ("0.0") + "s";
        UpdateIcons ();
    }
    private void AddTechnic (Func<technicResponce> act, string tag, int manaCost) {
        technics.Add (tag, new Technic (act, tag, manaCost));
    }
    private void AddTechnic (Func<technicResponce> act, string tag) {
        technics.Add (tag, new Technic (act, tag));
    }

    private void Update () {
        if (isOff)
            return;

        if (timer > 0) {
            timer -= Time.deltaTime;
            technicsTimer.text = timer.ToString("0.0") + "s";
            if (timer <= 0) {
                technicsTimer.text = timerInitValue.ToString ("0.0") + "s";
                SearchAndExecute ();
                buffer = "";
                armAnim.SetInteger ("signType", -1);
                UpdateIcons ();
            }
        }

        if (Input.GetMouseButtonDown (0) && buffer.Length < 6) {
            buffer += "0";
            timer = timerInitValue;
            armAnim.SetInteger ("signType", 0);
            armAnim.SetTrigger ("signChanged");

            UpdateIcons ();
        }
        if (Input.GetMouseButtonDown (1) && buffer.Length < 6) {
            buffer += "1";
            timer = timerInitValue;
            armAnim.SetInteger ("signType", 1);
            armAnim.SetTrigger ("signChanged");
            UpdateIcons ();
        }
        if (Input.GetMouseButtonDown (2) && buffer.Length < 6) {
            buffer += "2";
            timer = timerInitValue;
            armAnim.SetInteger ("signType", 2);
            armAnim.SetTrigger ("signChanged");
            UpdateIcons ();
        }

    }

    private void UpdateIcons () {
        for (int i = 0; i < buffer.Length; i++) {
            icons[i].sprite = iconSprites[buffer[i] - '0'];
            icons[i].color = Color.white;
        }
        for (int i = buffer.Length; i < icons.Length; i++) {
            icons[i].color = new Color (0, 0, 0, 0);
        }
    }


    private void SearchAndExecute (){
        if (isOff)
            return;

        if (ClonesManager.clones[ClonesManager.activeIndex].cloneType == CloneType.Simple && buffer != "0") {
            return;
        }

//        Debug.Log (buffer);
        if (!technics.ContainsKey (buffer)) {
            return;
        }
        technics[buffer].Execute();
        buffer = "";
    }
    private GameObject GetHoverObject (int layerMask = ~0, float maxDistance = 10000) {
        Ray ray = mainCamera.ScreenPointToRay (new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if(Physics.Raycast (ray, out hit, maxDistance)) {
//                        Debug.DrawLine (ray.origin, hit.point, Color.red, 1);
            if (((1<<hit.collider.gameObject.layer) & layerMask) != 0)
                return hit.transform.gameObject;
        }
        return null;
    }

    #region TECHNICS

    public technicResponce BlowFireParticle () {
        technicResponce responce = new technicResponce ();

        Instantiate (firePariticle, particlesSpawnPoint);
        return responce;
    }
    public technicResponce BlowWaterParticle () {
        technicResponce responce = new technicResponce ();

        Instantiate (waterParticle, particlesSpawnPoint);
        return responce;
    }
    public technicResponce EarthWall () {
        technicResponce responce = new technicResponce ();

        Vector3 pos = transform.position + orientation.forward * 2;
        pos.y = transform.position.y - 1f;
        Instantiate (earthWall, pos, orientation.rotation);
        return responce;
    }
    public technicResponce EarthPrison () {
        technicResponce responce = new technicResponce ();

        GameObject hover = GetHoverObject (interactableMask, 7);
        if (hover == null) {
            responce.isExecutedOK = false;
            return responce;
        }
        Vector3 RefSize = hover.GetComponent<MeshRenderer>().bounds.size;
        RefSize += new Vector3(0.5f, 0.5f, 0.5f);

        float estimatedManaCost = 6.4f * RefSize.x * RefSize.y * RefSize.z;
        if (estimatedManaCost > ClonesManager.clones[ClonesManager.activeIndex].mana) {
            responce.isExecutedOK = false;
            return responce;
        }
        responce.manaCost = estimatedManaCost;

        Transform prison = Instantiate(earthPrison, hover.transform.position, Quaternion.identity).transform;

        var Parent = hover.transform.parent;
        prison.transform.localScale = Parent ? new Vector3 (RefSize.x / Parent.lossyScale.x, RefSize.y / Parent.lossyScale.y, RefSize.z / Parent.lossyScale.z) : RefSize;

        return responce;
    }
    public technicResponce CreateSimpleClone () {
        technicResponce responce = new technicResponce ();

        GameObject clone = Instantiate (clonePrefab, camHolder.position + camHolder.forward * 3, Quaternion.identity);
        ClonesManager.AddClone (clone, CloneType.Simple);
        return responce;
    }
    public technicResponce CreateAdvancedClone () {
        technicResponce responce = new technicResponce ();

        GameObject clone = Instantiate (clonePrefab, camHolder.position + camHolder.forward * 3, Quaternion.identity);
        ClonesManager.AddClone (clone, CloneType.Advanced);
        return responce;
    }
    public technicResponce CreateShadowClone () {
        technicResponce responce = new technicResponce ();

        if (ClonesManager.clones[ClonesManager.activeIndex].mana / 2 < 2) {
            responce.isExecutedOK = false;
            return responce;
        }

        responce.manaCost = ClonesManager.clones[ClonesManager.activeIndex].mana / 2;
        GameObject clone = Instantiate (clonePrefab, camHolder.position + camHolder.forward * 3, Quaternion.identity);
        ClonesManager.AddClone (clone, CloneType.Shadow);
        return responce;
    }
    public technicResponce SwitchToClone () {
        technicResponce responce = new technicResponce ();

        GameObject hover = GetHoverObject (cloneMask, 10);
        if (hover == null) {
            responce.isExecutedOK = false;
            return responce;
        }

        ClonesManager.SwitchToClone (hover);

        return responce;
    }
    
    #endregion
}

public class Technic {
    public Func<technicResponce> start = null;
    public bool isCalculatedManaCost = false;
    public string tag = "-";
    public int manaCost = 0;

    public Technic (Func<technicResponce> resp, string _tag = "-", int _manaCost = 0) {
        start = resp;
        manaCost = _manaCost;
        tag = _tag;
    }
    public Technic (Func<technicResponce> resp, string _tag = "-") {
        start = resp;
        isCalculatedManaCost = true;
        tag = _tag;
    }
    public void Execute () {
        if (!isCalculatedManaCost && ClonesManager.clones[ClonesManager.activeIndex].mana < manaCost)
            return;
        technicResponce resp = start ();
        if(resp.isExecutedOK) {
            if (!isCalculatedManaCost)
                ClonesManager.clones[ClonesManager.activeIndex].mana -= manaCost;
            else
                ClonesManager.clones[ClonesManager.activeIndex].mana -= resp.manaCost;
        }
    }
}

public class technicResponce {
    public bool isExecutedOK = true;
    public float manaCost = 0;
}