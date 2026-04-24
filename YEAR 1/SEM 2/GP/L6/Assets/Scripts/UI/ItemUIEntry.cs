using System.Collections;
using TMPro;
using UnityEngine;

public class ItemUIEntry : MonoBehaviour
{
    public TMP_InputField itemNameInput;
    public TextMeshProUGUI itemStatsText;

    private ItemData myItem;
    private DatabaseManager dbManager;
    private Coroutine animationRoutine;

    public void Setup(ItemData item, DatabaseManager db)
    {
        myItem = item;
        dbManager = db;

        itemNameInput.text = item.itemName;
        itemNameInput.onEndEdit.RemoveAllListeners();
        itemNameInput.onEndEdit.AddListener(OnItemNameChanged);

        Color itemColor = Color.white;
        string stars = "";

        switch (item.rarity)
        {
            case ItemRarity.Common:
                itemColor = new Color(0.6f, 0.3f, 0.1f);
                stars = "*";
                break;
            case ItemRarity.Uncommon:
                itemColor = Color.green;
                stars = "**";
                break;
            case ItemRarity.Rare:
                itemColor = new Color(0.2f, 0.6f, 1f);
                stars = "***";
                break;
            case ItemRarity.Epic:
                itemColor = new Color(0.7f, 0.3f, 1f);
                stars = "****";
                break;
            case ItemRarity.Legendary:
                itemColor = new Color(1f, 0.5f, 0f);
                stars = "*****";
                break;
        }

        itemNameInput.textComponent.color = itemColor;
        itemStatsText.color = itemColor;

        string finalStats = $"({item.rarity} {stars}) | Dmg: {item.damage} | Dur: {item.durability}";

        if (item.primaryAbility != null)
        {
            finalStats += $"\n  • <i>{item.primaryAbility.type}</i>: {item.primaryAbility.description}";
        }

        if (item.secondaryAbility != null)
        {
            finalStats += $"\n  • <i>{item.secondaryAbility.type}</i>: {item.secondaryAbility.description}";
        }

        itemStatsText.text = finalStats;


 
        if (animationRoutine != null) StopCoroutine(animationRoutine);

        if (item.rarity == ItemRarity.Legendary)
        {
            animationRoutine = StartCoroutine(LegendaryGlowRoutine(itemColor));
        }
    }

    private void OnItemNameChanged(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        myItem.itemName = newName;
        dbManager.ForceSave();

        Debug.Log($"Item renamed to {newName} and saved successfully!");
    }

    private IEnumerator LegendaryGlowRoutine(Color baseColor)
    {
        while (true)
        {
            float wave = (Mathf.Sin(Time.time * 4f) + 1f) / 2f;

            Color glowColor = Color.Lerp(baseColor, Color.yellow, wave);

            itemNameInput.textComponent.color = glowColor;
            itemStatsText.color = glowColor;

            yield return null;
        }
    }
}