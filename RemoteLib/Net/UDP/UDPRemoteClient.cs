using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace RemoteLib.Net.UDP
{

    public class UdpRemoteClient
    {

        /// <summary>
        /// Occurs when the client has SUCCESSFULLY joined the server and passed all verifications.
        /// </summary>
        public static event EventHandler<UdpClientConnectionEventArgs> ClientJoined;

        /// <summary>
        /// Occurs when a client leaves the server. No current streams will be closed at the time this is called.
        /// However, they may not be connected.
        /// </summary>
        public static event EventHandler<UdpClientConnectionEventArgs> ClientLeft;

        /// <summary>
        /// Gets or sets the client socket.
        /// </summary>
        /// <value>
        /// The client socket.
        /// </value>
        public Socket Client { get; set; }

        /// <summary>
        /// Gets or sets the packet queue.
        /// </summary>
        /// <value>
        /// The packet queue.
        /// </value>
        public Queue<Packet> PacketQueue { get; set; }

        /// <summary>
        /// Gets or sets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        public PacketReader Reader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public EndPoint EndPoint { get; set; }


        internal Thread ReaderThread { get; set; }

        internal Thread WriterThread { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="UdpRemoteClient"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public UdpRemoteClient(Socket client)
        {
            PacketQueue = new Queue<Packet>();
            Reader = new PacketReader(client);
            Client = client;
            EndPoint = client.RemoteEndPoint;
        }

        internal void StartClient()
        {
            IsRunning = true;

            ReaderThread = new Thread(Reader.StartRead);
            WriterThread = new Thread(WriteThread);

            ReaderThread.Start();
            WriterThread.Start();

            if (ClientJoined != null)
            {
                ClientJoined(this, new UdpClientConnectionEventArgs(this));
            }
        }


        void WriteThread()
        {

            try
            {
                while (Reader.CanRead && IsRunning)
                {
                    if (PacketQueue.Count > 0)
                    {
                        var p = PacketQueue.Dequeue();
                        p.WritePacket();

                        byte header = p.PacketID;
                        byte[] data = p.DataWritten;

                        byte[] bytes = new byte[data.Length + 1];
                        bytes[0] = header;
                        for (int i = 0; i < data.Length; i++)
                        {
                            bytes[i + 1] = data[i];
                        }


                        Client.Send(bytes);
                        Packet.OnPacketSent(p);
                        continue;
                    }
                    Thread.Sleep(5);
                }
                Disconnect();
            }
            catch (IOException)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Enqueues the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The packet's place in the write queue.</returns>
        public int EnqueuePacket(Packet packet)
        {
            if (packet == null) throw new NullReferenceException("Packet must be initialized and filled");
            PacketQueue.Enqueue(packet);
            return PacketQueue.Count;
        }

        /// <summary>
        /// Enqueues the packet.
        /// </summary>
        /// <returns>The packet's place in the write queue.</returns>
        public int EnqueuePacket(byte header, byte[] data)
        {
            var packet = Packet.GetPacket(header);

            if (packet == null) throw new IndexOutOfRangeException("No packet with id: \"" + header + "\" found");

            PacketQueue.Enqueue(packet);
            return PacketQueue.Count;
        }

        /// <summary>
        /// Writes the packet now, instead of inserting it in the queue.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        public void WritePacketNow(Packet packet)
        {
            WritePacketNow(packet.PacketID, packet.DataWritten);
        }

        /// <summary>
        /// Writes the packet now, instead of inserting it in the queue.
        /// </summary>
        /// <param name="header">The header of the data packet.</param>
        /// <param name="data">The data.</param>
        public void WritePacketNow(byte header, byte[] data)
        {
            if (Client == null || !IsRunning) throw new ObjectDisposedException("Cannot write to a disposed socket");

            byte[] bytes = new byte[data.Length + 1];
            bytes[0] = header;
            for (int i = 0; i < data.Length; i++)
            {
                bytes[i + 1] = data[i];
            }


            Client.Send(bytes);

        }



        /// <summary>
        /// Disconnects this instance from any open connections.
        /// </summary>
        public void Disconnect()
        {
            IsRunning = false;

            if (ClientLeft != null)
            {
                ClientLeft(this, new UdpClientConnectionEventArgs(this));
            }

            Client.Close();
            Reader.CanRead = false;
        }

        public class UdpClientConnectionEventArgs : EventArgs
        {

            /// <summary>
            /// Gets the client.
            /// </summary>
            public UdpRemoteClient Client { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UdpClientConnectionEventArgs"/> class.
            /// </summary>
            /// <param name="client">The client.</param>
            public UdpClientConnectionEventArgs(UdpRemoteClient client)
            {
                Client = client;
            }
        }
    }
}

