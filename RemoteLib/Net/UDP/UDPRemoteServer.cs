using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.UDP
{
    public class UdpRemoteServer
    {
        private const int _DEFAULT_PORT = 5001;

        private readonly UdpClient mListener;
        private bool _shuttingDown;

        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public Dictionary<string, UdpRemoteClient> Clients { get; private set; }

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
        public IPEndPoint ServerEndPoint;

        /// <summary>
        /// Gets the port that the server is running on.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        public UdpRemoteServer()
            : this(_DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public UdpRemoteServer(int port)
        {
            Port = port;
            Clients = new Dictionary<string, UdpRemoteClient>(255);
            ServerEndPoint = new IPEndPoint(IPAddress.Any, port);

            mListener = new UdpClient(port);
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
            LocalIP = ServerEndPoint.Address;

            if (_shuttingDown)
                throw new AccessViolationException(
                    "Server is already running. You must call stop before calling start again.");

            // Size of our PacketInit packet //
            mListener.BeginReceive(Callback, null);
        }

        void Callback(IAsyncResult result)
        {
            UdpRemoteClient client = null;
            try
            {
                IPEndPoint point = ServerEndPoint;
                byte[] data = mListener.EndReceive(result, ref point);

                if (data.Length == 512)
                {

                    // PacketInit packetID //
                    if (data[0] == 0x00)
                    {
                        client = new UdpRemoteClient(point);
                        Clients.Add(point.ToString(), client);
                        client.StartClient();
                    }
                    else
                    {
                        client = Clients[point.ToString()];
                        if (client == null)
                            return;

                        client.LoadPacket(data);
                        byte id = data[0];
                        Packet p = Packet.GetPacket(id);
                        p.ReadPacket(client);
                        Packet.OnPacketRecieved(client, p);
                    }
                }
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.Disconnect();
                }
            }

            if (_shuttingDown) return;

            // Size of our PacketInit packet //
            mListener.BeginReceive(Callback, null);
        }


    }
}

