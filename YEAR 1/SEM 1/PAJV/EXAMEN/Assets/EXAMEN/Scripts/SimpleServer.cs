using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace PAJV
{
    public class SimpleServer : MonoBehaviour
    {
        [SerializeField] private XmlUnityServer riftServer;

        private struct PlayerData
        {
            public ushort ID;
            public int Slot;
            public int Health;
            public float R, G, B;
            public float X, Y, Z;
            public float RotY;
        }

        private Dictionary<ushort, PlayerData> connectedPlayers = new Dictionary<ushort, PlayerData>();
        private bool slot1Occupied = false;
        private bool slot2Occupied = false;

        private void Start()
        {
            Application.runInBackground = true;
            riftServer.Server.ClientManager.ClientConnected += HandleClientConnected;
            riftServer.Server.ClientManager.ClientDisconnected += HandleClientDisconnected;
        }

        private void HandleClientConnected(object sender, ClientConnectedEventArgs args)
        {
            if (connectedPlayers.Count >= 2)
            {
                using (Message msg = Message.Create(4, DarkRiftWriter.Create()))
                {
                    args.Client.SendMessage(msg, SendMode.Reliable);
                }
                args.Client.Disconnect();
                return;
            }

            int assignedSlot = 0;
            if (!slot1Occupied) { assignedSlot = 1; slot1Occupied = true; }
            else if (!slot2Occupied) { assignedSlot = 2; slot2Occupied = true; }

            float r = (assignedSlot == 1) ? 1f : 0f;
            float g = 0f;
            float b = (assignedSlot == 2) ? 1f : 0f;
            float startX = (assignedSlot == 1) ? -3f : 3f;

            PlayerData newPlayer = new PlayerData
            {
                ID = args.Client.ID,
                Slot = assignedSlot,
                Health = 100,
                R = r,
                G = g,
                B = b,
                X = startX,
                Y = 1f,
                Z = 0f,
                RotY = (assignedSlot == 1) ? 90f : -90f
            };

            connectedPlayers.Add(args.Client.ID, newPlayer);

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                WritePlayerSpawnData(writer, newPlayer);
                using (Message msg = Message.Create(0, writer))
                    args.Client.SendMessage(msg, SendMode.Reliable);
            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                WritePlayerSpawnData(writer, newPlayer);
                using (Message msg = Message.Create(0, writer))
                {
                    foreach (IClient connectedClient in riftServer.Server.ClientManager.GetAllClients())
                    {
                        if (connectedClient.ID != args.Client.ID)
                            connectedClient.SendMessage(msg, SendMode.Reliable);
                    }
                }
            }

            foreach (var existingPlayer in connectedPlayers.Values)
            {
                if (existingPlayer.ID == args.Client.ID) continue;

                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    WritePlayerSpawnData(writer, existingPlayer);
                    using (Message msg = Message.Create(0, writer))
                        args.Client.SendMessage(msg, SendMode.Reliable);
                }
            }

            args.Client.MessageReceived += HandleClientMessageReceived;
            Debug.Log($"[Server] Player connected on Slot {assignedSlot} (ID: {args.Client.ID})");
        }

        private void WritePlayerSpawnData(DarkRiftWriter writer, PlayerData p)
        {
            writer.Write(p.ID);
            writer.Write(p.Slot);
            writer.Write(p.Health);
            writer.Write(p.R); writer.Write(p.G); writer.Write(p.B);
            writer.Write(p.X); writer.Write(p.Y); writer.Write(p.Z);
        }

        private void HandleClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            if (connectedPlayers.ContainsKey(args.Client.ID))
            {
                var p = connectedPlayers[args.Client.ID];
                if (p.Slot == 1) slot1Occupied = false;
                if (p.Slot == 2) slot2Occupied = false;

                connectedPlayers.Remove(args.Client.ID);

                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(args.Client.ID);
                    writer.Write(p.Slot);
                    using (Message msg = Message.Create(3, writer))
                    {
                        foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                            client.SendMessage(msg, SendMode.Reliable);
                    }
                }
                Debug.Log($"[Server] ID {args.Client.ID} disconnected.");
            }
        }

        private void HandleClientMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            using (Message message = args.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
                if (message.Tag == 1) // Miscare
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float rotY = reader.ReadSingle();

                    if (connectedPlayers.ContainsKey(args.Client.ID))
                    {
                        var p = connectedPlayers[args.Client.ID];
                        p.X = x; p.Y = y; p.Z = z; p.RotY = rotY;
                        connectedPlayers[args.Client.ID] = p;
                    }

                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(args.Client.ID);
                        writer.Write(x); writer.Write(y); writer.Write(z);
                        writer.Write(rotY);

                        using (Message outMsg = Message.Create(1, writer))
                        {
                            foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                            {
                                if (client.ID != args.Client.ID)
                                    client.SendMessage(outMsg, SendMode.Unreliable);
                            }
                        }
                    }
                }

                // Tag 2: CHAT
                else if (message.Tag == 2)
                {
                    string text = reader.ReadString();

                    int senderSlot = 0;
                    if (connectedPlayers.ContainsKey(args.Client.ID))
                    {
                        senderSlot = connectedPlayers[args.Client.ID].Slot;
                    }

                    Debug.Log($"Chat from Slot {senderSlot}: {text}");

                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(senderSlot);
                        writer.Write(text);

                        using (Message outMsg = Message.Create(2, writer))
                        {
                            foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                                client.SendMessage(outMsg, SendMode.Reliable);
                        }
                    }
                }

                // Tag 5: DAMAGE (Hit)
                else if (message.Tag == 5)
                {
                    ushort targetID = reader.ReadUInt16();

                    if (connectedPlayers.ContainsKey(targetID))
                    {
                        var target = connectedPlayers[targetID];
                        target.Health -= 10;

               
                        if (target.Health <= 0)
                        {
                            Debug.Log($"[Combat] Player {targetID} (Slot {target.Slot}) died. Disconnecting...");

                            IClient clientToKick = riftServer.Server.ClientManager.GetClient(targetID);
                            if (clientToKick != null)
                            {
                                clientToKick.Disconnect();
                            }
                        }
                        else
                        {
                            
                            connectedPlayers[targetID] = target;

                            using (DarkRiftWriter writer = DarkRiftWriter.Create())
                            {
                                writer.Write(target.Slot);
                                writer.Write(target.Health);

                                using (Message hpMsg = Message.Create(5, writer))
                                {
                                    foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                                        client.SendMessage(hpMsg, SendMode.Reliable);
                                }
                            }

                            Debug.Log($"[Combat] Slot {target.Slot} HP: {target.Health}");
                        }
                       
                    }
                }
            }
        }
    }
}