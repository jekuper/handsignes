using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cinemachine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager instance;

    [SyncVar] public PlayerManager localPlayer;
    [SyncVar] public List<PlayerManager> players = new List<PlayerManager>();

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;


    private void Awake() {
        MakeInstance();
    }
    private void MakeInstance() {
        if (instance == null)
            instance = this;
    }

    public void ServerSpawnPlayer(NetworkConnection conn, int spawnIndex) {
        GameObject obj = Instantiate(playerPrefab, spawnPoints[spawnIndex % spawnPoints.Count].position, Quaternion.identity);
        obj.name = "player [" + conn.ToString() + "]";
        NetworkServer.Spawn(obj, conn);

        PlayerManager pm = obj.GetComponent<PlayerManager>();
        players.Add(pm);

        pm.playerNumber = conn.identity.GetComponent<GamePlayer>().playerNumber;

        TargetUpdateLocalPlayer(conn, pm);
    }

    [TargetRpc]
    public void TargetUpdateLocalPlayer(NetworkConnection targetConnection, PlayerManager obj) {
        localPlayer = obj;

        Transform cameraPos = obj.GetComponent<PlayerManager>().CameraPosition;

        virtualCamera.transform.SetParent(cameraPos);
        virtualCamera.transform.localPosition = Vector3.zero;
        virtualCamera.transform.localRotation = Quaternion.identity;

        virtualCamera.Follow = cameraPos;
    }

}
