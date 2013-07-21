using System.IO;
using System.Net.Sockets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.Packets
{
    public class PacketInit : Packet
    {
        public override byte PacketId
        {
            get { return 0x00; }
        }

        public override void ReadPacket(RemoteClient c)
        {
        }

        public override void WritePacket(RemoteClient c)
        {
        }

    }
}
