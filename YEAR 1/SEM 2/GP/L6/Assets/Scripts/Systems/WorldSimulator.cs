using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldSimulator : MonoBehaviour
{
    [Header("Dependencies")]
    public NPCGenerator npcGenerator;

    private RelationshipSystem relationshipSystem;
    private NarrativeJournal journal;
    private List<NPCAgent> activeAgents = new List<NPCAgent>();

    private int currentDay = 0;
    private const int MAX_DAYS = 20;
    private bool simulationEnded = false;

    void Start()
    {
        InitializeWorld();
    }

    private void InitializeWorld()
    {
        relationshipSystem = new RelationshipSystem();
        journal = new NarrativeJournal();
        activeAgents.Clear();
        currentDay = 0;
        simulationEnded = false;

        int npcCount = Random.Range(4, 7);
        for (int i = 0; i < npcCount; i++)
        {
            NPCData newData = npcGenerator.GenerateRandomNPC();

            int locationCount = System.Enum.GetValues(typeof(LocationType)).Length;
            newData.currentLocation = (LocationType)Random.Range(0, locationCount);

            activeAgents.Add(new NPCAgent(newData));
        }

        Debug.Log($"--- WORLD INITIALIZED WITH {npcCount} NPCs ---");
    }

    public void OnAdvanceDayClicked()
    {
        if (simulationEnded)
        {
            Debug.LogWarning("The simulation has already ended. Please restart to play again.");
            return;
        }

        currentDay++;
        Debug.Log($"\n=== STARTING DAY {currentDay} ===");

        List<NPCAgent> aliveAgents = activeAgents.Where(a => !a.Data.isDead).ToList();

        foreach (var agent in aliveAgents)
        {
            if (agent.Data.isDead) continue;

            List<NPCAgent> neighbors = aliveAgents.Where(a => a.Data.currentLocation == agent.Data.currentLocation && !a.Data.isDead).ToList();

            string actionLog = agent.ExecuteTick(neighbors, relationshipSystem, journal, currentDay);

            if (!string.IsNullOrEmpty(actionLog))
            {
                Debug.Log(actionLog);
            }
        }

        CheckEndConditions();
    }

    private void CheckEndConditions()
    {
        int aliveCount = activeAgents.Count(a => !a.Data.isDead);

        if (aliveCount <= 1 || currentDay >= MAX_DAYS)
        {
            simulationEnded = true;
            GenerateSummary(aliveCount);
        }
    }

    private void GenerateSummary(int aliveCount)
    {
        Debug.Log("\n==================================");
        Debug.Log("      SIMULATION CONCLUDED        ");
        Debug.Log("==================================");

        if (aliveCount == 0)
        {
            Debug.Log("Result: Mutually Assured Destruction. No one survived.");
        }
        else if (aliveCount == 1)
        {
            NPCAgent winner = activeAgents.First(a => !a.Data.isDead);
            Debug.Log($"Result: Battle Royale Winner! {winner.Data.npcName} ({winner.Data.npcClass}) is the sole survivor!");
        }
        else
        {
            Debug.Log($"Result: Time Limit Reached (Day {MAX_DAYS}). {aliveCount} NPCs managed to survive.");

            string survivors = "Survivors: ";
            foreach (var agent in activeAgents.Where(a => !a.Data.isDead))
            {
                survivors += $"{agent.Data.npcName} ({agent.Data.hp:F0} HP), ";
            }
            Debug.Log(survivors.TrimEnd(',', ' '));
        }
    }
}