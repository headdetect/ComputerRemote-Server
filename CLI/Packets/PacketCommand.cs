using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using System.IO;

namespace CLI.Packets {
    public class PacketCommand : Packet {

        /// <summary>
        /// Gets the command that was recieved.
        /// </summary>
        public string Command { get; private set; }

        public string Result { get; set; }

        public PacketCommand(string result)
        {
            Result = result;
        }

        public PacketCommand() { }

        public override byte PacketID {
            get { return 0x05; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
            Command = Packet.ReadString( c.NStream );
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
            _data = Packet.GetString(Result);
        }
    }
}
