using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCPServer;

namespace OmokServer
{
    /// <summary>
    /// The class that manage the connection within each thread
    /// 
    /// </summary>
    class ConnectionThread
    {
        public TcpListener threadListener;
        private static int connections = 0;

        public void HandleConnection()
        {
            int recv;
            byte[] data = new byte[1024];

            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            connections++;
            Console.WriteLine("New client accepted: {0} active connections",
                              connections);

            string welcome = "Welcome to my test server!" + PACKET_TYPE.END_OF_PACKET.ToString();
            data = Encoding.ASCII.GetBytes(welcome);
            //client.NoDelay = true;

            ns.Write(data, 0, data.Length);

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

                    
                }

                Console.WriteLine("Message received from {0}:", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                Console.WriteLine(fullPacketMessage.ToString());

                if (fullPacketMessage.ToString().Contains(PACKET_TYPE.PLACEMENT_PACKET.ToString()))
                {
                    //ns.Write(data, 0, recv);
                    data = new byte[1024];
                    //data = Encoding.ASCII.GetBytes("I've got your placement message! Thanks! " + PACKET_TYPE.END_OF_PACKET.ToString());
                    data = Encoding.ASCII.GetBytes("Calvert,Eu Kern,Zi Sheng,Player 1,Player 2,Player 3,Player 4,Player 5,Player6,Player 7,Player 8,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat,repeat");
                    ns.Write(data, 0, data.Length);
                }
                else
                {
                    data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("You sent me a packet that did not contain a placement message! " + PACKET_TYPE.END_OF_PACKET.ToString());
                    ns.Write(data, 0, data.Length);
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

        private TcpListener client;

        public ThreadedTCPServer()
        {
            //IPAddress ipAddress = Dns.Resolve("192.168.0.22").AddressList[0];
            //IPAddress ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            //IPAddress iPAddress("192.168.0.22");
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
