using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RematchCounter : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI textComponent;
    [SyncVar(hook = nameof(OnTextChanged))] 
    public string text;

    private void OnTextChanged (string oldText, string newText) {
        textComponent.text = newText;
    }
}
