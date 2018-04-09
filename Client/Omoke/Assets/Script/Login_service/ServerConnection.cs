using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
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

    [SerializeField]
    string lostConnectScene = "Server Disconnect";
    public string name = "";

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
        if (!server.IsConnected() && cts_connection_status != ConnectionStatus.NOT_CONNECTING)
        {
            cts_connection_status = ConnectionStatus.NOT_CONNECTING;
            this.gameObject.GetComponent<SceneChange>().ChangeScene(lostConnectScene);
        }
    }

    //Connect to server
    public void ConnecToServer(String serverIP, int portNumber)
    {
        if(portNumber < 0)
        {
            if (showText)
                showText.StartCoroutine(showText.DisplayText("Invalid port numberl", 3));
            return;
        }
        //Display some text for user feedback
        cts_connection_status = ConnectionStatus.CONNECTING;
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connecting to server", 3));
        //Try to connect to server if failed then return from function and change server to null again
        try
        {
            server = new ConnectionClass(serverIP, portNumber);
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
        PlayerPrefs.SetString("ServerIP", serverIP);
        PlayerPrefs.SetInt("ServerPort", portNumber);
        if (showText)
            showText.StartCoroutine(showText.DisplayText("Connection established", 3));
    }

    //Send set name to the server for recording
    public void SendName(string PlayerPrefName)
    {
        server.SendMessage(PACKET_TYPE.ASSIGN_NAME_PACKET, PlayerPrefs.GetString(PlayerPrefName, "Default001"));
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            name = server.RecieveFromQueue();
            if (a > b)
                break;
        }
        while (!name.Contains(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString()));

        name = Unpack(name);

        if (name != "No Message")
        {
            Debug.Log(name + "(In SendName)");
        }
    }

    //Send current status to the other gameobject that needs server
    public ConnectionStatus GetStatus()
    {
        return cts_connection_status;
    }

    //Get the list of players that is currently online
    public string GetPlayerList()
    {
        server.SendMessage(PACKET_TYPE.GET_ALL_CONNECTED_USERS, "Give me player list");
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
        while (!tempstring.Contains(PACKET_TYPE.GET_ALL_CONNECTED_USERS.ToString()));

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
        server.SendMessage(PACKET_TYPE.GET_MAP_DATA, "Give me map data");
        string tempstring = server.RecieveFromQueue();

        if (tempstring != "No Message")
            Debug.Log(tempstring + "(In GetMapData)");
        return tempstring;
    }

    public void CreateRoom()
    {
        server.SendMessage(PACKET_TYPE.CREATE_NEW_ROOM, "I'm Creating a room");
    }

    public string GetRoomList()
    {
        server.SendMessage(PACKET_TYPE.GET_ROOMS_TO_JOIN, "Give me Room list");
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.GET_ROOMS_TO_JOIN.ToString()));

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

    private string Unpack(string message)
    { 
        return message.Substring(message.IndexOf(":") + 1);
    }
}
