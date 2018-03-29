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

        IPEndPoint sender;
        EndPoint tmpRemote;

        private Thread recvThread = null;

        Queue<String> queueOfMessages;

        //Constructor to setup the struct
        public ConnectionClass(string theIpAddress, Int32 portNumber)
        {
            ipep = new IPEndPoint(IPAddress.Parse(theIpAddress), portNumber);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sender = new IPEndPoint(IPAddress.Any, 0);
            tmpRemote = (EndPoint)sender;
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

        void RecieveMessage()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int recv = server.ReceiveFrom(data, ref tmpRemote);

                queueOfMessages.Enqueue(String.Format(Encoding.ASCII.GetString(data, 0, recv)));
            }
        }

        public string RecieveFromQueue()
        {
            if (queueOfMessages.Count == 0)
                return "";

            return queueOfMessages.Dequeue();
        }

        public void ShutdownThread()
        {
            if (recvThread != null)
                recvThread.Abort();
        }
    }
}
