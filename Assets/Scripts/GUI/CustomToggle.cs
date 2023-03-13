using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public bool isOn = true;
    public EventTrigger.TriggerEvent customCallback;
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
        customCallback.Invoke(new BaseEventData(EventSystem.current));
    }
}
