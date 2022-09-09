using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOVchangeSource {
    WallRun,
    Dash,
}
public class FOVchangeCommand {
    public FOVchangeSource source;
    public float fov;
}

public class CameraFOVmanager : MonoBehaviour
{
    [SerializeField] private Camera[] affectedCameras;
    private List<FOVchangeCommand> commands = new List<FOVchangeCommand> ();

    public static CameraFOVmanager singleton;

    private void Start () {
        singleton = this;
    }

    private void Update () {
        float resultFOV = 90;

        commands.Sort ((x, y) => x.source.CompareTo (y.source));
        commands.Reverse ();

        if (commands.Count != 0) {
            resultFOV = commands[0].fov;
        }

        foreach (var item in affectedCameras) {
            item.fieldOfView = Mathf.Lerp (item.fieldOfView, resultFOV, 10f * Time.deltaTime);
        }

        commands.Clear ();
    }

    public void AddCommand (float fov, FOVchangeSource source) {
        commands.Add (new FOVchangeCommand { fov = fov, source = source });
    }
}
