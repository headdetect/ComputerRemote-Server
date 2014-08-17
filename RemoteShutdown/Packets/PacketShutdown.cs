using RemoteLib.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Packets
{
    public class PacketShutdown : Packet
    {

        public override byte PacketId
        {
            get { return 0x08; }
        }

        public override void ReadPacket(RemoteClient c)
        {
        }

        public override void WritePacket(RemoteClient c)
        {
        }
    }
}
