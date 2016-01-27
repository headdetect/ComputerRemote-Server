using System.Net.Sockets;
using RemoteLib.Net;

namespace CLI.Packets
{
    public class PacketMessage : Packet
    {

        /// <summary>
        /// Gets or sets the message in the packet.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketMessage"/> class.
        /// </summary>
        public PacketMessage()
        {
            Message = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PacketMessage(string message)
        {
            Message = message;
        }

        #region Inhereted Memebers

        public override byte PacketId => 0x04;

        public override void ReadPacket(RemoteClient c)
        {
            Message = c.ReadString();
        }

        public override void WritePacket(RemoteClient c)
        {
            c.WriteString(Message);
        }

        #endregion
    }
}
