using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class mapListItem : MonoBehaviour
{
    public string sceneName;
    public string mapName;
    [Multiline]
    public string mapStory;
    public TextMeshProUGUI text;
    public Sprite[] sprites;

    private void Start () {
        text.text = mapName;
    }
}
