using LibThreadedSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDemo
{
    class Program
    {
        private static ThreadedServer server;

        static void Main(string[] args)
        {
            server = new ThreadedServer(4242);
            server.ClientConnected += server_ClientConnected;
            server.ClientDisconnected += server_ClientDisconnected;
            server.ClientDataReceived += server_DataReceived;
            server.SocketError += server_SocketError;
            server.Start();

            Console.WriteLine($"Server started: {server.GetServerAddress()}");

            while(true)
            {
                string input = Console.ReadLine();

                if(input == "stop")
                {
                    server.Stop();
                    break;
                }

                byte[] data = Encoding.UTF8.GetBytes(input);

                server.Broadcast(data);
            }

            Console.WriteLine("Server stopped.");
            Console.ReadLine();
        }

        private static void server_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            ClientConnection client = e.ClientConnection;
            Console.WriteLine($"New Client #{client.ClientId} from {client.RemoteEndPoint} connected.");
        }

        private static void server_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            ClientConnection client = e.ClientConnection;
            Console.WriteLine($"Client #{client.ClientId} disconnected.");
        }

        private static void server_DataReceived(object sender, ClientDataReceivedEventArgs e)
        {
            ClientConnection client = e.ClientConnection;
            string dataString = Encoding.UTF8.GetString(e.Data);
            Console.WriteLine($"Data received from #{client.ClientId}: {dataString}");
        }

        private static void server_SocketError(object sender, SocketErrorEventArgs e)
        {
            Console.WriteLine($"{(e.IsCritical ? "Critical " : "")}Socket Error: {e.Message}");
        }
    }
}
