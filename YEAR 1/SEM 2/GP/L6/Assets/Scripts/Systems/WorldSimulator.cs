using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class WorldSimulator : MonoBehaviour
{
    [Header("Dependencies")]
    public NPCGenerator npcGenerator;
    public ItemGenerator itemGenerator;

    [Header("UI References (Visuals)")]
    public TextMeshProUGUI simulationLogText;
    public ScrollRect logScrollRect;
    public GameObject npcDotPrefab;
    public Transform[] locationZones;

    private RelationshipSystem relationshipSystem;
    private NarrativeJournal journal;
    private List<NPCAgent> activeAgents = new List<NPCAgent>();

    private int currentDay = 0;
    private const int MAX_DAYS = 20;
    private bool simulationEnded = false;

    public bool isSimulationRunning { get; private set; } = false;

    //void Start()
    //{
    //    InitializeWorld();
    //}

    //private void InitializeWorld()
    //{
    //    relationshipSystem = new RelationshipSystem();
    //    journal = new NarrativeJournal();
    //    activeAgents.Clear();
    //    currentDay = 0;
    //    simulationEnded = false;

    //    int npcCount = Random.Range(4, 7);
    //    for (int i = 0; i < npcCount; i++)
    //    {
    //        NPCData newData = npcGenerator.GenerateRandomNPC();

    //        int locationCount = System.Enum.GetValues(typeof(LocationType)).Length;
    //        newData.currentLocation = (LocationType)Random.Range(0, locationCount);

    //        int startingItems = Random.Range(0, 10);
    //        for (int j = 0; j < startingItems; j++)
    //        {
    //            newData.inventory.Add(itemGenerator.GenerateRandomItem());
    //        }

    //        activeAgents.Add(new NPCAgent(newData));
    //    }

    //    Debug.Log($"--- WORLD INITIALIZED WITH {npcCount} NPCs ---");
    //}

    public void StartSimulationWithData(List<NPCData> importedNPCs)
    {
        relationshipSystem = new RelationshipSystem();
        journal = new NarrativeJournal();
        activeAgents.Clear();
        currentDay = 0;
        simulationEnded = false;

        isSimulationRunning = true;

        if (simulationLogText != null)
            simulationLogText.text = "<b>--- SIMULATION INITIALIZED ---</b>\n";

        foreach (NPCData data in importedNPCs)
        {
            data.isDead = false;
            data.hp = data.maxHp;

            int locationCount = System.Enum.GetValues(typeof(LocationType)).Length;
            data.currentLocation = (LocationType)Random.Range(0, locationCount);

            activeAgents.Add(new NPCAgent(data));
        }

        string prepLog = $"ARENA PREPARED WITH {activeAgents.Count} COMBATANTS!\nPress [Advance Day] to advance time.";
        Debug.Log(prepLog);
        UpdateUIJournal(prepLog);

        UpdateMapVisuals();
    }

    public void OnAdvanceDayClicked()
    {
        if (!isSimulationRunning)
        {
            Debug.LogWarning("Simulation has not started yet! Please click 'Start Simulation' first.");
            return;
        }

        if (simulationEnded)
        {
            Debug.LogWarning("The simulation has already ended. Please restart to play again.");
            return;
        }

        currentDay++;
        string dayHeader = $"\n\n<color=#B854FF><b>=== STARTING DAY {currentDay} ===</b></color>";
        Debug.Log(dayHeader);
        UpdateUIJournal(dayHeader);

        List<NPCAgent> aliveAgents = activeAgents.Where(a => !a.Data.isDead).ToList();

        foreach (var agent in aliveAgents)
        {
            if (agent.Data.isDead) continue;

            List<NPCAgent> neighbors = aliveAgents.Where(a => a.Data.currentLocation == agent.Data.currentLocation && !a.Data.isDead).ToList();

            string actionLog = agent.ExecuteTick(neighbors, relationshipSystem, journal, currentDay);

            if (!string.IsNullOrEmpty(actionLog))
            {
                Debug.Log(actionLog);

                string hexColor = ColorUtility.ToHtmlStringRGB(GetClassColor(agent.Data.npcClass));
                string coloredLog = actionLog.Replace(agent.Data.npcName, $"<color=#{hexColor}><b>{agent.Data.npcName}</b></color>");

                UpdateUIJournal(coloredLog);
            }
        }

        UpdateMapVisuals();

        CheckEndConditions();
    }

    private void UpdateUIJournal(string newText)
    {
        if (simulationLogText != null)
        {
            simulationLogText.text += "\n" + newText;
            
            if (logScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                logScrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    private void UpdateMapVisuals()
    {
        ClearMap();

        foreach (NPCAgent agent in activeAgents)
        {
            if (agent.Data.isDead) continue;

            int locationIndex = (int)agent.Data.currentLocation;

            if (locationIndex >= 0 && locationIndex < locationZones.Length)
            {
                Transform targetZone = locationZones[locationIndex];
                SpawnNPCDot(agent.Data, targetZone);
            }
        }
    }

    private void ClearMap()
    {
        foreach (Transform zone in locationZones)
        {
            for (int i = zone.childCount - 1; i >= 0; i--)
            {
                Destroy(zone.GetChild(i).gameObject);
            }
        }
    }

    private void SpawnNPCDot(NPCData npcData, Transform location)
    {
        GameObject dot = Instantiate(npcDotPrefab, location);

        Vector2 randomOffset = Random.insideUnitCircle * 40f;
        dot.transform.localPosition = new Vector3(randomOffset.x, randomOffset.y, 0);

        TextMeshProUGUI dotText = dot.GetComponentInChildren<TextMeshProUGUI>();
        if (dotText != null && !string.IsNullOrEmpty(npcData.npcName))
        {
            dotText.text = npcData.npcName.Substring(0, 1).ToUpper();
        }

        Image dotImage = dot.GetComponent<Image>();
        if (dotImage != null)
        {
            dotImage.color = GetClassColor(npcData.npcClass);
        }
    }

    private Color GetClassColor(NPCClass npcClass)
    {
        switch (npcClass)
        {
            case NPCClass.Warrior: return new Color(0.8f, 0.2f, 0.2f); // red
            case NPCClass.Mage: return new Color(0.2f, 0.6f, 1f);      // blue
            case NPCClass.Rogue: return new Color(0.2f, 0.8f, 0.2f);   // green
            case NPCClass.Archer: return new Color(1f, 0.8f, 0f);      // yellow
            case NPCClass.Paladin: return new Color(1f, 0.5f, 0f);     // orange
            default: return Color.white;
        }
    }

    private void CheckEndConditions()
    {
        int aliveCount = activeAgents.Count(a => !a.Data.isDead);

        if (aliveCount <= 1 || currentDay >= MAX_DAYS)
        {
            simulationEnded = true;
            isSimulationRunning = false;
            GenerateSummary(aliveCount);
        }
    }

    private void GenerateSummary(int aliveCount)
    {
        Debug.Log("\n==================================");
        Debug.Log("      SIMULATION CONCLUDED        ");
        Debug.Log("==================================");

        UpdateUIJournal("\n<color=#FF0000>=============================</color>");
        UpdateUIJournal("<color=#FF0000>      SIMULATION CONCLUDED      </color>");
        UpdateUIJournal("<color=#FF0000>=============================</color>");

        if (aliveCount == 0)
        {
            Debug.Log("Result: Mutually Assured Destruction. No one survived.");
            UpdateUIJournal("Result: Mutually Assured Destruction. No one survived.");
        }
        else if (aliveCount == 1)
        {
            NPCAgent winner = activeAgents.First(a => !a.Data.isDead);
            Debug.Log($"Result: Battle Royale Winner! {winner.Data.npcName} ({winner.Data.npcClass}) is the sole survivor!");
            UpdateUIJournal($"<b>Result: Battle Royale Winner! <color=green>{winner.Data.npcName}</color> ({winner.Data.npcClass}) is the sole survivor!</b>");
        }
        else
        {
            UpdateUIJournal($"Result: Time Limit Reached (Day {MAX_DAYS}). {aliveCount} NPCs managed to survive.");
            Debug.Log($"Result: Time Limit Reached (Day {MAX_DAYS}). {aliveCount} NPCs managed to survive.");

            string survivors = "Survivors: ";
            foreach (var agent in activeAgents.Where(a => !a.Data.isDead))
            {
                survivors += $"{agent.Data.npcName} ({agent.Data.hp:F0} HP), ";
            }
            Debug.Log(survivors.TrimEnd(',', ' '));
            UpdateUIJournal(survivors.TrimEnd(',', ' '));
        }
    }
}