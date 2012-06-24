using System;

namespace ComputerRemote
{
	public class PacketPing : Packet
	{
		public PacketPing ()
		{
		}

		#region implemented abstract members of ComputerRemote.Packet
		public override PacketID PacketID {
			get {
				throw new NotImplementedException ();
			}
		}

		public override int Length {
			get {
				throw new NotImplementedException ();
			}
		}

		public override byte[] Data {
			get {
				throw new NotImplementedException ();
			}
		}

		public override void ReadPacket (Client c)
		{
			throw new NotImplementedException ();
		}

		public override void WritePacket (Client c)
		{
			throw new NotImplementedException ();
		}

		public override void HandlePacket (Client c)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

