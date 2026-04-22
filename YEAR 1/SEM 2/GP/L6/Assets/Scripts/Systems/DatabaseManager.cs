using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Header("Dependencies")]
    public SaveManager saveManager;
    public NPCGenerator npcGenerator;
    public ItemGenerator itemGenerator;

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
            NPCData newNpc = npcGenerator.GenerateRandomNPC();

            int startingItems = Random.Range(0, 10);
            for (int j = 0; j < startingItems; j++)
            {
                ItemData newItem = itemGenerator.GenerateRandomItem();
                newNpc.inventory.Add(newItem);

                masterItemList.Add(newItem);
            }

            masterNpcList.Add(newNpc);
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



    public bool IsDataReady()
    {
        return isDataReadyInRAM;
    }

    public List<NPCData> GetMasterNpcList()
    {
        return masterNpcList;
    }

    public void ForceSave()
    {
        saveManager.SaveGame(masterNpcList, masterItemList);
        Debug.Log("Auto-saved changes from UI.");
    }
}