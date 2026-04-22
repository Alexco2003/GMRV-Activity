using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCUICard : MonoBehaviour
{
    [Header("NPC Elements")]
    public TMP_InputField nameInputField;
    public TextMeshProUGUI statsText;

    [Header("Inventory Spawner")]
    public Transform inventoryContainer;
    public GameObject itemEntryPrefab;

    private NPCData myNPC;
    private DatabaseManager dbManager;

    public void Setup(NPCData npc, DatabaseManager db)
    {
        myNPC = npc;
        dbManager = db;

        nameInputField.text = npc.npcName;
        nameInputField.onEndEdit.RemoveAllListeners();
        nameInputField.onEndEdit.AddListener(OnNameChanged);

        statsText.text = $"Class: {npc.npcClass} | Personality: {npc.personality}\n" +
                         $"HP: {npc.hp} | DMG: {npc.damage} | Armor: {npc.armor}";


        for (int i = inventoryContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(inventoryContainer.GetChild(i).gameObject);
        }


        if (npc.inventory.Count == 0) return;

        foreach (ItemData item in npc.inventory)
        {
            GameObject spawnedItem = Instantiate(itemEntryPrefab, inventoryContainer);
            ItemUIEntry entryScript = spawnedItem.GetComponent<ItemUIEntry>();
            entryScript.Setup(item, dbManager);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void OnNameChanged(string newName)
    {
        if (string.IsNullOrEmpty(newName)) return;

        myNPC.npcName = newName;
        dbManager.ForceSave();
        Debug.Log($"NPC renamed to {newName} and saved successfully!");
    }
}