using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCPServer;
using System.Collections.Generic;
using System.Web;

namespace OmokServer
{
    /// <summary>
    /// The class that manage the connection within each thread
    /// 
    /// </summary>
    /// 
    
    public class GameInformation
    {
        //Map data
        private int[] mapData;

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

        public void HandleConnection()
        {
            byte[] data = new byte[1024];

            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            connections++;
            Console.WriteLine("New client accepted: {0} active connections",
                              connections);

            string welcome = "Welcome to my test server!";
            data = Encoding.ASCII.GetBytes(welcome);
            //client.NoDelay = true;

            ns.Write(data, 0, data.Length);

            inLobby = true;
            isSpectator = false;
            gameSession = null;

            //TestMapData();
            //Console.WriteLine(string.Join(", ", mapData));
            while (true)
            {
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
                        break;

                    Console.WriteLine("Message received from {0}:", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                    // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                    Console.WriteLine(fullPacketMessage.ToString());

                    if (fullPacketMessage.ToString().Contains(PACKET_TYPE.PLACEMENT_PACKET.ToString()))
                    {
                        //ns.Write(data, 0, recv);
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes("I've got your placement message! Thanks!");
                        //data = Encoding.ASCII.GetBytes("Calvert,Eu Kern,Zi Sheng,Player 1,Player 2,Player 3,Player 4,Player 5,Player6,Player 7,Player 8,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat");
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.LOBBY_LIST.ToString()))
                    {
                        data = new byte[1024];
                        //data = Encoding.ASCII.GetBytes("You sent me a packet that did not contain a placement message! " + PACKET_TYPE.END_OF_PACKET.ToString());
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.LOBBY_LIST.ToString() + ":" + "Calvert,Eu Kern,Zi Sheng,Player 1,Player 2,Player 3,Player 4,Player 5,Player6,Player 7,Player 8,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat");

                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.CREATE_ROOM.ToString()))
                    {
                        ThreadedTCPServer.CreateNewRoom(this);
                        inLobby = false;
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.SET_NAME_PACKET.ToString()))
                    {
                        clientName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":"));

                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes("Your name is " + clientName.ToString());
                        ns.Write(data, 0, data.Length);
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.JOIN_ROOM.ToString()))
                    {
                        string targetName = fullPacketMessage.ToString().Substring(fullPacketMessage.ToString().IndexOf(":"));
                        if (ThreadedTCPServer.JoinRoom(targetName, this))
                            Console.WriteLine("Success in joining room");
                        else
                            Console.WriteLine("No rooms found");
                    }
                    else if (fullPacketMessage.ToString().Contains(PACKET_TYPE.ROOM_LIST.ToString()))
                    {
                        data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(PACKET_TYPE.ROOM_LIST.ToString() + ":" + ThreadedTCPServer.GetHostedRoomList());
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
        static List<Tuple<ConnectionThread, ConnectionThread>> listOfGamesWaiting = new List<Tuple<ConnectionThread, ConnectionThread>>();
        static List<GameInformation> listOfOngoingGames = new List<GameInformation>();

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

        public static string GetHostedRoomList()
        {
            List<string> getNames = new List<string>();
            foreach (ConnectionThread iter in listOfHostedRooms)
            {
                getNames.Add(iter.clientName);
            }

            return string.Join(", ", getNames.ToArray());
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
            listOfHostedRooms.Add(theCreator);
        }

        public static bool JoinRoom(string targetName, ConnectionThread joiningPlayer)
        {
            foreach(ConnectionThread iter in listOfHostedRooms)
            {
                if (iter.clientName == targetName && !iter.inLobby)
                {
                    Tuple<ConnectionThread, ConnectionThread> newPair = new Tuple<ConnectionThread, ConnectionThread>(iter, joiningPlayer);
                    listOfGamesWaiting.Add(newPair);
                    listOfHostedRooms.Remove(iter);
                    return true;
                }
            }
            return false;
        }

        public static GameInformation StartNewGame()
        {
            GameInformation newGame = new GameInformation();
            newGame.GenerateEmptyMap();
            listOfOngoingGames.Add(newGame);
            return newGame;
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
                listOfConnections.Add(newconnection);

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
