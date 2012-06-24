using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerRemote {
    public class PacketSendInfo : Packet {
        public override PacketID PacketID {
            get { return global::ComputerRemote.PacketID.SendInfo; }
        }

        public override int Length {
            get { return Environment.MachineName.Length+ 2; }
        }

        public override byte[] Data {
            get { return Packet.GetString( Environment.MachineName ); }
        }

        public override void ReadPacket ( byte[] data ) {
        }

        public override void WritePacket ( Client c ) {
            c.NStream.WriteByte( ( byte ) PacketID );
            var shorted = GetInt( Length );
            c.NStream.Write( shorted, 0, shorted.Length );
            c.NStream.Write( Data, 0, Data.Length );
        }

        public override void HandlePacket ( Client c ) {
            throw new NotImplementedException();
        }
    }
}
