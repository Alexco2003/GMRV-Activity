using System.Collections.Generic;
using UnityEngine;

public class NarrativeJournal
{

    private List<string> attackTemplates = new List<string>
    {
        "{attacker} executed a {style} attack on {target} in the {loc}, dealing {dmg} damage.",
        "In the {loc}, {target} was ambushed by {attacker}, losing {dmg} HP.",
        "{attacker} ({cls}) struck {target} for {dmg} damage while traversing the {loc}."
    };

    private List<string> fleeTemplates = new List<string>
    {
        "Bleeding and at {hp}% HP, {attacker} fled the {loc} in terror.",
        "Realizing the danger, {attacker} escaped from the {loc} to survive.",
        "{attacker} barely survived the {loc}, running away with only {hp}% health left."
    };

    private List<string> tradeTemplates = new List<string>
    {
        "{attacker} generously gifted a [{item}] to {target} at the {loc}.",
        "A peaceful trade occurred in the {loc}: {attacker} gave {target} a [{item}].",
        "{attacker} and {target} strengthened their bond in the {loc} by exchanging a [{item}]."
    };

    public string GenerateLog(int day, ActionType action, NPCData actor, NPCData target = null, float value = 0f, string itemName = "")
    {
        string template = "";

        switch (action)
        {
            case ActionType.Attack:
                template = attackTemplates[Random.Range(0, attackTemplates.Count)];
                break;
            case ActionType.Survive:
                template = fleeTemplates[Random.Range(0, fleeTemplates.Count)];
                break;
            case ActionType.Trade:
                template = tradeTemplates[Random.Range(0, tradeTemplates.Count)];
                break;
            case ActionType.Idle:
                if (Random.value <= 0.2f) return $"[Day {day}] {actor.npcName} rested peacefully in the {actor.currentLocation}.";
                return "";
            default:
                return $"[Day {day}] {actor.npcName} performed an action in the {actor.currentLocation}.";
        }

        string log = template.Replace("{attacker}", actor.npcName);
        log = log.Replace("{loc}", actor.currentLocation.ToString());
        log = log.Replace("{cls}", actor.npcClass.ToString());

        string style = (actor.personality == NPCPersonality.Aggressive) ? "brutal" :
                       (actor.personality == NPCPersonality.Cunning) ? "calculated" : "standard";
        log = log.Replace("{style}", style);

        if (target != null) log = log.Replace("{target}", target.npcName);

        if (action == ActionType.Attack) log = log.Replace("{dmg}", Mathf.RoundToInt(value).ToString());
        if (action == ActionType.Survive) log = log.Replace("{hp}", Mathf.RoundToInt((actor.hp / actor.maxHp) * 100f).ToString());

        if (action == ActionType.Trade && !string.IsNullOrEmpty(itemName)) log = log.Replace("{item}", itemName);

        return $"[Day {day}] {log}";
    }
}