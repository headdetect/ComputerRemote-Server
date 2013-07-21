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
    

        public override byte PacketId {
            get { return 0x09; }
        }


        public override void ReadPacket ( RemoteClient c )
        {
            VideoID = c.ReadInt();
        }

        public override void WritePacket (RemoteClient c) {
            c.WriteString(Video.Location);
            c.WriteString(Video.Length.ToString("mm\\:ss"));
        }
    }
}
