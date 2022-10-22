using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public bool isOn = true;
    [SerializeField] GameObject checkmark;

    private void Start()
    {
        UpdateCheckmark();
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateCheckmark();
    }

    private void UpdateCheckmark()
    {
        if (isOn)
        {
            checkmark.SetActive(true);
        }
        else
        {
            checkmark.SetActive(false);
        }
    }
}
