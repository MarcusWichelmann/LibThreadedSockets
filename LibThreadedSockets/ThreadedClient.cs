using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibThreadedSockets
{
    public class ThreadedClient : TcpClient
    {
        private const int BufferSize = 4096;

        public event DataReceivedEventHandler DataReceived;
        public event DisconnectedEventHandler Disconnected;

        public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
        public delegate void DisconnectedEventHandler(object sender, EventArgs e);

        private byte[] buffer = new byte[BufferSize];

        public void StartReceiving()
        {
            if(!Client.Connected)
                throw new InvalidOperationException("Client is not connected.");

            BeginReceive();
        }

        public void Send(params object[] data) => Send(new DataPackage(data).Bytes);

        public void Send(byte[] data) => Client.Send(data);

        private void BeginReceive()
        {
            Client.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), Client);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int bytesReceived = Client.EndReceive(asyncResult);

                if(bytesReceived > 0)
                {
                    byte[] data = buffer.Take(bytesReceived).ToArray();
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
                }

                BeginReceive();
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
