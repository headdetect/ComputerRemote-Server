using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;

namespace TVRemoteGUI.Networking.Packets {
    /// <summary>
    /// A packet for sending reqests for videos,
    /// A packet for reading a list of videos that can be played.
    /// </summary>
    public class PacketVideo : Packet {

        public int VideoID { get; private set; }

        public string Video { get; set; }

        public PacketVideo ( string video ) {
            Video = video;
        }
    

        public override byte PacketID {
            get { return 0x09; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
            VideoID = Packet.ReadInt ( c.NStream );
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
            _data = Packet.GetString ( Video );
        }
    }
}
