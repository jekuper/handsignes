using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cinemachine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager instance;

    [SyncVar] public GameObject localPlayer;

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SyncVar] private List<GameObject> players = new List<GameObject>();

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
        players.Add(obj);

        obj.GetComponent<PlayerManager>().playerNumber = conn.identity.GetComponent<GamePlayer>().playerNumber;

        TargetUpdateLocalPlayer(conn, obj);
    }

    [TargetRpc]
    public void TargetUpdateLocalPlayer(NetworkConnection targetConnection, GameObject obj) {
        localPlayer = obj;

        virtualCamera.LookAt = obj.transform;
    }
}
