using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoadingMessageConfig
{
    public enum LoadingMessageType
    {
        Loading,
        Message
    }
    public LoadingMessageType type;

    public string tag;
    public string message = "emty message";
    public Color color = Color.white;
    public Action callback;

    public LoadingMessageConfig(string _tag, LoadingMessageType _type, string _message, Color? _color, Action _callback = null)
    {
        tag = _tag;
        type = _type;
        message = _message;
        color = _color ?? Color.white;

        callback = _callback;
    }
}
public class LoadingWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] GameObject gif;
    [SerializeField] GameObject okButton;

    [SerializeField] GameObject mainObject;

    private List<LoadingMessageConfig> messages = new List<LoadingMessageConfig>();

    public void ShowLoading(string tag, string message, Color? textColor = null)
    {
        messages.Add(new LoadingMessageConfig(tag, LoadingMessageConfig.LoadingMessageType.Loading, message, textColor));
    }
    public void ShowMessage(string tag, string message, Color? textColor = null, Action callback = null)
    {
        messages.Add(new LoadingMessageConfig(tag, LoadingMessageConfig.LoadingMessageType.Message, message, textColor, callback));
    }
    public void HideLoading(string tag)
    {
        int i;
        for (i = 0; i < messages.Count; i++)
        {
            if (messages[i].tag == tag)
            {
                break;
            }
        }
        if (i == messages.Count)
        {
            Debug.LogWarning("loading tag not found. Tag:" + tag);
            return;
        }
        messages.RemoveAt(i);
    }
    public void HideCurrent()
    {
        if (messages.Count == 0)
        {
            Debug.LogWarning ("no messages to remove.");
            return;
        }
        messages[messages.Count - 1].callback?.Invoke();
        messages.RemoveAt(messages.Count - 1);
    }
    private void Update()
    {
        if (messages.Count == 0)
        {
            mainObject.SetActive(false);
        }
        else
        {
            LoadingMessageConfig config = messages[messages.Count - 1];

            mainObject.SetActive(true);
            gif.SetActive(config.type == LoadingMessageConfig.LoadingMessageType.Loading);
            okButton.SetActive(config.type == LoadingMessageConfig.LoadingMessageType.Message);

            messageText.text = config.message;
            messageText.color = config.color;
        }
    }
}
