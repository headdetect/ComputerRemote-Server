using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteLib.Net.UDP
{
    public class UdpPacketWriter : PacketWriter
    {
        public UdpRemoteClient UdpRemote;

        public UdpPacketWriter(UdpRemoteClient client)
            : base(client)
        {
            UdpRemote = client;
        }

        protected override void SendPacket(Packet packet)
        {
            UdpRemote.WriteByte(packet.PacketId);
            packet.WritePacket(UdpRemote);
            UdpRemote.SendPacket();
        }
    }
}
