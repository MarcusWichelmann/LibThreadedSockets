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
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientConnection ClientConnection { get; }

        public ClientDisconnectedEventArgs(ClientConnection clientConnection)
        {
            ClientConnection = clientConnection;
        }
    }
}