using System.Collections.Generic;
using UnityEngine;

public class RelationshipSystem
{
    private Dictionary<int, Dictionary<int, float>> relationships = new Dictionary<int, Dictionary<int, float>>();

    private void EnsureRelationshipExists(int id1, int id2)
    {
        if (!relationships.ContainsKey(id1))
            relationships[id1] = new Dictionary<int, float>();

        if (!relationships.ContainsKey(id2))
            relationships[id2] = new Dictionary<int, float>();

        if (!relationships[id1].ContainsKey(id2))
            relationships[id1][id2] = Random.Range(-100f, 100f);

        if (!relationships[id2].ContainsKey(id1))
            relationships[id2][id1] = Random.Range(-100f, 100f);
    }

    public void ModifyRelationship(int initiatorId, int targetId, float delta)
    {
        if (initiatorId == targetId) return;

        EnsureRelationshipExists(initiatorId, targetId);

        relationships[initiatorId][targetId] += delta;
        relationships[initiatorId][targetId] = Mathf.Clamp(relationships[initiatorId][targetId], -100f, 100f);

        relationships[targetId][initiatorId] += (delta * 0.7f);
        relationships[targetId][initiatorId] = Mathf.Clamp(relationships[targetId][initiatorId], -100f, 100f);
    }

    public float GetRelationship(int lookingId, int targetId)
    {
        EnsureRelationshipExists(lookingId, targetId);
        return relationships[lookingId][targetId];
    }
}