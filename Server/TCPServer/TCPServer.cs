using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.IO;

//https://stackoverflow.com/questions/12652791/tcp-client-server-client-doesnt-always-read?rq=1
//Test build tmr from here.

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
        TcpClient client;
        NetworkStream ns;
        string ipAddress;
        Int32 portNumber;
        private Thread recvThread = null;

        Queue<String> queueOfMessages;

        //Constructor to setup the struct
        public ConnectionClass(string theIpAddress, Int32 thePort)
        {
            client = new TcpClient(theIpAddress, thePort);
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
            //ipep.Address = IPAddress.Parse(theIpAddress);
            client.Connect(theIpAddress, portNumber);
            ns = client.GetStream();
        }

        public void ChangePort(Int32 thePort)
        {
            //ipep.Port = portNumber;
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
        //public bool ConnectToServer()
        //{
        //    try
        //    {
        //        //server.Connect(ipep);
        //        //client.Connect()
        //    }
        //    catch (SocketException e)
        //    {
        //        return false;
        //    }

        //    //ns = new NetworkStream(server);
        //    ns = client.GetStream();
        //    return true;
        //}
        public bool CheckConnection()
        {
            if (IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .SingleOrDefault(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DisconnectFromServer()
        {
            //server.Shutdown(SocketShutdown.Send);
            client.GetStream().Close();
            client.Close();
        }
        #endregion Connect or Disconnect

        public void SendMessage(PACKET_TYPE packetType, string message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message);
            //server.SendTo(data, data.Length, SocketFlags.None, ipep);
            ns.Write(data, 0, data.Length);
        }

        public void SendMessage(PACKET_TYPE packetType, int message)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(packetType.ToString() + ":" + message.ToString());
            //server.SendTo(data, data.Length, SocketFlags.None, ipep);
            ns.Write(data, 0, data.Length);

        }

        public void SendPosition(int position)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(PACKET_TYPE.PLACEMENT_PACKET.ToString() + ":" + position.ToString());
            //server.SendTo(data, data.Length, SocketFlags.None, ipep);
            ns.Write(data, 0, data.Length);

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

                return fullPacketMessage.ToString();
            }

            return "Error in recv message!";
        }

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


                //byte[] data = new byte[1024];
                //int bytesRead = 0;
                //int chunk = 0;

                //while (bytesRead < 1024)
                //{
                //    //chunk = ns.Read(data, (int)bytesRead, data.Length - (int)bytesRead);
                //    //if (chunk == 0)
                //    //    break;
                //    //bytesRead += chunk;

                //    //if (Encoding.ASCII.GetString(data, 0, chunk).Substring(Encoding.ASCII.GetString(data, 0, chunk).Length - 2, 1) == PACKET_TYPE.END_OF_PACKET.ToString())
                //    //    break;

                //    chunk = ns.Read(data, 0, data.Length);
                //    if (Encoding.ASCII.GetString(data, 0, chunk).Substring(Encoding.ASCII.GetString(data, 0, chunk).Length - 2, 1) == PACKET_TYPE.END_OF_PACKET.ToString())
                //        break;
                //}

                //if (chunk == 0)
                //    break;

                ////int recv = ns.Read(data, 0, data.Length);
                ////if (recv == 0)
                ////    break;

                //lock (queueOfMessages)
                //    queueOfMessages.Enqueue(String.Format(new ASCIIEncoding().GetString(data)));
                ////queueOfMessages.Enqueue(String.Format(Encoding.ASCII.GetString(data, 0, chunk)));
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
