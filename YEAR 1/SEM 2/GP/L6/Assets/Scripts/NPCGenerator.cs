using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("Generator Configuration")]
    public List<string> firstNames = new List<string> { "Kael", "Lyra", "Grom", "Sylas", "Elara", "Thorin" };
    public List<string> lastNames = new List<string> { "Shadow", "Whisper", "Iron", "Sunwalker", "Blood", "Stone" };

    public void OnGenerateNPCButtonClicked()
    {
        NPCData newNPC = GenerateRandomNPC();
    }

    public NPCData GenerateRandomNPC()
    {
        NPCData newNPC = new NPCData();

        string randomFirstName = firstNames[Random.Range(0, firstNames.Count)];
        string randomLastName = lastNames[Random.Range(0, lastNames.Count)];
        newNPC.npcName = $"{randomFirstName} {randomLastName}";

        newNPC.npcClass = GetWeightedRandomClass();

        int personalityCount = System.Enum.GetValues(typeof(NPCPersonality)).Length;
        newNPC.personality = (NPCPersonality)Random.Range(0, personalityCount);

        AssignBaseStats(newNPC);

        ApplyPersonalityModifiers(newNPC);

        Debug.Log($"Generated: {newNPC.npcName} | {newNPC.npcClass} | {newNPC.personality} | HP: {newNPC.hp:F1} | Dmg: {newNPC.damage:F1} | Armor: {newNPC.armor:F1}");

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
                npc.hp = Random.Range(80f, 120f);
                npc.damage = Random.Range(15f, 25f);
                npc.armor = Random.Range(10f, 20f);
                break;
            case NPCClass.Mage:
                npc.hp = Random.Range(40f, 65f);
                npc.damage = Random.Range(30f, 50f);
                npc.armor = Random.Range(2f, 5f);
                break;
            case NPCClass.Rogue:
                npc.hp = Random.Range(55f, 80f);
                npc.damage = Random.Range(20f, 35f);
                npc.armor = Random.Range(5f, 10f);
                break;
            case NPCClass.Archer:
                npc.hp = Random.Range(60f, 85f);
                npc.damage = Random.Range(18f, 30f);
                npc.armor = Random.Range(4f, 8f);
                break;
            case NPCClass.Paladin:
                npc.hp = Random.Range(90f, 130f);
                npc.damage = Random.Range(12f, 20f);
                npc.armor = Random.Range(15f, 25f);
                break;
        }

        npc.maxHp = npc.hp;
    }

    private void ApplyPersonalityModifiers(NPCData npc)
    {
        switch (npc.personality)
        {
            case NPCPersonality.Aggressive:
                npc.damage *= 1.3f;
                npc.hp *= 0.9f;
                break;
            case NPCPersonality.Coward:
                npc.hp *= 0.8f;
                npc.armor *= 0.7f;
                break;
            case NPCPersonality.Brave:
                npc.hp *= 1.1f;
                npc.damage *= 1.1f;
                break;
            case NPCPersonality.Cunning:
                npc.damage *= 1.2f;
                npc.armor *= 1.1f;
                break;
            case NPCPersonality.Peaceful:
                npc.hp *= 1.15f;
                npc.damage *= 0.7f;
                break;
        }
    }
}