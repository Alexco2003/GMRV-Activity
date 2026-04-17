using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string itemType;
    public ItemRarity rarity;

    public float damage;
    public float durability;

    public ItemAbilityData primaryAbility;
    public ItemAbilityData secondaryAbility;

    public ItemData() { }
}