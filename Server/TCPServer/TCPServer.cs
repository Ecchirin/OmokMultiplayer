using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPServer
{
    public enum PACKET_TYPE
    {
        SET_NAME_PACKET = 1,
        PLACEMENT_PACKET,
        END_OF_PACKET,
        TOTAL_TYPES_OF_PACKETS,
    }

    public struct GameInformation
    {
        uint[] MapData;

        public GameInformation(int b)
        {
            MapData = new uint[255];

            for (int i = 0; i < 255; ++i)
                MapData[i] = 0;

            //MapData = theMapData;
        }
    }

    public class ConnectionClass
    {
        IPEndPoint ipep;
        Socket server;
        NetworkStream ns;

        private Thread recvThread = null;

        Queue<String> queueOfMessages;

        //Constructor to setup the struct
        public ConnectionClass(string theIpAddress, Int32 portNumber)
        {
            ipep = new IPEndPoint(IPAddress.Parse(theIpAddress), portNumber);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            queueOfMessages = new Queue<string>();

            if (recvThread != null)
                recvThread.Abort();
            else
            {
                recvThread = new Thread(new ThreadStart(RecieveMessage));
                recvThread.IsBackground = true;
                recvThread.Start();
            }
        }

        //This region contains the section to change ip or port
        #region Change IP or Port
        public void ChangeIPAddress(string theIpAddress)
        {
            ipep.Address = IPAddress.Parse(theIpAddress);
        }

        public void ChangePort(Int32 portNumber)
        {
            ipep.Port = portNumber;
        }
        #endregion Change IP or Port

        //This region handles the connection or disconnection to/from server
        #region Connect or Disconnect
        public bool ConnectToServer()
        {
            try
            {
                server.Connect(ipep);
            }
            catch (SocketException e)
            {
                return false;
            }

            ns = new NetworkStream(server);

            return true;
        }

        public void DisconnectFromServer()
        {
            server.Shutdown(SocketShutdown.Send);
        }
        #endregion Connect or Disconnect

        public void SendMessage(PACKET_TYPE packetType, string message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }

        public void SendMessage(PACKET_TYPE packetType, int message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message.ToString());
            server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }

        public void SendPosition(int position)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.PLACEMENT_PACKET.ToString() + ":" + position.ToString());
            server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }

        //public GameInformation RecvGameInfo()
        //{
        //    byte[] data = new byte[1024];
        //    int recv = server.ReceiveFrom(data, ref tmpRemote);

        //    unsafe
        //    {
        //        byte* p = (byte*)&recv;
        //    }
        //}

        public string TestRecieve()
        {
            byte[] data = new byte[1024];
            //int recv = ns.Read(data, 0, data.Length);

            //if (recv == 0)
            //    return "Disconnected";
            ////queueOfMessages.Enqueue(String.Format(Encoding.ASCII.GetString(data, 0, recv)));

            //if (String.Format(Encoding.ASCII.GetString(data, 0, recv)) != "")
            //    return String.Format(Encoding.ASCII.GetString(data, 0, recv));
            //else
            //    return "Blank Message?";

            int bytesRead = 0;
            int chunk = 0;

            while (true)
            {
                chunk = ns.Read(data, (int)bytesRead, data.Length - (int)bytesRead);
                if (chunk == 0)
                    break;
                bytesRead += chunk;

                if (Encoding.ASCII.GetString(data, 0, chunk).Substring(Encoding.ASCII.GetString(data, 0, chunk).Length - 2, 1) == PACKET_TYPE.END_OF_PACKET.ToString())
                    break;
            }

            if (chunk == 0)
                return "Disconnected";

            if (String.Format(Encoding.ASCII.GetString(data, 0, chunk)) != "")
                return String.Format(Encoding.ASCII.GetString(data, 0, chunk));
            else
                return "Blank Message?";

            //lock (queueOfMessages)
            //    queueOfMessages.Enqueue(String.Format(new ASCIIEncoding().GetString(data)));
        }

        void RecieveMessage()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int bytesRead = 0;
                int chunk = 0;

                while (bytesRead < 1024)
                {
                    chunk = ns.Read(data, (int)bytesRead, data.Length - (int)bytesRead);
                    if (chunk == 0)
                        break;
                    bytesRead += chunk;

                    if (Encoding.ASCII.GetString(data, 0, chunk).Substring(Encoding.ASCII.GetString(data, 0, chunk).Length - 2, 1) == PACKET_TYPE.END_OF_PACKET.ToString())
                        break;
                }

                if (chunk == 0)
                    break;

                //int recv = ns.Read(data, 0, data.Length);
                //if (recv == 0)
                //    break;

                lock(queueOfMessages)
                    queueOfMessages.Enqueue(String.Format(new ASCIIEncoding().GetString(data)));
                //queueOfMessages.Enqueue(String.Format(Encoding.ASCII.GetString(data, 0, chunk)));
            }
        }

        public string RecieveFromQueue()
        {
            //lock (queueOfMessages)
                if (queueOfMessages.Count == 0)
                return "No Count";

            //lock (queueOfMessages)
                return queueOfMessages.Dequeue();
        }

        public void ShutdownThread()
        {
            if (recvThread != null)
                recvThread.Abort();
        }
    }
}
