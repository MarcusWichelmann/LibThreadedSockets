using LibThreadedSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemo
{
    class Program
    {
        private static ThreadedClient client;

        private static bool isDisconnected = false;

        static void Main(string[] args)
        {
            client = new ThreadedClient();
            client.Connect("localhost", 4242);
            client.DataReceived += client_DataReceived;
            client.Disconnected += client_Disconnected;
            client.StartReceiving();

            Console.WriteLine("Connected.");

            while(!isDisconnected)
            {
                string input = Console.ReadLine();

                if(isDisconnected)
                    break;

                if(input == "stop")
                {
                    client.Close();
                    break;
                }

                byte[] data = Encoding.UTF8.GetBytes(input);

                client.Send(data);
            }

            Console.WriteLine("Connection closed.");
            Console.ReadLine();
        }

        private static void client_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string dataString = Encoding.UTF8.GetString(e.Data);
            Console.WriteLine($"Data received: {dataString}");
        }

        private static void client_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
            isDisconnected = true;
        }
    }
}
