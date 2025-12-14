using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    private int port;

    private Vector3 receivedPos = Vector3.zero;
    private Quaternion receivedRot = Quaternion.identity;
    private bool newDataReceived = false;

    public void Start()
    {
        /* Set a port to listen to (same port you will use on the headset) */
        port = 1098;
        /* Setup UDP for receiving data */
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    /* Receive messages via UDP */
    private void ReceiveData()
    {
        client = new UdpClient(port);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                string[] coords = text.Split(',');

                float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                float y = float.Parse(coords[1], CultureInfo.InvariantCulture);
                float z = float.Parse(coords[2], CultureInfo.InvariantCulture);

                float rx = float.Parse(coords[3], CultureInfo.InvariantCulture);
                float ry = float.Parse(coords[4], CultureInfo.InvariantCulture);
                float rz = float.Parse(coords[5], CultureInfo.InvariantCulture);
                float rw = float.Parse(coords[6], CultureInfo.InvariantCulture);

                receivedPos = new Vector3(x, y, z);
                receivedRot = new Quaternion(rx, ry, rz, rw);
                newDataReceived = true;

            }
            catch (Exception err)
            {
                print(err.ToString());
                Debug.LogError(err.ToString());
            }
        }
    }

    private void Update()
    {
        if (newDataReceived)
        {
            transform.position = receivedPos;
            transform.rotation = receivedRot;
            newDataReceived = false;
        }
    }

    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }
}