using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class technicsManager : MonoBehaviour {
    public static readonly int interactableMask = ((1 << 7));

    public float timerInitValue = .5f;

    private TextMeshProUGUI technicsTimer {get { return NetworkLevelData.singleton.TechnicsTimer; } }
    private UIPositionEffector technicsTimerEffector { get { return NetworkLevelData.singleton.technicsTimerEffector; } }
    private Image[] signsIcons { get { return NetworkLevelData.singleton.signsIcons; } }
    private Transform particlesSpawnPoint { get { return NetworkLevelData.singleton.ParticlesSpawnPoint; } }
    private UIPositionEffector[] signsEffectors { get { return NetworkLevelData.singleton.signsEffectors; } }

    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private Transform orientation;
    [SerializeField] private Animator armAnim;

    private Dictionary<string, Technic> technics = new Dictionary<string, Technic> ();

    private float timer = -1;
    public bool isRegeningMana = false;

    private string buffer = "";
    private bool isOff = false;

    public PhotonView PV;


    public void TurnOff () {
        SearchAndExecute ();
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
        if (NetworkDataBase.LocalInternalUserData.mouseState == mouseState.Technics) {
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

        PV = GetComponent<PhotonView> ();

        if (PV.AmOwner) {
            NetworkDataBase.technicDescription.Clear ();
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

        if (PV.AmOwner) {
            //        timer = timerInitValue;
            technicsTimer.text = timerInitValue.ToString ("0.0");
            UpdateIcons ();
        }
    }
    private void AddTechnic (Func<technicExecutionResult> act, string tag, int manaCost, string description, string name) {
        technics.Add (tag, new Technic (act, tag, manaCost, description, name));
        if (PV.AmOwner) {
            NetworkDataBase.technicDescription.Add (tag, new TechnicDescription (tag, name, description, false, manaCost));
        }
    }
    private void AddTechnic (Func<technicExecutionResult> act, string tag, string description, string name) {
        technics.Add (tag, new Technic (act, tag, description, name));
        if (PV.AmOwner) {
            NetworkDataBase.technicDescription.Add (tag, new TechnicDescription (tag, name, description, true, -9999));
        }
    }

    private void Update () {
        if (isOff == false && PV.AmOwner) {
            if (timer > 0) {
                timer -= Time.deltaTime;
                technicsTimer.text = timer.ToString ("0.0");
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

    private void SearchAndExecute () {
        if (isOff)
            return;

        //        if (ClonesManager.clones[ClonesManager.activeIndex].cloneType == CloneType.Simple && buffer != "0") {
        //            return;
        //        }

        //        Debug.Log (buffer);
        if (!NetworkDataBase.technicDescription.ContainsKey (buffer)) {
            return;
        }
        technics[buffer].Execute ();
        buffer = "";
    }

    private GameObject GetHoverObject (int layerMask = ~0, float maxDistance = 10000) {
        Transform cam = NetworkDataBase.localPlayerManager.controller.cameraPosition;
        Ray ray = new Ray (cam.position, cam.forward);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, maxDistance)) {
            if (((1 << hit.collider.gameObject.layer) & layerMask) != 0)
                return hit.transform.gameObject;
        }
        return null;
    }

    #region TECHNICS

    public technicExecutionResult BlowFireParticle () {
        technicExecutionResult responce = new technicExecutionResult ();
        if (!PV.AmOwner) {
            responce.isExecutedOK = false;
            return responce;
        }

        GameObject firePariticleInst = PhotonNetwork.Instantiate ("technics/firePariticle", particlesSpawnPoint.position, particlesSpawnPoint.rotation);
        return responce;
    }
    public technicExecutionResult ToogleManaRegen () {
        technicExecutionResult responce = new technicExecutionResult ();

        if (isRegeningMana)
            isRegeningMana = false;
        else
            isRegeningMana = true;
        return responce;
    }
    public technicExecutionResult BlowWaterParticle () {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject waterPariticleInst = PhotonNetwork.Instantiate ("technics/waterParticle", particlesSpawnPoint.position, particlesSpawnPoint.rotation);
        return responce;
    }
    public technicExecutionResult EarthWall () {
        technicExecutionResult responce = new technicExecutionResult ();

        Vector3 pos = transform.position + orientation.forward * 2;
        pos.y = transform.position.y - 1f;

        GameObject wall = PhotonNetwork.Instantiate ("technics/earthWall", pos, orientation.rotation);

        return responce;
    }
    public technicExecutionResult LavaFloor () {
        technicExecutionResult responce = new technicExecutionResult ();

        Vector3 pos = transform.position;
        pos.y = transform.position.y - 1f;

        GameObject lavaFloorInst = PhotonNetwork.Instantiate ("technics/lavaFloor", pos, Quaternion.identity);

        //TODO: change lava floor logic
        //lavaFloorInst.GetComponent<LavaFloor> ().teamIndex = NetworkDataBase.data[connection].teamIndex;
        return responce;
    }
    public technicExecutionResult EarthPrison () {
        technicExecutionResult responce = new technicExecutionResult ();

        GameObject hover = GetHoverObject (interactableMask, 12);
        if (hover == null) {
            responce.isExecutedOK = false;
            return responce;
        }
        Transform skin = hover.transform.Find ("Orientation/HumanFPS/cyborgMesh");
        if (skin == null) {
            Debug.LogError ("CRITICAL ERROR: player mesh not found!!");
        }
        Vector3 RefSize = skin.GetComponent<Renderer> ().bounds.size;
        RefSize += new Vector3 (1f, 1f, 1f);

        float estimatedManaCost = 7f * RefSize.x * RefSize.y * RefSize.z;
        if (estimatedManaCost > NetworkDataBase.localProfile.mana) {
            responce.isExecutedOK = false;
            return responce;
        }
        responce.manaCost = estimatedManaCost;

        Transform prison = PhotonNetwork.Instantiate ("technics/earthPrison", hover.transform.position, Quaternion.identity).transform;

        var Parent = hover.transform.parent;
        prison.transform.localScale = Parent ? new Vector3 (RefSize.x / Parent.lossyScale.x, RefSize.y / Parent.lossyScale.y, RefSize.z / Parent.lossyScale.z) : RefSize;

        return responce;
    }
    public technicExecutionResult SetKatanaWater () {
        technicExecutionResult responce = new technicExecutionResult ();
        NetworkDataBase.localProfile.katanaState = KatanaState.Water;
        NetworkDataBase.localProfile.UnSetBodyState (BodyState.OnFire);
        return responce;
    }
    public technicExecutionResult SetKatanaFire () {
        technicExecutionResult responce = new technicExecutionResult ();
        NetworkDataBase.localProfile.katanaState = KatanaState.Fire;
        NetworkDataBase.localProfile.UnSetBodyState (BodyState.Wet);
        return responce;
    }
    public technicExecutionResult SetKatanaElectro () {
        technicExecutionResult responce = new technicExecutionResult ();
        NetworkDataBase.localProfile.katanaState = KatanaState.Electro;
        NetworkDataBase.localProfile.UnSetBodyState (BodyState.Earth);
        return responce;
    }
    public technicExecutionResult SetKatanaEarth () {
        technicExecutionResult responce = new technicExecutionResult ();
        NetworkDataBase.localProfile.katanaState = KatanaState.Earth;
        NetworkDataBase.localProfile.UnSetBodyState (BodyState.ElectroShock);
        return responce;
    }
    #endregion


}

public class Technic {
    public Func<technicExecutionResult> start = null;
    public bool isCalculatedManaCost = false;
    public string tag = "-";
    public int manaCost = 0;
    public string description = "";
    public string name = "";

    public Technic (Func<technicExecutionResult> resp, string _tag = "-", int _manaCost = 0, string _description = "", string _name = "") {
        start = resp;
        manaCost = _manaCost;
        tag = _tag;
        description = _description;
        name = _name;
    }
    public Technic (Func<technicExecutionResult> resp, string _tag = "-", string _description = "", string _name = "") {
        start = resp;
        isCalculatedManaCost = true;
        tag = _tag;
        description = _description;
        name = _name;
    }
    public void Execute () {
        string authorNickname = PhotonNetwork.LocalPlayer.NickName;
        if (!isCalculatedManaCost && NetworkDataBase.GetPlayerProfile(authorNickname).mana < manaCost)
            return;
        technicExecutionResult resp = start ();
        if (resp.isExecutedOK) {
            if (!isCalculatedManaCost)
                NetworkDataBase.GetPlayerProfile (authorNickname).TrySpendMana(manaCost);
            else
                NetworkDataBase.GetPlayerProfile (authorNickname).TrySpendMana(resp.manaCost);
        }
    }
}

public class technicExecutionResult {
    public bool isExecutedOK = true;
    public float manaCost = 0;
}