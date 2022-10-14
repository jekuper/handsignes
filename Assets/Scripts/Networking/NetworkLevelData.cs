using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class NetworkLevelData : MonoBehaviour
{
    public Transform SpawnPoint;

    public Camera Cam, OverflowCam;
    public PostProcessVolume pp;
    public Image ThrowableImage;
    public TextMeshProUGUI ThrowableCounter;
    public TextMeshProUGUI TechnicsTimer;
    public UIPositionEffector technicsTimerEffector;
    public Image[] signsIcons;
    public UIPositionEffector[] signsEffectors;
    public UIPositionEffector[] bodyStatesEffectors;
    public Image[] bodyStatesImages;

    public Transform ParticlesSpawnPoint;
    public Transform CamHolder;

    public MoveCamera CameraMovement;

    public static NetworkLevelData singleton;

    private void Awake () {
        singleton = this;
    }

}
