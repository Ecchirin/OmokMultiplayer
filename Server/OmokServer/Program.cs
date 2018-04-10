using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCPServer;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Decode XY position = Y pos * col + X pos
/// Decode Index pos into XY
/// y = index / col
/// X = index % col
/// </summary>
namespace OmokServer
{

    public struct OngoingGame
    {
        public ConnectionThread theHost, theOpponent;
    }

    public class GameInformation
    {
        //Map data
        private int[] mapData;

        //Player Data
        public OngoingGame thePlayers;

        public GameInformation(ConnectionThread theHost, ConnectionThread theOpponent)
        {
            thePlayers.theHost = theHost;
            thePlayers.theOpponent = theOpponent;
            GenerateEmptyMap();
        }

        public void GenerateEmptyMap()
        {
            mapData = new int[225];
            for (int i = 0; i < 225; ++i)
            {
                mapData[i] = 0;
            }
        }

        public void SetPlacement(int index, int playerIndex)
        {
            if (playerIndex == 1 || playerIndex == 2)
                mapData[index] = playerIndex;
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
        public bool inLobby, isSpectator;
        public GameInformation gameSession;

        //Identification
        public string clientName;

        public TcpListener threadListener;
        private static int connections = 0;

        public TcpClient client;
        public NetworkStream ns;

        public void StartConnection()
        {
            client = threadListener.AcceptTcpClient();
            ns = client.GetStream();
            connections++;
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

            inLobby = true;
            isSpectator = false;
            gameSession = null;

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

                    if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_MY_MOVE.ToString()))
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes("I've got your placement message! Thanks!");
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_ALL_CONNECTED_USERS.ToString()))
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_ALL_CONNECTED_USERS.ToString() + ":" + ThreadedTCPServer.GetListOfConnectedUsers());
                        Console.WriteLine(ThreadedTCPServer.GetListOfConnectedUsers());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.CREATE_NEW_ROOM.ToString()))
                    {
                        ThreadedTCPServer.CreateNewRoom(this);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString()))
                    {
                        clientName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1) + ThreadedTCPServer.GiveUniqueID();

                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.ASSIGN_NAME_PACKET.ToString() + ":" + clientName.ToString());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.JOIN_ROOM.ToString()))
                    {
                        data = new byte[1024];
                        string targetName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":") + 1);

                        if (ThreadedTCPServer.JoinRoom(targetName, this))
                        {
                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString() + ":" + targetName);

                            byte[] data2 = new byte[1024];
                            data2 = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_SUCCESS.ToString() + ":" + clientName);
                            ThreadedTCPServer.SendMessageToOthers(this, data2);
                        }
                        else
                            data = Encoding.ASCII.GetBytes(PACKET_TYPE.JOIN_ROOM_FAILURE.ToString() + ":" + "Failed to join room.");

                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.GET_ROOMS_TO_JOIN.ToString()))
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.GET_ROOMS_TO_JOIN.ToString() + ":" + ThreadedTCPServer.GetHostedRoomList());
                        ns.Write(data, 0, data.Length);
                    }
                    else
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.ERROR_PACKET.ToString() + ":" + "You sent unknown packet?");
                        ns.Write(data, 0, data.Length);
                    }
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

        public static void SendMessageToOthers(ConnectionThread theSender, byte[] theMessage)
        {
            ConnectionThread theReceiver = null;

            foreach (OngoingGame iter in listOfGamesWaiting)
            {
                if (iter.theHost.clientName == theSender.clientName)
                {
                    theReceiver = iter.theOpponent;
                    break;
                }
                else if (iter.theOpponent.clientName == theSender.clientName)
                {
                    theReceiver = iter.theHost;
                    break;
                }
            }

            if (theReceiver == null)
            {
                foreach (GameInformation iter in listOfOngoingGames)
                {
                    if (iter.thePlayers.theHost.clientName == theSender.clientName)
                    {
                        theReceiver = iter.thePlayers.theOpponent;
                        break;
                    }
                    else if (iter.thePlayers.theOpponent.clientName == theSender.clientName)
                    {
                        theReceiver = iter.thePlayers.theHost;
                        break;
                    }
                }
            }

            if (theReceiver != null)
            {
                //TcpClient tempClient = theReceiver.threadListener.AcceptTcpClient();
                //NetworkStream ns = tempClient.GetStream();
                if (theReceiver.ns.CanWrite)
                    theReceiver.ns.Write(theMessage, 0, theMessage.Length);
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
                    OngoingGame newGame = new OngoingGame();
                    newGame.theHost = iter;
                    newGame.theOpponent = joiningPlayer;
                    listOfGamesWaiting.Add(newGame);
                    listOfHostedRooms.Remove(iter);
                    return true;
                }
            }
            return false;
        }

        public static GameInformation StartNewGame(ConnectionThread theHost, ConnectionThread theOpponent)
        {
            GameInformation newGame = new GameInformation(theHost, theOpponent);
            newGame.GenerateEmptyMap();
            listOfOngoingGames.Add(newGame);
            return newGame;
        }

        public static void ClientDisconnected(ConnectionThread theClient)
        {
            foreach (GameInformation iter in listOfOngoingGames)
            {
                if (iter.thePlayers.theHost.clientName == theClient.clientName)
                {
                    iter.thePlayers.theOpponent.inLobby = true;
                    listOfOngoingGames.Remove(iter);
                    break;
                }
                else if (iter.thePlayers.theOpponent.clientName == theClient.clientName)
                {
                    iter.thePlayers.theHost.inLobby = true;
                    listOfOngoingGames.Remove(iter);
                    break;
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
