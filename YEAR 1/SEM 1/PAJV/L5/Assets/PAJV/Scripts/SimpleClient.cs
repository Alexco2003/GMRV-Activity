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

        [Header("Prefabs")]
        [SerializeField] private GameObject playerPrefab;

        [Header("Chat References")]
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private TextMeshProUGUI chatHistoryText;

        private Dictionary<ushort, GameObject> spawnedPlayers = new Dictionary<ushort, GameObject>();
        private List<string> chatMessages = new List<string>();

        private PlayerController localPlayerController;

        private void Start()
        {
            riftClient.ConnectInBackground(IPAddress.Parse(ipAddress), 4296, 4297, true, OnConnected);

            if (chatInput != null)
                chatInput.onSubmit.AddListener(SendChatMessage);
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
                using (Message msg = Message.Create(2, writer)) // Tag 2 Chat
                {
                    riftClient.SendMessage(msg, SendMode.Reliable);
                }
            }

            Debug.Log($"Sent chat message: {text}");

            chatInput.text = "";
            chatInput.ActivateInputField();
        }

        private void HandleMessage(object sender, MessageReceivedEventArgs args)
        {
            using (Message message = args.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
                if (message.Tag == 0)
                {
                    ushort id = reader.ReadUInt16();
                    float r = reader.ReadSingle();
                    float g = reader.ReadSingle();
                    float b = reader.ReadSingle();

                    GameObject obj = Instantiate(playerPrefab);
                    obj.name = $"Player_{id}";

                    obj.GetComponent<Renderer>().material.color = new Color(r, g, b);

                    if (id == riftClient.ID)
                    {
                        obj.AddComponent<PlayerController>();
                        localPlayerController = obj.GetComponent<PlayerController>();
                        localPlayerController.Initialize(riftClient);

                        obj.transform.position = new Vector3(0, 1, 0);
                    }
                    else
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        obj.transform.position = new Vector3(x, y, z);
                    }

                    spawnedPlayers.Add(id, obj);
                    Debug.Log($"Spawned Player {id}");
                }

                else if (message.Tag == 1)
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

                    Debug.Log($"Received movement update for Player {id}");
                }
           
                else if (message.Tag == 2)
                {
                    ushort senderId = reader.ReadUInt16();
                    string text = reader.ReadString();
                    UpdateChatUI($"Player {senderId}: {text}");
                    Debug.Log($"Received chat message from Player {senderId}: {text}");
                }
        
                else if (message.Tag == 3)
                {
                    ushort id = reader.ReadUInt16();
                    if (spawnedPlayers.ContainsKey(id))
                    {
                        Destroy(spawnedPlayers[id]);
                        spawnedPlayers.Remove(id);
                    }

                    Debug.Log($"Player {id} disconnected and was removed.");
                }
            }
        }

        private void UpdateChatUI(string newMsg)
        {
            chatMessages.Add(newMsg);

            if (chatMessages.Count > 5)
            {
                chatMessages.RemoveAt(0);
            }

            chatHistoryText.text = string.Join("\n", chatMessages);
        }
    }
}