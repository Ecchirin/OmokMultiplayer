using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCPServer;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace OmokServer
{
    public struct OngoingGame
    {
        public ConnectionThread theHost, theOpponent;
        public int hostIndex, opponentIndex;
        public bool hostTurn, opponentTurn;
        public bool isAiGame;

        public OngoingGame(ConnectionThread theHost, ConnectionThread theOpponent, bool aiGame = false)
        {
            this.theHost = theHost;
            this.theOpponent = theOpponent;
            hostIndex = opponentIndex = 0;
            hostTurn = opponentTurn = false;
            isAiGame = aiGame;
        }
    }

    public class GameInformation
    {
        //Map data
        private int[] mapData;

        bool isAIGame = false;

        //Player Data
        public OngoingGame thePlayers;
        int theWinner = 0;

        int playerOnePicture = 0, playerTwoPicture = 0;

        Queue<int> getList = new Queue<int>();

        bool isRenjuRules = true;

        //List of spectators
        public List<ConnectionThread> listOfSpectators = new List<ConnectionThread>();

        public void AddNewSpectator(ConnectionThread newSpectator)
        {
            listOfSpectators.Add(newSpectator);
        }

        public GameInformation(ConnectionThread theHost, ConnectionThread theOpponent, bool aiGame = false, bool setRenju = true, int playerOne = 0, int playerTwo = 0)
        {
            thePlayers = new OngoingGame(theHost, theOpponent);
            GenerateEmptyMap();

            if (!aiGame)
                InitPlayers();
            else
                InitAI();

            isRenjuRules = setRenju;
            playerOnePicture = playerOne;
            playerTwoPicture = playerTwo;
            Console.WriteLine("INITED");
        }

        public void GetPlayerOnePicture()
        {
            byte[] data = new byte[1024];

            Console.WriteLine("P1 Picture is : " + playerOnePicture);
            //Send to host first
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_ONE_PICTURE.ToString() + ":" +
                playerOnePicture);
            do
            {
                thePlayers.theHost.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theHost.ns.CanWrite);

            if (isAIGame)
                return;

            //Send to opponent
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_ONE_PICTURE.ToString() + ":" +
                playerOnePicture);
            do
            {
                thePlayers.theOpponent.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theOpponent.ns.CanWrite);
        }

        public void GetPlayerTwoPicture()
        {
            byte[] data = new byte[1024];

            Console.WriteLine("P2 Picture is : " + playerTwoPicture);

            //Send to host first
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_TWO_PICTURE.ToString() + ":" +
                playerTwoPicture);
            do
            {
                thePlayers.theHost.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theHost.ns.CanWrite);

            if (isAIGame)
                return;

            //Send to opponent
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_TWO_PICTURE.ToString() + ":" +
                playerTwoPicture);
            do
            {
                thePlayers.theOpponent.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theOpponent.ns.CanWrite);
        }

        int CheckPlacementOfMap(int x, int y)
        {
            int theArrayIndex = ConnectionClass.ConvertXYPositionToIndex(x, y);
            if (theArrayIndex < 0 || theArrayIndex >= 225)
                return 5;

            return mapData[theArrayIndex];
        }

        void GetAllBlackConnected(int x, int y)
        {
            RecursiveCheckForConnectedBlacks(x, y);
        }

        void RecursiveCheckForConnectedBlacks(int x, int y, int dirX = 0, int dirY = 0)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return;

            x += dirX;
            y += dirY;

            if (CheckPlacementOfMap(x, y) == 1)
            {
                if (getList.Contains(ConnectionClass.ConvertXYPositionToIndex(x, y)))
                    return;

                getList.Enqueue(ConnectionClass.ConvertXYPositionToIndex(x, y));
                Console.WriteLine("QUEUED SOMETHiNG");

                RecursiveCheckForConnectedBlacks(x, y, 0, 1);
                RecursiveCheckForConnectedBlacks(x, y, 1, 1);
                RecursiveCheckForConnectedBlacks(x, y, 1, 0);
                RecursiveCheckForConnectedBlacks(x, y, 1, -1);
                RecursiveCheckForConnectedBlacks(x, y, 0, -1);
                RecursiveCheckForConnectedBlacks(x, y, -1, -1);
                RecursiveCheckForConnectedBlacks(x, y, -1, 0);
                RecursiveCheckForConnectedBlacks(x, y, -1, 1);
            }

            return;
        }

        bool CheckIfWin(int arrayIndex)
        {
            //test new code

            int x, y;
            x = y = 0;
            ConnectionClass.ConvertArrayPositionToXY(arrayIndex, out x, out y);

            if (isRenjuRules)
            {
                if (CheckPlacementOfMap(x, y) == 1)
                {
                    GetAllBlackConnected(x, y);

                    if (ForbiddenPointFinder.IsOverline(x, y, mapData))
                    {
                        Console.WriteLine("Forbidden Move @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                        theWinner = 2;
                        return true;
                    }

                    if (getList.Count > 0)
                    {
                        int x1, y1;
                        x1 = y1 = 0;

                        while (getList.Count > 0)
                        {
                            ConnectionClass.ConvertArrayPositionToXY(getList.Dequeue(), out x1, out y1);
                            Console.WriteLine("DEQUEUING LIST, X1 = " + x1 + " Y1 = " + y1);

                            if (ForbiddenPointFinder.IsDoubleFour(x1, y1, mapData) || ForbiddenPointFinder.IsDoubleThree(x1, y1, mapData))
                            {
                                Console.WriteLine("Forbidden Move @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                                theWinner = 2;
                                return true;
                            }
                        }

                    }
                    else
                    {
                        if (ForbiddenPointFinder.IsDoubleFour(x, y, mapData) || ForbiddenPointFinder.IsDoubleThree(x, y, mapData))
                        {
                            Console.WriteLine("Forbidden Move @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                            theWinner = 2;
                            return true;
                        }
                    }
                }

                if (ForbiddenPointFinder.IsFive(x, y, mapData, CheckPlacementOfMap(x, y)))
                {
                    Console.WriteLine("5 in a row found @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                    theWinner = CheckPlacementOfMap(x, y);
                    return true;
                }
            }
            else
            {
                if (ForbiddenPointFinder.IsOverline(x, y, mapData))
                {
                    Console.WriteLine("No Rules Overline win");
                    theWinner = CheckPlacementOfMap(x, y);
                    return true;
                }
                if (ForbiddenPointFinder.IsFive(x, y, mapData, CheckPlacementOfMap(x, y)))
                {
                    Console.WriteLine("No rules 5 in a row win");
                    theWinner = CheckPlacementOfMap(x, y);
                    return true;
                }
            }
            return false;
        }

        void InitPlayers()
        {
            isAIGame = false;

            Random rand = new Random();
            if (rand.Next(1, 2) == 1)
            {
                thePlayers.hostIndex = 1;
                thePlayers.opponentIndex = 2;
                thePlayers.hostTurn = true;
                thePlayers.opponentTurn = false;

                byte[] data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                    /* Map Data */
                    string.Join(",", GetMapData()) + ":" +
                    /* Whos Turn */
                    (thePlayers.hostTurn ? "1" : "0") + ":" +
                    /* index num */
                    "1" + ":" +
                    /* Winner */
                    "0");
                do
                {
                    thePlayers.theHost.ns.Write(data, 0, data.Length);
                    Console.WriteLine("HOST TURN IS FIRST SENT");
                }
                while (!thePlayers.theHost.ns.CanWrite);

                data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                     /* Map Data */
                     string.Join(",", GetMapData()) + ":" +
                     /* Whos Turn 1 or 0 */
                     (thePlayers.opponentTurn ? "1" : "0") + ":" +
                     /* index num 1 or 2 */
                     "2" + ":" +
                     /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                     "0");
                do
                {
                    thePlayers.theOpponent.ns.Write(data, 0, data.Length);
                    Console.WriteLine("OPPO TURN IS 2nd SENT");
                }
                while (!thePlayers.theOpponent.ns.CanWrite);
            }
            else
            {
                thePlayers.hostIndex = 2;
                thePlayers.opponentIndex = 1;
                thePlayers.opponentTurn = true;
                thePlayers.hostTurn = false;
                byte[] data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                     /* Map Data */
                     string.Join(",", GetMapData()) + ":" +
                     /* Whos Turn 1 or 0 */
                     (thePlayers.hostTurn ? "1" : "0") + ":" +
                     /* index num 1 or 2 */
                     "2" + ":" +
                     /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                     "0");
                do
                {
                    thePlayers.theHost.ns.Write(data, 0, data.Length);
                    Console.WriteLine("HOST TURN IS 2nd SENT");
                }
                while (!thePlayers.theHost.ns.CanWrite);

                data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                    /* Map Data */
                    string.Join(",", GetMapData()) + ":" +
                    /* Whos Turn 1 or 0 */
                    (thePlayers.opponentTurn ? "1" : "0") + ":" +
                    /* index num 1 or 2 */
                    "1" + ":" +
                    /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                    "0");
                do
                {
                    thePlayers.theOpponent.ns.Write(data, 0, data.Length);
                    Console.WriteLine("OPPO TURN IS 1st SENT");
                }
                while (!thePlayers.theOpponent.ns.CanWrite);
            }
        }

        void InitAI()
        {
            isAIGame = true;

            Random rand = new Random();
            if (rand.Next(1, 2) == 1)
            {
                thePlayers.hostIndex = 1;
                thePlayers.opponentIndex = 2;
                thePlayers.hostTurn = true;
                thePlayers.opponentTurn = false;

                byte[] data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                    /* Map Data */
                    string.Join(",", GetMapData()) + ":" +
                    /* Whos Turn */
                    (thePlayers.hostTurn ? "1" : "0") + ":" +
                    /* index num */
                    "1" + ":" +
                    /* Winner */
                    "0");
                do
                {
                    thePlayers.theHost.ns.Write(data, 0, data.Length);
                    Console.WriteLine("HOST TURN IS FIRST SENT");
                }
                while (!thePlayers.theHost.ns.CanWrite);
            }
            else
            {
                thePlayers.hostIndex = 2;
                thePlayers.opponentIndex = 1;
                thePlayers.opponentTurn = true;
                thePlayers.hostTurn = false;
                byte[] data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                     /* Map Data */
                     string.Join(",", GetMapData()) + ":" +
                     /* Whos Turn 1 or 0 */
                     (thePlayers.hostTurn ? "1" : "0") + ":" +
                     /* index num 1 or 2 */
                     "2" + ":" +
                     /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                     "0");
                do
                {
                    thePlayers.theHost.ns.Write(data, 0, data.Length);
                    Console.WriteLine("HOST TURN IS 2nd SENT");
                }
                while (!thePlayers.theHost.ns.CanWrite);
            }
        }

        public void SendTurnToSpectators(byte[] data)
        {
            if (listOfSpectators.Any())
            {
                foreach (ConnectionThread iter in listOfSpectators)
                {
                    if (iter == null)
                        continue;
                    do
                    {
                        iter.ns.Write(data, 0, data.Length);
                    }
                    while (!iter.ns.CanWrite);
                }
            }
        }

        public void ForceUpdate()
        {
            byte[] data = new byte[1024];

            Console.WriteLine("TheWinner = " + theWinner);

            //Send to host first
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                /* Map Data */
                string.Join(",", GetMapData()) + ":" +
                /* Whos Turn */
                (thePlayers.hostTurn ? "1" : "0") + ":" +
                /* index num */
                thePlayers.hostIndex.ToString() + ":" +
                /* Winner */
                theWinner);
            do
            {
                thePlayers.theHost.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theHost.ns.CanWrite);

            if (isAIGame)
                return;

            //Send to opponent
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                 /* Map Data */
                 string.Join(",", GetMapData()) + ":" +
                 /* Whos Turn 1 or 0 */
                 (thePlayers.opponentTurn ? "1" : "0") + ":" +
                 /* index num 1 or 2 */
                 thePlayers.opponentIndex.ToString() + ":" +
                 /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                 theWinner);
            do
            {
                thePlayers.theOpponent.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theOpponent.ns.CanWrite);
        }

        public void SendTurnInfo(int placementIndex)
        {
            bool successPlacement = false;
            byte[] data = new byte[1024];

            Console.WriteLine("Current turn is " + (thePlayers.hostTurn ? "the Host." : "the Opponent"));
            if (thePlayers.hostTurn)
            {
                successPlacement = SetPlacement(placementIndex, thePlayers.hostIndex);
            }
            else if (thePlayers.opponentTurn)
            {
                successPlacement = SetPlacement(placementIndex, thePlayers.opponentIndex);
            }

            if (!successPlacement)
            {
                //return same data cause invalid move. Client should check this before sending it to me. but safety net is best
                //Send to host first
                data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                    /* Map Data */
                    string.Join(",", GetMapData()) + ":" +
                    /* Whos Turn */
                    (thePlayers.hostTurn ? "1" : "0") + ":" +
                    /* index num */
                    thePlayers.hostIndex.ToString() + ":" +
                    /* Winner */
                    theWinner);
                do
                {
                    thePlayers.theHost.ns.Write(data, 0, data.Length);
                }
                while (!thePlayers.theHost.ns.CanWrite);

                //Send to opponent
                data = new byte[1024];
                data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                     /* Map Data */
                     string.Join(",", GetMapData()) + ":" +
                     /* Whos Turn 1 or 0 */
                     (thePlayers.opponentTurn ? "1" : "0") + ":" +
                     /* index num 1 or 2 */
                     thePlayers.opponentIndex.ToString() + ":" +
                     /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                     theWinner);
                do
                {
                    thePlayers.theOpponent.ns.Write(data, 0, data.Length);
                }
                while (!thePlayers.theOpponent.ns.CanWrite);

                Console.WriteLine("Invalid move on board, please try again");

                //terminate here cause invalid move.
                return;
            }

            //Check if win
            if (CheckIfWin(placementIndex))
                ThreadedTCPServer.GameHasEnded(thePlayers);

            thePlayers.hostTurn = !thePlayers.hostTurn;
            thePlayers.opponentTurn = !thePlayers.opponentTurn;

            //Send to host first
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                /* Map Data */
                string.Join(",", GetMapData()) + ":" +
                /* Whos Turn */
                (thePlayers.hostTurn ? "1" : "0") + ":" +
                /* index num */
                thePlayers.hostIndex.ToString() + ":" +
                /* Winner */
                theWinner);

            Console.WriteLine("HOST INDEX = " + thePlayers.hostIndex);

            if (thePlayers.hostIndex == 1)
                SendTurnToSpectators(data);

            do
            {
                thePlayers.theHost.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theHost.ns.CanWrite);

            if (isAIGame)
                return;

            //Send to opponent
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_RAW_GAME_INFO.ToString() + ":" +
                 /* Map Data */
                 string.Join(",", GetMapData()) + ":" +
                 /* Whos Turn 1 or 0 */
                 (thePlayers.opponentTurn ? "1" : "0") + ":" +
                 /* index num 1 or 2 */
                 thePlayers.opponentIndex.ToString() + ":" +
                 /* Winner 0 = nobody, 1 = p1, 2 = p2 */
                 theWinner);

            if (thePlayers.opponentIndex == 1)
                SendTurnToSpectators(data);

            do
            {
                thePlayers.theOpponent.ns.Write(data, 0, data.Length);
            }
            while (!thePlayers.theOpponent.ns.CanWrite);

            Console.WriteLine("New turn is " + (thePlayers.hostTurn ? "the Host." : "the Opponent"));

        }

        void GenerateEmptyMap()
        {
            mapData = new int[225];
            for (int i = 0; i < 225; ++i)
            {
                mapData[i] = 0;
            }
        }

        public bool SetPlacement(int index, int playerIndex)
        {
            if (mapData[index] == 0)
            {
                mapData[index] = playerIndex;
                return true;
            }

            return false;
        }

        public int[] GetMapData()
        {
            return mapData;
        }
    }

    /// <summary>
    /// The class that manage the connection within each thread
    /// 
    /// </summary>
    /// 

    public class ConnectionThread
    {
        /// <summary>
        /// Each connection should hold the unique Game ID
        /// It also contains if player is in lobby
        /// </summary>

        //Stuff for Games
        public bool inLobby, isSpectator, isReady, inGame, hostMadeRenju;
        public GameInformation gameSession;

        //Identification
        public string clientName;

        public TcpListener threadListener;
        private static int connections = 0;

        public TcpClient client;
        public NetworkStream ns;

        public int myPicture = 0;
        public int aiPicture = 1;

        public void StartConnection()
        {
            client = threadListener.AcceptTcpClient();
            ns = client.GetStream();
            connections++;

            inLobby = true;
            isSpectator = false;
            isReady = false;
            gameSession = null;
            inGame = false;
            hostMadeRenju = true;

            Console.WriteLine("New client accepted: {0} active connections",
                              connections);
        }

        public void HandleConnection()
        {
            byte[] data = new byte[1024];
            
            string welcome = "Welcome to my test server!";
            data = Encoding.ASCII.GetBytes(welcome);
            //client.NoDelay = true;

            ns.Write(data, 0, data.Length);
            //ThreadedTCPServer.AddNewConnectedClient(this);

            //TestMapData();
            //Console.WriteLine(string.Join(", ", mapData));
            while (true)
            {
                if (!ThreadedTCPServer.IsClientConnected(client))
                {
                    ThreadedTCPServer.ClientDisconnected(this);
                    break;
                }
                //data = new byte[1024];
                //recv = ns.Read(data, 0, data.Length);
                //if (recv == 0)
                //    break;
                StringBuilder fullPacketMessage = new StringBuilder();

                if (ns.CanRead)
                {
                    data = new byte[client.ReceiveBufferSize];
                    int numbersOfBytesRead;

                    do
                    {
                        numbersOfBytesRead = ns.Read(data, 0, data.Length);
                        if (numbersOfBytesRead <= 0)
                            break;

                        fullPacketMessage.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numbersOfBytesRead));
                    } while (ns.DataAvailable);

                    if (numbersOfBytesRead <= 0)
                    {
                        ThreadedTCPServer.ClientDisconnected(this);
                        break;
                    }

                    Console.WriteLine("Message received from {0}:", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                    // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                    Console.WriteLine(fullPacketMessage.ToString());

                    #region CheckWhatPacketIGet
                    if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_ALL_CONNECTED_USERS.ToString()))
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_ALL_CONNECTED_USERS.ToString() + ":" + ThreadedTCPServer.GetListOfConnectedUsers());
                        Console.WriteLine(ThreadedTCPServer.GetListOfConnectedUsers());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.CREATE_NEW_ROOM.ToString()))
                    {
                        Console.WriteLine("Creating new room");
                        ThreadedTCPServer.CreateNewRoom(this);
                        inLobby = false;
                        isReady = false;
                        inGame = false;
                        isSpectator = false;
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString()))
                    {
                        clientName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1) + ThreadedTCPServer.GiveUniqueID();

                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString() + ":" + clientName.ToString());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.FORCED_UPDATE.ToString()))
                    {
                        if (gameSession == null)
                        {

                        }
                        else
                            gameSession.ForceUpdate();
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_PLAYER_ONE_PICTURE.ToString()))
                    {
                        if (gameSession == null)
                        {
                            ThreadedTCPServer.GetPlayerOnePicture(this);
                        }
                        else
                            gameSession.GetPlayerOnePicture();
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_PLAYER_TWO_PICTURE.ToString()))
                    {
                        if (gameSession == null)
                        {
                            ThreadedTCPServer.GetPlayerTwoPicture(this);
                        }
                        else
                            gameSession.GetPlayerTwoPicture();
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.JOIN_ROOM_SPECTATE.ToString()))
                    {
                        data = new byte[1024];
                        string targetName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1);

                        if (ThreadedTCPServer.JoinSpectateRoom(targetName, this))
                        {
                            inLobby = false;
                            isReady = false;
                            inGame = true;
                            isSpectator = true;

                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString() + ":" + ThreadedTCPServer.GetIndexPlacementOfPlayers(targetName));
                            do
                            {
                                ns.Write(data, 0, data.Length);
                                Console.WriteLine("Sent to joiner!!");
                            }
                            while (!ns.CanWrite);
                        }
                        else
                        {
                            inLobby = true;
                            isReady = false;
                            inGame = false;
                            isSpectator = true;


                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_FAILURE.ToString() + ":" + "Failed to join room.");
                            ns.Write(data, 0, data.Length);
                        }
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.UNSET_AI_GAME.ToString()))
                    {
                        inLobby = false;
                        isReady = false;
                        inGame = false;
                        isSpectator = false;
                        ThreadedTCPServer.RemoveAIGame(this);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_AI_GAME.ToString()))
                    {
                        inLobby = false;
                        isReady = false;
                        inGame = false;
                        isSpectator = false;
                        ThreadedTCPServer.CreateAIGame(this);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_PICTURE.ToString()))
                    {
                        myPicture = Int32.Parse(fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1));
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_AI_PICTURE.ToString()))
                    {
                        aiPicture = Int32.Parse(fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1));
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.JOIN_ROOM.ToString()))
                    {
                        inLobby = false;
                        isReady = false;
                        inGame = false;
                        isSpectator = false;
                        aiPicture = 1;
                        myPicture = 0;

                        data = new byte[1024];
                        string targetName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1);

                        if (ThreadedTCPServer.JoinRoom(targetName, this))
                        {
                            isReady = false;
                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString() + ":" + targetName);
                            do
                            {
                                ns.Write(data, 0, data.Length);
                                Console.WriteLine("Sent to joiner!!");
                            }
                            while (!ns.CanWrite);
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(data, 0, data.Length);
                            //    Console.WriteLine("Sent to joiner!!");
                            //}
                            ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.JOIN_ROOM_SUCCESS);
                        }
                        else
                        {
                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_FAILURE.ToString() + ":" + "Failed to join room.");
                            ns.Write(data, 0, data.Length);
                        }
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.LEAVE_ROOM.ToString()))
                    {
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.LEAVE_ROOM);
                        inLobby = true;
                        isReady = false;
                        inGame = false;
                        isSpectator = false;
                        hostMadeRenju = true;
                        aiPicture = 1;
                        myPicture = 0;
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.PLAYER_IS_READY.ToString()))
                    {
                        isReady = true;
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.PLAYER_IS_READY);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.PLAYER_UNREADY.ToString()))
                    {
                        isReady = false;
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.PLAYER_UNREADY);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.UNSET_RENJU_RULES.ToString()))
                    {
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.UNSET_RENJU_RULES);
                        hostMadeRenju = false;
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.UNSET_RENJU_RULES.ToString() + ":" + hostMadeRenju);
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_RENJU_RULES.ToString()))
                    {
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.SET_RENJU_RULES);
                        hostMadeRenju = true;
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.SET_RENJU_RULES.ToString() + ":" + hostMadeRenju);
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.START_GAME.ToString()))
                    {
                        ThreadedTCPServer.SendMessageToOthers(this, PACKET_TYPE.START_GAME);
                        inLobby = false;
                        isReady = false;
                        inGame = true;
                        isSpectator = false;
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_ROOMS_TO_JOIN.ToString()))
                    {
                        //inLobby = true;
                        //isReady = false;
                        //inGame = false;
                        //isSpectator = false;

                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_ROOMS_TO_JOIN.ToString() + ":" + ThreadedTCPServer.GetHostedRoomList());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_ROOMS_TO_SPECTATE.ToString()))
                    {
                        //inLobby = true;
                        //isReady = false;
                        //inGame = false;
                        //isSpectator = false;

                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_ROOMS_TO_SPECTATE.ToString() + ":" + ThreadedTCPServer.GetSpectatorRooms());
                        ns.Write(data, 0, data.Length);
                    }
                    else
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.ERROR_PACKET.ToString() + ":" + "You sent unknown packet?");
                        ns.Write(data, 0, data.Length);
                    }

                    if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_MY_MOVE.ToString()))
                    {
                        Console.WriteLine("Got your move packet!");
                        int placementIndex = Int32.Parse(fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1));
                        gameSession.SendTurnInfo(placementIndex);
                    }

                    #endregion CheckWhatPacketIGet

                    //else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.MAP_DATA.ToString()))
                    //{
                    //    data = new byte[1024];
                    //    data = Encoding.ASCII.GetBytes(string.Join(", ", mapData));

                    //    ns.Write(data, 0, data.Length);
                    //}
                }
            }

            ThreadedTCPServer.ClientDisconnected(this);

            ns.Close();
            client.Close();
            connections--;
            Console.WriteLine("Client disconnected: {0} active connections",
                               connections);
        }
    }

    /// <summary>
    /// The class will assign new connections to new threads
    /// 
    /// Try to use non-obsolete codes
    /// </summary>
    public class ThreadedTCPServer
    {
        private TcpListener client;
        static List<ConnectionThread> listOfConnections = new List<ConnectionThread>();
        static List<ConnectionThread> listOfHostedRooms = new List<ConnectionThread>();
        static List<OngoingGame> listOfGamesWaiting = new List<OngoingGame>();
        static List<GameInformation> listOfOngoingGames = new List<GameInformation>();
        static int uniqueID = 0;

        //Directional winning check
        public static int[] DirectionalCheckX = new int[4] { 1, 0, 1, 1 };
        public static int[] DirectionalCheckY = new int[4] { 0, 1, 1, -1 };

        /// <summary>
        /// Tools for converting XY Pos into Index
        /// </summary>
        /// <returns>Index position of array</returns>
        static int ConvertXYPosition(int positionX, int positionY)
        {
            return positionY * 15 + positionX;
        }

        /// <summary>
        /// Tools for converting index into position X and Y
        /// </summary>
        static void ConvertArrayPosition(int index, ref int positionX, ref int positionY)
        {
            positionX = index % 15;
            positionY = index / 15;
        }

        public static void GameHasEnded(OngoingGame theGameInfo)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                if (iter.thePlayers.theHost.clientName == theGameInfo.theHost.clientName && iter.thePlayers.theOpponent.clientName == theGameInfo.theOpponent.clientName)
                {
                    iter.thePlayers.theHost.inLobby = true;
                    iter.thePlayers.theHost.inGame = false;
                    iter.thePlayers.theHost.isReady = false;
                    iter.thePlayers.theOpponent.inLobby = true;
                    iter.thePlayers.theOpponent.inGame = false;
                    iter.thePlayers.theOpponent.isReady = false;

                    listOfOngoingGames.Remove(iter);
                    break;
                }
            }
        }

        public static bool IsClientConnected(TcpClient theClient)
        {
            if (theClient.Connected)
            {
                if ((theClient.Client.Poll(0, SelectMode.SelectWrite)) && (!theClient.Client.Poll(0, SelectMode.SelectError)))
                {
                    byte[] buffer = new byte[1];
                    if (theClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SendMessageToOthers(ConnectionThread theSender, PACKET_TYPE thePacketHeader, string extraMessage = "")
        {
            byte[] data = new byte[1024];
            ConnectionThread theReceiver = null;

            foreach (GameInformation iter in listOfOngoingGames)
            {
                if (iter.thePlayers.theHost.clientName == theSender.clientName)
                {
                    theReceiver = iter.thePlayers.theOpponent;
                    if (thePacketHeader == PACKET_TYPE.OPPONENT_DISCONNECTED)
                    {
                        listOfOngoingGames.Remove(iter);
                        theReceiver.inLobby = true;
                        theReceiver.isReady = false;
                        theReceiver.myPicture = 0;
                        theReceiver.aiPicture = 1;
                    }
                    break;
                }
                else if (iter.thePlayers.theOpponent.clientName == theSender.clientName)
                {
                    theReceiver = iter.thePlayers.theHost;
                    if (thePacketHeader == PACKET_TYPE.OPPONENT_DISCONNECTED)
                    {
                        listOfOngoingGames.Remove(iter);
                        theReceiver.inLobby = true;
                        theReceiver.isReady = false;
                        theReceiver.myPicture = 0;
                        theReceiver.aiPicture = 1;
                    }
                    break;
                }
            }

            if (theReceiver == null)
            {
                foreach (OngoingGame iter in listOfGamesWaiting)
                {
                    if (iter.theHost.clientName == theSender.clientName)
                    {
                        theReceiver = iter.theOpponent;
                        if (thePacketHeader == PACKET_TYPE.LEAVE_ROOM)
                        {
                            listOfGamesWaiting.Remove(iter);

                            if (iter.isAiGame)
                                return;

                            theReceiver.inLobby = true;
                            ThreadedTCPServer.CreateNewRoom(theReceiver);
                            theReceiver.isReady = false;
                            theSender.isReady = false;
                            theReceiver.hostMadeRenju = theSender.hostMadeRenju = true;
                            theReceiver.aiPicture = theSender.aiPicture = 1;
                            theReceiver.myPicture = theSender.myPicture = 0;
                        }
                        else if (thePacketHeader == PACKET_TYPE.OPPONENT_DISCONNECTED)
                        {
                            listOfGamesWaiting.Remove(iter);

                            if (iter.isAiGame)
                                return;

                            listOfHostedRooms.Add(theReceiver);
                            theReceiver.isReady = false;
                        }
                        else if (thePacketHeader == PACKET_TYPE.START_GAME)
                        {
                            Console.WriteLine("Value 1 : " + theSender.isReady.ToString() + " Value 2 : " + theReceiver.isReady.ToString());
                            if (theReceiver.isReady && theSender.isReady || iter.isAiGame)
                            {
                                thePacketHeader = PACKET_TYPE.START_GAME_SUCCESS;
                                listOfGamesWaiting.Remove(iter);
                                iter.theHost.inGame = iter.theOpponent.inGame = true;
                                iter.theHost.gameSession = iter.theOpponent.gameSession = ThreadedTCPServer.StartNewGame(iter.theHost, iter.theOpponent, iter.isAiGame, theSender.hostMadeRenju, iter.theHost.myPicture, (iter.isAiGame == true ? iter.theHost.aiPicture : iter.theOpponent.myPicture));
                                theReceiver.isReady = false;
                                theSender.isReady = false;

                                //if (iter.isAiGame)
                                //    return;
                            }
                            else
                                thePacketHeader = PACKET_TYPE.START_GAME_FAILURE;
                        }
                        break;
                    }
                    else if (iter.theOpponent.clientName == theSender.clientName)
                    {
                        theReceiver = iter.theHost;
                        if (thePacketHeader == PACKET_TYPE.LEAVE_ROOM)
                        {
                            listOfGamesWaiting.Remove(iter);
                            theReceiver.inLobby = true;
                            ThreadedTCPServer.CreateNewRoom(theReceiver);
                            theReceiver.isReady = false;
                            theSender.isReady = false;
                            theReceiver.hostMadeRenju = theSender.hostMadeRenju = true;
                        }
                        else if (thePacketHeader == PACKET_TYPE.OPPONENT_DISCONNECTED)
                        {
                            listOfGamesWaiting.Remove(iter);
                            listOfHostedRooms.Add(theReceiver);
                            theReceiver.isReady = false;
                        }
                        else if (thePacketHeader == PACKET_TYPE.START_GAME)
                        {
                            Console.WriteLine("Value 1 : " + theSender.isReady.ToString() + " Value 2 : " + theReceiver.isReady.ToString());
                            if (theReceiver.isReady && theSender.isReady || iter.isAiGame)
                            {
                                thePacketHeader = PACKET_TYPE.START_GAME_SUCCESS;
                                listOfGamesWaiting.Remove(iter);
                                ThreadedTCPServer.StartNewGame(iter.theHost, iter.theOpponent, iter.isAiGame, iter.theHost.hostMadeRenju, iter.theHost.myPicture, (iter.isAiGame == true ? iter.theHost.aiPicture : iter.theOpponent.myPicture));
                                theReceiver.isReady = false;
                                theSender.isReady = false;

                                //if (iter.isAiGame)
                                //    return;
                            }
                            else
                                thePacketHeader = PACKET_TYPE.START_GAME_FAILURE;
                        }
                        break;
                    }
                }
            }

            if (theReceiver == null)
            {
                foreach (ConnectionThread iter in listOfHostedRooms)
                {
                    if (iter.clientName == theSender.clientName && thePacketHeader == PACKET_TYPE.LEAVE_ROOM)
                    {
                        listOfHostedRooms.Remove(iter);
                        theSender.inLobby = true;
                        theSender.isReady = false;
                        theSender.hostMadeRenju = true;
                        return;
                    }
                }
            }

            if (theReceiver != null)
            {
                if (thePacketHeader == PACKET_TYPE.JOIN_ROOM_SUCCESS)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString() + ":" + theSender.clientName);
                    theReceiver.isReady = false;
                    theSender.isReady = false;
                    theReceiver.myPicture = theSender.myPicture = 0;
                    theReceiver.aiPicture = theSender.aiPicture = 1;
                    Console.WriteLine(theSender.clientName + " Connected to the room of " + theReceiver.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.LEAVE_ROOM)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.LEAVE_ROOM.ToString() + ":" + theSender.clientName);
                    Console.WriteLine(theSender.clientName + " Left the room of " + theReceiver.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.UNSET_RENJU_RULES)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.UNSET_RENJU_RULES.ToString() + ":" + theSender.hostMadeRenju);
                    Console.WriteLine(theSender.clientName + " UnSet Renju Rules " + theReceiver.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.SET_RENJU_RULES)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.SET_RENJU_RULES.ToString() + ":" + theSender.hostMadeRenju);
                    Console.WriteLine(theSender.clientName + " Set Renju Rules " + theReceiver.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.OPPONENT_DISCONNECTED)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.OPPONENT_DISCONNECTED.ToString() + ":" + theSender.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.PLAYER_IS_READY)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.PLAYER_IS_READY.ToString() + ":" + theSender.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.PLAYER_UNREADY)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.PLAYER_UNREADY.ToString() + ":" + theSender.clientName);
                }
                else if (thePacketHeader == PACKET_TYPE.START_GAME_SUCCESS)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.START_GAME_SUCCESS.ToString() + ":" + extraMessage);
                    do
                    {
                        theSender.ns.Write(data, 0, data.Length);
                        Console.WriteLine("Sent data to sender too");
                    }
                    while (!theSender.ns.CanWrite);
                    //if (theSender.ns.CanWrite)
                    //{
                    //    //Console.WriteLine(theReceiver.clientName);
                    //    theSender.ns.Write(data, 0, data.Length);
                    //    Console.WriteLine("Sent data to sender too");
                    //}
                }
                else if (thePacketHeader == PACKET_TYPE.START_GAME_FAILURE)
                {
                    data = Encoding.ASCII.GetBytes(PACKET_TYPE.START_GAME_FAILURE.ToString() + ":" + extraMessage);
                    do
                    {
                        theSender.ns.Write(data, 0, data.Length);
                        Console.WriteLine("Sent data to sender too");
                    }
                    while (!theSender.ns.CanWrite);
                    //if (theSender.ns.CanWrite)
                    //{
                    //    //Console.WriteLine(theReceiver.clientName);
                    //    theSender.ns.Write(data, 0, data.Length);
                    //    Console.WriteLine("Sent data to sender too");
                    //}
                }

                //TcpClient tempClient = theReceiver.threadListener.AcceptTcpClient();
                //NetworkStream ns = tempClient.GetStream();
                do
                {
                    theReceiver.ns.Write(data, 0, data.Length);
                    Console.WriteLine("Above Statement Success");
                }
                while (!theReceiver.ns.CanWrite);

                //if (theReceiver.ns.CanWrite)
                //{
                //    //Console.WriteLine(theReceiver.clientName);
                //    theReceiver.ns.Write(data, 0, data.Length);
                //    Console.WriteLine("Above Statement Success");
                //}
                //ns.Close();
                //tempClient.Close();
            }
        }

        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GiveUniqueID()
        {
            return "#0" + (uniqueID++).ToString();
        }

        public static void AddNewConnectedClient(ConnectionThread newConnection)
        {
            listOfConnections.Add(newConnection);
        }

        public static string GetSpectatorRooms()
        {
            List<string> getNames = new List<string>();
            bool hasGames = false;

            foreach (GameInformation iter in listOfOngoingGames)
            {
                getNames.Add(iter.thePlayers.theHost.clientName);
                hasGames = true;
            }

            if (hasGames)
                return string.Join(",", getNames.ToArray());
            else
                return "No Games Found";
        }

        public static string GetHostedRoomList()
        {
            List<string> getNames = new List<string>();
            bool hasGames = false;

            foreach (ConnectionThread iter in listOfHostedRooms)
            {
                if (iter.clientName == null || iter.clientName == "")
                    continue;

                getNames.Add(iter.clientName);
                hasGames = true;
            }

            if (hasGames)
                return string.Join(",", getNames.ToArray());
            else
                return "No Games Found";
        }

        public static string GetListOfConnectedUsers()
        {
            List<string> getNames = new List<string>();
            foreach (ConnectionThread iter in listOfConnections)
            {
                if (iter.clientName == null || iter.clientName == "")
                    continue;

                getNames.Add(iter.clientName);
            }
            return string.Join(",", getNames.ToArray());
        }

        //public static string GetListOfOngoingGames()
        //{
        //    List<string> getNames = new List<string>();
        //    foreach (ConnectionThread iter in listOfOngoingGames)
        //    {
        //        getNames.Add(iter.clientName);
        //    }

        //    return string.Join(", ", getNames.ToArray());
        //}

        public static void CreateNewRoom(ConnectionThread theCreator)
        {
            if (theCreator.inLobby)
                listOfHostedRooms.Add(theCreator);

            theCreator.inLobby = false;
        }

        public static bool JoinRoom(string targetName, ConnectionThread joiningPlayer)
        {
            foreach(ConnectionThread iter in listOfHostedRooms)
            {
                if (iter.clientName == targetName && !iter.inLobby)
                {
                    OngoingGame newGame = new OngoingGame(iter, joiningPlayer);
                    listOfGamesWaiting.Add(newGame);
                    listOfHostedRooms.Remove(iter);
                    return true;
                }
            }
            return false;
        }

        public static bool CreateAIGame(ConnectionThread theHost)
        {
            foreach (ConnectionThread iter in listOfHostedRooms)
            {
                if (iter.clientName == theHost.clientName && !iter.inLobby)
                {
                    OngoingGame newGame = new OngoingGame(iter, iter, true);
                    listOfGamesWaiting.Add(newGame);
                    listOfHostedRooms.Remove(iter);
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveAIGame(ConnectionThread theHost)
        {
            foreach (OngoingGame iter in listOfGamesWaiting)
            {
                if (iter.theHost.clientName == theHost.clientName && iter.isAiGame)
                {
                    listOfGamesWaiting.Remove(iter);
                    //theHost.inLobby = true;
                    //CreateNewRoom(theHost);
                    listOfHostedRooms.Add(theHost);
                    return true;
                }
            }
            return false;
        }

        public static void GetPlayerOnePicture(ConnectionThread theSender)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                foreach (ConnectionThread spectator in iter.listOfSpectators)
                {
                    if (spectator.clientName == theSender.clientName)
                    {
                        byte[] data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_ONE_PICTURE.ToString() + ":" +
                            iter.thePlayers.theHost.myPicture);

                        spectator.ns.Write(data, 0, data.Length);
                    }
                }
            }
        }

        public static void GetPlayerTwoPicture(ConnectionThread theSender)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                foreach (ConnectionThread spectator in iter.listOfSpectators)
                {
                    if (spectator.clientName == theSender.clientName)
                    {
                        byte[] data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_PLAYER_TWO_PICTURE.ToString() + ":" +
                            iter.thePlayers.theOpponent.myPicture);

                        spectator.ns.Write(data, 0, data.Length);
                    }
                }
            }
        }

        public static bool JoinSpectateRoom(string targetName, ConnectionThread joiningPlayer)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                if (iter.thePlayers.theHost.clientName == targetName)
                {
                    iter.AddNewSpectator(joiningPlayer);
                    joiningPlayer.inLobby = false;
                    Console.WriteLine("Spectator True");
                    return true;
                }
            }
                    Console.WriteLine("Spectator False");
            return false;
        }

        public static string GetIndexPlacementOfPlayers(string targetName)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                if (iter.thePlayers.theHost.clientName == targetName)
                {
                    if (iter.thePlayers.hostIndex == 1)
                    {
                        return (iter.thePlayers.theHost.clientName + "," + iter.thePlayers.theOpponent.clientName);
                    }
                    else if (iter.thePlayers.opponentIndex == 1)
                    {
                        return (iter.thePlayers.theOpponent.clientName + "," + iter.thePlayers.theHost.clientName);
                    }   
                }
            }

            return "Not Found";
        }


        public static GameInformation StartNewGame(ConnectionThread theHost, ConnectionThread theOpponent, bool isAiGame = false, bool setRenju = true, int playerOne = 0, int playerTwo = 0)
        {
            GameInformation newGame = new GameInformation(theHost, theOpponent, isAiGame, setRenju, playerOne, playerTwo);
            listOfOngoingGames.Add(newGame);
            return newGame;
        }
        
        public static void ClientDisconnected(ConnectionThread theClient)
        {
            ThreadedTCPServer.SendMessageToOthers(theClient, PACKET_TYPE.OPPONENT_DISCONNECTED);

            foreach (ConnectionThread iter in listOfHostedRooms)
            {
                if (iter.clientName == theClient.clientName)
                {
                    listOfHostedRooms.Remove(iter);
                    break;
                }
            }

            foreach (GameInformation iter in listOfOngoingGames)
            {
                //if (iter.thePlayers.theHost.clientName == theClient.clientName)
                //{
                //    iter.thePlayers.theOpponent.inLobby = true;
                //    listOfOngoingGames.Remove(iter);
                //    break;
                //}
                //else if (iter.thePlayers.theOpponent.clientName == theClient.clientName)
                //{
                //    iter.thePlayers.theHost.inLobby = true;
                //    listOfOngoingGames.Remove(iter);
                //    break;
                //}

                if (iter.listOfSpectators.Any())
                {
                    foreach (ConnectionThread getName in iter.listOfSpectators)
                    {
                        if (getName.clientName == theClient.clientName)
                        {
                            iter.listOfSpectators.Remove(getName);
                            break;
                        }
                    }
                }
            }

            foreach (ConnectionThread iter in listOfConnections)
            {
                if (iter.clientName == theClient.clientName)
                {
                    listOfConnections.Remove(iter);
                    break;
                }
            }
        }

        public ThreadedTCPServer()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            Console.WriteLine(ipAddress.ToString());
            try
            {
                client = new TcpListener(ipAddress, 7777);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //client = new TcpListener(9050);
            client.Start();

            Console.WriteLine("Waiting for clients...");
            while (true)
            {
                while (!client.Pending())
                {
                    Thread.Sleep(1000);
                }

                ConnectionThread newconnection = new ConnectionThread();
                newconnection.threadListener = this.client;
                newconnection.StartConnection();
                //listOfConnections.Add(newconnection);
                AddNewConnectedClient(newconnection);

                Thread newthread = new Thread(new
                          ThreadStart(newconnection.HandleConnection));
                newthread.Start();
            }
        }

        public static void Main(string[] args)
        {
            ThreadedTCPServer server = new ThreadedTCPServer();
        }
    }
}
