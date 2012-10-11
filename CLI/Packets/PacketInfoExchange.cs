using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;

namespace CLI.Packets {
    public class PacketInfoExchange : Packet {

        #region Inhereted Memebers 

        public override byte PacketID {
            get { return 0x05; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
            //Request for data

            c.PacketQueue.Enqueue( new PacketInfoExchange() );
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
            //Sending data

            _data = Packet.GetString( System.Environment.MachineName );
        }

        #endregion
    }
}
