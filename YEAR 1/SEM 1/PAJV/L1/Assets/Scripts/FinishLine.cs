using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour
{
    private int player1LapCount = 0;
    private int player2LapCount = 0;
    private bool isRaceFinished = false;

    [SerializeField] 
    private int lapsToFinish = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (isRaceFinished) return;

        NotifyTimerForPlayer(other);

        if (other.CompareTag("Player 1"))
        {
            Debug.Log("Finish Line Crossed by Player 1");
            player1LapCount++;
            if (player1LapCount > lapsToFinish)
            {
                Debug.Log("Player 1 Wins!");
                isRaceFinished = true;
            }
        }
        else if (other.CompareTag("Player 2"))
        {
            Debug.Log("Finish Line Crossed by Player 2");
            player2LapCount++;
            if (player2LapCount > lapsToFinish)
            {
                Debug.Log("Player 2 Wins!");
                isRaceFinished = true;
            }
        }

    }

    private void NotifyTimerForPlayer(Collider other)
    {
        try
        {
            string playerTag = other.tag;
            var timers = FindObjectsByType<TimeElapsed>(FindObjectsSortMode.None);
            foreach (var t in timers)
            {
                if (string.Equals(t.PlayerTag, playerTag, StringComparison.Ordinal))
                {
                    if (playerTag == "Player 1" && player1LapCount>=1)
                    {
                        t.RegisterLap();
                    }
                    else if (playerTag == "Player 2" && player2LapCount>=1)
                    {
                        t.RegisterLap();
                    }
               
                    return;
                }
            }
        
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"FinishLine.NotifyTimerForPlayer exception: {ex}");
        }
    }

    public bool IsRaceFinished()
    {
        return isRaceFinished;
    }
}
