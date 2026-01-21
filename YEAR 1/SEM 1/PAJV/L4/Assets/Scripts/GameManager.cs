using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject charSelectPanel;
    public GameObject gamePanel;
    public GameObject leaderboardPanel;

    [Header("Login UI")]
    public TMP_InputField idInput;
    public TMP_InputField emailInput;

    [Header("Character Creation UI")]
    public TMP_InputField heroNameInput;

    [Header("Game UI")]
    public TMP_Text playerInfoText;
    public TMP_Text gameStatsText;
    public Transform spawnArea;
    public GameObject collectiblePrefab;

    [Header("Leaderboard UI")]
    public TMP_Text leaderboardContentText;

    private string myPlayFabId;
    private int characterType = 0;
    private string characterName = "Unknown";
    private bool isPlaying = false;
    private float gameStartTime;

    private int localLevel = 1;
    private int localXP = 0;
    private int localCollectibles = 0;

    void Start()
    {
        
        loginPanel.SetActive(true);
        charSelectPanel.SetActive(false);
        gamePanel.SetActive(false);
        leaderboardPanel.SetActive(false);

        if (PlayerPrefs.HasKey("LastCustomID"))
            idInput.text = PlayerPrefs.GetString("LastCustomID");
    }

    public void OnLoginClick()
    {
        string customId = idInput.text;
        if (string.IsNullOrEmpty(customId))
        {
            customId = System.Guid.NewGuid().ToString();
            idInput.text = customId;
        }

        PlayerPrefs.SetString("LastCustomID", customId);

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserData = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
        
    }

    private void OnLoginSuccess(LoginResult result)
    {
        myPlayFabId = result.PlayFabId;
       
        Debug.Log("Login Success.");

        bool hasNameSaved = false;
        if (result.InfoResultPayload.UserData != null && result.InfoResultPayload.UserData.ContainsKey("PlayerName"))
        {
            hasNameSaved = true;
        }

        if (result.NewlyCreated || !hasNameSaved)
        {
            InitializeNewPlayer();
        }
        else
        {
            loginPanel.SetActive(false);
            StartGame();
        }
    }

    private void InitializeNewPlayer()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "InitializeNewPlayer",
            FunctionParameter = new { Email = emailInput.text }
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
            res => {
                Debug.Log("Player Initialized.");
                loginPanel.SetActive(false);
                charSelectPanel.SetActive(true);
            },
            OnError);
    }

    public void SelectCharacter(int type)
    {
        
        if (string.IsNullOrEmpty(heroNameInput.text))
        {
            Debug.LogError("Please enter a hero name!");
            return;
        }

        characterType = type;
        characterName = heroNameInput.text;

        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerName", characterName },
                { "CharacterType", characterType.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
        result => {

            UpdateUserTitleDisplayNameRequest nameRequest = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = characterName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(nameRequest,
                nameResult => {
                    Debug.Log("Title Display Name Updated.");
                    Debug.Log("Character Data Saved!");
                    charSelectPanel.SetActive(false);
                    StartGame();

                }, OnError);
        },
        OnError);
    }

    private void StartGame()
    {
        gamePanel.SetActive(true);
        isPlaying = true;
        gameStartTime = Time.time;

        UpdateUIStats();
        GetPlayerStats();

        StartCoroutine(SpawnRoutine());
    }

    private void UpdateUIStats()
    {
        if (gameStatsText != null)
        {
            gameStatsText.text = $"Level: {localLevel} | XP: {localXP} | Items: {localCollectibles}";
        }

    }

    void UpdateGameDataUI()
    {

        if (playerInfoText)
        {
            string className = (characterType == 1) ? "Villager" : "Warrior";
            playerInfoText.text = $"Hero: {characterName} | Class: {className}";
        }

    }

    private void GetPlayerStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result => {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "Level") localLevel = stat.Value;
                    if (stat.StatisticName == "XP") localXP = stat.Value;
                    if (stat.StatisticName == "Collectibles") localCollectibles = stat.Value;
                }
                UpdateUIStats();
            }, OnError);

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                dataResult => {

                    if (dataResult.Data.ContainsKey("PlayerName"))
                        characterName = dataResult.Data["PlayerName"].Value;

                    if (dataResult.Data.ContainsKey("CharacterType"))
                        characterType = int.Parse(dataResult.Data["CharacterType"].Value);

                    UpdateGameDataUI();
                    isPlaying = true;
                    gameStartTime = Time.time;
                    StartCoroutine(SpawnRoutine());

                }, OnError);
    }


    IEnumerator SpawnRoutine()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(Random.Range(1.5f, 4f));
            SpawnButton();
        }
    }

    void SpawnButton()
    {
        if (collectiblePrefab == null || spawnArea == null) return;

        GameObject btnObj = Instantiate(collectiblePrefab, spawnArea);

        RectTransform areaRect = spawnArea.GetComponent<RectTransform>();
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();

        float width = (areaRect.rect.width / 2) - (btnRect.rect.width / 2);
        float height = (areaRect.rect.height / 2) - (btnRect.rect.height / 2);

        Vector2 randomPos = new Vector2(Random.Range(-width, width), Random.Range(-height, height));
        btnRect.anchoredPosition = randomPos;

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            OnCollectibleClicked();
            Destroy(btnObj);
        });

        Destroy(btnObj, 3f);
    }

    void OnCollectibleClicked()
    {
  
        localCollectibles++;
        UpdateUIStats();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "CollectItem",
            FunctionParameter = new { CharacterType = characterType },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result => {
            var jsonResult = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(result.FunctionResult.ToString());

            if (jsonResult.ContainsKey("newLevel")) localLevel = System.Convert.ToInt32(jsonResult["newLevel"]);
            if (jsonResult.ContainsKey("totalXP")) localXP = System.Convert.ToInt32(jsonResult["totalXP"]);

            UpdateUIStats();

            if (jsonResult.ContainsKey("leveledUp") && (bool)jsonResult["leveledUp"])
            {
                Debug.Log("Level Up! Waiting 60 seconds to send email...");
        
                StartCoroutine(WaitAndSendEmail());
            }

        }, OnError);
    }


    IEnumerator WaitAndSendEmail()
    {
 

        yield return new WaitForSeconds(60f);

        Debug.Log("60 seconds passed. Sending email request...");

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "SendLevelUpEmail",
            FunctionParameter = { }
        },
        res => Debug.Log("Email sent successfully!"),
        OnError);
    }


    public void OpenLeaderboardMenu()
    {
        gamePanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        isPlaying = false;

        if (leaderboardContentText) leaderboardContentText.text = "Select a category...";
    }

    public void CloseLeaderboardMenu()
    {
        leaderboardPanel.SetActive(false);
        gamePanel.SetActive(true);
        isPlaying = true;
        StartCoroutine(SpawnRoutine());
    }

    public void GetLeaderboard(string statName)
    {
        if (leaderboardContentText) leaderboardContentText.text = "Loading...";

        string actualLeaderboardName = statName + "Leaderboard";

        var request = new GetLeaderboardRequest
        {
            StatisticName = statName,
            StartPosition = 0,
            MaxResultsCount = 20
        };

        PlayFabClientAPI.GetLeaderboard(request, result => {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"--- {statName} TOP ---");

            foreach (var item in result.Leaderboard)
            {
                string nameToShow = string.IsNullOrEmpty(item.DisplayName) ? item.PlayFabId : item.DisplayName;
                sb.AppendLine($"{item.Position + 1}. {nameToShow} : {item.StatValue}");
            }

            if (leaderboardContentText) leaderboardContentText.text = sb.ToString();

        }, OnError);
    }

    void OnApplicationQuit()
    {
        if (myPlayFabId != null)
        {
            int sessionTime = (int)(Time.time - gameStartTime);
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpdateSessionStats",
                FunctionParameter = new { TimeSeconds = sessionTime }
            };
            PlayFabClientAPI.ExecuteCloudScript(request, res => Debug.Log("Session Saved"), OnError);
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }
}