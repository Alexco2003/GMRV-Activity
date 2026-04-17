using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("Item Configuration")]
    public float baseBudget = 100f;

    public List<string> itemPrefixes = new List<string>
    {
        "Rusty", "Shining", "Cursed", "Ancient", "Glimmering", "Broken",
        "Bronze", "Iron", "Steel", "Silver", "Gold", "Mithril", "Obsidian",
        "Wooden", "Leather", "Heavy", "Light", "Masterwork", "Mystic", "Savage"
    };

    public List<string> itemTypes = new List<string>
    { 
        "Sword", "Axe", "Staff", "Bow", "Dagger", "Shield", "Mace", "Crossbow", "Wand",
        "Helmet", "Chestplate", "Pauldrons", "Gauntlets", "Greaves", "Boots", "Cloak", "Amulet"
    };

    public void OnGenerateItemButtonClicked()
    {
        ItemData newItem = GenerateRandomItem();
    }

    public ItemData GenerateRandomItem()
    {
        ItemData newItem = new ItemData();

        string prefix = itemPrefixes[Random.Range(0, itemPrefixes.Count)];
        newItem.itemType = itemTypes[Random.Range(0, itemTypes.Count)];
        newItem.itemName = $"{prefix} {newItem.itemType}";

        newItem.rarity = GetRandomRarity();

        DistributeStats(newItem);

        AssignSpecialAbilities(newItem);

        string abilityLog = newItem.primaryAbility != null ? $" | Ability 1: {newItem.primaryAbility.type}: {newItem.primaryAbility.description}" : "";
        abilityLog += newItem.secondaryAbility != null ? $" | Ability 2: {newItem.secondaryAbility.type}: {newItem.secondaryAbility.description}" : "";

        Debug.Log($"Generated Item: {newItem.itemName} [{newItem.rarity}] | Dmg: {newItem.damage:F1} | Dur: {newItem.durability:F1}{abilityLog}");
        return newItem;
    }

    private ItemRarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= 2f) return ItemRarity.Legendary;
        if (roll <= 8f) return ItemRarity.Epic;
        if (roll <= 20f) return ItemRarity.Rare;
        if (roll <= 45f) return ItemRarity.Uncommon;

        return ItemRarity.Common;                        
    }

    private void DistributeStats(ItemData item)
    {
        float multiplier = 1.0f;

        switch (item.rarity)
        {
            case ItemRarity.Common: multiplier = 1.0f; break;
            case ItemRarity.Uncommon: multiplier = 1.3f; break;
            case ItemRarity.Rare: multiplier = 1.7f; break;
            case ItemRarity.Epic: multiplier = 2.2f; break;
            case ItemRarity.Legendary: multiplier = 3.0f; break;
        }

        float totalBudget = baseBudget * multiplier;

        float damagePercentage = Random.Range(0.4f, 0.7f);
        item.damage = totalBudget * damagePercentage;

        item.durability = totalBudget - item.damage;
    }

    private void AssignSpecialAbilities(ItemData item)
    {
        if (item.rarity == ItemRarity.Epic || item.rarity == ItemRarity.Legendary)
        {

            item.primaryAbility = GenerateAbilityStats(GetRandomAbilityType());

            if (item.rarity == ItemRarity.Legendary)
            {
                SpecialAbilityType secondType;
                do
                {
                    secondType = GetRandomAbilityType();
                }
                while (secondType == item.primaryAbility.type);

                item.secondaryAbility = GenerateAbilityStats(secondType);
            }
        }
    }

    private SpecialAbilityType GetRandomAbilityType()
    {
        int count = System.Enum.GetValues(typeof(SpecialAbilityType)).Length;
        return (SpecialAbilityType)Random.Range(0, count);
    }

    private ItemAbilityData GenerateAbilityStats(SpecialAbilityType type)
    {
        ItemAbilityData newAbility = new ItemAbilityData();
        newAbility.type = type;

        switch (type)
        {
            case SpecialAbilityType.Poison:
                newAbility.value1 = Random.Range(3f, 8f);
                newAbility.value2 = Random.Range(3, 6);
                newAbility.description = $"{newAbility.value1:F1} dmg/tick for {newAbility.value2} ticks";
                break;

            case SpecialAbilityType.Lifesteal:
                newAbility.value1 = Random.Range(15f, 30f);
                newAbility.description = $"Heals {newAbility.value1:F1}% of damage dealt";
                break;

            case SpecialAbilityType.FireDamage:
                newAbility.value1 = Random.Range(5f, 15f);
                newAbility.value2 = 2f;
                newAbility.description = $"+{newAbility.value1:F1} dmg & burns for {newAbility.value2} ticks";
                break;

            case SpecialAbilityType.IceSlow:
                newAbility.value1 = Random.Range(20f, 50f);
                newAbility.value2 = Random.Range(2f, 4f);
                newAbility.description = $"Reduces target speed by {newAbility.value1:F1}% for {newAbility.value2:F1}s";
                break;

            case SpecialAbilityType.Thunder:
                newAbility.value1 = Random.Range(15f, 35f);
                newAbility.value2 = 1f;
                newAbility.description = $"{newAbility.value1:F1}% chance to stun for {newAbility.value2}s";
                break;
        }

        return newAbility;
    }
}