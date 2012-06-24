using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ComputerRemote;

namespace RemoteLib.Packets {
    public class PacketReader {

        public bool CanRead { get; set; }

        private BinaryReader mReader;
        private Client c;

        public PacketReader ( Client c ) {
            CanRead = true;
            mReader = new BinaryReader( c.NStream );
            this.c = c;
        }

        public Packet ReadPacket ( ) {
            try {
                byte id = mReader.ReadByte();
                Packet p = Packet.GetPacket( ( PacketID ) id );
                if ( p.Length > 0 ) {
                    byte[] data = mReader.ReadBytes( p.Length );
                    p.ReadPacket( data );
                }
                p.ReadPacket( null );
                return p;
            }
            catch { return null; }
        }

        public void StartRead ( ) {
            while ( CanRead ) {
                var p = ReadPacket();
                if ( p == null ) return;
                p.HandlePacket( c );
            }
        }
    }
}
