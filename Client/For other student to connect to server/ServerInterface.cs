using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCPServer;

[RequireComponent (typeof(ServerConnection))]
[RequireComponent (typeof(SceneChange))]
[RequireComponent (typeof(Make_Singleton))]

///<summary>
///<para>
///Utilities Class
///</para>
///<para>
/// 유틸리티 메소드
///</para>
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
    /// <para>
    /// Please enter the names of the scenes before starting the game.
    /// </para>
    /// <para>
    /// 게임을 시작하기 전에 장면의 이름을 입력하십시오.
    /// </para>
    /// </summary>
    [SerializeField]
    string waitingRoomSceneName = "The name of your Scene";

    /// <summary>
    /// <para>
    /// Please enter the names of the scenes before starting the game.
    /// </para>
    /// <para>
    /// 게임을 시작하기 전에 장면의 이름을 입력하십시오.
    /// </para>
    /// </summary>
    [SerializeField]
    string gameRoomSceneName = "The name of your Scene";

    /// <summary>
    /// <para>
    /// Please enter the names of the scenes before starting the game.
    /// </para>
    /// <para>
    /// 게임을 시작하기 전에 장면의 이름을 입력하십시오.
    /// </para>
    /// </summary>
    [SerializeField]
    string disconnectedRoomSceneName = "The name of your Scene";

    /// <summary>
    /// <para>
    /// This are just some variables for debugging while unity is running.
    /// </para>
    /// <para>
    /// 단일성이 실행되는 동안 디버깅을위한 변수 일뿐입니다.
    /// </para>
    /// </summary>
    [SerializeField]
    string serverIPAdrress = "127.0.0.1";

    /// <summary>
    /// <para>
    /// This are just some variables for debugging while unity is running.
    /// </para>
    /// <para>
    /// 단일성이 실행되는 동안 디버깅을위한 변수 일뿐입니다.
    /// </para>
    /// </summary>
    [SerializeField]
    int serverPortNumber = 7777;

    /// <summary>
    /// <para>
    /// This are just some variables for debugging while unity is running.
    /// </para>
    /// <para>
    /// 단일성이 실행되는 동안 디버깅을위한 변수 일뿐입니다.
    /// </para>
    /// </summary>
    [SerializeField]
    string userName = "";


    /// <summary>
    /// <para>
    /// Initialize the Game Object
    /// </para>
    /// <para>
    /// 게임 개체 초기화
    /// </para>
    /// </summary>
    private void Awake()
    {
        this.gameObject.tag = "ServerService";
    }

    /// <summary>
    /// <para>
    /// Links up the Scenes for the SceneChanger to change scenes.
    /// </para>
    /// <para>
    /// SceneChanger가 장면을 변경하기 위해 Scene을 연결합니다.
    /// </para>
    /// </summary>
    void Start () {
        GetComponent<ServerConnection>().SetWaitingRoomName(waitingRoomSceneName);
        GetComponent<ServerConnection>().SetGameRoomName(gameRoomSceneName);
        GetComponent<ServerConnection>().SetServerDisconnectedName(disconnectedRoomSceneName);
    }

    // Update is called once per frame
    void Update () {
		
	}


    #region Set server port / ip address and connect to server
    /// <summary>
    /// <para>
    /// Function to set the Server's IP Address
    /// </para>
    /// <para>
    /// 서버의 IP 주소를 설정하는 기능
    /// </para>
    /// </summary>
    /// <param name="ipAddress">
    /// <para>
    /// Enter the server ip address. Takes in a String.
    /// </para>
    /// <para>
    /// 서버 IP 주소를 입력하십시오. String를 취합니다.
    /// </para>
    /// </param>
    public void SetServerIPAddress(string ipAddress)
    {
        serverIPAdrress = ipAddress;
    }

    /// <summary>
    /// <para>
    /// Function to set port number.
    /// </para>
    /// <para>
    /// 포트 번호를 설정하는 기능.
    /// </para>
    /// </summary>
    /// <param name="portNumber">
    /// <para>
    /// Enter the port number into here. Takes in string. Function will convert it into a int
    /// </para>
    /// <para>
    /// 여기에 포트 번호를 입력하십시오. 문자열을받습니다. 함수는이를 Int로 변환합니다.
    /// </para>
    /// </param>
    public void SetServerPortNumber(string portNumber)
    {
        int.TryParse(portNumber, out serverPortNumber);
    }


    /// <summary>
    /// <para>
    /// This function connects to the server. Please call this function only when IP address and port number is assigned.
    /// </para>
    /// <para>
    /// 이 함수는 서버에 연결합니다. IP 주소와 포트 번호가 지정된 경우에만이 기능을 호출하십시오.
    /// </para>
    /// </summary>
    public void ConnectToServer()
    {
        GetComponent<ServerConnection>().ConnecToServer(serverIPAdrress, serverPortNumber);
    }
    #endregion

    #region Set user name & sent username to server
    /// <summary>
    /// <para>
    /// Please set your name before entering the lobby. This is important as I will be using it for the room name.
    /// </para>
    /// <para>
    /// 로비에 들어가기 전에 이름을 정하십시오. 내가 방 이름으로 사용할 때 이것은 중요합니다.
    /// </para>
    /// </summary>
    /// <param name="myName">
    /// <para>
    /// Enter the user name for the player. Takes in a string.
    /// </para>
    /// <para>
    /// 플레이어의 사용자 이름을 입력하십시오. 문자열을받습니다.
    /// </para>
    /// </param>
    public void SetName (string myName)
    {
        if (myName.Contains(","))
            return;
        userName = myName;
        PlayerPrefs.SetString("name", userName);
    }

    /// <summary>
    /// <para>
    /// Once the user name is set, please call this function next. It will tell the server what name you have decided on.
    /// The function will update your name with a index number to seperate similar names.
    /// </para>
    /// <para>
    /// 사용자 이름이 설정되면 다음에이 기능을 호출하십시오. 그것은 당신이 결정한 이름을 서버에 알려줍니다.
    /// 이 기능은 이름을 색인 번호로 업데이트하여 유사한 이름을 구분합니다.
    /// </para>
    /// </summary>
    public void SendNameToServer()
    {
        userName = GetComponent<ServerConnection>().SendName("name");
    }
    #endregion

    #region Main-menu Get a list of players in game
    /// <summary>
    /// <para>
    /// This function gets a list of player names that are connected to the server.
    /// </para>
    /// <para>
    /// 이 함수는 서버에 연결된 플레이어 이름 목록을 가져옵니다.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a string array of names.
    /// </para>
    /// <para>
    /// 이름의 캐릭터 라인 배열을 리턴합니다.
    /// </para>
    /// </returns>
    public string[] GetConnectedPlayers()
    {
        return GetComponent<ServerConnection>().GetPlayerList().SplitStringIntoArray();
    }
    #endregion

    #region Get a list of rooms in the play lobby / Get a list of rooms in the spectate lobby
    /// <summary>
    /// <para>
    /// This function get the list of rooms that are waiting for players to join them
    /// </para>
    /// <para>
    /// 이 함수는 플레이어가 그들을 기다리고있는 방 목록을 얻습니다.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a string array of rooms.
    /// </para>
    /// <para>
    /// 방의 문자열 배열을 반환합니다.
    /// </para>
    /// </returns>
    public string[] GetListOfRoomsInPlayLobby()
    {
        return GetComponent<ServerConnection>().GetRoomList().SplitStringIntoArray();
    }

    /// <summary>
    /// <para>
    /// This function gets the list of rooms that are already in a game. This is for spectating.
    /// </para>
    /// <para>
    /// 이 함수는 이미 게임에있는 방 목록을 가져옵니다. 이것은 관중을위한 것입니다.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a string array of rooms.
    /// </para>
    /// <para>
    /// 방의 문자열 배열을 반환합니다.
    /// </para>
    /// </returns>
    public string[] GetListOfRoomsInSpectateLobby()
    {
        return GetComponent<ServerConnection>().GetSpectateRoomList().SplitStringIntoArray();
    }
    #endregion

    #region Join / Create / leave a room spectator / player

    /// <summary>
    /// <para>
    /// Use this function to join the room that you wish.
    /// </para>
    /// <para>
    /// 이 기능을 사용하여 원하는 방에 참여하십시오.
    /// </para>
    /// </summary>
    /// <param name="roomName">
    /// <para>
    /// Enter the room name that was selected.
    /// </para>
    /// <para>
    /// 선택한 객실 이름을 입력하십시오.
    /// </para>
    /// </param>
    /// <param name="isSpectator">
    /// <para>
    /// If the user is spectating. Please set this to true.
    /// </para>
    /// <para>
    /// 사용자가 관중 인 경우. 이것을 true로 설정하십시오.
    /// </para>
    /// </param>
    public void JoinRoom(string roomName, bool isSpectator = false)
    {
        if(!isSpectator)
            GetComponent<ServerConnection>().JoinRoom(roomName);
        else
            GetComponent<ServerConnection>().JoinRoomSpectator(roomName);
    }

    /// <summary>
    /// <para>
    /// A simple function for creating a room. It will use the player name as the room name.
    /// </para>
    /// <para>
    /// 방을 만드는 간단한 기능. 플레이어 이름을 방 이름으로 사용합니다.
    /// </para>
    /// </summary>
    public void CreateRoom()
    {
        GetComponent<ServerConnection>().CreateRoom();
    }

    /// <summary>
    /// <para>
    /// A simple function for leaving the room. It will work for spectators and players.
    /// </para>
    /// <para>
    /// 방을 나오기위한 간단한 기능. 관중과 선수들에게 효과적입니다.
    /// </para>
    /// </summary>
    public void LeaveRoom()
    {
        GetComponent<ServerConnection>().LeaveTheRoom();
    }
    #endregion

    #region Player (Ready / Unready) in room
    /// <summary>
    /// <para>
    /// A function to tell the server that the player is ready.
    /// </para>
    /// <para>
    /// 플레이어에게 준비되었음을 서버에 알리는 함수.
    /// </para>
    /// </summary>
    /// <param name="ready">
    /// <para>
    /// Enter true or false to specify if player is ready
    /// </para>
    /// <para>
    /// 플레이어가 준비되었는지를 지정하려면 true 또는 false를 입력하십시오.
    /// </para>
    /// </param>
    public void SetPlayerReady(bool ready)
    {
        GetComponent<ServerConnection>().RoomSetReady(ready);
    }

    /// <summary>
    /// <para>
    /// Function to see if the opponent is ready.
    /// </para>
    /// <para>
    /// 상대방이 준비되었는지 확인하는 기능.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a bool. If true, the opponent is ready.
    /// </para>
    /// <para>
    /// bool을 반환합니다. 참이면 상대방이 준비가되었습니다.
    /// </para>
    /// </returns>
    public bool GetOpponentReady()
    {
        return GetComponent<ServerConnection>().opponentIsReady;
    }
    #endregion

    #region Get host and opponent names
    /// <summary>
    /// <para>
    /// This function returns the host name.
    /// </para>
    /// <para>
    /// 이 함수는 호스트 이름을 반환합니다.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a string that is the host name.
    /// </para>
    /// <para>
    /// 호스트 이름 인 문자열을 반환합니다.
    /// </para>
    /// </returns>
    public string GetHostName()
    {
        return GetComponent<ServerConnection>().GetPlayerOne();
    }

    /// <summary>
    /// <para>
    /// This function gets the opponent name.
    /// </para>
    /// <para>
    /// 이 함수는 상대 이름을 가져옵니다.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a String that is the opponent name
    /// </para>
    /// <para>
    /// 상대 이름 인 String을 반환합니다.
    /// </para>
    /// </returns>
    public string GetOpponentName()
    {
        return GetComponent<ServerConnection>().GetPlayerTwo();
    }
    #endregion

    #region Set game mode (AI Game (Singleplayer))
    /// <summary>
    /// <para>
    /// Set game mode to VS AI. (Single player)
    /// </para>
    /// <para>
    /// 게임 모드를 VS AI로 설정하십시오. (싱글 플레이어)
    /// </para>
    /// </summary>
    public void SetAIGameMode()
    {
        GetComponent<ServerConnection>().SetAIGame();
    }

    /// <summary>
    /// <para>
    /// Set the game mode to PVP. (player Vs player)
    /// </para>
    /// <para>
    /// 게임 모드를 PVP로 설정하십시오. (플레이어 대 플레이어)
    /// </para>
    /// </summary>
    public void UnSetAIGameMode()
    {
        GetComponent<ServerConnection>().UnSetAIGame();
    }
    #endregion

    #region Set moves on board
    /// <summary>
    /// <para>
    /// This is a move placement function. When the player or AI makes a move, Please call this function
    /// </para>
    /// <para>
    /// 이것은 이동 배치 기능입니다. 플레이어 나 인공 지능이 움직이면이 기능을 호출하십시오.
    /// </para>
    /// </summary>
    /// <param name="x">
    /// <para>
    /// Takes in an int value of X. 0 > 14
    /// </para>
    /// <para>
    /// X의 int 치를 취합니다. 0> 14
    /// </para>
    /// </param>
    /// <param name="y">
    /// <para>
    /// Takes in an int value of Y. 0 > 14
    /// </para>
    /// <para>
    /// int 치의 Y를 취합니다. 0> 14
    /// </para>
    /// </param>
    public void SetMoveOnBoard(int x, int y)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x, y));
    }

    /// <summary>
    /// <para>
    /// Overloaded function for accepting the array index position instead of XY.
    /// </para>
    /// <para>
    /// XY 대신 배열 인덱스 위치를 받아들이는 오버로드 된 함수입니다.
    /// </para>
    /// </summary>
    /// <param name="arrayNumber">
    /// <para>
    /// Takes in an int value of the Array Index Position for the board. 0 - 224.
    /// </para>
    /// <para>
    /// 보드의 Array Index Position의 int 값을받습니다. 0 - 224.
    /// </para>
    /// </param>
    public void SetMoveOnBoard(int arrayNumber)
    {
        GetComponent<ServerConnection>().SetMoveOnBoard(arrayNumber);
    }
    #endregion

    #region Game map data / Get your turn / Get winner / Get player index number
    /// <summary>
    /// <para>
    /// Call this function to get the map data.
    /// This functions is recommended to be placed inside the update function to fetch accurate data.
    /// </para>
    /// 이 함수를 호출하여지도 데이터 가져 오기
    /// <para></para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns an int array of size 225. Each value represent the the status of the board. 0 = empty, 1 = black, 2 = white.
    /// </para>
    /// <para>
    /// 크기 225의 int 배열을 반환합니다. 각 값은 보드 상태를 나타냅니다. 0 = 비어 있음, 1 = 검은 색, 2 = 흰색.
    /// </para>
    /// </returns>
    public int[] GetMapData()
    {
        return GetComponent<ServerConnection>().GetMapData();
    }

    /// <summary>
    /// <para>
    /// Call this function to get the map data.
    /// This functions is recommended to be placed inside the update function to fetch accurate data.
    /// </para>
    /// 이 함수를 호출하여지도 데이터 가져 오기
    /// <para></para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns an int[Y,X] double array of size X = 15 and Y = 15 . Each value represent the the status of the board. 0 = empty, 1 = black, 2 = white.
    /// </para>
    /// <para>
    /// 크기 X = 15 및 Y = 15의 int [Y, X] double 배열을 리턴합니다. 각 값은 보드의 상태를 나타냅니다. 0 = 비어 있음, 1 = 검은 색, 2 = 흰색.
    /// </para>
    /// </returns>
    public int[,]GetMapDataDoubleArray()
    {
        int[,] myArray = new int[15,15];
        int[] newMapData = GetMapData();
        for(int i = 0; i < 15; ++i)
        {
            for(int j = 0; j < 15; ++j)
            {
                myArray[i, j] = newMapData[(i * 15) + j];
            }
        }
        return myArray;
    }

    /// <summary>
    /// <para>
    /// Function to check if it is your turn.
    /// This functions is recommended to be placed inside the update function to fetch accurate data.
    /// </para>
    /// <para>
    /// 자신의 차례인지 확인하는 기능.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns a bool. If true means it is your turn
    /// </para>
    /// <para>
    /// bool을 반환합니다. 사실이라면 당신의 차례입니다.
    /// </para>
    /// </returns>
    public bool GetYourTurn()
    {
        return GetComponent<ServerConnection>().GetMyTurn();
    }

    /// <summary>
    /// <para>
    /// Function to check if there was any winner.
    /// This functions is recommended to be placed inside the update function to fetch accurate data.
    /// </para>
    /// <para>
    /// 승자가 있는지 확인하는 기능.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns an int. 0 = nobody, 1 = black, 2 = white
    /// </para>
    /// <para>
    /// int를 돌려줍니다. 0 = 아무도, 1 = 검정, 2 = 흰색
    /// </para>
    /// </returns>
    public int GetWinner() //0 - no one, 1 - player one, 2 - player two
    {
        return GetComponent<ServerConnection>().GetWinner();
    }

    /// <summary>
    /// <para>
    /// Check the current player is Black or White.
    /// This functions is recommended to be placed inside the update function to fetch accurate data.
    /// </para>
    /// <para>
    /// 현재 플레이어가 검정 또는 흰색인지 확인하십시오.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns an int value. 1 = Black, 2 = white.
    /// </para>
    /// <para>
    /// int 치를 돌려줍니다. 1 = 검정, 2 = 흰색.
    /// </para>
    /// </returns>
    public int GetMyIndex()
    {
        return GetComponent<ServerConnection>().MyNumber();
    }
    #endregion

    #region Enable / Disable / Get Active Renju Rules
    /// <summary>
    /// <para>
    /// Set Renju rules active or not active (true - active, false - not active)
    /// </para>
    /// <para>
    /// Renju 규칙을 활성화 또는 비활성화로 설정합니다 (true - active, false - not active).
    /// </para>
    /// </summary>
    public void SetRenjuRules(bool active)
    {
        GetComponent<ServerConnection>().SetRenjuRules(active);
    }

    /// <summary>
    /// <para>
    /// Get if Renju rules are active or not
    /// </para>
    /// Renju 규칙이 활성화되었는지 여부 확인
    /// </summary>
    /// <returns>
    /// <para>
    /// Returns true for active and false for not active
    /// </para>
    /// <para>
    /// 활성 상태이면 true를 반환하고 비활성 상태이면 false를 반환합니다.
    /// </para>
    /// </returns>
    public bool GetRenjuRules()
    {
        return GetComponent<ServerConnection>().GetRenjuRules();
    }
#endregion
}