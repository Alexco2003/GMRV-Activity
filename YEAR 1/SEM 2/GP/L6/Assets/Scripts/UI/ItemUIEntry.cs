using UnityEngine;
using TMPro;

public class ItemUIEntry : MonoBehaviour
{
    public TMP_InputField itemNameInput;
    public TextMeshProUGUI itemStatsText;

    private ItemData myItem;
    private DatabaseManager dbManager;

    public void Setup(ItemData item, DatabaseManager db)
    {
        myItem = item;
        dbManager = db;

        itemNameInput.text = item.itemName;

        itemNameInput.onEndEdit.RemoveAllListeners();
        itemNameInput.onEndEdit.AddListener(OnItemNameChanged);

        string finalStats = $"({item.rarity}) | Dmg: {item.damage} | Dur: {item.durability}";

        if (item.primaryAbility != null)
        {
            finalStats += $"\n  • <i>{item.primaryAbility.type}</i>: {item.primaryAbility.description}";
        }

        if (item.secondaryAbility != null)
        {
            finalStats += $"\n  • <i>{item.secondaryAbility.type}</i>: {item.secondaryAbility.description}";
        }

        itemStatsText.text = finalStats;
    }

    private void OnItemNameChanged(string newName)
    {
        if (string.IsNullOrEmpty(newName)) return;

        myItem.itemName = newName;
        dbManager.ForceSave();

        Debug.Log($"Item renamed to {newName} and saved successfully!");
    }
}