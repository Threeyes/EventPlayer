using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example_SliderModifier : MonoBehaviour
{

    static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
    /// <summary>
    /// Update the value without invoke the relate Event
    /// </summary>
    /// <param name="value"></param>
    public void SetValueWithoutNotify(float value)
    {
        //PS:If you are using latest version of Unity, you can just use Slider.SetValueWithoutNotify
        Slider slider = GetComponent<Slider>();
        var originalEvent = slider.onValueChanged;
        slider.onValueChanged = emptySliderEvent;
        slider.value = value;
        slider.onValueChanged = originalEvent;
    }
}
