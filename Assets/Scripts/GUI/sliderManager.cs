using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sliderManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Slider slider;

    private void Start () {
//        slider.onValueChanged.AddListener(DisplayValue);
    }
    public void DisplayValue (float a) {
        valueText.text = a.ToString ("0");
    }
}
