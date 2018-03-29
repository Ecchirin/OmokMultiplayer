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
    NOT_CONNECTING = 0,
    CONNECTING,
    CONNECTED,
};

public class ServerConnection : MonoBehaviour {

    public TextDisplay showText = null;

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
        if (server == null)
            return;
        //All these should change later
        if (Input.GetKey(KeyCode.H) && server != null)
        {
            server.SendPosition(50);
        }

        if (Input.GetKey(KeyCode.S) && server != null)
        {
            Debug.Log("Disconnecting from server...");
            if (showText)
                showText.StartCoroutine(showText.DisplayText("Disconnecting from server...", 3));
            server.ShutdownThread();
            server.DisconnectFromServer();
        }

        if(server.RecieveFromQueue() != "")
        {
            Debug.Log(server.RecieveFromQueue());
        }
    }

    public void ConnecToServer(String serverIP)
    {
        cts_connection_status = ConnectionStatus.CONNECTING;
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connecting to server", 3));
        server = new ConnectionClass(serverIP, 7777);
        if (server.ConnectToServer())
            Debug.Log("Connected");
        else
        {
            Debug.Log("Unable to connect");
            cts_connection_status = ConnectionStatus.NOT_CONNECTING;
            if (showText)
                showText.StartCoroutine(showText.DisplayText("Cannot connect to server try again", 3));
            return;
        }
        cts_connection_status = ConnectionStatus.CONNECTED;
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connection established", 3));
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
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Disconnecting from server...", 3));
        cts_connection_status = ConnectionStatus.NOT_CONNECTING;
        server.ShutdownThread();
        server.DisconnectFromServer();
    }

    public ConnectionStatus GetStatus()
    {
        return cts_connection_status;
    }
}
