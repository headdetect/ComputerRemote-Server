using System;
using System.Net.Sockets;
using System.Collections.Generic;
using RemoteLib.Packets;
using System.Threading;

namespace ComputerRemote {
    public class Client {

        public TcpClient ClientSocket { get; set; }

        public NetworkStream NStream { get; set; }

        public Queue<Packet> PacketQueue { get; set; }

        public PacketReader Reader { get; set; }


        internal Thread ReaderThread { get; set; }

        internal Thread WriterThread { get; set; }


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
        }


        void WriteThread () {
            while ( Reader.CanRead ) {
                if ( PacketQueue.Count > 0 ) {
                    var p = PacketQueue.Dequeue();
                    p.WritePacket( this );
                    continue;
                }
                Thread.Sleep( 5 );
            }
        }

        /// <summary>
        /// Handles the login.
        /// </summary>
        /// <param name="packet">The packet.</param>
        internal void HandleLogin ( PacketLogin packet ) {

        }

        internal void HandleGetInfo ( PacketGetInfo packetGetInfo ) {
            PacketQueue.Enqueue( new PacketSendInfo() );
            Disconnect();
        }

        public void Disconnect () {
            NStream.Close();
            ClientSocket.Close();
            Reader.CanRead = false;
        }
    }
}

