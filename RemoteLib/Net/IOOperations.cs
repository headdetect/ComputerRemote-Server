using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteLib.Net
{
    public static class IOOperations
    {
        #region primitive -> byte[] 

        /// <summary>
        /// Gets a byte array from a double.
        /// </summary>
        /// <param name="d">The double.</param>
        /// <returns>the byte array</returns>
        public static byte[] GetDouble(double d)
        {
            return BitConverter.GetBytes(d);
        }

        /// <summary>
        /// Gets a byte array from a double.
        /// </summary>
        /// <param name="f">The float.</param>
        /// <returns>the byte array</returns>
        public static byte[] GetFloat(float f)
        {
            return BitConverter.GetBytes(f);
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

        #endregion

        #region byte[] -> primitive

        /// <summary>
        /// Reads a string from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">Starting index to read bytes</param>
        /// <param name="length">Length of the string</param>
        /// <returns></returns>
        public static string ReadString(byte[] bytes, int start, int length)
        {
            return Encoding.UTF8.GetString(bytes, start, bytes.Length);
        }

        
        /// <summary>
        /// Reads an integer from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static int ReadInt(byte[] bytes, int start)
        {
			return BitConverter.ToInt32(bytes, start);
        }

        /// <summary>
        /// Reads an integer from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static double ReadDouble(byte[] bytes, int start)
        {
            return BitConverter.Int64BitsToDouble(ReadLong(bytes, start));
        }

        /// <summary>
        /// Reads an integer from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static long ReadLong(byte[] bytes, int start)
        {
            return BitConverter.ToInt64(bytes, start);
        }

        /// <summary>
        /// Reads an integer from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static short ReadShort(byte[] bytes, int start)
        {
            return BitConverter.ToInt16(bytes, start);
        }

        /// <summary>
        /// Read a float from the specified byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static float ReadFloat(byte[] bytes, int start)
        {
            return BitConverter.ToSingle(bytes, start);
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">Starting index to read bytes</param>
        /// <returns></returns>
        public static bool ReadBoolean(byte[] bytes, int start)
        {
            return BitConverter.ToBoolean(bytes, start);
        }

        #endregion


        #region stream -> primitive

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
            return ReadDouble(bytes, start);
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
            return ReadLong(bytes, start);
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
            return ReadShort(bytes, start);
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
        /// Reads an integer from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static int ReadInt(Stream mStream, int start = 0)
        {
            byte[] bytes = new byte[4];
            mStream.Read(bytes, start, 4);
            return ReadInt(bytes, start);
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
        /// Reads the float.
        /// </summary>
        /// <param name="mStrea">The m strea.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static float ReadFloat(Stream mStrea, int start = 0)
        {
            byte[] bytes = new byte[4];
            mStrea.Read(bytes, start, 4);
            return ReadFloat(bytes, start);
        }

        /// <summary>
        /// Reads the byte.
        /// </summary>
        /// <param name="mstream">The mstream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static byte ReadByte(Stream mstream, int start = 0)
        {
            return (byte)mstream.ReadByte();
        }

        #endregion

        #region socket -> primitive

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
            return ReadShort(bytes, 0);
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
            return ReadInt(bytes, 0);
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
            return ReadLong(bytes, 0);
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
            return ReadDouble(bytes, 0);
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

        #endregion
    }
}
