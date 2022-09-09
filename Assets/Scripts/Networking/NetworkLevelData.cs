using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkLevelData : MonoBehaviour
{
    public Transform SpawnPoint;

    public Camera Cam, OverflowCam;
    public Image ThrowableImage;
    public TextMeshProUGUI ThrowableCounter;
    public TextMeshProUGUI TechnicsTimer;
    public UIPositionEffector technicsTimerEffector;
    public Image[] signsIcons;
    public UIPositionEffector[] signsEffectors;

    public Transform ParticlesSpawnPoint;
    public Transform CamHolder;

    public MoveCamera CameraMovement;

    public static NetworkLevelData singleton;

    private void Awake () {
        singleton = this;
    }

}
