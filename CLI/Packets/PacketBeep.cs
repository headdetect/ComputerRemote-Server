using System.Net.Sockets;
using RemoteLib.Net;

namespace CLI.Packets
{
    public class PacketBeep : Packet
    {
        public override byte PacketID
        {
            get { return 0x06; }
        }

        public override byte[] DataWritten
        {
            get { return new byte[0]; }
        }

        public override void ReadPacket(Socket c)
        {
        }

        public override void WritePacket()
        {
        }
    }
}
