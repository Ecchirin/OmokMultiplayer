﻿using System.Collections;
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
        //Check for server
        if (server == null)
            return;

        //All these should change later
        if (Input.GetKey(KeyCode.H))
        {
            //server.SendPosition(50);
            GetPlayerList();
            GetMapData();
            //Debug.Log(server.RecieveFromQueue());
        }



        //Dequeue extra messages that are sent
        string tempstring = server.RecieveFromQueue();

        if (tempstring != "No Message")
            Debug.Log(tempstring + "(In Update)");
    }

    //Called at every few frames
    private void FixedUpdate()
    {
        if (server == null)
            return;
        if (!server.CheckConnection())
            cts_connection_status = ConnectionStatus.NOT_CONNECTING;
    }

    //Connect to server
    public void ConnecToServer(String serverIP)
    {
        //Display some text for user feedback
        cts_connection_status = ConnectionStatus.CONNECTING;
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connecting to server", 3));
        //Try to connect to server if failed then return from function and change server to null again
        try
        {
            server = new ConnectionClass(serverIP, 7777);
        }
        catch(Exception e)
        {
            Debug.Log("Unable to connect");
            cts_connection_status = ConnectionStatus.NOT_CONNECTING;
            server = null;
            if (showText)
                showText.StartCoroutine(showText.DisplayText("Cannot connect to server try again", 3));
            return;
        }
        //Display some text for user feedback
        cts_connection_status = ConnectionStatus.CONNECTED;
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connection established", 3));
    }

    //Send set name to the server for recording
    public void SendName(string PlayerPrefName)
    {
        server.SendMessage(PACKET_TYPE.SET_NAME_PACKET, PlayerPrefs.GetString(PlayerPrefName, "Default001"));
    }

    //Send current status to the other gameobject that needs server
    public ConnectionStatus GetStatus()
    {
        return cts_connection_status;
    }

    //Get the list of players that is currently online
    public string GetPlayerList()
    {
        server.SendMessage(PACKET_TYPE.LOBBY_LIST, "Give me player list");
        //WaitForSeconds(1);
        string tempstring /*= server.RecieveFromQueue()*/;

        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.LOBBY_LIST.ToString()));

        tempstring = Unpack(tempstring);

        if (tempstring != "No Message")
        {
            Debug.Log(tempstring + "(In GetPlayerList)");
            return tempstring;
        }
        return "Unable to fetch list";
    }

    //Get the data of the map
    public string GetMapData()
    {
        server.SendMessage(PACKET_TYPE.MAP_DATA, "Give me map data");
        string tempstring = server.RecieveFromQueue();

        if (tempstring != "No Message")
            Debug.Log(tempstring + "(In GetMapData)");
        return tempstring;
    }

    public void CreateRoom()
    {
        server.SendMessage(PACKET_TYPE.CREATE_ROOM, "");
    }

    public string GetRoomList()
    {
        server.SendMessage(PACKET_TYPE.ROOM_LIST, "Give me Room list");
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.ROOM_LIST.ToString()));

        tempstring = Unpack(tempstring);

        if (tempstring != "No Message")
        {
            Debug.Log(tempstring + "(In GetRoomList)");
            return tempstring;
        }
        return "No Rooms Available.";
    }

    public void JoinRoom(string roomName)
    {
        if (roomName == "")
            return;
        server.SendMessage(PACKET_TYPE.JOIN_ROOM, roomName);
    }

    //When gameobject detect application has been closed this will close the connection to the server (prevent server crash)
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

    string Unpack(string message)
    {
        message = message.Substring(message.IndexOf(":")+1);
        return message;
    }
}
