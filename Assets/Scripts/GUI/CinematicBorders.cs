using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicBorders : UIPositionEffector
{
    public void MoveToCenter()
    {
        Debug.Log(Screen.width + " " + Screen.height);
        SetTarget(new Vector2(0, Screen.height / 2));
    }
}
