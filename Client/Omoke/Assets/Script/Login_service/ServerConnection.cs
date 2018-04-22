using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

    
    //Room swap
    [SerializeField]
    string lostConnectScene = "Server Disconnect";
    [SerializeField]
    string goToRoom = "Room";
    [SerializeField]
    string goToGameRoom = "Game Room";
    public string userName = "";
    public string opponentName = "";

    //Room
    public bool inRoom = false;
    public bool inGame = false;
    public bool opponentInRoom = false;
    public bool opponentIsReady = false;
    public bool isHost = false;
    //Spectator
    public bool isSpectator = false;
    string firstPlayer = "";
    string secondPlayer = "";

    //Connection to server
    ConnectionClass server = null;

    //Game package
    [SerializeField]
    float timerCountDown = 5;
    private DateTime currentTime = DateTime.Now;
    private DateTime targetTime = DateTime.Now;
    public bool receiveNewCurrentGamePacket = false;
    CurrentGameInfo currentGame;

    CurrentGameInfo prevData;

    ConnectionStatus cts_connection_status = ConnectionStatus.NOT_CONNECTING;

    //Used to initialise objects
    private void Start()
    {
        cts_connection_status = ConnectionStatus.NOT_CONNECTING;
        server = null;
        //currentGame = server.CreateGameInformation();
    }

    // Update is called once per frame
    void Update()
    {
        //Check for server
        if (server == null)
            return;

        //Dequeue extra messages that are sent
        if (!inRoom && !inGame)
        {
            string tempstring = server.RecieveFromQueue();

            if (tempstring != "No Message")
                Debug.Log(tempstring + "(In Update)");
        }
        else if(inRoom && !inGame)
        {
            RoomUpdate();
            //Debug.Log("(In Room Update)");
        }
        else if (!inRoom && inGame)
        {
            GameRoomUpdate();
            //Debug.Log("(In Game Update)");
            //Debug.Log("In game room");
        }
    }

    bool CheckIfDataIsValid(CurrentGameInfo checkThisData)
    {
        if (checkThisData.mapData.Length != 225)
            return false;

        return true;
    }

    void GameRoomUpdate()
    {
        string tempstring = server.RecieveFromQueue();
        Debug.Log("GameRoomUpdate: " + tempstring);
        if(tempstring.Contains(PACKET_TYPE.GET_RAW_GAME_INFO.ToString()))
        {

            server.TranslatePacketIntoGameInformation(tempstring, out currentGame);
            if (CheckIfDataIsValid(currentGame))
                CopyData(currentGame);
            else
                currentGame = prevData;

            receiveNewCurrentGamePacket = true;
            //Debug.Log(currentGame.theWinner + " is the winner");
            //Debug.Log("GOT A PACKET OF GAME DATA");
            //Debug.Log(currentGame.isYourTurn + " isit your turn?");
            newTimersForGameUpdate();
        }
        else if (tempstring.Contains(PACKET_TYPE.OPPONENT_DISCONNECTED.ToString()))
        {
            LeaveTheRoom();
            this.GetComponent<SceneChange>().ChangeScene("Room menu");
        }
        if(isHost)
            currentTime = DateTime.Now;
        if(currentTime > targetTime)
        {
            targetTime = DateTime.Now.AddSeconds(timerCountDown);
            server.SendMessage(PACKET_TYPE.FORCED_UPDATE, "FORCING UPDATE!!!!!!!!!");
        }
    }

    void RoomUpdate()
    {
        string tempstring = server.RecieveFromQueue();
        Debug.Log("Room Update: " + tempstring);
        if (tempstring.Contains(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString()))
        {
            tempstring = Unpack(tempstring);
            opponentName = tempstring;
            opponentInRoom = true;
            Debug.Log("Opponent Joined the game:" + opponentName);
        }
        else if(tempstring.Contains(PACKET_TYPE.LEAVE_ROOM.ToString()) || tempstring.Contains(PACKET_TYPE.OPPONENT_DISCONNECTED.ToString()))
        {
            Debug.Log("Opponent Left the room: " + opponentName);
            opponentName = "";
            opponentInRoom = false;
            opponentIsReady = false;
            isHost = true;
        }
        else if (tempstring.Contains(PACKET_TYPE.PLAYER_IS_READY.ToString()))
        {
            opponentIsReady = true;
            //Debug.Log("Opponent ready");
        }
        else if (tempstring.Contains(PACKET_TYPE.PLAYER_UNREADY.ToString()))
        {
            opponentIsReady = false;
            //Debug.Log("Opponent not ready");
        }
        else if (tempstring.Contains(PACKET_TYPE.START_GAME_SUCCESS.ToString()))
        {
            inRoom = false;
            opponentInRoom = false;
            opponentIsReady = false;
            //isHost = false;
            inGame = true;
            receiveNewCurrentGamePacket = false;
            this.GetComponent<SceneChange>().ChangeScene(goToGameRoom);
            //Debug.Log("Game success: " + tempstring);
            //Debug.Log("Change to game already");
        }
        else if (tempstring.Contains(PACKET_TYPE.START_GAME_FAILURE.ToString()))
        {
            LeaveTheRoom();
            this.GetComponent<SceneChange>().ChangeScene("Room menu");
        }
        else if (tempstring.Contains(PACKET_TYPE.GET_RAW_GAME_INFO.ToString()))
        {

            server.TranslatePacketIntoGameInformation(tempstring, out currentGame);
            if (CheckIfDataIsValid(currentGame))
                CopyData(currentGame);
            else
                currentGame = prevData;

            receiveNewCurrentGamePacket = true;
            //Debug.Log(currentGame.theWinner + " is the winner");
            //Debug.Log("GOT A PACKET OF GAME DATA");
            //Debug.Log(currentGame.isYourTurn + " isit your turn?");
        }
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

    public void SetAIGame()
    {
        server.SendMessage(PACKET_TYPE.SET_AI_GAME, "Setting AI Game");
    }

    public void UnSetAIGame()
    {
        server.SendMessage(PACKET_TYPE.UNSET_AI_GAME, "Setting AI Game");
    }

    //Send click location of board
    public void SetMoveOnBoard(int boardArray)
    {
        server.SendMessage(PACKET_TYPE.SET_MY_MOVE, boardArray);
        string tempstring = "";
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            if (a > b)
                return;
        }
        while (!tempstring.Contains(PACKET_TYPE.GET_RAW_GAME_INFO.ToString()));
        server.TranslatePacketIntoGameInformation(tempstring, out currentGame);
        if (CheckIfDataIsValid(currentGame))
            CopyData(currentGame);
        else
            currentGame = prevData;

        receiveNewCurrentGamePacket = true;
        Debug.Log("GOT A PACKET OF GAME DATA");
        Debug.Log(currentGame.theWinner + " is the winner");
        Debug.Log(currentGame.isYourTurn + " isit your turn?");
        newTimersForGameUpdate();
    }

    void CopyData(CurrentGameInfo theGameInfo)
    {
        prevData.mapData = theGameInfo.mapData;
        prevData.theWinner = theGameInfo.theWinner;
        prevData.isYourTurn = theGameInfo.isYourTurn;
        prevData.myIndexNumber = theGameInfo.myIndexNumber;
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
        currentGame = server.CreateGameInformation();
        CopyData(currentGame);

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
            userName = server.RecieveFromQueue();
            Debug.Log("SendName: " + userName);
            if (a > b)
                break;
        }
        while (!userName.Contains(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString()));

        userName = Unpack(userName);

        if (userName != "No Message")
        {
            Debug.Log(userName + "(In SendName)");
            PlayerPrefs.SetString("IGN", userName);
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
            Debug.Log("GetplayerList: " + tempstring);
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
    public int[] GetMapData()
    {
        return currentGame.mapData;
    }

    //Get the winner
    public int GetWinner()
    {
        return currentGame.theWinner;
    }
    
    public bool GetMyTurn()
    {
        //Debug.Log(currentGame);
        return currentGame.isYourTurn;
    }

    public int MyNumber()
    {
        Debug.Log(currentGame.myIndexNumber + " is my number");
        return currentGame.myIndexNumber;
    }
    //Create a room with your name
    public void CreateRoom()
    {
        isHost = true;
        inRoom = true;
        server.SendMessage(PACKET_TYPE.CREATE_NEW_ROOM, "I'm Creating a room");
    }

    //Get a list of rooms from the server
    public string GetRoomList()
    {
        server.SendMessage(PACKET_TYPE.GET_ROOMS_TO_JOIN, "Give me Room list");
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            Debug.Log("GetRoomList: " + tempstring);
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

    public string GetSpectateRoomList()
    {
        server.SendMessage(PACKET_TYPE.GET_ROOMS_TO_SPECTATE, "Give me Room spectate list");
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            Debug.Log("GetSpectateRoomList: " + tempstring);
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.GET_ROOMS_TO_SPECTATE.ToString()));

        tempstring = Unpack(tempstring);

        if (tempstring != "No Message")
        {
            Debug.Log(tempstring + "(In GetSpectateRoomList)");
            return tempstring;
        }
        return "No Rooms Available.";
    }

    //Join the room that is named specified
    public void JoinRoom(string roomName)
    {
        //if (roomName == "")
        //    return;
        opponentIsReady = false;
        server.SendMessage(PACKET_TYPE.JOIN_ROOM, roomName);
        isHost = false;
        //Check server reply
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            Debug.Log("JoinRoom: " + tempstring);
            //Debug.Log(a + tempstring);
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString()));

        if (tempstring.Contains(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString()))
        {
            inRoom = true;
            tempstring = Unpack(tempstring);
            Debug.Log(tempstring + "(In JoinRoom)");
            opponentName = tempstring;
            this.gameObject.GetComponent<SceneChange>().ChangeScene(goToRoom);
            opponentInRoom = true;
            return;
        }
        //Debug.Log(tempstring + "(In JoinRoom)");
    }

    //Joining room as spectator
    public void JoinRoomSpectator(string roomName)
    {
        //if (roomName == "")
        //    return;
        //opponentIsReady = false;
        server.SendMessage(PACKET_TYPE.JOIN_ROOM_SPECTATE, roomName);
        isHost = false;
        //Check server reply
        string tempstring;
        DateTime b = DateTime.Now.AddSeconds(3);
        do
        {
            DateTime a = DateTime.Now;
            tempstring = server.RecieveFromQueue();
            Debug.Log("JoinRoomSpectator: " + tempstring);
            //Debug.Log(a + tempstring);
            if (a > b)
                break;
        }
        while (!tempstring.Contains(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString()));

        if (tempstring.Contains(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString()))
        {
            isSpectator = inRoom = true;
            tempstring = Unpack(tempstring);
            Debug.Log(tempstring + "(In JoinRoomSpectator)");
            string[] strArray = tempstring.Split(',');
            firstPlayer = strArray[0];
            secondPlayer = strArray[1];
            this.gameObject.GetComponent<SceneChange>().ChangeScene(goToGameRoom);
            return;
        }
    }

    public string getOpponentName()
    {
        return opponentName;
    }

    //Get player one and two names
    public string GetPlayerOne()
    {
        return firstPlayer;
    }

    public string GetPlayerTwo()
    {
        return secondPlayer;
    }

    //When leaving room reset all booleans
    public void LeaveTheRoom()
    {
        isSpectator = inRoom = isHost = inGame = opponentIsReady = opponentInRoom = false;
        server.SendMessage(PACKET_TYPE.LEAVE_ROOM, "I'm Leaving the room");
    }

    //Set the flag of ready or not to the server so the other client or the game will be able to start
    public void RoomSetReady(bool readyOrNot)
    {
        server.SendMessage((readyOrNot == true ? PACKET_TYPE.PLAYER_IS_READY : PACKET_TYPE.PLAYER_UNREADY), "");
        Debug.Log("Player setting ready or not");
    }

    //If this is host send start game
    public void RoomStartGame()
    {
        if (isHost)
        {
            server.SendMessage(PACKET_TYPE.START_GAME, "");
            Debug.Log("Host starting game");
            return;
        }
        Debug.Log("Waiting for host to start");
    }

    //Return to login will need to destroy this object
    public void ServerDestroy()
    {
        Destroy(this.gameObject);
    }

    private void newTimersForGameUpdate()
    {
        currentTime = DateTime.Now;
        targetTime = DateTime.Now.AddSeconds(timerCountDown);
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

    //Unpack the messages received from the server
    private string Unpack(string message)
    { 
        return message.Substring(message.IndexOf(":") + 1);
    }
}
