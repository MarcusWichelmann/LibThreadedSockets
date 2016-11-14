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
    public class ThreadedServer
    {
        private const int DefaultBacklogSize = 100;

        public int Port { get; private set; }
        public IPAddress LocalAddress { get; private set; }
        public int BacklogSize { get; private set; } // verbindungspuffer bis angenommen

        public List<ClientConnection> Connections { get; private set; } = new List<ClientConnection>();

        public event ClientConnectedEventHandler ClientConnected;
        public event ClientDisconnectedEventHandler ClientDisconnected;
        public event ClientDataReceivedEventHandler ClientDataReceived;
        public event SocketErrorEventHandler SocketError;

        public delegate void ClientDisconnectedEventHandler(object sender, ClientDisconnectedEventArgs e);
        public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
        public delegate void ClientDataReceivedEventHandler(object sender, ClientDataReceivedEventArgs e);
        public delegate void SocketErrorEventHandler(object sender, SocketErrorEventArgs e);

        private Socket socket;
        private ManualResetEvent waitingForConnection = new ManualResetEvent(false);

        private CancellationTokenSource stopListeningTokenSource;

        private int nextClientId = 0;

        public ThreadedServer(int port, IPAddress localAddress = null, int backlogSize = DefaultBacklogSize)
        {
            LocalAddress = localAddress ?? IPAddress.Any;
            Port = port;
            BacklogSize = backlogSize;
        }

        public string GetServerAddress() => $"{LocalAddress}:{Port}";

        public void Start(IPAddress localAddress, int newPort, int backlogSize = DefaultBacklogSize)
        {
            LocalAddress = localAddress;
            Port = newPort;
            BacklogSize = backlogSize;

            Start();
        }

        public void Start(int newPort)
        {
            Port = newPort;

            Start();
        }

        public void Start()
        {
            waitingForConnection.Reset();

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(LocalAddress, Port));
            socket.Listen(BacklogSize);

            stopListeningTokenSource = new CancellationTokenSource();

            Task listenTask = Task.Run(() => {
                try
                {
                    while(!stopListeningTokenSource.IsCancellationRequested)
                    {
                        waitingForConnection.Reset();

                        socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                        waitingForConnection.WaitOne();
                    }
                }
                catch(Exception ex)
                {
                    if(!stopListeningTokenSource.IsCancellationRequested)
                        SocketError?.Invoke(this, new SocketErrorEventArgs(ex.Message, true));
                }
            });
        }

        public void Stop()
        {
            if(stopListeningTokenSource == null || socket == null)
                throw new InvalidOperationException("The server isn't started.");

            stopListeningTokenSource.Cancel();

            foreach(ClientConnection clientConnection in Connections.ToArray())
                DisconnectClient(clientConnection);

            socket.Close();

            Connections.Clear();
            nextClientId = 0;
        }

        public void Broadcast(byte[] data)
        {
            foreach(ClientConnection clientConnection in Connections)
                Send(clientConnection, data);
        }

        public void Send(ClientConnection clientConnection, params object[] data) => Send(clientConnection, new DataPackage(data).Bytes);

        public void Send(ClientConnection clientConnection, byte[] data)
        {
            try
            {
                clientConnection.Send(data);
            }
            catch(Exception ex)
            {
                if(!stopListeningTokenSource.IsCancellationRequested)
                {
                    Connections.Remove(clientConnection);

                    ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(clientConnection));
                    SocketError?.Invoke(this, new SocketErrorEventArgs(ex.Message, false));
                }
            }
        }

        public void DisconnectClient(ClientConnection clientConnection)
        {
            Socket connectionSocket = clientConnection.ConnectionSocket;

            try
            {
                connectionSocket.Shutdown(SocketShutdown.Both);
                connectionSocket.Close();
            }
            catch(Exception ex)
            {
                if(!stopListeningTokenSource.IsCancellationRequested)
                    SocketError?.Invoke(this, new SocketErrorEventArgs(ex.Message, false));
            }

            Connections.Remove(clientConnection);

            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(clientConnection));
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            waitingForConnection.Set();

            if(stopListeningTokenSource.IsCancellationRequested)
                return;

            try
            {
                Socket socket = (Socket)asyncResult.AsyncState;
                Socket connectionSocket = socket.EndAccept(asyncResult);

                ClientConnection clientConnection = new ClientConnection(nextClientId++, connectionSocket);
                Connections.Add(clientConnection);

                ClientConnected?.Invoke(this, new ClientConnectedEventArgs(clientConnection));

                BeginReceive(clientConnection);
            }
            catch(Exception ex)
            {
                if(!stopListeningTokenSource.IsCancellationRequested)
                    SocketError?.Invoke(this, new SocketErrorEventArgs(ex.Message, false));
            }
        }

        private void BeginReceive(ClientConnection clientConnection)
        {
            clientConnection.ConnectionSocket.BeginReceive(clientConnection.Buffer, 0, ClientConnection.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientConnection);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ClientConnection clientConnection = (ClientConnection)asyncResult.AsyncState;
            Socket connectionSocket = clientConnection.ConnectionSocket;

            try
            {
                int bytesReceived = connectionSocket.EndReceive(asyncResult);

                if(bytesReceived > 0)
                {
                    byte[] data = clientConnection.Buffer.Take(bytesReceived).ToArray();
                    ClientDataReceived?.Invoke(this, new ClientDataReceivedEventArgs(clientConnection, data));
                }

                BeginReceive(clientConnection);
            }
            catch(Exception ex)
            {
                if(!stopListeningTokenSource.IsCancellationRequested)
                {
                    Connections.Remove(clientConnection);

                    ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(clientConnection));
                    SocketError?.Invoke(this, new SocketErrorEventArgs(ex.Message, false));
                }
            }
        }
    }
}
