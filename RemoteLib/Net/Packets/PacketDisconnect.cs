using System.IO;
using System.Net.Sockets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.Packets {
    public class PacketDisconnect : Packet {

        /// <summary>
        /// Gets the reason for disconnecting.
        /// </summary>
        public string DisconnectReason { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDisconnect"/> class.
        /// </summary>
        public PacketDisconnect () {
            DisconnectReason = "Socket Closed";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDisconnect"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public PacketDisconnect ( string reason ) {
            DisconnectReason = reason;
        }

        #region Inhereted Members


        private byte[] _data;
        public override byte[] DataWritten {
            get {
                if ( _data == null ) {
                    return new byte[0];
                }

                return _data;
            }
        }

        public override byte PacketID {
            get { return 0x01; }
        }

        public override void ReadPacket ( Socket c ) {
            throw new IOException( "Is a write only packet" );
        }

        public override void WritePacket () {

            //Even though it is sent to the client. The client will still be disconnected.
            //This gives the client a reason to why it was disconnected.

            _data =  PacketReader.GetString( DisconnectReason );
        }

        #endregion


    }
}
