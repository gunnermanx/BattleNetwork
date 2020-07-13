using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField]
    private Slider progressionSlider;
    [SerializeField]
    private Slider actualSlider;
    [SerializeField]
    private Text energyCountText;

    private float progressRate = 0f;

    private bool canUpdate;

    public void InitializeWithMaxAndInterval(int current, int max)
    {
        progressionSlider.maxValue = max;
        actualSlider.maxValue = max;
        SetToValue(current, 0);
    }

    public void SetToValue(int newValue, float timeTillNextValue)
    {
        if (timeTillNextValue != 0)
        {
            canUpdate = true;
        }

        actualSlider.value = newValue;
        progressionSlider.value = newValue;
        progressRate = 1f / timeTillNextValue;
        energyCountText.text = newValue.ToString();
    }

    private void Update()
    {
        if (canUpdate)
        {
            float progressionDelta = Time.deltaTime * progressRate;
            float progressionValue = progressionSlider.value + progressionDelta;
            float progressionCap = actualSlider.value + 1;

            progressionSlider.value = Mathf.Clamp(progressionValue, 0f, progressionCap);
        }        
    }
}
