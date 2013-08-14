using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        private readonly Queue<QueueBlob> readQueue;

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
            readQueue = new Queue<QueueBlob>();
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

            new Thread(FlushData).Start();

            mListener.BeginReceive(Callback, null);
        }

        void Callback(IAsyncResult result)
        {
            if (String.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = "ReadCallbackThread";
            UdpRemoteClient client = null;
            try
            {
                IPEndPoint point = ServerEndPoint;
                byte[] data = mListener.EndReceive(result, ref point);

                if (data.Length == 1025)
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
                        var stringy = point.ToString();
                        if (!String.IsNullOrWhiteSpace(stringy))
                        {
                            readQueue.Enqueue(new QueueBlob(point.ToString(), data));
                        }
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


        private void FlushData()
        {
            Thread.CurrentThread.Name = "FlushReadQueueThread";
            while (!_shuttingDown)
            {
                if (readQueue.Count <= 0)
                {
                    Thread.Sleep(5);
                    continue;
                }

                var qBlob = readQueue.Dequeue();

                if (qBlob.Data == null || qBlob.Key == null)
                    continue;

                var client = Clients[qBlob.Key];
                if (client == null)
                    return;

                client.LoadPacket(qBlob.Data);
                byte id = qBlob.Data[0];
                Packet p = Packet.GetPacket(id);
                p.ReadPacket(client);
                Packet.OnPacketRecieved(client, p);
            }

        }

        internal struct QueueBlob
        {
            internal string Key;
            internal byte[] Data;

            internal QueueBlob(string key, byte[] data)
            {
                Key = key;
                Data = data;
            }

        }


    }


}

