using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.UDP
{
    public class UdpRemoteServer
    {

        private const int _DEFAULT_PORT = 5001;

        private readonly Socket mListener;
        private bool _shuttingDown;


        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public List<UdpRemoteClient> Clients { get; private set; }

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
        public UdpRemoteServer()
            : this(IPAddress.Any)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public UdpRemoteServer(int port) : this(IPAddress.Any, port) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="bindToIp">The bind to ip.</param>
        public UdpRemoteServer(string bindToIp)
            : this(IPAddress.Parse(bindToIp))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="bindToIp">The the ip to bind the server to.</param>
        /// <param name="port">The port.</param>
        public UdpRemoteServer(string bindToIp, int port)
            : this(IPAddress.Parse(bindToIp), port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="address">The address to bind to.</param>
        public UdpRemoteServer(IPAddress address)
            : this(address, _DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public UdpRemoteServer(IPAddress address, int port)
        {
            Clients = new List<UdpRemoteClient>(255);
            IpEnd = new IPEndPoint(address, port);

            //TODO: Support IPv6
            mListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }



        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _shuttingDown = true;
            mListener.Close();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            LocalIP = IpEnd.Address;

            if (!_shuttingDown)
                throw new AccessViolationException(
                    "Server is already running. You must call stop before calling start again.");

            mListener.BeginAccept(Callback, null);
        }

        void Callback(IAsyncResult result)
        {

            UdpRemoteClient client = null;
            try
            {
                var socket = mListener.EndAccept(result);
                client = new UdpRemoteClient(socket);
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
                mListener.BeginAccept(Callback, null);


        }


    }
}

