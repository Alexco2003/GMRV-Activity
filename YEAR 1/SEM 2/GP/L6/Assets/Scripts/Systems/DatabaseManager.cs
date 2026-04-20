using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Header("Dependencies")]
    public SaveManager saveManager;
    public NPCGenerator npcGenerator;

    public WorldSimulator worldSimulator;

    private List<NPCData> masterNpcList = new List<NPCData>();
    private List<ItemData> masterItemList = new List<ItemData>();

    private bool isDataReadyInRAM = false;

    public void OnGenerateAndSaveClicked()
    {

        if (worldSimulator.isSimulationRunning)
        {
            Debug.LogWarning("Cannot generate new data while the simulation is running!");
            return;
        }

        Debug.Log("Generating a new configuration...");
        masterNpcList.Clear();
        masterItemList.Clear();

        int npcCount = Random.Range(4, 7);
        for (int i = 0; i < npcCount; i++)
        {
            masterNpcList.Add(npcGenerator.GenerateRandomNPC());
        }

        saveManager.SaveGame(masterNpcList, masterItemList);

        isDataReadyInRAM = true;
        Debug.Log("NEW configuration successfully generated and saved!");

    }

    public void OnLoadOldClicked()
    {
        if (worldSimulator.isSimulationRunning)
        {
            Debug.LogWarning("Cannot load data while the simulation is running!");
            return;
        }

        if (isDataReadyInRAM)
        {
            Debug.LogWarning("You already generated a new configuration this session! It's pointless to load now.");
            return;
        }

        GameSaveData loadedData = saveManager.LoadGame();

        if (loadedData.savedNPCs == null || loadedData.savedNPCs.Count == 0)
        {
            Debug.LogError("No old save exists! You must hit GENERATE NEW NPCs.");
        }
        else
        {
            masterNpcList = loadedData.savedNPCs;
            masterItemList = loadedData.savedItems;

            isDataReadyInRAM = true;
            Debug.Log($"Successfully loaded {masterNpcList.Count} NPCs from the old save!");

        }
    }

    public void OnStartSimClicked()
    {
        if (worldSimulator.isSimulationRunning)
        {
            Debug.LogWarning("Simulation is already running!");
            return;
        }

        if (!isDataReadyInRAM || masterNpcList.Count == 0)
        {
            Debug.LogError("ERROR: You have no loaded database! Hit Load or Generate before starting the simulation.");
            return;
        }

        Debug.Log("Starting Simulation with current data...");

        worldSimulator.StartSimulationWithData(masterNpcList);
    }

    public void OnDeleteSaveClicked()
    {

        if (worldSimulator.isSimulationRunning)
        {
            Debug.LogWarning("Cannot delete data while the simulation is running!");
            return;
        }

        saveManager.DeleteSaveData();

        masterNpcList.Clear();
        masterItemList.Clear();
        isDataReadyInRAM = false;

        Debug.Log("Database wiped clean from RAM and Disk! You can now test a fresh start.");
    }
}