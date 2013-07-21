using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace RemoteLib.Net.TCP
{

    public class TcpRemoteClient : StreamedRemoteClient
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TcpClient TcpClient { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TcpRemoteClient"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public TcpRemoteClient(TcpClient client) : base(client.GetStream())
        {
            TcpClient = client;
        }

        public void StartClient()
        {
            PacketReader.StartReadAsync();
            PacketWriter.StartWriteAsync();

            OnClientJoined();
        }

        /// <summary>
        /// Disconnects this instance from any open connections.
        /// </summary>
        public override void Disconnect()
        {
            PacketReader.StopRead();
            PacketWriter.StopWrite();

            TcpClient.Close();

            OnClientLeft();
        }
    }
}

