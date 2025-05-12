using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplayer : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI Text;

    void Start()
    {
        SetTextFromSlider();
    }

    void Update()
    {
        SetTextFromSlider();
    }

    void SetTextFromSlider()
    {
        var format = (int)Slider.value >= 10 ? "00.0" : "0.00";
        Text.text = Slider.value.ToString(format, CultureInfo.InvariantCulture);
    }
}
