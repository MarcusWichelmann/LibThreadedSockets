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
    public class SocketErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public bool IsCritical { get; }

        public SocketErrorEventArgs(string message, bool isCritical)
        {
            Message = message;
            IsCritical = isCritical;
        }
    }
}