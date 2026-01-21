using PlayFab.Json;
using System;
using UnityEngine;

[Serializable]
public class PlayerAvatarData
{
    [field: SerializeField]
    public string OutfitId { get; set; }

    [field: SerializeField]
    public string HairStyleId { get; set; }

    [field: SerializeField]
    public string NecklaceAccessoryId { get; set; }

    public string ToJson() => PlayFabSimpleJson.SerializeObject(this);
}
