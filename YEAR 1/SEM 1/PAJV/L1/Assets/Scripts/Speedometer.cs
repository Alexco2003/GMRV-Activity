using System;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private PrometeoCarController car;

    private void OnEnable()
    {
        if (car != null)
        {
            car.OnSpeedChanged += UpdateUI;
            return;
        }
    }

    private void OnDisable()
    {
        if (car != null)
            car.OnSpeedChanged -= UpdateUI;
    }

    private void UpdateUI(float speed)
    {
        if (speedText != null)
            speedText.text = $"{Mathf.RoundToInt(speed).ToString():0} km/h";
    }
}