using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using TCPServer;

/// <summary>
/// This class will use the server interface for connection and packet sending
/// This class will tell other gameobject connection status and 
/// </summary>

public enum ConnectionStatus
{
    NOT_CONNECTING,
    CONNECTING,
    CONNECTED,
};

public class ServerConnection : MonoBehaviour {

    public Transform Cube;
    public String serverIP_Display;

    ConnectionClass server = null;

    ConnectionStatus cts_connection_status = ConnectionStatus.NOT_CONNECTING;

    private void Start()
    {
        cts_connection_status = ConnectionStatus.NOT_CONNECTING;
        server = null;
    }

    // Update is called once per frame
    void Update()
    {
        //All these should change later
        if (Input.GetKey(KeyCode.H) && server != null)
        {
            server.SendPosition(50);
        }

        if (Input.GetKey(KeyCode.S) && server != null)
        {
            Debug.Log("Disconnecting from server...");
            server.DisconnectFromServer();
        }
    }

    public void ConnecToServer(String serverIP)
    {
        cts_connection_status = ConnectionStatus.CONNECTING;
        serverIP_Display = serverIP;
        Debug.Log(Cube.position);
        server = new ConnectionClass(serverIP, 7777);
        if (server.ConnectToServer())
            Debug.Log("Connected");
        else
        {
            Debug.Log("Unable to connect");
            cts_connection_status = ConnectionStatus.NOT_CONNECTING;
            return;
        }

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint tmpRemote = (EndPoint)sender;

        Debug.Log(String.Format("Message received from {0}:", tmpRemote.ToString()));
        cts_connection_status = ConnectionStatus.CONNECTED;
    }

    public void SendName(string PlayerPrefName)
    {
        server.SendMessage(PACKET_TYPE.SET_NAME_PACKET, PlayerPrefs.GetString(PlayerPrefName, "Default001"));
    }

    private void OnApplicationQuit()
    {
        if (server == null)
            return;
        Debug.Log("Disconnecting from server...");
        cts_connection_status = ConnectionStatus.NOT_CONNECTING;
        server.DisconnectFromServer();
    }
}
