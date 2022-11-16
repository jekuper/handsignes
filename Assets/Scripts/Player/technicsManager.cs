using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class technicsManager : NetworkBehaviour {
    public static readonly int interactableMask = ((1 << 7));


    public float timerInitValue = .5f;

    [SerializeField]private TextMeshProUGUI technicsTimer;
    [SerializeField]private UIPositionEffector technicsTimerEffector;
    [SerializeField]private Sprite[] iconSprites;
    [SerializeField]private Image[] signsIcons;
    [SerializeField]private UIPositionEffector[] signsEffectors;
    [SerializeField] private Transform orientation;
    [SerializeField] private Animator armAnim;
    [SerializeField] private GameObject firePariticle, waterParticle, earthWall, earthPrison, lavaFloor;

    private Dictionary<string, Technic> technics = new Dictionary<string, Technic>();
    
    [SyncVar]
    private List<ParticlesSync> stopAfterDeath = new List<ParticlesSync> ();
    [SyncVar]
    private float timer = -1;
    [SyncVar]
    public bool isRegeningMana = false;

    private string buffer = "";
    private bool isOff = false;

    public NetworkIdentity idenity;


    public void TurnOff () {
        SearchAndExecute();
        buffer = "";
        isOff = true;
        HideSingsIcons ();
        HideTimer ();
        timer = 0.001f; 
        UpdateIcons ();
        armAnim.SetInteger ("signType", -1);
    }
    public void TurnOn () {
        buffer = "";
        isOff = false;
        if (NetworkDataBase.LocalInternalUserData.mouseState == mouseState.Technics)
        {
            ShowTimer ();
            timer = 0.001f;
            UpdateIcons ();
        }
        armAnim.SetInteger ("signType", -1);
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

        idenity = GetComponent<NetworkIdentity> ();

        if (hasAuthority)
        {
            NetworkDataBase.technicDescription.Clear();
        }

        AddTechnic (BlowFireParticle, "01210", 150, "creates flow of fire. Each particle have 1 damage. 5 seconds long", "fire flow");
        AddTechnic (BlowWaterParticle, "12010", 150, "creates flow of water. Each particle have 0.5 damage. 5 seconds long", "water flow");
        AddTechnic (EarthWall, "0210", 60, "creates a wall in direction you are looking", "wall");
        AddTechnic (EarthPrison, "01012", "creates a box around certain player. Before and during using aim on your target player", "earth prison");
        AddTechnic (LavaFloor, "02012", 120, "replaces floor with lava under you", "Lava Floor");
        AddTechnic (ToogleManaRegen, "2", 0, "Toogles mana regenration. You can not move during mana regen", "regen mana");
        AddTechnic (SetKatanaWater, "00", 0, "sets your katana mode to water. If player's body has wet status, than player's view blurs and damage from electro katana is doubled. Dispells body's fire state", "katana water");
        AddTechnic (SetKatanaFire, "02", 0, "sets your katana mode to fire. If player's body has fire status, than player gets constant damage untill he dispells it. Dispells body's wet state", "katana fire");
        AddTechnic (SetKatanaElectro, "01", 0, "sets your katana mode to electro. If player's body has electro status and player doesn't dispell it during next 5 seconds, than he gets stunned for 3 seconds. Dispells body's earth state", "katana electro");
        AddTechnic (SetKatanaEarth, "000", 0, "sets your katana mode to earth. Move speed decreased by 20%. Dash turns off. Dispells body's electro state", "katana earth");
        
        if (hasAuthority) {
            //        timer = timerInitValue;
            technicsTimer.text = timerInitValue.ToString ("0.0");
            UpdateIcons ();
        }
    }
    private void AddTechnic (Func<NetworkConnectionToClient, technicExecutionResult> act, string tag, int manaCost, string description, string name) {
        technics.Add (tag, new Technic (act, tag, manaCost, description, name));
        if (hasAuthority)
        {
            NetworkDataBase.technicDescription.Add(tag, new TechnicDescription(tag, name, description, false, manaCost));
        }
    }
    private void AddTechnic (Func<NetworkConnectionToClient, technicExecutionResult> act, string tag, string description, string name) {
        technics.Add (tag, new Technic (act, tag, description, name));
        if (hasAuthority)
        {
            NetworkDataBase.technicDescription.Add(tag, new TechnicDescription(tag, name, description, true, -9999));
        }
    }

    private void Update () {
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

            if (Input.GetMouseButtonDown (0) && Cursor.lockState == CursorLockMode.Locked && buffer.Length < 6) {
                buffer += "0";
                timer = timerInitValue;
                armAnim.SetInteger ("signType", 0);

                UpdateIcons ();
            }
            if (Input.GetMouseButtonDown (1) && Cursor.lockState == CursorLockMode.Locked && buffer.Length < 6) {
                buffer += "1";
                timer = timerInitValue;
                armAnim.SetInteger ("signType", 1);
                UpdateIcons ();
            }
            if (Input.GetMouseButtonDown (2) && Cursor.lockState == CursorLockMode.Locked && buffer.Length < 6) {
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
        if (!NetworkDataBase.technicDescription.ContainsKey (buffer)) {
            return;
        }
        CmdExecute (buffer);
        buffer = "";
    }
    [Command]
    public void CmdExecute (string technicTag)
    {
        technics[technicTag].Execute (idenity.connectionToClient);
    }

    private GameObject GetHoverObject (NetworkConnectionToClient connection, int layerMask = ~0, float maxDistance = 10000) {
        //Transform cam = connection.identity.GetComponent<NetworkPlayerManager> ().gamePlayerManager.cameraPos;
        //Ray ray = new Ray(cam.position, cam.forward);
        //RaycastHit hit;
        //if (Physics.Raycast (ray, out hit, maxDistance)) {
        //    if (((1 << hit.collider.gameObject.layer) & layerMask) != 0)
        //        return hit.transform.gameObject;
        //}
        return null;
    }

    #region TECHNICS
    
    [Server]
    public technicExecutionResult BlowFireParticle (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject firePariticleInst = Instantiate (firePariticle);
        stopAfterDeath.Add (firePariticleInst.GetComponent<ParticlesSync> ());
        NetworkServer.Spawn (firePariticleInst, connection);
//        firePariticleInst.GetComponent<ParticlesSync> ().target = particlesSpawnPoint;
        return responce;
    }
    [Server]
    public technicExecutionResult ToogleManaRegen(NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();

        if (isRegeningMana)
            isRegeningMana = false;
        else
            isRegeningMana = true;
        return responce;
    }
    [Server]
    public technicExecutionResult BlowWaterParticle (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject waterPariticleInst = Instantiate (waterParticle);
        stopAfterDeath.Add (waterPariticleInst.GetComponent<ParticlesSync> ());
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
    public technicExecutionResult LavaFloor(NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();

        Vector3 pos = transform.position;
        pos.y = transform.position.y - 1f;

        GameObject lavaFloorInst = Instantiate(lavaFloor, pos, Quaternion.identity);

        NetworkServer.Spawn(lavaFloorInst);
        lavaFloorInst.GetComponent<LavaFloor>().teamIndex = NetworkDataBase.data[connection].teamIndex;
        return responce;
    }
    [Server]
    public technicExecutionResult EarthPrison (NetworkConnectionToClient connection) {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject hover = GetHoverObject (connection, interactableMask, 12);
        if (hover == null) {
            responce.isExecutedOK = false;
            return responce;
        }
        Transform skin = hover.transform.Find ("Orientation/HumanFPS/cyborgMesh");
        if (skin == null) {
            Debug.LogError ("CRITICAL ERROR: player mesh not found!!");
        }
        Vector3 RefSize = skin.GetComponent<Renderer> ().bounds.size;
        RefSize += new Vector3(1f, 1f, 1f);

        float estimatedManaCost = 7f * RefSize.x * RefSize.y * RefSize.z;
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
    [Server]
    public technicExecutionResult SetKatanaWater (NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();
        NetworkDataBase.data[connection].katanaState = KatanaState.Water;
        NetworkBRManager.brSingleton.UnSetBodyState(connection, BodyState.OnFire);
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.data[connection]);
        return responce;
    }
    [Server]
    public technicExecutionResult SetKatanaFire(NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();
        NetworkDataBase.data[connection].katanaState = KatanaState.Fire;
        NetworkBRManager.brSingleton.UnSetBodyState(connection, BodyState.Wet);
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.data[connection]);
        return responce;
    }
    [Server]
    public technicExecutionResult SetKatanaElectro(NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();
        NetworkDataBase.data[connection].katanaState = KatanaState.Electro;
        NetworkBRManager.brSingleton.UnSetBodyState(connection, BodyState.Earth);
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.data[connection]);
        return responce;
    }
    [Server]
    public technicExecutionResult SetKatanaEarth(NetworkConnectionToClient connection)
    {
        technicExecutionResult responce = new technicExecutionResult();
        NetworkDataBase.data[connection].katanaState = KatanaState.Earth;
        NetworkBRManager.brSingleton.UnSetBodyState(connection, BodyState.ElectroShock);
        connection.identity.GetComponent<NetworkPlayerManager>().TargetUpdateProfileData(NetworkDataBase.data[connection]);
        return responce;
    }
    #endregion

    //TODO: prevent array overflow with nulls
    [ServerCallback]
    private void OnDestroy () {
        foreach (var item in stopAfterDeath) {
            if (item != null)
                item.Stop ();
        }
    }
}

public class Technic {
    public Func<NetworkConnectionToClient, technicExecutionResult> start = null;
    public bool isCalculatedManaCost = false;
    public string tag = "-";
    public int manaCost = 0;
    public string description = "";
    public string name = "";

    public Technic (Func<NetworkConnectionToClient, technicExecutionResult> resp, string _tag = "-", int _manaCost = 0, string _description = "", string _name = "") {
        start = resp;
        manaCost = _manaCost;
        tag = _tag;
        description = _description;
        name = _name;
    }
    public Technic (Func<NetworkConnectionToClient, technicExecutionResult> resp, string _tag = "-", string _description = "", string _name = "") {
        start = resp;
        isCalculatedManaCost = true;
        tag = _tag;
        description = _description;
        name = _name;
    }
    [Server]
    public void Execute (NetworkConnectionToClient connection)
    {
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