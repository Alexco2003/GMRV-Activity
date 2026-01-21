using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class PlayFabExamples : MonoBehaviour
    {
        [SerializeField]
        private PlayerAvatarData playerAvatarData;

        [SerializeField]
        [ReadOnly]
        private PlayerAvatarData currentPlayerAvatarData;

        [Button]
        public void LoginWithCustomID()
        {
            var request = new LoginWithCustomIDRequest
            {
                CreateAccount = true,
                CustomId = PlayFabSettings.DeviceUniqueIdentifier

            };
            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                    Debug.Log($"Login successful! {result.PlayFabId}");
                },

                error =>
                {
                    Debug.LogError($"Login failed: {error.GenerateErrorReport()}");
                });

            // start timer
        }

        private void OnApplicationQuit()
        {
            // end timer
            
        }

        [Button]
        private void SavePlayerAvatarData()
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
        {
            { "AvatarData", playerAvatarData.ToJson() }
        }
            };
            PlayFabClientAPI.UpdateUserData(request,
                result => Debug.Log("Data has been saved remotely!"),
                error => Debug.LogError(error.GenerateErrorReport())
            );
        }

        [Button]
        public void GetAvatarData()
        {
            var request = new GetUserDataRequest
            {
                Keys = new List<string> { "AvatarData" }

            };
            PlayFabClientAPI.GetUserData(request,
                result =>
                {
                    if (result.Data != null && result.Data.TryGetValue("AvatarData", out var record))
                    {
                        currentPlayerAvatarData = PlayFabSimpleJson.DeserializeObject<PlayerAvatarData>(record.Value);
                        Debug.Log($"Avatar Data retrieved: OutfitId={currentPlayerAvatarData.OutfitId}, HairStyleId={currentPlayerAvatarData.HairStyleId}, NecklaceAccessoryId={currentPlayerAvatarData.NecklaceAccessoryId}");

                    }
                    else
                    {
                        Debug.Log("No avatar data found.");
                    }
                },
                OnPlayFabError);
        }

        private void OnPlayFabError(PlayFabError error)
        {
            Debug.LogError($"PlayFab API call failed: {error.GenerateErrorReport()}");
        }

        [Button]
        private void CallCloudScript()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "incrementCounter",
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                //var counterValue = Convert.ToInt32(result.FunctionResult);
                //Debug.Log($"Returned counter value: {counterValue}");

                var resultDict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(result.FunctionResult.ToString());
                if (resultDict != null && resultDict.TryGetValue("Counter", out var counter))
                {
                    Debug.Log($"Counter: {counter}");
                }
                else
                {
                    Debug.Log("Counter not found in the response");
                }
            },
            error =>
            {
                Debug.LogError($"Cloud Script error: {error.GenerateErrorReport()}");
            });
        }


    }

}

