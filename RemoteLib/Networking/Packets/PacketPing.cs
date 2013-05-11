using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteLib;
using RemoteLib.Networking;

namespace ComputerRemote.Networking.Packets {
    public class PacketPing : Packet {

        public override byte PacketID { get { return 0x00; } }

        public override void ReadPacket ( Client c ) { }

        public override void WritePacket ( Client c ) { }


        public override byte[] DataWritten {
            get { return new byte[ 0 ]; }
        }
    }
}
