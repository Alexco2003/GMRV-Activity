using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPSend : MonoBehaviour
{
    /* Set the IP of your computer */
    [SerializeField]
    private string IP;
    [SerializeField]
    private Transform targetToTrack;

    private int port;
    IPEndPoint remoteEndPoint;
    UdpClient client;

    public void Start()
    {
        
        
        /* Set a port to send messages to. You can use 1098 */
        port = 1098;
        /* Setup UDP connection for sending messages */
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    /* Send data via UDP */
    private void SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
            Debug.LogError(err.ToString());
        }
    }

    public void Update()
    {
        /* Send data to the tracking app */

        // SendMessage("Hello world!");

        if (targetToTrack != null)
        {
            Vector3 pos = targetToTrack.position;
            Quaternion rot = targetToTrack.rotation;

            string message = $"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w}";
            Debug.Log(message);
            SendMessage(message);
        }
    }
}