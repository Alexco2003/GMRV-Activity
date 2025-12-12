using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeElapsed : MonoBehaviour
{
    [SerializeField] private Text elapsedText;

    [SerializeField] private Text bestLapText;

    [SerializeField] private string playerTag;

    // true = MM:SS.s, false = seconds with one decimal (e.g. 12.3s)
    [SerializeField] private bool useMinutesSecondsFormat = true;

    public string PlayerTag => playerTag;

    private float startTime;
    private bool running;
    private float bestLapTime = float.MaxValue;

    private void Awake()
    {
        
            StartTimer();
            bestLapText.text = useMinutesSecondsFormat ? FormatMinSec(0) : $"{0:0.0}s";
    }

    private void Update()
    {
        if (!running || elapsedText == null) return;

        float elapsed = Time.time - startTime;
        elapsedText.text = useMinutesSecondsFormat ? FormatMinSec(elapsed) : $"{elapsed:0.0}s";
    }

    public void StartTimer()
    {
        startTime = Time.time;
        running = true;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
        running = true;
        if (elapsedText != null)
        {
            if (useMinutesSecondsFormat) elapsedText.text = FormatMinSec(0f);
            else elapsedText.text = "0.0s";
        }
    }

    public void RegisterLap()
    {
        if (!running)
        {
          
            StartTimer();
            return;
        }

        float lapTime = Time.time - startTime;

        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
            if (bestLapText != null)
            {
                bestLapText.text = useMinutesSecondsFormat ? FormatMinSec(bestLapTime) : $"{bestLapTime:0.0}s";
            }
        }

   
        StartTimer();
    }

    public void StopTimer()
    {
        running = false;
    }

    private static string FormatMinSec(float t)
    {
        int minutes = (int)(t / 60f);
        float seconds = t % 60f;
        return string.Format("{0:00}:{1:00.0}", minutes, seconds);
    }
}