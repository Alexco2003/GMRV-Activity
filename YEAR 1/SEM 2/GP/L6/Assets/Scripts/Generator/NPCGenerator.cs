using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("Generator Configuration")]
    public List<string> firstNames = new List<string>
    {
        "Kael", "Lyra", "Grom", "Sylas", "Elara", "Thorin",
        "Aria", "Fenrir", "Darius", "Evelynn", "Jace", "Vex",
        "Seraphine", "Rengar", "Talon", "Morgana", "Garen", "Lux",
        "Ezio", "Aloy", "Geralt", "Ciri", "Arthas", "Jaina"
    };

    public List<string> lastNames = new List<string>
    {
        "Shadow", "Whisper", "Iron", "Sunwalker", "Blood", "Stone",
        "Storm", "Lightbringer", "Windrunner", "Hellscream", "Proudmoore",
        "Swift", "Darkblade", "Moon", "Stargazer", "Fireforge", "Frost",
        "Steelskin", "Silvertongue", "Gloom", "Dawn", "Duskrider"
    };

    private static int nextNpcId = 1;
    public void OnGenerateNPCButtonClicked()
    {
        NPCData newNPC = GenerateRandomNPC();
    }

    public NPCData GenerateRandomNPC()
    {
        NPCData newNPC = new NPCData();

        newNPC.id = nextNpcId;
        nextNpcId++;

        string randomFirstName = firstNames[Random.Range(0, firstNames.Count)];
        string randomLastName = lastNames[Random.Range(0, lastNames.Count)];
        newNPC.npcName = $"{randomFirstName} {randomLastName}";

        newNPC.npcClass = GetWeightedRandomClass();

        int personalityCount = System.Enum.GetValues(typeof(NPCPersonality)).Length;
        newNPC.personality = (NPCPersonality)Random.Range(0, personalityCount);

        AssignBaseStats(newNPC);

        ApplyPersonalityModifiers(newNPC);

        Debug.Log($"Generated [ID: {newNPC.id}]: {newNPC.npcName} | {newNPC.npcClass} | {newNPC.personality} | HP: {newNPC.hp} | Dmg: {newNPC.damage} | Armor: {newNPC.armor}");

        return newNPC;
    }

    private NPCClass GetWeightedRandomClass()
    {

        float[] weights = { 40f, 5f, 20f, 20f, 15f };
        float totalWeight = 0;

        foreach (float w in weights)
        {
            totalWeight += w;
        }

        float randomVal = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomVal <= cumulativeWeight)
            {
                return (NPCClass)i;
            }
        }

        return NPCClass.Warrior;
    }

    private void AssignBaseStats(NPCData npc)
    {
        switch (npc.npcClass)
        {
            case NPCClass.Warrior:
                npc.hp = Mathf.RoundToInt(Random.Range(80f, 120f));
                npc.damage = Mathf.RoundToInt(Random.Range(15f, 25f));
                npc.armor = Mathf.RoundToInt(Random.Range(10f, 20f));
                break;
            case NPCClass.Mage:
                npc.hp = Mathf.RoundToInt(Random.Range(40f, 65f));
                npc.damage = Mathf.RoundToInt(Random.Range(30f, 50f));
                npc.armor = Mathf.RoundToInt(Random.Range(2f, 5f));
                break;
            case NPCClass.Rogue:
                npc.hp = Mathf.RoundToInt(Random.Range(55f, 80f));
                npc.damage = Mathf.RoundToInt(Random.Range(20f, 35f));
                npc.armor = Mathf.RoundToInt(Random.Range(5f, 10f));
                break;
            case NPCClass.Archer:
                npc.hp = Mathf.RoundToInt(Random.Range(60f, 85f));
                npc.damage = Mathf.RoundToInt(Random.Range(18f, 30f));
                npc.armor = Mathf.RoundToInt(Random.Range(4f, 8f));
                break;
            case NPCClass.Paladin:
                npc.hp = Mathf.RoundToInt(Random.Range(90f, 130f));
                npc.damage = Mathf.RoundToInt(Random.Range(12f, 20f));
                npc.armor = Mathf.RoundToInt(Random.Range(15f, 25f));
                break;
        }
        npc.maxHp = npc.hp;
    }

    private void ApplyPersonalityModifiers(NPCData npc)
    {
        switch (npc.personality)
        {
            case NPCPersonality.Aggressive:
                npc.damage = Mathf.RoundToInt(npc.damage * 1.3f);
                npc.hp = Mathf.RoundToInt(npc.hp * 0.9f);
                break;
            case NPCPersonality.Coward:
                npc.hp = Mathf.RoundToInt(npc.hp * 0.8f);
                npc.armor = Mathf.RoundToInt(npc.armor * 0.7f);
                break;
            case NPCPersonality.Brave:
                npc.hp = Mathf.RoundToInt(npc.hp * 1.1f);
                npc.damage = Mathf.RoundToInt(npc.damage * 1.1f);
                break;
            case NPCPersonality.Cunning:
                npc.damage = Mathf.RoundToInt(npc.damage * 1.2f);
                npc.armor = Mathf.RoundToInt(npc.armor * 1.1f);
                break;
            case NPCPersonality.Peaceful:
                npc.hp = Mathf.RoundToInt(npc.hp * 1.15f);
                npc.damage = Mathf.RoundToInt(npc.damage * 0.7f);
                break;
        }
    }
}