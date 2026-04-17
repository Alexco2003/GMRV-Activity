using UnityEngine;

[System.Serializable]
public class NPCData
{
    public string npcName;
    public NPCClass npcClass;
    public NPCPersonality personality;

    public float hp;
    public float maxHp;
    public float damage;
    public float armor;

    public LocationType currentLocation;
    public bool isDead = false;

    public NPCData() { }
}