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


        public override byte PacketId {
            get { return 0x01; }
        }

        public override void ReadPacket ( RemoteClient c ) {
            throw new IOException( "Is a write only packet" );
        }

        public override void WritePacket(RemoteClient c)
        {
            c.WriteString(DisconnectReason);
        }

        #endregion


    }
}
