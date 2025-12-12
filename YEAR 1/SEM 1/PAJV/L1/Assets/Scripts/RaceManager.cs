using System.Collections;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private FinishLine finishLine;
    private bool _replayTriggered;


    [SerializeField] private GameObject player1UI;

    [SerializeField] private GameObject player2UI;

    private void Update()
    {
        if (_replayTriggered) return;
        if (finishLine == null) return;

        if (finishLine.IsRaceFinished())
        {
            _replayTriggered = true;
            HideAllPlayerUI();
            StartReplayAll();
        }
    }

    
    public void StartReplayAll()
    {
        var cars = FindObjectsByType<PrometeoCarController>(FindObjectsSortMode.None);
        foreach (var car in cars)
        {
            
            car.StopRecording();
          
            car.ResetToStartState();
            car.StartReplay();
        }
    }

    private void HideAllPlayerUI()
    {
        if (player1UI != null)
            player1UI.SetActive(false);

        if (player2UI != null)
            player2UI.SetActive(false);
    }
}