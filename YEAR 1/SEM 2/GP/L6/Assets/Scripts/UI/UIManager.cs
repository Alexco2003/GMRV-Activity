using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public DatabaseManager dbManager;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject databasePanel;

    [Header("Scroll View Setup")]
    public Transform contentContainer;
    public GameObject npcCardPrefab;

    public void OnViewDatabaseClicked()
    {
        if (!dbManager.IsDataReady())
        {
            Debug.LogError("ERROR: No data loaded! Please Load or Generate first.");
            return;
        }

        mainMenuPanel.SetActive(false);
        databasePanel.SetActive(true);

        StartCoroutine(PopulateDatabaseRoutine());
    }

    public void OnCloseDatabaseClicked()
    {
        databasePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private IEnumerator PopulateDatabaseRoutine()
    {
        for (int i = contentContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(contentContainer.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();

        List<NPCData> npcs = dbManager.GetMasterNpcList();

        foreach (NPCData npc in npcs)
        {
            GameObject card = Instantiate(npcCardPrefab, contentContainer);
            NPCUICard cardScript = card.GetComponent<NPCUICard>();
            cardScript.Setup(npc, dbManager);
        }
    }
}