using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerRemote {
   public class PacketGetInfo : Packet {

       public override PacketID PacketID {
           get { return global::ComputerRemote.PacketID.GetInfo;  }
       }

       public override int Length {
           get { return 0; }
       }

       public override byte[] Data {
           get { return null; }
       }

       public override void ReadPacket ( byte[] data ) {
       }

       public override void WritePacket ( Client c ) {
           
       }

       public override void HandlePacket ( Client c ) {
           c.HandleGetInfo(this);
       }
   }
}
