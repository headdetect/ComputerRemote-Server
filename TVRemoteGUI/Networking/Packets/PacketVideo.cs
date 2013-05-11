using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using RemoteLib;
using RemoteLib.Networking;
using TVRemoteGUI.Windows.Interop;

namespace TVRemoteGUI.Networking.Packets {
    /// <summary>
    /// A packet for sending reqests for videos,
    /// A packet for reading a list of videos that can be played.
    /// </summary>
    public class PacketVideo : Packet {

        public int VideoID { get; private set; }

        public Video Video { get; set; }

        public PacketVideo ( Video video ) {
            Video = video;
        }

        public PacketVideo () {
            //Read-Only
        }
    

        public override byte PacketID {
            get { return 0x09; }
        }

        private byte[] _data;
        public override byte[] DataWritten {
            get { return _data; }
        }

        public override void ReadPacket ( Client c ) {
            VideoID = Packet.ReadInt ( c.NStream );
        }

        public override void WritePacket ( Client c ) {
            _data = Packet.GetString ( Video.Location )
                    .Concat(Packet.GetString( Video.Length.ToString("mm\\:ss") ) ).ToArray();
        }
    }
}
