using RemoteLib.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteLib.Utils
{
    public class Stats
    {

        public static long TotalBytesSent { get; private set; }


        static Stats()
        {
            Packet.PacketSent += Packet_PacketSent;
            Packet.PacketRecieved += Packet_PacketRecieved;
        }

        static void Packet_PacketRecieved(object sender, Packet.PacketEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void Packet_PacketSent(object sender, Packet.PacketEventArgs e)
        {
            var len = e.Packet.DataWritten.Length;
            TotalBytesSent += len + 1;
        }
    }
}
