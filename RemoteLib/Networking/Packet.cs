using System;
using System.Text;
using System.Net;
using System.IO;
using ComputerRemote;
using ComputerRemote.Networking.Packets;

namespace RemoteLib.Networking {

    /// <summary>
    /// A class containing many tools for packet manipulation. Packet inherentence begins here.
    /// </summary>
    public abstract class Packet {

        #region Packet Registration
        /// <summary>
        /// Array of all the registered packet types.
        /// 0-3 are System Level packets, and cannot be modified.
        /// </summary>
        public readonly static Type[] PacketTypes = new Type[ 0xFF ];

        static Packet () {

            PacketTypes[ 0 ] = typeof( PacketPing );
            PacketTypes[ 1 ] = typeof( PacketDisconnect );
            PacketTypes[ 2 ] = typeof( PacketInit );

        }

        /// <summary>
        /// Registers the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public static void RegisterPacket ( Packet packet ) {
            RegisterPacket( packet.GetType(), packet.PacketID );
        }

        /// <summary>
        /// Registers the packet.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        public static void RegisterPacket ( Type type, int id ) {

            if ( id > byte.MaxValue || id < byte.MinValue ) {
                throw new ArgumentOutOfRangeException( "id", "Id must be in bounds of byte size (0-254)" );
            }

            if ( id >= 0 && id <= 2 ) {
                throw new ArgumentException( "The specified ID is reserved for the system" );
            }

            if ( PacketTypes[ id ] != null ) {
                throw new ArgumentException( "PacketID is already taken, please use a different ID or unregister the current one" );
            }

            PacketTypes[ id ] = type;

        }

        /// <summary>
        /// Unregisters the packet from the system.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void UnregisterPacket ( Type type ) {
            if ( type == typeof( PacketPing ) || type == typeof( PacketDisconnect ) || type == typeof( PacketInit ) ) {
                throw new ArgumentException( "Cannot unregister system packet" );
            }

            for ( int i = 0; i < 0xFF; i++ ) {
                if ( PacketTypes[ i ] == type ) {
                    PacketTypes[ i ] = null;
                }
            }
        }

        /// <summary>
        /// Unregisters the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public static void UnregisterPacket ( Packet packet ) {
            UnregisterPacket( packet.GetType() );
        }

        #endregion


        #region Inherited

        /// <summary>
        /// Gets the packet ID (must be 4 - 254).
        /// </summary>
        public abstract byte PacketID { get; }

        /// <summary>
        /// Gets the data written. If is a read-only packet, just return "null"
        /// </summary>
        public abstract byte[] DataWritten { get; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is a server packet.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a server packet; otherwise, <c>false</c>.
        /// </value>
        protected bool IsServerPacket { get; set; }

        /// <summary>
        /// Reads the packet. The "Data" property MUST be set or problems will occur.
        /// </summary>
        /// <param name="c">The client stream to read from.</param>
        public abstract void ReadPacket ( Client c );


        /// <summary>
        /// Writes the packet to the client stream.
        /// </summary>
        /// <param name="c">The c.</param>
        public abstract void WritePacket ( Client c );
        #endregion


        #region Tools

        /// <summary>
        /// Gets an empty packet, usually a packet to be filled with data.
        /// </summary>
        /// <param name="pId">The p id.</param>
        /// <param name="isServer">if set to <c>true</c> is a server sent packet.</param>
        /// <returns></returns>
        public static Packet GetPacket ( int pId, bool isServer = true ) {
            byte id = (byte) pId;
            Type t = PacketTypes[ id ];
            if ( t == null ) {
                return null;
            }
            Packet p = (Packet) Activator.CreateInstance( PacketTypes[ (byte) pId ] );
            p.IsServerPacket = isServer;
            return p;
        }


        /// <summary>
        /// Gets a byte array from a string. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>a byte array</returns>
        public static byte[] GetString ( string s ) {
            byte[] a = new byte[ s.Length + 4 ];
            GetInt( s.Length ).CopyTo( a, 0 );
            Encoding.UTF8.GetBytes( s ).CopyTo( a, 4 );
            return a;
        }

        /// <summary>
        /// Gets a byte array from an integer. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetInt ( int s ) {
            return BitConverter.GetBytes(  s  );
        }

        /// <summary>
        /// Gets a byte array from a long. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetLong ( long s ) {
            return BitConverter.GetBytes(  s  );
        }

        /// <summary>
        /// Gets a byte array from a short. The correct version for this system.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetShort ( short s ) {
            return BitConverter.GetBytes(  s  );
        }

        /// <summary>
        /// Gets a byte array from a boolean. The correct version for this system.
        /// </summary>
        /// <param name="b">the boolean to convert.</param>
        /// <returns></returns>
        public static byte[] GetBoolean ( bool b ) {
            return new[] { b ? (byte) 0x01 : (byte) 0x00 };
        }



        /// <summary>
        /// Reads an integer from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static int ReadInt ( Stream mStream, int start = 0 ) {
            byte[] bytes = new byte[ 4 ];
            mStream.Read( bytes, start, 4 );
            return ReadInt( bytes );
        }

        /// <summary>
        /// Reads a double from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static double ReadDouble ( Stream mStream, int start = 0 ) {
            byte[] bytes = new byte[ 8 ];
            mStream.Read( bytes, start, 8 );
            return ReadDouble( bytes );
        }

        /// <summary>
        /// Reads a long from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long ReadLong ( Stream mStream, int start = 0 ) {
            byte[] bytes = new byte[ 8 ];
            mStream.Read( bytes, start, 8 );
            return ReadLong( bytes );
        }

        /// <summary>
        /// Reads a short from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static short ReadShort ( Stream mStream, int start = 0 ) {
            byte[] bytes = new byte[ 2 ];
            mStream.Read( bytes, start, 2 );
            return ReadShort( bytes );
        }

        /// <summary>
        /// Reads a boolean from the specified stream. The correct method for this system.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <returns></returns>
        public static bool ReadBoolean ( Stream mStream ) {
            return new BinaryReader( mStream ).ReadBoolean();
        }

        /// <summary>
        /// Reads a string from the specified byte array. Not recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReadString ( byte[] bytes, int start = 0, int count = 64 ) {
            return Encoding.UTF8.GetString(bytes, start, count);
        }

        /// <summary>
        /// Reads a string from the specified stream.
        /// </summary>
        /// <param name="mStream">The m stream.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static string ReadString ( Stream mStream, int start = 0 ) {
            int len = ReadInt( mStream );

            byte[] bytes = new byte[ len ];
            mStream.Read( bytes, start, len );
            return Encoding.UTF8.GetString( bytes ).Trim();
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static int ReadInt ( byte[] bytes ) {
            return IPAddress.HostToNetworkOrder( BitConverter.ToInt32( bytes, 0 ) );
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static double ReadDouble ( byte[] bytes, int start = 0 ) {
            return BitConverter.Int64BitsToDouble( ReadLong( bytes, start ) );
        }

        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long ReadLong ( byte[] bytes, int start = 0 ) {
            return IPAddress.HostToNetworkOrder( BitConverter.ToInt64( bytes, start ) );
        }


        /// <summary>
        /// Reads an integer from the specified byte array. Not Recommended
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static short ReadShort ( byte[] bytes, int start = 0 ) {
            return IPAddress.HostToNetworkOrder( BitConverter.ToInt16( bytes, start ) );
        }

        #endregion


        #region Event Handlers

        internal static void OnPacketRecieved ( Packet p ) {
            if ( PacketRecieved != null ) {
                PacketRecieved( null, new PacketEventArgs( p ) );
            }
        }

        internal static void OnPacketSent ( Packet p ) {
            if ( PacketSent != null ) {
                PacketSent( null, new PacketEventArgs( p ) );
            }
        }

        /// <summary>
        /// Occurs when a packet is recieved.
        /// </summary>
        public static event EventHandler<PacketEventArgs> PacketRecieved;

        /// <summary>
        /// Occurs when a packet is sent.
        /// </summary>
        public static event EventHandler<PacketEventArgs> PacketSent;



        public class PacketEventArgs : EventArgs {

            /// <summary>
            /// Gets or sets the packet of the event.
            /// </summary>
            /// <value>
            /// The packet.
            /// </value>
            public Packet Packet { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketEventArgs"/> class.
            /// </summary>
            /// <param name="packet">The packet.</param>
            public PacketEventArgs ( Packet packet ) {
                Packet = packet;
            }
        }


        #endregion


    }
}

