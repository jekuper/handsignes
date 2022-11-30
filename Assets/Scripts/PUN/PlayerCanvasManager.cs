using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCanvasManager : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] TextMeshProUGUI nicknameText;
    [SerializeField] RectTransform healthBarForeground;
    [SerializeField] RectTransform healthBarBackground;
    private void Update() {
        if (controller.manager != null)
            nicknameText.text = controller.manager.localNickname;
        UpdateHealthBar ();
    }
    public void UpdateHealthBar()
    {
        if (controller.manager == null ||
            controller.manager.playerProfile == null)
            return;
        healthBarForeground.sizeDelta = new Vector2((healthBarBackground.rect.width) * (controller.manager.playerProfile.health / controller.manager.playerProfile.maxHealth), healthBarForeground.sizeDelta.y);
    }
}
