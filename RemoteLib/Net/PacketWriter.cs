using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using RemoteLib.Net.UDP;

namespace RemoteLib.Net
{
    public class PacketWriter
    {
        /// <summary>
        /// Gets or sets the packet queue.
        /// </summary>
        /// <value>
        /// The packet queue.
        /// </value>
        public Queue<Packet> PacketQueue { get; set; }

        public bool CanWrite { get; set; }

        private readonly RemoteClient _client;

        private readonly object _locker = new object();

        public PacketWriter(RemoteClient client)
        {
            PacketQueue = new Queue<Packet>();

            _client = client;
            CanWrite = true;
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
            SendPacket(packet);
            Packet.OnPacketSent(_client, packet);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void StopWrite()
        {
            CanWrite = false;
            PacketQueue.Clear();
        }

        /// <summary>
        /// Starts the writing process.
        /// </summary>
        public void StartWrite()
        {

            try
            {
                while (CanWrite)
                {
                    if (PacketQueue.Count > 0)
                    {
                        lock (_locker)
                        {
                            var p = PacketQueue.Dequeue();
                            SendPacket(p);
                            Packet.OnPacketSent(_client, p);
                        }
                        continue;
                    }
                    Thread.Sleep(5);
                }
            }
            catch (IOException)
            {
                _client.Disconnect();
            }
        }

        /// <summary>
        /// Starts the writing process in a non-thread blocking manner
        /// </summary>
        public void StartWriteAsync()
        {
            new Thread(StartWrite).Start();
        }


        protected virtual void SendPacket(Packet packet)
        {
            _client.WriteByte(packet.PacketId);
            packet.WritePacket(_client);
        }

    }
}
