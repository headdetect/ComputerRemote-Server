using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using RemoteLib.Net.TCP;
using System.Net.Sockets;
using RemoteLib.Net.UDP;

namespace RemoteLib.Net
{
    public class PacketReader
    {

        /// <summary>
        /// Gets or sets a value indicating whether this instance can read incoming packets.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can read incoming packets; otherwise, <c>false</c>.
        /// </value>
        public bool CanRead { get; set; }

        private readonly RemoteClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReader"/> class.
        /// </summary>
        /// <param name="client">The socket.</param>
        public PacketReader(RemoteClient client)
        {
            CanRead = true;
            _client = client;
        }

        /// <summary>
        /// Reads a packet from the current set client stream.
        /// </summary>
        /// <returns></returns>
        public virtual Packet ReadPacket()
        {
            try
            {
                
                byte id = _client.ReadByte();
                Packet p = Packet.GetPacket(id);
                p.ReadPacket(_client);
                return p;
            }
            catch { _client.Disconnect(); return null; }
        }

        /// <summary>
        /// Starts a looping system for reading incoming packets. Will block thread until the reader is closed.
        /// </summary>
        public void StartRead()
        {

            while (CanRead)
            {
                try
                {
                    var p = ReadPacket();
                    if (p == null) return;
                    Packet.OnPacketRecieved(_client, p);
                }
                catch { _client.Disconnect(); }
            }

        }

        public void StartReadAsync()
        {
            new Thread(StartRead).Start();
        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        public void StopRead()
        {
            CanRead = false;
        }
    }
}
