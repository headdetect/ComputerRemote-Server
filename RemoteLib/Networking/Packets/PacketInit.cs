using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RemoteLib;
using RemoteLib.Networking;

namespace ComputerRemote.Networking.Packets {
    public class PacketInit : Packet {

        /// <summary>
        /// Gets the user agent from the connecting client.
        /// </summary>
        public string UserAgent { get; private set; }

        public override byte PacketID {
            get { return 0x02; }
        }

        public override void ReadPacket ( Client c ) {
            UserAgent = Packet.ReadString( c.NStream );
        }

        public override void WritePacket ( Client c ) {
            throw new IOException( "Is a readonly packet" );
        }

        //Is readonly packet
        public override byte[] DataWritten {
            get { return null; }
        }
    }
}
