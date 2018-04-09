using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TCPServer
{
    public enum PACKET_TYPE
    {
        ASSIGN_NAME_PACKET = 0,

        CREATE_NEW_ROOM,
        ROOM_CREATED_SUCCESS,
        ROOM_CREATED_FAILURE,

        JOIN_ROOM,
        JOIN_ROOM_SUCCESS,
        JOIN_ROOM_FAILURE,

        SET_PLAYER_READY,
        PLAYER_IS_READY,

        START_GAME,
        START_GAME_SUCCESS,
        START_GAME_FAILURE,

        LEAVE_ROOM,

        GET_ALL_CONNECTED_USERS,
        GET_ROOMS_TO_JOIN,
        GET_ROOMS_TO_SPECTATE,
        GET_CURRENT_USERS_IN_LOBBY,

        SET_MY_MOVE,
        MOVE_REJECTED,
        MOVE_ACCEPTED,
        GET_OPPONENT_MOVE,
        GET_MAP_DATA,

        OPPONENT_DISCONNECTED,

        TOTAL_TYPES_OF_PACKETS,
    }


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

    public class ConnectionClass
    {
        TcpClient client;
        NetworkStream ns;
        string ipAddress;
        Int32 portNumber;
        private Thread recvThread = null;

        Queue<String> queueOfMessages;

        //Constructor to setup the struct
        public ConnectionClass(string theIpAddress, Int32 thePort)
        {
            //client = new TcpClient(theIpAddress, thePort);
            client = new TcpClient();
            try
            {
                client.Connect(theIpAddress, thePort);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't connect. Reason can be :\r\n1.Server is down.\r\n2.You lost internet connection");
            }

            queueOfMessages = new Queue<string>();
            ns = client.GetStream();

            ipAddress = theIpAddress;
            portNumber = thePort;

            if (recvThread != null)
                recvThread.Abort();

            recvThread = new Thread(new ThreadStart(RecieveMessage));
            recvThread.IsBackground = true;
            recvThread.Start();
        }

        //This region contains the section to change ip or port
        #region Change IP or Port
        public void ChangeIPAddress(string theIpAddress)
        {
            client.Connect(theIpAddress, portNumber);
            ns = client.GetStream();
        }

        public void ChangePort(Int32 thePort)
        {
            client.Connect(ipAddress, thePort);
            ns = client.GetStream();
        }

        public void ChangeIPAddressAndPort(string theIpAddress, Int32 thePort)
        {
            client.Connect(theIpAddress, thePort);
            ns = client.GetStream();
        }
        #endregion Change IP or Port

        //This region handles the connection or disconnection to/from server
        #region CheckConnection or Disconnect

        /// <summary>
        /// Checks the connection state
        /// </summary>
        /// <returns>True on connected. False on disconnected.</returns>
        public bool IsConnected()
        {
            return client.Connected;
        }

        public void DisconnectFromServer()
        {
            client.Client.Shutdown(SocketShutdown.Both);
            client.GetStream().Close();
            client.Close();
        }
        #endregion Connect or Disconnect

        public void SendMessage(PACKET_TYPE packetType, string message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message);
            ns.Write(data, 0, data.Length);
        }

        public void SendMessage(PACKET_TYPE packetType, int message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message.ToString());
            ns.Write(data, 0, data.Length);

        }

        public void SendPosition(int position)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.SET_MY_MOVE.ToString() + ":" + position.ToString());
            ns.Write(data, 0, data.Length);

        }

        //public string TestRecieve()
        //{
        //    if (ns.CanRead)
        //    {
        //        byte[] data = new byte[client.ReceiveBufferSize];
        //        StringBuilder fullPacketMessage = new StringBuilder();
        //        int numbersOfBytesRead;
                
        //        do
        //        {
        //            numbersOfBytesRead = ns.Read(data, 0, data.Length);
        //            if (numbersOfBytesRead <= 0)
        //                break;
        //            fullPacketMessage.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numbersOfBytesRead));
        //        } while (ns.DataAvailable);

        //        return fullPacketMessage.ToString();
        //    }

        //    return "Error in recv message!";
        //}

        void RecieveMessage()
        {
            while (true)
            {
                if (ns.CanRead)
                {
                    byte[] data = new byte[client.ReceiveBufferSize];
                    StringBuilder fullPacketMessage = new StringBuilder();
                    int numbersOfBytesRead;

                    do
                    {
                        numbersOfBytesRead = ns.Read(data, 0, data.Length);
                        if (numbersOfBytesRead <= 0)
                            break;
                        fullPacketMessage.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numbersOfBytesRead));
                    } while (ns.DataAvailable);

                    lock(queueOfMessages)
                        queueOfMessages.Enqueue(fullPacketMessage.ToString());
                }
                else
                {
                    lock(queueOfMessages)
                        queueOfMessages.Enqueue("Error in recv message! Were you disconnected? ");
                }
            }
        }

        public string RecieveFromQueue()
        {
            string tempString;

            lock (queueOfMessages)
            {
                if (queueOfMessages.Count > 0)
                    tempString = queueOfMessages.Dequeue();
                else
                    tempString = "No Message";
            }

            return tempString;
        }

        public void ShutdownThread()
        {
            if (recvThread != null)
                recvThread.Abort();
        }
    }
}
