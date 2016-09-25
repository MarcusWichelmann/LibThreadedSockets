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
    public class ClientConnection
    {
        public int ClientId { get; }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return (IPEndPoint)ConnectionSocket.RemoteEndPoint;
            }
        }

        internal const int BufferSize = 4096;

        internal Socket ConnectionSocket { get; }

        internal byte[] Buffer { get; } = new byte[BufferSize];

        internal ClientConnection(int clientId, Socket connectionSocket)
        {
            ClientId = clientId;
            ConnectionSocket = connectionSocket;
        }
    }
}