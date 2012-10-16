using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using System.IO;

namespace CLI.Packets {
    public class PacketCommand : Packet {

        /// <summary>
        /// Gets the command that was recieved.
        /// </summary>
        public string Command { get; private set; }

        public override byte PacketID {
            get { return 0x05; }
        }

        public override byte[] DataWritten {
            get { return new byte[ 0 ];  }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
            Command = Packet.ReadString( c.NStream );
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
            throw new IOException( "Is a readonly packet" );
        }
    }
}
