using RemoteLib.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Packets
{
    public class PacketMessageBox : Packet
    {
        /// <summary>
        /// Gets the message that was recieved.
        /// </summary>
        public string Message { get; private set; }

        public PacketMessageBox(string message)
        {
            Message = message;
        }

        public PacketMessageBox() { }

        public override byte PacketId
        {
            get { return 0x10; }
        }

        public override void ReadPacket(RemoteClient c)
        {
            Message = c.ReadString();
        }

        public override void WritePacket(RemoteClient c)
        {
            c.WriteString(Message);
        }
    }
}
