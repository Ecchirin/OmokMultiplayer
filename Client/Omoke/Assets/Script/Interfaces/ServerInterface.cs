using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCPServer;

[RequireComponent (typeof(ServerConnection))]
[RequireComponent (typeof(SceneChange))]
[RequireComponent (typeof(Make_Singleton))]

///<summary>
///Utilities methods
///유틸리티 메소드
///</summary>
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
    /// 게임을 시작하기 전에 장면의 이름을 입력하십시오.
    /// </summary>
    [SerializeField]
    string waitingRoomSceneName = "The name of your Scene";
    [SerializeField]
    string gameRoomSceneName = "The name of your Scene";

    /// <summary>
    /// This are just some variables for debugging while unity is running.
    /// 단일성이 실행되는 동안 디버깅을위한 변수 일뿐입니다.
    /// </summary>
    [SerializeField]
    string serverIPAdrress = "127.0.0.1";
    [SerializeField]
    int serverPortNumber = 7777;
    [SerializeField]
    string userName = "";


    /// <summary>
    /// Initialize the Game Object
    /// 게임 개체 초기화
    /// </summary>
    private void Awake()
    {
        this.gameObject.tag = "ServerService";
    }

    /// <summary>
    /// Links up the Scenes for the SceneChanger to change scenes.
    /// SceneChanger가 장면을 변경하기 위해 Scene을 연결합니다.
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
    /// 서버의 IP 주소를 설정하는 기능
    /// </summary>
    /// <param name="ipAddress">
    /// Enter the server ip address. Takes in a String.
    /// 서버 IP 주소를 입력하십시오. String를 취합니다.
    /// </param>
    public void SetServerIPAddress(string ipAddress)
    {
        serverIPAdrress = ipAddress;
    }

    /// <summary>
    /// Function to set port number.
    /// 포트 번호를 설정하는 기능.
    /// </summary>
    /// <param name="portNumber">
    /// Enter the port number into here. Takes in string. Function will convert it into a int
    /// 여기에 포트 번호를 입력하십시오. 문자열을받습니다. 함수는이를 Int로 변환합니다.
    /// </param>
    public void SetServerPortNumber(string portNumber)
    {
        int.TryParse(portNumber, out serverPortNumber);
    }


    /// <summary>
    /// This function connects to the server. Please call this function only when IP address and port number is assigned.
    /// 이 함수는 서버에 연결합니다. IP 주소와 포트 번호가 지정된 경우에만이 기능을 호출하십시오.
    /// </summary>
    public void ConnectToServer()
    {
        GetComponent<ServerConnection>().ConnecToServer(serverIPAdrress, serverPortNumber);
    }
#endregion

#region Set user name & sent username to server
    /// <summary>
    /// Please set your name before entering the lobby. This is important as I will be using it for the room name.
    /// 로비에 들어가기 전에 이름을 정하십시오. 내가 방 이름으로 사용할 때 이것은 중요합니다.
    /// </summary>
    /// <param name="myName">
    /// Enter the user name for the player. Takes in a string.
    /// 플레이어의 사용자 이름을 입력하십시오. 문자열을받습니다.
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
    /// 사용자 이름이 설정되면 다음에이 기능을 호출하십시오. 그것은 당신이 결정한 이름을 서버에 알려줍니다.
    /// 이 기능은 이름을 색인 번호로 업데이트하여 유사한 이름을 구분합니다.
    /// </summary>
    public void SendNameToServer()
    {
        userName = GetComponent<ServerConnection>().SendName("name");
    }
#endregion

#region Main-menu Get a list of players in game
    /// <summary>
    /// This function gets a list of player names that are connected to the server.
    /// 이 함수는 서버에 연결된 플레이어 이름 목록을 가져옵니다.
    /// </summary>
    /// <returns>
    /// Returns a string array of names.
    /// 이름의 캐릭터 라인 배열을 리턴합니다.
    /// </returns>
    public string[] GetConnectedPlayers()
    {
        return GetComponent<ServerConnection>().GetPlayerList().SplitStringIntoArray();
    }
#endregion

#region Get a list of rooms in the play lobby / Get a list of rooms in the spectate lobby
    /// <summary>
    /// This function get the list of rooms that are waiting for players to join them
    /// 이 함수는 플레이어가 그들을 기다리고있는 방 목록을 얻습니다.
    /// </summary>
    /// <returns>
    /// Returns a string array of rooms.
    /// 방의 문자열 배열을 반환합니다.
    /// </returns>
    public string[] GetListOfRoomsInPlayLobby()
    {
        return GetComponent<ServerConnection>().GetRoomList().SplitStringIntoArray();
    }

    /// <summary>
    /// This function gets the list of rooms that are already in a game. This is for spectating.
    /// 이 함수는 이미 게임에있는 방 목록을 가져옵니다. 이것은 관중을위한 것입니다.
    /// </summary>
    /// <returns>
    /// Returns a string array of rooms.
    /// 방의 문자열 배열을 반환합니다.
    /// </returns>
    public string[] GetListOfRoomsInSpectateLobby()
    {
        return GetComponent<ServerConnection>().GetSpectateRoomList().SplitStringIntoArray();
    }
#endregion

#region Join / Create / leave a room spectator / player

    /// <summary>
    /// Use this function to join the room that you wish.
    /// 이 기능을 사용하여 원하는 방에 참여하십시오.
    /// </summary>
    /// <param name="roomName">
    /// Enter the room name that was selected.
    /// 선택한 객실 이름을 입력하십시오.
    /// </param>
    /// <param name="isSpectator">
    /// If the user is spectating. Please set this to true.
    /// 사용자가 관중 인 경우. 이것을 true로 설정하십시오.
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
    /// 방을 만드는 간단한 기능. 플레이어 이름을 방 이름으로 사용합니다.
    /// </summary>
    public void CreateRoom()
    {
        GetComponent<ServerConnection>().CreateRoom();
    }

    /// <summary>
    /// A simple function for leaving the room. It will work for spectators and players.
    /// 방을 나오기위한 간단한 기능. 관중과 선수들에게 효과적입니다.
    /// </summary>
    public void LeaveRoom()
    {
        GetComponent<ServerConnection>().LeaveTheRoom();
    }
#endregion

#region (Ready / Unready)
    /// <summary>
    /// A function to tell the server that the player is ready.
    /// 플레이어에게 준비되었음을 서버에 알리는 함수.
    /// </summary>
    /// <param name="ready">
    /// Enter true or false to specify if player is ready
    /// 플레이어가 준비되었는지를 지정하려면 true 또는 false를 입력하십시오.
    /// </param>
    public void SetPlayerReady(bool ready)
    {
        GetComponent<ServerConnection>().RoomSetReady(ready);
    }

    /// <summary>
    /// Function to see if the opponent is ready.
    /// 상대방이 준비되었는지 확인하는 기능.
    /// </summary>
    /// <returns>
    /// Returns a bool. If true, the opponent is ready.
    /// bool을 반환합니다. 참이면 상대방이 준비가되었습니다.
    /// </returns>
    public bool GetOpponentReady()
    {
        return GetComponent<ServerConnection>().opponentIsReady;
    }
#endregion

#region Get host and opponent names
    /// <summary>
    /// This function returns the host name.
    /// 이 함수는 호스트 이름을 반환합니다.
    /// </summary>
    /// <returns>
    /// Returns a string that is the host name.
    /// 호스트 이름 인 문자열을 반환합니다.
    /// </returns>
    public string GetHostName()
    {
        return GetComponent<ServerConnection>().GetPlayerOne();
    }

    /// <summary>
    /// This function gets the opponent name.
    /// 이 함수는 상대 이름을 가져옵니다.
    /// </summary>
    /// <returns>
    /// Returns a String that is the opponent name
    /// 상대 이름 인 String을 반환합니다.
    /// </returns>
    public string GetOpponentName()
    {
        return GetComponent<ServerConnection>().GetPlayerTwo();
    }
#endregion

#region Set game mode (AI Game (Singleplayer))
    /// <summary>
    /// Set game mode to VS AI. (Single player)
    /// 게임 모드를 VS AI로 설정하십시오. (싱글 플레이어)
    /// </summary>
    public void SetAIGameMode()
    {
        GetComponent<ServerConnection>().SetAIGame();
    }

    /// <summary>
    /// Set the game mode to PVP. (player Vs player)
    /// 게임 모드를 PVP로 설정하십시오. (플레이어 대 플레이어)
    /// </summary>
    public void UnSetAIGameMode()
    {
        GetComponent<ServerConnection>().UnSetAIGame();
    }
    #endregion

#region Set moves on board
    /// <summary>
    /// This is a move placement function. When the player or AI makes a move, Please call this function
    /// 이것은 이동 배치 기능입니다. 플레이어 나 인공 지능이 움직이면이 기능을 호출하십시오.
    /// </summary>
    /// <param name="x">
    /// Takes in an int value of X. 0 > 14
    /// X의 int 치를 취합니다. 0> 14
    /// </param>
    /// <param name="y">
    /// Takes in an int value of Y. 0 > 14
    /// int 치의 Y를 취합니다. 0> 14
    /// </param>
    public void SetMoveOnBoard(int x, int y)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x, y));
    }

    /// <summary>
    /// Overloaded function for accepting the array index position instead of XY.
    /// XY 대신 배열 인덱스 위치를 받아들이는 오버로드 된 함수입니다.
    /// </summary>
    /// <param name="arrayNumber">
    /// Takes in an int value of the Array Index Position for the board. 0 - 224.
    /// 보드의 Array Index Position의 int 값을받습니다. 0 - 224.
    /// </param>
    public void SetMoveOnBoard(int arrayNumber)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(arrayNumber);
    }
#endregion

///<summary>
///This region of functions is recommended to be placed inside the update function to fetch accurate data.
///이 함수 영역은 정확한 데이터를 가져 오기 위해 업데이트 함수 내에 배치하는 것이 좋습니다.
/// </summary>
#region Game map data / Get your turn / Get winner / Get player index number
    /// <summary>
    /// Call this function to get the map data
    /// 이 함수를 호출하여지도 데이터 가져 오기
    /// </summary>
    /// <returns>
    /// Returns an int array of size 225. Each value represent the the status of the board. 0 = empty, 1 = black, 2 = white.
    /// 크기 225의 int 배열을 반환합니다. 각 값은 보드 상태를 나타냅니다. 0 = 비어 있음, 1 = 검은 색, 2 = 흰색.
    /// </returns>
    public int[] GetMapData()
    {
        return GetComponent<ServerConnection>().GetMapData();
    }

    /// <summary>
    /// Function to check if it is your turn.
    /// 자신의 차례인지 확인하는 기능.
    /// </summary>
    /// <returns>
    /// Returns a bool. If true means it is your turn
    /// bool을 반환합니다. 사실이라면 당신의 차례입니다.
    /// </returns>
    public bool GetYourTurn()
    {
        return GetComponent<ServerConnection>().GetMyTurn();
    }

    /// <summary>
    /// Function to check if there was any winner.
    /// 승자가 있는지 확인하는 기능.
    /// </summary>
    /// <returns>
    /// Returns an int. 0 = nobody, 1 = black, 2 = white
    /// int를 돌려줍니다. 0 = 아무도, 1 = 검정, 2 = 흰색
    /// </returns>
    public int GetWinner() //0 - no one, 1 - player one, 2 - player two
    {
        return GetComponent<ServerConnection>().GetWinner();
    }

    /// <summary>
    /// Check the current player is Black or White.
    /// 현재 플레이어가 검정 또는 흰색인지 확인하십시오.
    /// </summary>
    /// <returns>
    /// Returns an int value. 1 = Black, 2 = white.
    /// int 치를 돌려줍니다. 1 = 검정, 2 = 흰색.
    /// </returns>
    public int GetMyIndex()
    {
        return GetComponent<ServerConnection>().MyNumber();
    }
#endregion
}