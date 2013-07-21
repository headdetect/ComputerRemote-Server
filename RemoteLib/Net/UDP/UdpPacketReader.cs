using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace RemoteLib.Net.UDP
{
    class UdpPacketReader : PacketReader
    {

        protected UdpRemoteClient UdpClient;

        public UdpPacketReader(UdpRemoteClient client)
            : base(client)
        {
            UdpClient = client;
        }

        public override Packet ReadPacket()
        {
            return null;
        }
    }
}
