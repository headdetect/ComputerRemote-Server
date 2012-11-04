using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;

namespace CLI.Packets {
    public class PacketBeep : Packet {
        public override byte PacketID {
            get { return 0x06; }
        }

        public override byte[] DataWritten {
            get { return new byte[0]; }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
        }
    }
}
