using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace RemoteLib.Net.UDP
{

    public class UdpRemoteClient : RemoteClient
    {

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; set; }


        /// <summary>
        /// Gets the UDP client.
        /// </summary>
        /// <value>
        /// The UDP client.
        /// </value>
        public Socket Client { get; private set; }

        private byte[] currentPacketRead;
        private int currentReadIndex = 0;

        private byte[] currentPacketSend = new byte[0];


        /// <summary>
        /// Initializes a new instance of the <see cref="UdpRemoteClient"/> class.
        /// </summary>
        /// <param name="endPoint">The endpoint to connect to.</param>
        public UdpRemoteClient(IPEndPoint endPoint)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Connect(endPoint);

            PacketReader = new UdpPacketReader(this);
            PacketWriter = new UdpPacketWriter(this);
        }

        internal void StartClient()
        {
            IsRunning = true;

            PacketReader.StartReadAsync();
            PacketWriter.StartWriteAsync();

            OnClientJoined();
        }

        public override void WriteByte(byte i)
        {
            byte[] data = new [] { i };
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteShort(short i)
        {
            byte[] data = IOOperations.GetShort(i);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteInt(int i)
        {
            byte[] data = IOOperations.GetInt(i);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteLong(long i)
        {
            byte[] data = IOOperations.GetLong(i);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteFloat(float i)
        {
            byte[] data = IOOperations.GetFloat(i);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteDouble(double i)
        {
            byte[] data = IOOperations.GetDouble(i);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteString(string s)
        {
            byte[] data = IOOperations.GetString(s);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }

        public override void WriteBoolean(bool b)
        {
            byte[] data = IOOperations.GetBoolean(b);
            currentPacketSend = currentPacketSend.Concat(data).ToArray();
        }


        public override byte ReadByte()
        {
            CheckBytesLoaded();

            byte read = currentPacketRead[currentReadIndex];
            currentReadIndex += 1;

            return read;
        }

        public override short ReadShort()
        {
            CheckBytesLoaded();
            short read = IOOperations.ReadShort(currentPacketRead, currentReadIndex);
            currentReadIndex += 2;
            return read;
        }

        public override int ReadInt()
        {
            CheckBytesLoaded();
            int read = IOOperations.ReadInt(currentPacketRead, currentReadIndex);
            currentReadIndex += 4;
            return read;
        }

        public override long ReadLong()
        {
            CheckBytesLoaded();
            long read = IOOperations.ReadLong(currentPacketRead, currentReadIndex);
            currentReadIndex += 8;
            return read;
        }

        public override float ReadFloat()
        {
            CheckBytesLoaded();
            float read = IOOperations.ReadFloat(currentPacketRead, currentReadIndex);
            currentReadIndex += 4;
            return read;
        }

        public override double ReadDouble()
        {
            CheckBytesLoaded();
            double read = IOOperations.ReadDouble(currentPacketRead, currentReadIndex);
            currentReadIndex += 8;
            return read;
        }

        public override string ReadString()
        {
            CheckBytesLoaded();
            int len = ReadInt();
            string read = IOOperations.ReadString(currentPacketRead, currentReadIndex, len);
            currentReadIndex += len;
            return read;
        }

        public override bool ReadBoolean()
        {
            CheckBytesLoaded();
            bool read = IOOperations.ReadBoolean(currentPacketRead, currentReadIndex);
            currentReadIndex += 1;
            return read;
        }

        /// <summary>
        /// Load bytes for client. The way that udp is set up you want to read the bytes you need instead of loading them from a stream. 
        /// </summary>
        /// <param name="bytes">bytes to load into</param>
        public void LoadPacket(byte[] bytes)
        {
            currentPacketRead = bytes;
        }


        /// <summary>
        /// Flushes the packet for this instance.
        /// </summary>
        public void SendPacket()
        {
            byte[] payloadPadded = new byte[512];

            if (payloadPadded.Length < currentPacketSend.Length)
            {
                Disconnect();
                throw new IOException("Max buffer size exceeded. Max size: 512");
            }

            Array.Copy(currentPacketSend, payloadPadded, currentPacketSend.Length);

            Client.Send(payloadPadded);

            currentPacketSend = new byte[0];
        }

        private void CheckBytesLoaded()
        {
            if(currentPacketRead == null)
                throw new IOException("You must load bytes before attempting to read them. Call LoadPacket(byte[]) to read data from them.");
        }

        /// <summary>
        /// Disconnects this instance from any open connections.
        /// </summary>
        public override void Disconnect()
        {
            IsRunning = false;
            PacketWriter.StopWrite();
            PacketReader.StopRead();

            OnClientLeft();
        }
    }
}

