using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using ComputerRemote.Networking;
using System.IO;

namespace ComputerRemote {
    public class Client {

        /// <summary>
        /// Occurs when the client has SUCCESSFULLY joined the server and passed all verifications.
        /// </summary>
        public static event EventHandler<ClientConnectionEventArgs> ClientJoined;

        /// <summary>
        /// Occurs when a client leaves the server. No current streams will be closed at the time this is called.
        /// However, they may not be connected.
        /// </summary>
        public static event EventHandler<ClientConnectionEventArgs> ClientLeft;

        /// <summary>
        /// Gets or sets the client socket.
        /// </summary>
        /// <value>
        /// The client socket.
        /// </value>
        public TcpClient ClientSocket { get; set; }

        /// <summary>
        /// Gets or sets the Network stream.
        /// </summary>
        /// <value>
        /// The Network stream.
        /// </value>
        public NetworkStream NStream { get; set; }

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
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public Client ( TcpClient client ) {
            ClientSocket = client;
            NStream = client.GetStream();
            PacketQueue = new Queue<Packet>();

            Reader = new PacketReader( this );

        }

        internal void StartClient () {
            ReaderThread = new Thread( new ThreadStart( Reader.StartRead ) );
            WriterThread = new Thread( new ThreadStart( WriteThread ) );

            ReaderThread.Start();
            WriterThread.Start();

            if ( ClientJoined != null ) {
                ClientJoined( this, new ClientConnectionEventArgs( this ) );
            }
        }


        void WriteThread () {

            try {
                while ( Reader.CanRead ) {
                    if ( PacketQueue.Count > 0 ) {
                        var p = PacketQueue.Dequeue();
                        p.WritePacket( this );

                        NStream.WriteByte( p.PacketID );
                        NStream.Write( p.DataWritten, 0, p.DataWritten.Length );
                        continue;
                    }
                    Thread.Sleep( 5 );
                }
            }
            catch ( IOException ) {
                Disconnect();
            }
        }



        /// <summary>
        /// Disconnects this instance from any open connections.
        /// </summary>
        public void Disconnect () {
            if ( ClientLeft != null ) {
                ClientLeft( this, new ClientConnectionEventArgs( this ) );
            }

            NStream.Close();
            ClientSocket.Close();
            Reader.CanRead = false;
        }

        public class ClientConnectionEventArgs : EventArgs {

            /// <summary>
            /// Gets the client.
            /// </summary>
            public Client Client { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ClientConnectionEventArgs"/> class.
            /// </summary>
            /// <param name="client">The client.</param>
            public ClientConnectionEventArgs ( Client client ) {
                this.Client = client;
            }
        }
    }
}

