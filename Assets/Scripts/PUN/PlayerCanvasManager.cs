using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasManager : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] RectTransform healthBarForeground;
    [SerializeField] RectTransform healthBarBackground;

    private void Update()
    {
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        if (controller.manager == null ||
            controller.manager.playerProfile == null)
            return;
        healthBarForeground.sizeDelta = new Vector2((healthBarBackground.rect.width) * (controller.manager.playerProfile.health / controller.manager.playerProfile.maxHealth), healthBarForeground.sizeDelta.y);
    }
}
