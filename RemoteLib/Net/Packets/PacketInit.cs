using System.IO;
using System.Net.Sockets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net.Packets
{
    public class PacketInit : Packet
    {

        /// <summary>
        /// Gets the user agent from the connecting client.
        /// </summary>
        public string UserAgent { get; private set; }

        public override byte PacketID
        {
            get { return 0x02; }
        }

        public override void ReadPacket(Socket c)
        {
            UserAgent = PacketReader.ReadString(c);
        }

        public override void WritePacket()
        {
            throw new IOException("Is a readonly packet");
        }

        //Is readonly packet
        public override byte[] DataWritten
        {
            get { return null; }
        }
    }
}
