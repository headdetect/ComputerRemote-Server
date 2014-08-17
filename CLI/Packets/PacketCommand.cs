using RemoteLib.Net;
using System.Net.Sockets;

namespace CLI.Packets {
    public class PacketCommand : Packet {

        /// <summary>
        /// Gets the command that was recieved.
        /// </summary>
        public string Command { get; private set; }

        public string Result { get; set; }

        public PacketCommand ( string result ) {
            Result = result;
        }

        public PacketCommand () { }

        public override byte PacketId {
            get { return 0x01; }
        }

        public override void ReadPacket ( RemoteClient c )
        {
        }

        public override void WritePacket (RemoteClient c) {
        }
    }
}
