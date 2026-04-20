using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public List<NPCData> savedNPCs = new List<NPCData>();
    public List<ItemData> savedItems = new List<ItemData>();
}

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/gamedata.json";
    }

    public void SaveGame(List<NPCData> npcs, List<ItemData> items)
    {
        GameSaveData dataToSave = new GameSaveData();
        dataToSave.savedNPCs = npcs;
        dataToSave.savedItems = items;

        string json = JsonUtility.ToJson(dataToSave, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved successfully to: " + saveFilePath);
    }

    public GameSaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Data loaded successfully!");
            return loadedData;
        }
        else
        {
            Debug.LogWarning("No save file found. A new empty database will be created.");
            return new GameSaveData();
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted successfully! It's gone forever.");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }
}