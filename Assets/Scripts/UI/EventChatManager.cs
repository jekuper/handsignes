using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventChatManager : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI textComponent;
    [SerializeField] private TextMeshProUGUI smallTextComponent;

    private Coroutine smallTextAnimation = null;

    private void Update () {
        if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
            return;
        if (Input.GetKeyDown (KeyCode.Return)) {
            if (textComponent.alpha > 0.5) {
                textComponent.alpha = 0;
                smallTextComponent.gameObject.SetActive (true);
            } else {
                textComponent.alpha = 1;
                if (smallTextAnimation != null)
                    StopCoroutine (smallTextAnimation);
                smallTextComponent.alpha = 0;
                smallTextComponent.gameObject.SetActive(false);
            }
        }
    }
    public void DisplayMessage (string message) {
        textComponent.text = message + "\n" + textComponent.text;
        smallTextComponent.text = message + "\nPress ENTER to open chat";
        if (smallTextAnimation != null)
            StopCoroutine (smallTextAnimation);
        smallTextAnimation = StartCoroutine (animate());
    }
    private IEnumerator animate () {
        float timer = 4f;
        while(smallTextComponent.alpha < 0.9) {
            smallTextComponent.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame ();
        }
        smallTextComponent.alpha = 1;

        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame ();
        }

        while (smallTextComponent.alpha > 0.1) {
            smallTextComponent.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame ();
        }
        smallTextComponent.alpha = 0;
    }
}
