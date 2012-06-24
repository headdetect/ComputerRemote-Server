using System;

namespace ComputerRemote
{
	public class PacketLogin : Packet
	{
	
		public string Name {get; set;}
		public string EncodedPass {get; set;}

		#region implemented abstract members of ComputerRemote.Packet
		public override PacketID PacketID {
			get {
				return PacketID.Login;
			}
		}

		public override int Length {
			get {
				if (Data != null)
					return ReadShort (Data);
				else
					return -1;
			}
		}

		public override byte[] Data {
			get {
				byte[] total = new byte[Name.Length + 2 + EncodedPass.Length + 2];
				GetShort((short)Name.Length).CopyTo(total, 0);
				GetString(Name).CopyTo(total, 2);
				
				GetShort ((short)EncodedPass.Length).CopyTo(total, Name.Length + 2);
				GetString(EncodedPass).CopyTo(total, Name.Length + 4); 
				return total;
			}
		}

		public override void ReadPacket (Client p)
		{
			short nameLength = ReadShort(p.NStream, 2);
			Name = ReadString(p.NStream, 4, nameLength);
			
			short passLength = ReadShort(p.NStream, 4 + nameLength);
			EncodedPass = ReadString(p.NStream, 6 + nameLength, passLength);
		}

		public override void WritePacket (Client p)
		{
			throw new NotImplementedException ();
		}

		public override void HandlePacket (Client c)
		{
			c.HandleLogin(this);
		}
		#endregion
	}
}

