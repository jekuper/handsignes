using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    }

    #region Throwable
    public void UpdateThrowable () {
        UpdateThrowableIcon ();
        UpdateThrowableCount ();
    }
    public void UpdateThrowableIcon () {
        UpdateThrowableIcon(NetworkDataBase.LocalUserData.throwableInUse);
    }
    public void UpdateThrowableIcon(throwableType type) {
        ThrowableImage.sprite = ThrowableIcons[(int)type];
    }
    public void UpdateThrowableCount () {
        if (NetworkDataBase.LocalUserData.throwableInUse == throwableType.Kunai) {
            UpdateThrowableCount (NetworkDataBase.LocalUserData.kunaiCount);
        } else if(NetworkDataBase.LocalUserData.throwableInUse == throwableType.Shuriken) {
            UpdateThrowableCount (NetworkDataBase.LocalUserData.shurikenCount);
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
        HealthCounter.text = NetworkDataBase.LocalUserData.health.ToString();
        healthBarForeground.sizeDelta = new Vector2 ((healthBarBackground.rect.width) * (NetworkDataBase.LocalUserData.health / NetworkDataBase.LocalUserData.healthMax), healthBarForeground.sizeDelta.y);
    }
    #endregion
    #region ManaGUI
    public void UpdateManaCounter () {
        ManaCounter.text = NetworkDataBase.LocalUserData.mana.ToString ("0");
        manaBarForeground.sizeDelta = new Vector2 ((manaBarBackground.rect.width) * (NetworkDataBase.LocalUserData.mana / NetworkDataBase.LocalUserData.manaMax), manaBarForeground.sizeDelta.y);
    }
    #endregion
}
