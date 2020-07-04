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


    public void InitializeWithMaxAndInterval(int max, float interval)
    {
        progressionSlider.maxValue = max;
        actualSlider.maxValue = max;

        progressRate = 1f / interval;   // we always want to progress 1 unit
    }

    public void SetToValue(int newValue, float timeTillNextValue)
    {
        actualSlider.value = newValue;
        progressionSlider.value = newValue;
        progressRate = 1f / timeTillNextValue;
        energyCountText.text = newValue.ToString();
    }

    private void Update()
    {
        float progressionDelta = Time.deltaTime * progressRate;
        float progressionValue = progressionSlider.value + progressionDelta;
        float progressionCap = actualSlider.value + 1;

        progressionSlider.value = Mathf.Clamp(progressionValue, 0f, progressionCap);
    }
}
