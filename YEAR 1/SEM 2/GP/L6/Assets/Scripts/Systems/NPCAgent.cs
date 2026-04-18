using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCAgent
{
    public NPCData Data { get; private set; }

    public NPCAgent(NPCData data)
    {
        this.Data = data;
    }

    public string ExecuteTick(List<NPCAgent> neighbors, RelationshipSystem relSys, NarrativeJournal journal, int currentDay)
    {
        neighbors = neighbors.Where(n => n.Data.id != this.Data.id).ToList();

        float aggroMultiplier = 1.0f;
        float tradeBonus = 0f;
        float moveBonus = 0f;

        switch (Data.currentLocation)
        {
            case LocationType.Dungeon: aggroMultiplier = 1.5f; break;
            case LocationType.Tavern: aggroMultiplier = 0.6f; tradeBonus = 0.40f; break;
            case LocationType.Market: aggroMultiplier = 0.4f; tradeBonus = 0.60f; break;
            case LocationType.Forest: aggroMultiplier = 1.2f; moveBonus = 0.20f; break;
            case LocationType.Plains: aggroMultiplier = 1.0f; moveBonus = 0.10f; break;
        }



        if (Data.personality == NPCPersonality.Coward && (Data.hp / Data.maxHp) < 0.30f)
        {
            Data.currentLocation = GetRandomLocation(Data.currentLocation);
            return journal.GenerateLog(currentDay, ActionType.Survive, Data);
        }



        List<NPCAgent> enemies = neighbors.Where(n => relSys.GetRelationship(Data.id, n.Data.id) < -10f).ToList();

        if ((Data.personality == NPCPersonality.Aggressive || Data.personality == NPCPersonality.Cunning) && enemies.Count > 0)
        {
            NPCAgent target = null;

            if (Data.personality == NPCPersonality.Cunning)
            {
                target = enemies.OrderBy(e => e.Data.hp).First();
            }
            else
            {
                target = enemies[Random.Range(0, enemies.Count)];
            }

            float actualDamage = Data.damage * aggroMultiplier;
            target.Data.hp -= actualDamage;

            relSys.ModifyRelationship(target.Data.id, Data.id, -20f);

            if (target.Data.hp <= 0)
            {
                target.Data.hp = 0;
                target.Data.isDead = true;

                foreach (var witness in neighbors)
                {
                    if (witness.Data.id != target.Data.id)
                    {
                        relSys.ModifyRelationship(witness.Data.id, Data.id, -30f);
                    }
                }
            }

            return journal.GenerateLog(currentDay, ActionType.Attack, Data, target.Data, actualDamage);
        }



        List<NPCAgent> allies = neighbors.Where(n => relSys.GetRelationship(Data.id, n.Data.id) > 10f).ToList();

        if (allies.Count > 0)
        {
            float tradeChance = 0.25f + tradeBonus;
            if (Random.value <= tradeChance)
            {
                NPCAgent tradePartner = allies[Random.Range(0, allies.Count)];

                relSys.ModifyRelationship(Data.id, tradePartner.Data.id, 10f);
                relSys.ModifyRelationship(tradePartner.Data.id, Data.id, 10f);

                return journal.GenerateLog(currentDay, ActionType.Trade, Data, tradePartner.Data);
            }
        }



        float exploreChance = 0.20f + moveBonus;
        if (Random.value <= exploreChance)
        {
            Data.currentLocation = GetRandomLocation(Data.currentLocation);
            return $"[Day {currentDay}] {Data.npcName} explored the world and traveled to the {Data.currentLocation}.";
        }



        foreach (var neighbor in neighbors)
        {
            relSys.ModifyRelationship(Data.id, neighbor.Data.id, 2f);
        }

        return journal.GenerateLog(currentDay, ActionType.Idle, Data);
    }

    private LocationType GetRandomLocation(LocationType currentLocation)
    {
        int locationCount = System.Enum.GetValues(typeof(LocationType)).Length;
        LocationType newLoc;
        do
        {
            newLoc = (LocationType)Random.Range(0, locationCount);
        }
        while (newLoc == currentLocation);

        return newLoc;
    }
}