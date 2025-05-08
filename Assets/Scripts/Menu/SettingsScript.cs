using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider Slider;

    void Start()
    {
        Slider.minValue = 0.1f;
        Slider.maxValue = 10f;
        LoadSliderValue();
    }
    
    void Update()
    {
        PlayerPrefs.SetFloat("Sensetivity", Slider.value);
    }

    void LoadSliderValue()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
            Slider.value = PlayerPrefs.GetFloat("Sensetivity");
    }
}
