using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class technicsManager : NetworkBehaviour {
    public static readonly int interactableMask = ((1 << 8) | (1 << 7));
    public static readonly int cloneMask = (1 << 8);


    public float timerInitValue = .5f;

    [SerializeField]private TextMeshProUGUI technicsTimer;
    [SerializeField]private UIPositionEffector technicsTimerEffector;
    [SerializeField]private Sprite[] iconSprites;
    [SerializeField]private Image[] signsIcons;
    [SerializeField]private UIPositionEffector[] signsEffectors;
    [SerializeField] private Transform orientation;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator armAnim;

    [SerializeField] private GameObject firePariticle, waterParticle, earthWall, earthPrison, clonePrefab;

    private Rigidbody rb;
    private Dictionary<string, Technic> technics = new Dictionary<string, Technic> ();
    [SyncVar]
    private float timer = -1;
    private string buffer = "";
    private bool isOff = false;

    public NetworkIdentity idenity;


    public void TurnOff () {
        buffer = "";
        HideSingsIcons ();
        HideTimer ();
        timer = 0.001f; 
        UpdateIcons ();
        armAnim.SetInteger ("signType", -1);
        isOff = true;
    }
    public void TurnOn () {
        buffer = "";
        ShowTimer ();
        timer = 0.001f;
        UpdateIcons ();
        armAnim.SetInteger ("signType", -1);
        isOff = false;
    }
    private void HideSingsIcons () {
        for (int i = 0; i < signsEffectors.Length; i++)
            HideSignIcon (i);
    }
    private void HideTimer () {
        technicsTimerEffector.SetFromIndex (0);
    }
    private void HideSignIcon (int index) {
        signsEffectors[index].SetFromIndex (0);
    }
    private void ShowTimer () {
        technicsTimerEffector.SetFromIndex (1);
    }
    private void ShowSignIcon (int index) {
        signsEffectors[index].SetFromIndex (1);
    }


    private void Start () {
        technicsTimer = NetworkLevelData.singleton.TechnicsTimer;
        technicsTimerEffector = NetworkLevelData.singleton.technicsTimerEffector;
        signsIcons = NetworkLevelData.singleton.signsIcons;
        signsEffectors = NetworkLevelData.singleton.signsEffectors;
        mainCamera = NetworkLevelData.singleton.Cam;

        rb = GetComponent<Rigidbody> ();
        idenity = GetComponent<NetworkIdentity> ();

        AddTechnic (BlowFireParticle, "01210", 200);
        AddTechnic (BlowWaterParticle, "12010", 200);
        AddTechnic (EarthWall, "0210", 60);
        AddTechnic (EarthPrison, "01012");


        //        timer = timerInitValue;
        technicsTimer.text = timerInitValue.ToString ("0.0");
        UpdateIcons ();
    }

    private void AddTechnic (Func<NetworkConnectionToClient, technicExecutionResult> act, string tag, int manaCost) {
        technics.Add (tag, new Technic (act, tag, manaCost));
    }
    private void AddTechnic (Func<NetworkConnectionToClient, technicExecutionResult> act, string tag) {
        technics.Add (tag, new Technic (act, tag));
    }

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }
        if (isOff == false && hasAuthority) {
            if (timer > 0) {
                timer -= Time.deltaTime;
                technicsTimer.text = timer.ToString("0.0");
                if (timer <= 0) {
                    technicsTimer.text = timerInitValue.ToString ("0.0");
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

                UpdateIcons ();
            }
            if (Input.GetMouseButtonDown (1) && buffer.Length < 6) {
                buffer += "1";
                timer = timerInitValue;
                armAnim.SetInteger ("signType", 1);
                UpdateIcons ();
            }
            if (Input.GetMouseButtonDown (2) && buffer.Length < 6) {
                buffer += "2";
                timer = timerInitValue;
                armAnim.SetInteger ("signType", 2);
                UpdateIcons ();
            }
        }
    }

    private void UpdateIcons () {
        for (int i = 0; i < buffer.Length; i++) {
            signsIcons[i].sprite = iconSprites[buffer[i] - '0'];
            ShowSignIcon (i);
        }
        for (int i = buffer.Length; i < signsIcons.Length; i++) {
            HideSignIcon (i);
        }
    }


    private void SearchAndExecute (){
        if (isOff)
            return;

//        if (ClonesManager.clones[ClonesManager.activeIndex].cloneType == CloneType.Simple && buffer != "0") {
//            return;
//        }

//        Debug.Log (buffer);
        if (!technics.ContainsKey (buffer)) {
            return;
        }
        CmdExecute (buffer);
        buffer = "";
    }
    [Command]
    public void CmdExecute (string technicTag) {
        technics[technicTag].Execute (idenity.connectionToClient);
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
    
    [Server]
    public technicExecutionResult BlowFireParticle (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject firePariticleInst = Instantiate (firePariticle);
        NetworkServer.Spawn (firePariticleInst, GetComponent<NetworkIdentity>().connectionToClient);
//        firePariticleInst.GetComponent<ParticlesSync> ().target = particlesSpawnPoint;
        return responce;
    }
    [Server]
    public technicExecutionResult BlowWaterParticle (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject waterPariticleInst = Instantiate (waterParticle);
        NetworkServer.Spawn (waterPariticleInst, GetComponent<NetworkIdentity> ().connectionToClient);
//        waterPariticleInst.GetComponent<ParticlesSync> ().target = particlesSpawnPoint;
        return responce;
    }
    [Server]
    public technicExecutionResult EarthWall (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        Vector3 pos = transform.position + orientation.forward * 2;
        pos.y = transform.position.y - 1f;

        GameObject wall = Instantiate (earthWall, pos, orientation.rotation);

        NetworkServer.Spawn (wall);
        return responce;
    }
    [Server]
    public technicExecutionResult EarthPrison (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject hover = GetHoverObject (interactableMask, 7);
        if (hover == null) {
            responce.isExecutedOK = false;
            return responce;
        }
        Vector3 RefSize = hover.GetComponentInChildren<Renderer>().bounds.size;
        RefSize += new Vector3(0.5f, 0.5f, 0.5f);

        float estimatedManaCost = 6.4f * RefSize.x * RefSize.y * RefSize.z;
        if (estimatedManaCost > NetworkDataBase.data[connection].mana) {
            responce.isExecutedOK = false;
            return responce;
        }
        responce.manaCost = estimatedManaCost;

        Transform prison = Instantiate(earthPrison, hover.transform.position, Quaternion.identity).transform;

        var Parent = hover.transform.parent;
        prison.transform.localScale = Parent ? new Vector3 (RefSize.x / Parent.lossyScale.x, RefSize.y / Parent.lossyScale.y, RefSize.z / Parent.lossyScale.z) : RefSize;

        NetworkServer.Spawn (prison.gameObject, GetComponent<NetworkIdentity> ().connectionToClient);

        return responce;
    }
    
    #endregion
}

public class Technic {
    public Func<NetworkConnectionToClient, technicExecutionResult> start = null;
    public bool isCalculatedManaCost = false;
    public string tag = "-";
    public int manaCost = 0;

    public Technic (Func<NetworkConnectionToClient, technicExecutionResult> resp, string _tag = "-", int _manaCost = 0) {
        start = resp;
        manaCost = _manaCost;
        tag = _tag;
    }
    public Technic (Func<NetworkConnectionToClient, technicExecutionResult> resp, string _tag = "-") {
        start = resp;
        isCalculatedManaCost = true;
        tag = _tag;
    }
    [Server]
    public void Execute (NetworkConnectionToClient connection) {
        if (!isCalculatedManaCost && NetworkDataBase.data[connection].mana < manaCost)
            return;
        technicExecutionResult resp = start (connection);
        if(resp.isExecutedOK) {
            if (!isCalculatedManaCost)
                NetworkDataBase.data[connection].mana -= manaCost;
            else
                NetworkDataBase.data[connection].mana -= resp.manaCost;
        }
        connection.identity.GetComponent<NetworkPlayerManager> ().TargetUpdateProfileData (NetworkDataBase.data[connection]);
    }
}

public class technicExecutionResult {
    public bool isExecutedOK = true;
    public float manaCost = 0;
}