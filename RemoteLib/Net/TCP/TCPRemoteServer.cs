using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace RemoteLib.Net.TCP
{
    public class TcpRemoteServer
    {

        private const int _DEFAULT_PORT = 45903;

        private readonly TcpListener mListener;
        private bool _shuttingDown;

        /// <summary>
        /// Gets if the server is running.
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public List<TcpRemoteClient> Clients { get; private set; }

        /// <summary>
        /// Gets the local IP of the server.
        /// </summary>
        public IPAddress LocalIP { get; private set; }


        /// <summary>
        /// Gets the ip end point.
        /// </summary>
        /// <value>
        /// The ip end.
        /// </value>
        public IPEndPoint IpEnd { get; private set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        public TcpRemoteServer()
            : this(IPAddress.Any)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public TcpRemoteServer(int port) : this(IPAddress.Any, port) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="bindToIp">The bind to ip.</param>
        public TcpRemoteServer(string bindToIp)
            : this(IPAddress.Parse(bindToIp))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="bindToIp">The the ip to bind the server to.</param>
        /// <param name="port">The port.</param>
        public TcpRemoteServer(string bindToIp, int port)
            : this(IPAddress.Parse(bindToIp), port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="address">The address to bind to.</param>
        public TcpRemoteServer(IPAddress address)
            : this(address, _DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public TcpRemoteServer(IPAddress address, int port)
        {
            Clients = new List<TcpRemoteClient>(255);
            IpEnd = new IPEndPoint(address, port);
            mListener = new TcpListener(IpEnd);
        }



        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Running = false;
            _shuttingDown = true;
            mListener.Stop();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Running = true;
            mListener.Start();
            mListener.BeginAcceptTcpClient(CallBack, null);
            LocalIP = IpEnd.Address;
        }

        void CallBack(IAsyncResult result)
        {

            TcpRemoteClient client = null;

            try
            {
                client = new TcpRemoteClient(mListener.EndAcceptTcpClient(result)); //Thread stuck until client connects
                Clients.Add(client);
                client.StartClient();
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.Disconnect();
                }
            }

            if (!_shuttingDown)
                mListener.BeginAcceptTcpClient(CallBack, null);


        }


    }
}

