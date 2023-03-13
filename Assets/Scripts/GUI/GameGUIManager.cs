using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameGUIManager : MonoBehaviour
{
    public static GameGUIManager singleton;

    [Header("Throwable")]
    [SerializeField] TextMeshProUGUI ThrowableCounter;
    [SerializeField] Image ThrowableImage;
    [SerializeField] List<Sprite> ThrowableIcons;

    [Header ("Health")]
    [SerializeField] TextMeshProUGUI HealthCounter;
    [SerializeField] RectTransform healthBarForeground;
    [SerializeField] RectTransform healthBarBackground;

    [Header ("Mana")]
    [SerializeField] TextMeshProUGUI ManaCounter;
    [SerializeField] RectTransform manaBarForeground;
    [SerializeField] RectTransform manaBarBackground;

    private void Awake () {
        singleton = this;
    }
    private void Update () {
        UpdateHealthCounter ();
        UpdateManaCounter ();
        UpdateThrowable ();
    }

    #region Throwable
    public void UpdateThrowable () {
        UpdateThrowableIcon ();
        UpdateThrowableCount ();
    }
    public void UpdateThrowableIcon () {
        if (NetworkDataBase.localProfile == null)
            return;
        UpdateThrowableIcon(NetworkDataBase.localProfile.throwableInUse);
    }
    public void UpdateThrowableIcon(throwableType type) {
        ThrowableImage.sprite = ThrowableIcons[(int)type];
    }
    public void UpdateThrowableCount () {
        if (NetworkDataBase.localProfile != null && NetworkDataBase.localProfile.throwableInUse == throwableType.Kunai) {
            UpdateThrowableCount (NetworkDataBase.localProfile.kunai);
        }
    }
    public void UpdateThrowableCount (int count) {
        ThrowableCounter.text = count.ToString ();
        if (count == 0) {
            ThrowableImage.color = new Color (1, 1, 1, .5f);
        } else {
            ThrowableImage.color = new Color (1, 1, 1, 1f);
        }
    }
    #endregion
    #region HealthGUI
    public void UpdateHealthCounter () {
        if (NetworkDataBase.localProfile == null)
            return;
        HealthCounter.text = NetworkDataBase.localProfile.health.ToString("0.0");
        healthBarForeground.sizeDelta = new Vector2 ((healthBarBackground.rect.width) * (NetworkDataBase.localProfile.health / NetworkDataBase.localProfile.maxHealth), healthBarForeground.sizeDelta.y);
    }
    #endregion
    #region ManaGUI
    public void UpdateManaCounter () {
        if (NetworkDataBase.localProfile == null)
            return;
        ManaCounter.text = NetworkDataBase.localProfile.mana.ToString("0.0");
        manaBarForeground.sizeDelta = new Vector2 ((manaBarBackground.rect.width) * (NetworkDataBase.localProfile.mana / NetworkDataBase.localProfile.maxMana), manaBarForeground.sizeDelta.y);
    }
    #endregion
}
