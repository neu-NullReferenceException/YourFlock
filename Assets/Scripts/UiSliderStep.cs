using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to a Unity.UI Slider component to control the step size within the slider range.
/// </summary>
[RequireComponent(typeof(Slider))]
public class UiSliderStep : MonoBehaviour
{

    [Tooltip("The desired difference between neighbouring values of the Slider component.")]
    [MinAttribute(0.0001f)]
    public float StepSize = 0.0001f;

    private Slider _slider;
    //private float delete;

    void Start()
    {
        _slider = GetComponent<Slider>();
        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(ClampSliderValue);
        }
    }

    /// <summary>
    /// Calculates the nearest stepped value and updates the Slider component.
    /// </summary>
    /// <param name="value">Current slider value</param>
    public void ClampSliderValue(float value)
    {
        if (_slider != null && StepSize > 0)
        {
            float steppedValue = Mathf.Round(value / StepSize) * StepSize;
            if (steppedValue != value)
            {
                _slider.value = steppedValue;
                //Debug.Log(string.Format("New stepped Slider value: {0}", _slider.value));
            }

#if UNITY_EDITOR
            // ha valaki nem tudna számolni....
            int _numberOfSteps = (int)((_slider.maxValue - _slider.minValue) / StepSize);
            if (_numberOfSteps < 1 || steppedValue < _slider.minValue || steppedValue > _slider.maxValue)
            {
                Debug.LogWarning(string.Format("StepSize is too large. Consider reducing StepSize to less than {0}.", _slider.maxValue - _slider.minValue));
            }
#endif
        }
    }
}