using RemoteLib.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Packets
{
    public class PacketRestart : Packet
    {

        public override byte PacketId
        {
            get { return 0x09; }
        }

        public override void ReadPacket(RemoteClient c)
        {
        }

        public override void WritePacket(RemoteClient c)
        {
        }
    }
}
