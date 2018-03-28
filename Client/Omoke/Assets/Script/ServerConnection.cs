using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServerConnection : MonoBehaviour {

    public Transform Cube;
    public String serverIP_Display;

    byte[] data = new byte[1024];
    IPEndPoint ipep = null;
    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H) && ipep != null)
        {
            Debug.Log("Sending Position");

            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(Cube.position.ToString());
            server.SendTo(data, data.Length, SocketFlags.None, ipep);

            Debug.Log("Function Ended");
        }

        if (Input.GetKey(KeyCode.S) && ipep != null)
        {
            Debug.Log("Disconnecting from server...");
            server.Shutdown(SocketShutdown.Send);
            //server.Close();
        }
    }

    public void ConnecToServer(String serverIP)
    {
        serverIP_Display = serverIP;
        Debug.Log(Cube.position);
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 7777);
        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server.");
            return;
        }

        string welcome = "Hello, what's up?";
        data = Encoding.ASCII.GetBytes(welcome);
        server.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint tmpRemote = (EndPoint)sender;

        data = new byte[1024];
        int recv = server.ReceiveFrom(data, ref tmpRemote);

        Debug.Log(String.Format("Message received from {0}:", tmpRemote.ToString()));
        Debug.Log(String.Format(Encoding.ASCII.GetString(data, 0, recv)));

        data = Encoding.ASCII.GetBytes(Cube.position.ToString());
        server.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    public void SendName(string PlayerPrefName)
    {
        byte[] data = new byte[1024];
        data = Encoding.ASCII.GetBytes(PlayerPrefs.GetString(PlayerPrefName,"User"));
        server.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    private void OnApplicationQuit()
    {
        if (ipep == null)
            return;
        Debug.Log("Disconnecting from server...");
        server.Shutdown(SocketShutdown.Send);
    }
}
