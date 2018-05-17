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
    public static class ForbiddenPointFinder
    {
        //Directional winning check
        public static int[] DirectionalCheckX = new int[4] { 1, 0, 1, 1 };
        public static int[] DirectionalCheckY = new int[4] { 0, 1, 1, -1 };

        static int CheckPlacementOfMap(int x, int y, int[] mapData)
        {
            int theArrayIndex = ConnectionClass.ConvertXYPositionToIndex(x, y);
            if (theArrayIndex < 0 || theArrayIndex >= 225)
                return 25;

            return mapData[theArrayIndex];
        }

        public static bool IsFive(int x, int y, int[] mapData, int colorIndex)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            int directionalCheck, forwardPlacements, backwardPlacements, checkX, checkY;
            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];
            int prevIndex = thePlacementIndex;

            if (thePlacementIndex == 0)
            {
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = colorIndex;
                thePlacementIndex = colorIndex;
            }

            for (directionalCheck = 0; directionalCheck < 4; ++directionalCheck)
            {
                forwardPlacements = backwardPlacements = 0;
                for (checkX = x + DirectionalCheckX[directionalCheck],
                     checkY = y + DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX += DirectionalCheckX[directionalCheck],
                     checkY += DirectionalCheckY[directionalCheck])
                    forwardPlacements++;

                for (checkX = x - DirectionalCheckX[directionalCheck],
                     checkY = y - DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX -= DirectionalCheckX[directionalCheck],
                     checkY -= DirectionalCheckY[directionalCheck])
                    backwardPlacements++;

                if (forwardPlacements + backwardPlacements == 4)
                {
                    if (prevIndex == 0)
                        mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
                    return true;
                }
            }

            if (prevIndex == 0)
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
            return false;
        }

        public static bool IsOverline(int x, int y, int[] mapData)
        {
            int directionalCheck, forwardPlacements, backwardPlacements, checkX, checkY;
            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];

            for (directionalCheck = 0; directionalCheck < 4; ++directionalCheck)
            {
                forwardPlacements = backwardPlacements = 0;
                for (checkX = x + DirectionalCheckX[directionalCheck],
                     checkY = y + DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX += DirectionalCheckX[directionalCheck],
                     checkY += DirectionalCheckY[directionalCheck])
                    forwardPlacements++;

                for (checkX = x - DirectionalCheckX[directionalCheck],
                     checkY = y - DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX -= DirectionalCheckX[directionalCheck],
                     checkY -= DirectionalCheckY[directionalCheck])
                    backwardPlacements++;

                if (forwardPlacements + backwardPlacements > 4)
                    return true;
            }

            return false;
        }

        public static bool IsThree(int x, int y, int[] mapData, int colorIndex)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            int directionalCheck, forwardPlacements, backwardPlacements, checkX, checkY;
            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];
            int prevIndex = thePlacementIndex;

            if (thePlacementIndex == 0)
            {
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = colorIndex;
                thePlacementIndex = colorIndex;
            }

            for (directionalCheck = 0; directionalCheck < 4; ++directionalCheck)
            {
                forwardPlacements = backwardPlacements = 0;
                for (checkX = x + DirectionalCheckX[directionalCheck],
                     checkY = y + DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX += DirectionalCheckX[directionalCheck],
                     checkY += DirectionalCheckY[directionalCheck])
                    forwardPlacements++;

                for (checkX = x - DirectionalCheckX[directionalCheck],
                     checkY = y - DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX -= DirectionalCheckX[directionalCheck],
                     checkY -= DirectionalCheckY[directionalCheck])
                    backwardPlacements++;

                if (forwardPlacements + backwardPlacements == 2)
                {
                    if (prevIndex == 0)
                        mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
                    return true;
                }
            }

            if (prevIndex == 0)
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
            return false;
        }

        public static bool IsFour(int x, int y, int[] mapData, int colorIndex)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            int directionalCheck, forwardPlacements, backwardPlacements, checkX, checkY;
            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];
            int prevIndex = thePlacementIndex;

            if (thePlacementIndex == 0)
            {
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = colorIndex;
                thePlacementIndex = colorIndex;
            }

            for (directionalCheck = 0; directionalCheck < 4; ++directionalCheck)
            {
                forwardPlacements = backwardPlacements = 0;
                for (checkX = x + DirectionalCheckX[directionalCheck],
                     checkY = y + DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX += DirectionalCheckX[directionalCheck],
                     checkY += DirectionalCheckY[directionalCheck])
                    forwardPlacements++;

                for (checkX = x - DirectionalCheckX[directionalCheck],
                     checkY = y - DirectionalCheckY[directionalCheck];
                     CheckPlacementOfMap(checkX, checkY, mapData) == thePlacementIndex;
                     checkX -= DirectionalCheckX[directionalCheck],
                     checkY -= DirectionalCheckY[directionalCheck])
                    backwardPlacements++;
                
                if (forwardPlacements + backwardPlacements == 3)
                {
                    if (prevIndex == 0)
                        mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
                    return true;
                }
            }

            if (prevIndex == 0)
                mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)] = 0;
            return false;
        }

        static bool RecursiveCheckForFive(int[] mapData, int x, int y, int dirX, int dirY, int colorIndex)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            if (CheckPlacementOfMap(x, y, mapData) == (colorIndex == 1 ? 2 : 1))
                return false;

            if (CheckPlacementOfMap(x, y, mapData) == 0 && IsFive(x, y, mapData, colorIndex))
                return true;

            else if (CheckPlacementOfMap(x, y, mapData) == 0 && !IsFive(x, y, mapData, colorIndex))
                return false;

            return RecursiveCheckForFive(mapData, x + dirX, y + dirY, dirX, dirY, colorIndex);
        }

        static bool RecursiveCheckForFour(int[] mapData, int x, int y, int dirX, int dirY, int colorIndex)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            if (CheckPlacementOfMap(x, y, mapData) == (colorIndex == 1 ? 2 : 1))
                return false;

            if (CheckPlacementOfMap(x, y, mapData) == 0 && IsFour(x, y, mapData, colorIndex))
                return true;

            else if (CheckPlacementOfMap(x, y, mapData) == 0 && !IsFour(x, y, mapData, colorIndex))
                return false;

            return RecursiveCheckForFour(mapData, x + dirX, y + dirY, dirX, dirY, colorIndex);
        }

        public static int IsOpenFour(int x, int y, int[] mapData)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return 0;

            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];

            int numberOfOpenFours = 0;

            //check forward horizontal
            if (RecursiveCheckForFive(mapData, x, y, 1, 0, thePlacementIndex) && RecursiveCheckForFive(mapData, x, y, -1, 0, thePlacementIndex))
                numberOfOpenFours++;

            if (RecursiveCheckForFive(mapData, x, y, 0, 1, thePlacementIndex) && RecursiveCheckForFive(mapData, x, y, 0, -1, thePlacementIndex))
                numberOfOpenFours++;

            if (RecursiveCheckForFive(mapData, x, y, 1, 1, thePlacementIndex) && RecursiveCheckForFive(mapData, x, y, -1, -1, thePlacementIndex))
                numberOfOpenFours++;

            if (RecursiveCheckForFive(mapData, x, y, -1, 1, thePlacementIndex) && RecursiveCheckForFive(mapData, x, y, 1, -1, thePlacementIndex))
                numberOfOpenFours++;

            return numberOfOpenFours;
        }

        public static bool IsDoubleFour(int x, int y, int[] mapData)
        {
            if (IsOpenFour(x, y, mapData) >= 2)
                return true;

            return false;
        }

        public static int IsOpenThree(int x, int y, int[] mapData)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return 0;

            int thePlacementIndex = mapData[ConnectionClass.ConvertXYPositionToIndex(x, y)];

            int numberOfOpenThrees = 0;

            //check forward horizontal
            if (RecursiveCheckForFour(mapData, x, y, 1, 0, thePlacementIndex) && RecursiveCheckForFour(mapData, x, y, -1, 0, thePlacementIndex))
                numberOfOpenThrees++;

            if (RecursiveCheckForFour(mapData, x, y, 0, 1, thePlacementIndex) && RecursiveCheckForFour(mapData, x, y, 0, -1, thePlacementIndex))
                numberOfOpenThrees++;

            if (RecursiveCheckForFour(mapData, x, y, 1, 1, thePlacementIndex) && RecursiveCheckForFour(mapData, x, y, -1, -1, thePlacementIndex))
                numberOfOpenThrees++;

            if (RecursiveCheckForFour(mapData, x, y, -1, 1, thePlacementIndex) && RecursiveCheckForFour(mapData, x, y, 1, -1, thePlacementIndex))
                numberOfOpenThrees++;

            return numberOfOpenThrees;
        }

        public static bool IsDoubleThree(int x, int y, int[] mapData)
        {
            if (IsOpenThree(x, y, mapData) >= 2)
                return true;

            return false;
        }
    }

    public enum PACKET_TYPE
    {
        ASSIGN_NAME_PACKET = 0,

        CREATE_NEW_ROOM,
        ROOM_CREATED_SUCCESS,
        ROOM_CREATED_FAILURE,

        JOIN_ROOM,
        JOIN_ROOM_SPECTATE,
        JOIN_ROOM_SUCCESS,
        JOIN_ROOM_FAILURE,

        SET_PLAYER_READY,
        PLAYER_IS_READY,
        PLAYER_UNREADY,

        START_GAME,
        START_GAME_SUCCESS,
        START_GAME_FAILURE,

        LEAVE_ROOM,

        GET_ALL_CONNECTED_USERS,
        GET_ROOMS_TO_JOIN,
        GET_ROOMS_TO_SPECTATE,
        GET_CURRENT_USERS_IN_LOBBY,
        GET_RAW_GAME_INFO,

        SET_AI_GAME,
        UNSET_AI_GAME,
        FORCED_UPDATE,
        SET_MY_MOVE,
        MOVE_REJECTED,
        MOVE_ACCEPTED,
        GET_OPPONENT_MOVE,
        GET_MAP_DATA,

        SET_RENJU_RULES,
        UNSET_RENJU_RULES,

        GET_PLAYER_ONE_PICTURE,
        GET_PLAYER_TWO_PICTURE,
        SET_PICTURE,
        SET_AI_PICTURE,

        OPPONENT_DISCONNECTED,

        ERROR_PACKET,

        TOTAL_TYPES_OF_PACKETS,
    }

    public struct CurrentGameInfo
    {
        public int[] mapData;
        public bool isYourTurn;
        public int myIndexNumber;
        public int theWinner;
    }

    public class ConnectionClass
    {
        #region Utilities
        /// <summary>
        /// Tools for converting XY Pos into Index
        /// </summary>
        /// <returns>Index position of array</returns>
        public static int ConvertXYPositionToIndex(int positionX, int positionY)
        {
            return positionY * 15 + positionX;
        }

        /// <summary>
        /// Tools for converting index into position X and Y
        /// </summary>
        public static void ConvertArrayPositionToXY(int index, out int positionX, out int positionY)
        {
            positionX = index % 15;
            positionY = index / 15;
        }

        public static IEnumerable<int> StringToIntList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                    yield return num;
            }
        }

        public void TranslatePacketIntoGameInformation(string theMessage, out CurrentGameInfo theGame)
        {
            //Remove packet header from message
            theMessage = theMessage.Substring(theMessage.IndexOf(":") + 1);

            //Get map data
            string mapDataString = theMessage.Substring(0, theMessage.IndexOf(":"));
            theGame.mapData = StringToIntList(mapDataString).ToArray();

            //cut map data away
            theMessage = theMessage.Substring(theMessage.IndexOf(":") + 1);
            theGame.isYourTurn = (theMessage.Substring(0, 1) == 1.ToString() ? true : false);

            //cut away boolean check of player turn
            theMessage = theMessage.Substring(theMessage.IndexOf(":") + 1);
            Int32.TryParse(theMessage.Substring(0, 1), out theGame.myIndexNumber);

            //Get index 
            theMessage = theMessage.Substring(theMessage.IndexOf(":") + 1);
            Int32.TryParse(theMessage.Substring(0, 1), out theGame.theWinner);
        }

        public CurrentGameInfo CreateGameInformation()
        {
            CurrentGameInfo newGame = new CurrentGameInfo();
            newGame.mapData = new int[225];
            for (int i = 0; i < 225; ++i)
            {
                newGame.mapData[i] = 0;
            }

            newGame.isYourTurn = false;
            newGame.theWinner = 0;
            newGame.myIndexNumber = 0;

            return newGame;
        }
        #endregion

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

            do
            {
                ns.Write(data, 0, data.Length);
            }
            while (!ns.CanWrite);
        }

        public void SendMessage(PACKET_TYPE packetType, int message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message.ToString());
            do
            {
                ns.Write(data, 0, data.Length);
            }
            while (!ns.CanWrite);
        }

        public void SendPosition(int position)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.SET_MY_MOVE.ToString() + ":" + position.ToString());
            do
            {
                ns.Write(data, 0, data.Length);
            }
            while (!ns.CanWrite);
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
