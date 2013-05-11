using System.IO;
using ComputerRemote;

namespace RemoteLib.Networking {
    public class PacketReader {

        /// <summary>
        /// Gets or sets a value indicating whether this instance can read incoming packets.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can read incoming packets; otherwise, <c>false</c>.
        /// </value>
        public bool CanRead { get; set; }

        private readonly BinaryReader mReader;
        private readonly Client c;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReader"/> class.
        /// </summary>
        /// <param name="c">The client it is reading from.</param>
        public PacketReader ( Client c ) {
            CanRead = true;
            mReader = new BinaryReader( c.NStream );
            this.c = c;
        }

        /// <summary>
        /// Reads a packet from the current set client stream.
        /// </summary>
        /// <returns></returns>
        public Packet ReadPacket () {
            try {
                byte id = mReader.ReadByte();
                Packet p = Packet.GetPacket( id );
                p.ReadPacket( c );
                return p;
            }
            catch { c.Disconnect(); return null; }
        }

        /// <summary>
        /// Starts a looping system for reading incoming packets. Will block thread until the reader is closed.
        /// </summary>
        public void StartRead () {
            while ( CanRead ) {
                var p = ReadPacket();
                if ( p == null ) return;
                Packet.OnPacketRecieved( p );
            }
        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        /// <param name="closeClientStream">
        /// if set to <c>true</c> you want to close the client stream as well. 
        /// Note: this may cause problems closing the client stream elsewhere
        /// </param>
        public void Close ( bool closeClientStream = false ) {
            if ( closeClientStream ) {
                c.Disconnect();
            }

            mReader.Close();
            mReader.Dispose();

            CanRead = false;
        }
    }
}
