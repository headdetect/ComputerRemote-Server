using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using RemoteLib;
using RemoteLib.Networking;

namespace CLI.Packets {
    public class PacketMessage : Packet {

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
        public PacketMessage () {
            this.Message = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PacketMessage ( string message ) {
            this.Message = message;
        }

        #region Inhereted Memebers

        public override byte PacketID {
            get { return 0x04; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( Client c ) {
            Message = Packet.ReadString( c.NStream );
        }

        public override void WritePacket ( Client c ) {
            _data = Packet.GetString( Message );
        }

        #endregion
    }
}
