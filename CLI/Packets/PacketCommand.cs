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

        public override byte PacketID {
            get { return 0x05; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( Socket c ) {
            Command = PacketReader.ReadString ( c );
        }

        public override void WritePacket () {
            _data = PacketReader.GetString ( Result );
        }
    }
}
