using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCPServer;

[RequireComponent (typeof(ServerConnection))]
[RequireComponent (typeof(SceneChange))]
[RequireComponent (typeof(Make_Singleton))]

///<summary>
///Utilities methods
/// </summary>
public static class ExtensionMethods
{
    public static string[] SplitStringIntoArray(this string source)
    {
        return source.Split(',');
    }
}

public class ServerInterface : MonoBehaviour {

    /// <summary>
    /// Please enter the names of the scenes before starting the game.
    /// </summary>
    [SerializeField]
    string waitingRoomSceneName = "The name of your Scene";
    [SerializeField]
    string gameRoomSceneName = "The name of your Scene";

    /// <summary>
    /// This are just some variables for debugging while unity is running.
    /// </summary>
    [SerializeField]
    string serverIPAdrress = "127.0.0.1";
    [SerializeField]
    int serverPortNumber = 7777;
    [SerializeField]
    string userName = "";


    /// <summary>
    /// Initialize the Game Object
    /// </summary>
    private void Awake()
    {
        this.gameObject.tag = "ServerService";
    }

    /// <summary>
    /// Links up the Scenes for the SceneChanger to change scenes.
    /// </summary>
    void Start () {
        GetComponent<ServerConnection>().SetWaitingRoomName(waitingRoomSceneName);
        GetComponent<ServerConnection>().SetGameRoomName(gameRoomSceneName);
    }

    // Update is called once per frame
    void Update () {
		
	}


#region Set server port / ip address and connect to server
    /// <summary>
    /// Function to set the Server's IP Address
    /// </summary>
    /// <param name="ipAddress">
    /// Enter the server ip address. Takes in a String.
    /// </param>
    public void SetServerIPAddress(string ipAddress)
    {
        serverIPAdrress = ipAddress;
    }

    /// <summary>
    /// Function to set port number.
    /// </summary>
    /// <param name="portNumber">
    /// Enter the port number into here. Takes in string. Function will convert it into a int
    /// </param>
    public void SetServerPortNumber(string portNumber)
    {
        int.TryParse(portNumber, out serverPortNumber);
    }


    /// <summary>
    /// this functions connect to the server. Please call this function only when IP address and port number is assigned.
    /// </summary>
    public void ConnectToServer()
    {
        GetComponent<ServerConnection>().ConnecToServer(serverIPAdrress, serverPortNumber);
    }
#endregion

#region Set user name & sent username to server
    /// <summary>
    /// Please set your name before entering the lobby. This is important as I will be using it for the room name.
    /// </summary>
    /// <param name="myName">
    /// Enter the user name for the player. Takes in a string.
    /// </param>
    public void SetName (string myName)
    {
        if (myName.Contains(","))
            return;
        userName = myName;
        PlayerPrefs.SetString("name", userName);
    }

    /// <summary>
    /// Once the user name is set, please call this function next. It will tell the server what name you have decided on.
    /// The function will update your name with a index number to seperate similar names.
    /// </summary>
    public void SendNameToServer()
    {
        userName = GetComponent<ServerConnection>().SendName("name");
    }
#endregion

#region Main-menu Get a list of players in game
    /// <summary>
    /// This function gets a list of player names that are connected to the server.
    /// </summary>
    /// <returns>
    /// Returns a string array of names. 
    /// </returns>
    public string[] GetConnectedPlayers()
    {
        return GetComponent<ServerConnection>().GetPlayerList().SplitStringIntoArray();
    }
    #endregion

#region Get a list of rooms in the play lobby / Get a list of rooms in the spectate lobby
    /// <summary>
    /// This function get the list of rooms that are waiting for players to join them
    /// </summary>
    /// <returns>
    /// Returns a string array of rooms.
    /// </returns>
    public string[] GetListOfRoomsInPlayLobby()
    {
        return GetComponent<ServerConnection>().GetRoomList().SplitStringIntoArray();
    }
    
    /// <summary>
    /// This function get the list of rooms that are already in game. This is for spectating.
    /// </summary>
    /// <returns>
    /// Returns a string array of rooms.
    /// </returns>
    public string[] GetListOfRoomsInSpectateLobby()
    {
        return GetComponent<ServerConnection>().GetSpectateRoomList().SplitStringIntoArray();
    }
#endregion

#region Join / Create / leave a room spectator / player

    /// <summary>
    /// Use this function to join the room that you wish. 
    /// </summary>
    /// <param name="roomName">
    /// Enter the room name that was selected.
    /// </param>
    /// <param name="isSpectator">
    /// If the user is spectating. Please set this to true.
    /// </param>
    public void JoinRoom(string roomName, bool isSpectator = false)
    {
        if(!isSpectator)
            GetComponent<ServerConnection>().JoinRoom(roomName);
        else
            GetComponent<ServerConnection>().JoinRoomSpectator(roomName);
    }

    /// <summary>
    /// A simple function for creating a room. It will use the player name as the room name.
    /// </summary>
    public void CreateRoom()
    {
        GetComponent<ServerConnection>().CreateRoom();
    }

    /// <summary>
    /// A simple function for leaving the room. It will work for spectators or players.
    /// </summary>
    public void LeaveRoom()
    {
        GetComponent<ServerConnection>().LeaveTheRoom();
    }
#endregion

#region (Ready / Unready)
    /// <summary>
    /// A function to tell the server that the player is ready.
    /// </summary>
    /// <param name="ready">
    /// Enter true or false to specify if player is ready
    /// </param>
    public void SetPlayerReady(bool ready)
    {
        GetComponent<ServerConnection>().RoomSetReady(ready);
    }

    /// <summary>
    /// Function to see if opponent is ready.
    /// </summary>
    /// <returns>
    /// Returns a bool. If true, opponent is ready.
    /// </returns>
    public bool GetOpponentReady()
    {
        return GetComponent<ServerConnection>().opponentIsReady;
    }
#endregion

#region Get host and opponent names
    /// <summary>
    /// This function returns the host name.
    /// </summary>
    /// <returns>
    /// Returns a string that is the host name.
    /// </returns>
    public string GetHostName()
    {
        //if (GetComponent<ServerConnection>().isHost)
        //    return userName;
        return GetComponent<ServerConnection>().GetPlayerOne();
    }

    /// <summary>
    /// This function gets the opponent name.
    /// </summary>
    /// <returns>
    /// Returns a string that is the opponent name
    /// </returns>
    public string GetOpponentName()
    {
        return GetComponent<ServerConnection>().GetPlayerTwo();
    }
#endregion

#region Set game mode (AI Game (Singleplayer))
    /// <summary>
    /// Set game mode to VS AI.
    /// </summary>
    public void SetAIGameMode()
    {
        GetComponent<ServerConnection>().SetAIGame();
    }

    /// <summary>
    /// Set the game mode to PVP.
    /// </summary>
    public void UnSetAIGameMode()
    {
        GetComponent<ServerConnection>().UnSetAIGame();
    }
#endregion

#region Set moves on board
    /// <summary>
    /// This is a move placement function. When the player or AI makes a move, Please call this function
    /// </summary>
    /// <param name="x">
    /// Takes in an int value of X. 0 > 14
    /// </param>
    /// <param name="y">
    /// Takes in an int value of Y. 0 > 14
    /// </param>
    public void SetMoveOnBoard(int x, int y)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x, y));
    }

    /// <summary>
    /// Overloaded function for accepting the array index position instead of XY.
    /// </summary>
    /// <param name="arrayNumber">
    /// Takes in an int value of the Array Index Position for the board. 0 - 224.
    /// </param>
    public void SetMoveOnBoard(int arrayNumber)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(arrayNumber);
    }
#endregion

    ///<summary>
    ///This region of functions is reccomended to be placed inside the update function to fetch accurate data.
    /// </summary>
#region Game map data / Get your turn / Get winner / Get player index number
    /// <summary>
    /// Call this function to get the map data
    /// </summary>
    /// <returns>
    /// Returns an int array of size 225. Each value represent the the status of the board. 0 = empty, 1 = black, 2 = white.
    /// </returns>
    public int[] GetMapData()
    {
        return GetComponent<ServerConnection>().GetMapData();
    }

    /// <summary>
    /// Function to check if it is your turn.
    /// </summary>
    /// <returns>
    /// Returns a bool. If true means it is your turn
    /// </returns>
    public bool GetYourTurn()
    {
        return GetComponent<ServerConnection>().GetMyTurn();
    }

    /// <summary>
    /// Function to check if there was any winner.
    /// </summary>
    /// <returns>
    /// Returns an int. 0 = nobody, 1 = black, 2 = white
    /// </returns>
    public int GetWinner() //0 - no one, 1 - player one, 2 - player two
    {
        return GetComponent<ServerConnection>().GetWinner();
    }

    /// <summary>
    /// Check the current player is Black or White.
    /// </summary>
    /// <returns>
    /// Returns an int value. 1 = Black, 2 = white.
    /// </returns>
    public int GetMyIndex()
    {
        return GetComponent<ServerConnection>().MyNumber();
    }
#endregion
}