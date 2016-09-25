using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibThreadedSockets
{
    public class ClientDataReceivedEventArgs : EventArgs
    {
        public ClientConnection ClientConnection { get; }
        public byte[] Data { get; }

        public ClientDataReceivedEventArgs(ClientConnection clientConnection, byte[] data)
        {
            ClientConnection = clientConnection;
            Data = data;
        }
    }
}