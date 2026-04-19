using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public int id;
    public string npcName;
    public NPCClass npcClass;
    public NPCPersonality personality;

    public float hp;
    public float maxHp;
    public float damage;
    public float armor;

    public LocationType currentLocation;
    public bool isDead = false;

    public List<ItemData> inventory = new List<ItemData>();

    public NPCData() { }
}