using System.Net.Sockets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.Packets
{
    public class PacketPing : Packet
    {

        public override byte PacketID { get { return 0x00; } }

        public override void ReadPacket(Socket c) { }

        public override void WritePacket() { }


        public override byte[] DataWritten
        {
            get { return new byte[0]; }
        }
    }
}
