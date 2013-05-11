using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using RemoteLib;
using RemoteLib.Networking;

namespace CLI.Packets {
    public class PacketBeep : Packet {
        public override byte PacketID {
            get { return 0x06; }
        }

        public override byte[] DataWritten {
            get { return new byte[0]; }
        }

        public override void ReadPacket ( Client c ) {
        }

        public override void WritePacket ( Client c ) {
        }
    }
}
