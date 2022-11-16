using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasManager : MonoBehaviour
{
    [SerializeField] PlayerProfile data;
    [SerializeField] RectTransform healthBarForeground;
    [SerializeField] RectTransform healthBarBackground;

    private void Update()
    {
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        healthBarForeground.sizeDelta = new Vector2((healthBarBackground.rect.width) * (data.health / data.maxHealth), healthBarForeground.sizeDelta.y);
    }
}
