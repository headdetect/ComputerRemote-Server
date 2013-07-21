using System.Net.Sockets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.Packets
{
    public class PacketPing : Packet
    {

        public override byte PacketId { get { return 0x02; } }

        public override void ReadPacket(RemoteClient c) { }

        public override void WritePacket(RemoteClient c) { }

    }
}
