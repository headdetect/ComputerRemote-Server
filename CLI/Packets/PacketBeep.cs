using System.Net.Sockets;
using RemoteLib.Net;

namespace CLI.Packets
{
    public class PacketBeep : Packet
    {
        public override byte PacketId
        {
            get { return 0x06; }
        }

        public override void ReadPacket(RemoteClient c)
        {
        }

        public override void WritePacket(RemoteClient c)
        {
        }
    }
}
