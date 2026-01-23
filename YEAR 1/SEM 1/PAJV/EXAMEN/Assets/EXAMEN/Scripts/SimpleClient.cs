using System;
using System.Collections.Generic;
using System.Net;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;
using TMPro;

namespace PAJV
{
    public class SimpleClient : MonoBehaviour
    {
        [Header("Networking")]
        [SerializeField] private UnityClient riftClient;
        [SerializeField] private string ipAddress = "127.0.0.1";

        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;

        [Header("UI Fighting")]
        [SerializeField] private GameObject panelP1;
        [SerializeField] private GameObject panelP2;
        [SerializeField] private RectTransform healthRectP1;
        [SerializeField] private RectTransform healthRectP2;

        [Header("UI Chat")]
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private TextMeshProUGUI chatHistoryText;

        private float maxWidthP1;
        private float maxWidthP2;
        private Dictionary<ushort, GameObject> spawnedPlayers = new Dictionary<ushort, GameObject>();
        private List<string> chatMessages = new List<string>();


        private PlayerController localPlayerController;

        private void Start()
        {
        
            if (panelP1)
            {
                if (healthRectP1) maxWidthP1 = healthRectP1.rect.width;
                panelP1.SetActive(false);
            }
            if (panelP2)
            {
                if (healthRectP2) maxWidthP2 = healthRectP2.rect.width;
                panelP2.SetActive(false);
            }

   
            if (chatInput != null)
                chatInput.onSubmit.AddListener(SendChatMessage);

            riftClient.ConnectInBackground(IPAddress.Parse(ipAddress), 4296, 4297, true, OnConnected);
        }

        private void Update()
        {
           
            if (localPlayerController != null && chatInput != null)
            {
                localPlayerController.InputDisabled = chatInput.isFocused;
            }
        }

        private void OnConnected(Exception e)
        {
            if (riftClient.ConnectionState == ConnectionState.Connected)
            {
                Debug.Log("Connected to server!");
                riftClient.MessageReceived += HandleMessage;
            }
        }

        private void SendChatMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(text);
                using (Message msg = Message.Create(2, writer))
                {
                    riftClient.SendMessage(msg, SendMode.Reliable);
                }
            }
            chatInput.text = "";
            chatInput.ActivateInputField();
        }

        private void UpdateChatUI(string newMsg)
        {
            chatMessages.Add(newMsg);
            if (chatMessages.Count > 5) chatMessages.RemoveAt(0);
            if (chatHistoryText) chatHistoryText.text = string.Join("\n", chatMessages);
        }
        

        private void UpdateHealthUI(RectTransform rect, float currentHp, float maxWidth)
        {
            if (rect == null) return;
            float percentage = Mathf.Clamp01(currentHp / 100f);
            rect.sizeDelta = new Vector2(percentage * maxWidth, rect.sizeDelta.y);
        }

        private void HandleMessage(object sender, MessageReceivedEventArgs args)
        {
            using (Message message = args.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
                if (message.Tag == 0) // Spawn
                {
                    ushort id = reader.ReadUInt16();
                    int slot = reader.ReadInt32();
                    int hp = reader.ReadInt32();
                    float r = reader.ReadSingle();
                    float g = reader.ReadSingle();
                    float b = reader.ReadSingle();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();

                    if (!spawnedPlayers.ContainsKey(id))
                    {
                        GameObject obj = Instantiate(playerPrefab);
                        obj.name = $"Player_{id}_Slot{slot}";
                        obj.transform.position = new Vector3(x, y, z);
                        obj.GetComponent<Renderer>().material.color = new Color(r, g, b);

               
                        PlayerController pc = obj.GetComponent<PlayerController>();
                        if (pc == null) pc = obj.AddComponent<PlayerController>();

                        bool isLocal = (id == riftClient.ID);
                        pc.Initialize(riftClient, id, isLocal);

                        if (isLocal) localPlayerController = pc; 

                        spawnedPlayers.Add(id, obj);
                    }

                    if (slot == 1) { panelP1.SetActive(true); UpdateHealthUI(healthRectP1, hp, maxWidthP1); }
                    else if (slot == 2) { panelP2.SetActive(true); UpdateHealthUI(healthRectP2, hp, maxWidthP2); }
                }

                else if (message.Tag == 1) // Movement
                {
                    ushort id = reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float rotY = reader.ReadSingle();

                    if (spawnedPlayers.ContainsKey(id))
                    {
                        spawnedPlayers[id].transform.position = new Vector3(x, y, z);
                        spawnedPlayers[id].transform.rotation = Quaternion.Euler(0, rotY, 0);
                    }
                }

                // Chat
                else if (message.Tag == 2)
                {
                    
                    int senderSlot = reader.ReadInt32();
                    string text = reader.ReadString();

                    string colorHex = (senderSlot == 1) ? "#FF0000" : "#0000FF";

                    string formattedMsg = $"<color={colorHex}>Player {senderSlot}:</color> {text}";

                    UpdateChatUI(formattedMsg);
                }

                else if (message.Tag == 3) // Disconnect
                {
                    ushort id = reader.ReadUInt16();
                    int slot = reader.ReadInt32();

                    if (spawnedPlayers.ContainsKey(id))
                    {
                        Destroy(spawnedPlayers[id]);
                        spawnedPlayers.Remove(id);
                    }
                    if (slot == 1) panelP1.SetActive(false);
                    if (slot == 2) panelP2.SetActive(false);
                }

                else if (message.Tag == 5) // Health Update
                {
                    int slot = reader.ReadInt32();
                    int newHp = reader.ReadInt32();

                    if (slot == 1) UpdateHealthUI(healthRectP1, newHp, maxWidthP1);
                    else if (slot == 2) UpdateHealthUI(healthRectP2, newHp, maxWidthP2);
                }
            }
        }
    }
}