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

    [SerializeField]
    string waitingRoomSceneName = "";
    [SerializeField]
    string gameRoomSceneName = "";

    [SerializeField]
    string serverIPAdrress = "127.0.0.1";
    [SerializeField]
    int serverPortNumber = 7777;
    [SerializeField]
    string userName = "";

    private void Awake()
    {
        this.gameObject.tag = "ServerService";
    }

    // Use this for initialization
    void Start () {
        GetComponent<ServerConnection>().SetWaitingRoomName(waitingRoomSceneName);
        GetComponent<ServerConnection>().SetGameRoomName(gameRoomSceneName);
    }

    // Update is called once per frame
    void Update () {
		
	}


#region Set server port / ip address and connect to server
    public void SetServerIPAddress(string ipAddress)
    {
        serverIPAdrress = ipAddress;
    }

    public void SetServerPortNumber(string portNumber)
    {
        int.TryParse(portNumber, out serverPortNumber);
    }

    public void ConnectToServer()
    {
        GetComponent<ServerConnection>().ConnecToServer(serverIPAdrress, serverPortNumber);
    }
#endregion

#region Set user name & sent username to server
    public void SetName (string myName)
    {
        if (myName.Contains(","))
            return;
        userName = myName;
        PlayerPrefs.SetString("name", userName);
    }

    public void SendNameToServer()
    {
        userName = GetComponent<ServerConnection>().SendName("name");
    }
#endregion

#region Main-menu Get a list of players in game
    public string[] GetConnectedPlayers()
    {
        return GetComponent<ServerConnection>().GetPlayerList().SplitStringIntoArray();
    }
    #endregion

#region Get a list of rooms in the play lobby / Get a list of rooms in the spectate lobby
    public string[] GetListOfRoomsInPlayLobby()
    {
        return GetComponent<ServerConnection>().GetRoomList().SplitStringIntoArray();
    }
    
    public string[] GetListOfRoomsInSpectateLobby()
    {
        return GetComponent<ServerConnection>().GetSpectateRoomList().SplitStringIntoArray();
    }
#endregion

#region Join / Create / leave a room spectator / player
    public void JoinRoom(string roomName, bool isSpectator = false)
    {
        if(!isSpectator)
            GetComponent<ServerConnection>().JoinRoom(roomName);
        else
            GetComponent<ServerConnection>().JoinRoomSpectator(roomName);
    }

    public void CreateRoom()
    {
        GetComponent<ServerConnection>().CreateRoom();
    }

    public void LeaveRoom()
    {
        GetComponent<ServerConnection>().LeaveTheRoom();
    }
#endregion

#region (Ready / Unready)
    public void SetPlayerReady(bool ready)
    {
        GetComponent<ServerConnection>().RoomSetReady(ready);
    }

    public bool GetOpponentReady()
    {
        return GetComponent<ServerConnection>().opponentIsReady;
    }
#endregion

#region Get host and opponent names
    public string GetHostName()
    {
        //if (GetComponent<ServerConnection>().isHost)
        //    return userName;
        return GetComponent<ServerConnection>().GetPlayerOne();
    }

    public string GetOpponentName()
    {
        return GetComponent<ServerConnection>().GetPlayerTwo();
    }
#endregion

#region Set game mode (AI Game (Singleplayer))
    public void SetAIGameMode()
    {
        GetComponent<ServerConnection>().SetAIGame();
    }

    public void UnSetAIGameMode()
    {
        GetComponent<ServerConnection>().UnSetAIGame();
    }
#endregion

#region Set moves on board
    public void SetMoveOnBoard(int x, int y)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x, y));
    }
    public void SetMoveOnBoard(int arrayNumber)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(arrayNumber);
    }
#endregion

#region Game map data / Get your turn / Get winner / Get player index number
    public int[] GetMapData()
    {
        return GetComponent<ServerConnection>().GetMapData();
    }

    public bool GetYourTurn()
    {
        return GetComponent<ServerConnection>().GetMyTurn();
    }

    public int GetWinner() //0 - no one, 1 - player one, 2 - player two
    {
        return GetComponent<ServerConnection>().GetWinner();
    }

    public int GetMyIndex()
    {
        return GetComponent<ServerConnection>().MyNumber();
    }
#endregion
}