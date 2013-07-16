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
            this.Message = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PacketMessage(string message)
        {
            this.Message = message;
        }

        #region Inhereted Memebers

        public override byte PacketID
        {
            get { return 0x04; }
        }

        private byte[] _data;
        public override byte[] DataWritten
        {
            get { return _data; }
        }

        public override void ReadPacket(Socket c)
        {
            Message = PacketReader.ReadString(c);
        }

        public override void WritePacket()
        {
            _data = PacketReader.GetString(Message);
        }

        #endregion
    }
}
