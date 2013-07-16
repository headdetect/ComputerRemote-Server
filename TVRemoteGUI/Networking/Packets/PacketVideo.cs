using System.Linq;
using System.Net.Sockets;
using RemoteLib.Net;
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

        public override void ReadPacket ( Socket c ) {
            VideoID = PacketReader.ReadInt(c);
        }

        public override void WritePacket () {
            _data = PacketReader.GetString(Video.Location)
                    .Concat(PacketReader.GetString(Video.Length.ToString("mm\\:ss"))).ToArray();
        }
    }
}
