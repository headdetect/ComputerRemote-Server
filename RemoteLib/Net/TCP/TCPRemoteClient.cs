using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace RemoteLib.Net.TCP
{

    public class TcpRemoteClient
    {

        /// <summary>
        /// Occurs when the client has SUCCESSFULLY joined the server and passed all verifications.
        /// </summary>
        public static event EventHandler<TcpClientConnectionEventArgs> ClientJoined;

        /// <summary>
        /// Occurs when a client leaves the server. No current streams will be closed at the time this is called.
        /// However, they may not be connected.
        /// </summary>
        public static event EventHandler<TcpClientConnectionEventArgs> ClientLeft;

        /// <summary>
        /// Gets or sets the Network stream.
        /// </summary>
        /// <value>
        /// The Network stream.
        /// </value>
        public NetworkStream NStream
        {
            get { return TcpClient.GetStream(); }
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TcpClient TcpClient { get; private set; }

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


        internal Thread ReaderThread { get; set; }

        internal Thread WriterThread { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteClient"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public TcpRemoteClient(TcpClient client)
        {
            TcpClient = client;
            PacketQueue = new Queue<Packet>();

            Reader = new PacketReader(client.Client);
        }

        internal void StartClient()
        {
            ReaderThread = new Thread(Reader.StartRead);
            WriterThread = new Thread(WriteThread);

            ReaderThread.Start();
            WriterThread.Start();

            if (ClientJoined != null)
            {
                ClientJoined(this, new TcpClientConnectionEventArgs(this));
            }
        }


        void WriteThread()
        {

            try
            {
                while (Reader.CanRead)
                {
                    if (PacketQueue.Count > 0)
                    {
                        var p = PacketQueue.Dequeue();
                        p.WritePacket();

                        NStream.WriteByte(p.PacketID);
                        NStream.Write(p.DataWritten, 0, p.DataWritten.Length);
                        continue;
                    }
                    Thread.Sleep(5);
                }
            }
            catch (IOException)
            {
                Disconnect();
            }
        }



        /// <summary>
        /// Disconnects this instance from any open connections.
        /// </summary>
        public void Disconnect()
        {
            if (ClientLeft != null)
            {
                ClientLeft(this, new TcpClientConnectionEventArgs(this));
            }

            NStream.Close();
            TcpClient.Close();
            Reader.CanRead = false;
        }

        public class TcpClientConnectionEventArgs : EventArgs
        {

            /// <summary>
            /// Gets the client.
            /// </summary>
            public TcpRemoteClient TcpRemoteClient { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="TcpClientConnectionEventArgs"/> class.
            /// </summary>
            /// <param name="client">The client.</param>
            public TcpClientConnectionEventArgs(TcpRemoteClient client)
            {
                TcpRemoteClient = client;
            }
        }
    }
}

