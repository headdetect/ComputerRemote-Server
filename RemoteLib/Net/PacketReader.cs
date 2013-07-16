using System;
using System.IO;
using System.Net;
using System.Text;
using RemoteLib.Net.TCP;
using System.Net.Sockets;

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

        private readonly Socket _socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReader"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public PacketReader(Socket socket)
        {
            CanRead = true;
            _socket = socket;
        }

        /// <summary>
        /// Reads a packet from the current set client stream.
        /// </summary>
        /// <returns></returns>
        public Packet ReadPacket()
        {
            try
            {
                byte id = ReadByte(_socket);
                Packet p = Packet.GetPacket(id);
                p.ReadPacket(_socket);
                return p;
            }
            catch { _socket.DisconnectAsync(new SocketAsyncEventArgs()); return null; }
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
                    Packet.OnPacketRecieved(p);
                }
                catch { _socket.DisconnectAsync(new SocketAsyncEventArgs()); }
            }

        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        /// <param name="closeClientStream">
        /// if set to <c>true</c> you want to close the client stream as well. 
        /// Note: this may cause problems closing the client stream elsewhere
        /// </param>
        public void Close(bool closeClientStream = false)
        {
            if (closeClientStream)
            {
                _socket.DisconnectAsync(new SocketAsyncEventArgs());
            }

            CanRead = false;
        }

        /// <summary>
        /// Gets a byte array from a string. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>a byte array</returns>
        public static byte[] GetString(string s)
        {
            byte[] a = new byte[s.Length + 4];
            GetInt(s.Length).CopyTo(a, 0);
            Encoding.UTF8.GetBytes(s).CopyTo(a, 4);
            return a;
        }

        /// <summary>
        /// Gets a byte array from an integer. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetInt(int s)
        {
            return BitConverter.GetBytes(s);
        }

        /// <summary>
        /// Gets a byte array from a long. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetLong(long s)
        {
            return BitConverter.GetBytes(s);
        }

        /// <summary>
        /// Gets a byte array from a short. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetShort(short s)
        {
            return BitConverter.GetBytes(s);
        }

        /// <summary>
        /// Gets a byte array from a boolean. The correct version for this system.
        /// </summary>
        /// <param name="b">the boolean to convert.</param>
        /// <returns></returns>
        public static byte[] GetBoolean(bool b)
        {
            return new[] { b ? (byte)0x01 : (byte)0x00 };
        }



        /// <summary>
        /// Reads an integer from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static int ReadInt(Stream mStream, int start = 0)
        {
            byte[] bytes = new byte[4];
            mStream.Read(bytes, start, 4);
            return ReadInt(bytes);
        }

        /// <summary>
        /// Reads the int.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static int ReadInt(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;
            byte[] bytes = new byte[4];
            socket.ReceiveFrom(bytes, ref from);
            return ReadInt(bytes);
        }

        /// <summary>
        /// Reads the double.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static double ReadDouble(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;
            byte[] bytes = new byte[8];
            socket.ReceiveFrom(bytes, ref from);
            return ReadDouble(bytes);
        }

        /// <summary>
        /// Reads the long.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static long ReadLong(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;
            byte[] bytes = new byte[8];
            socket.ReceiveFrom(bytes, ref from);
            return ReadLong(bytes);
        }

        /// <summary>
        /// Reads the short.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static short ReadShort(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;
            byte[] bytes = new byte[2];
            socket.ReceiveFrom(bytes, ref from);
            return ReadShort(bytes);
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static bool ReadBoolean(Socket socket)
        {
            return ReadByte(socket) == 0x01;
        }



        /// <summary>
        /// Reads a double from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static double ReadDouble(Stream mStream, int start = 0)
        {
            byte[] bytes = new byte[8];
            mStream.Read(bytes, start, 8);
            return ReadDouble(bytes);
        }

        /// <summary>
        /// Reads a long from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long ReadLong(Stream mStream, int start = 0)
        {
            byte[] bytes = new byte[8];
            mStream.Read(bytes, start, 8);
            return ReadLong(bytes);
        }

        /// <summary>
        /// Reads a short from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static short ReadShort(Stream mStream, int start = 0)
        {
            byte[] bytes = new byte[2];
            mStream.Read(bytes, start, 2);
            return ReadShort(bytes);
        }

        /// <summary>
        /// Reads a boolean from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <returns></returns>
        public static bool ReadBoolean(Stream mStream)
        {
            return new BinaryReader(mStream).ReadBoolean();
        }

        /// <summary>
        /// Reads a string from the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static string ReadString(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;

            int len = ReadInt(socket);

            byte[] bytes = new byte[len];
            socket.ReceiveFrom(bytes, ref from);
            return Encoding.UTF8.GetString(bytes).Trim();
        }


        /// <summary>
        /// Reads a string from the specified byte array. Not recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReadString(byte[] bytes, int start = 0, int count = 64)
        {
            return Encoding.UTF8.GetString(bytes, start, count);
        }

        /// <summary>
        /// Reads a string from the specified stream.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static string ReadString(Stream mStream, int start = 0)
        {
            int len = ReadInt(mStream);

            byte[] bytes = new byte[len];
            mStream.Read(bytes, start, len);
            return Encoding.UTF8.GetString(bytes).Trim();
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static int ReadInt(byte[] bytes)
        {
            return IPAddress.HostToNetworkOrder(BitConverter.ToInt32(bytes, 0));
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static double ReadDouble(byte[] bytes, int start = 0)
        {
            return BitConverter.Int64BitsToDouble(ReadLong(bytes, start));
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long ReadLong(byte[] bytes, int start = 0)
        {
            return IPAddress.HostToNetworkOrder(BitConverter.ToInt64(bytes, start));
        }


        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static short ReadShort(byte[] bytes, int start = 0)
        {
            return IPAddress.HostToNetworkOrder(BitConverter.ToInt16(bytes, start));
        }

        /// <summary>
        /// Reads the byte from the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public static byte ReadByte(Socket socket)
        {
            EndPoint from = socket.RemoteEndPoint;
            byte[] bytes = new byte[1];
            socket.ReceiveFrom(bytes, ref from);
            return bytes[0];
        }
    }
}
