using System.IO;
using RemoteLib.Net;
using System.Net.Sockets;

namespace TVRemoteGUI.Networking.Packets
{
    public class PacketControl : Packet
    {

        public ControlType Control { get; private set; }

        public int Value { get; private set; }

        public string ValueString { get; private set; }

        public override byte PacketID
        {
            get { return 0x0a; }
        }


        public override byte[] DataWritten
        {
            get { return new byte[0]; }
        }

        public override void ReadPacket(Socket c)
        {
            Control = (ControlType)PacketReader.ReadShort(c);
            Value = -1;
            switch (Control)
            {
                case ControlType.Play:
                    ValueString = PacketReader.ReadString(c);
                    break;
                case ControlType.Rewind:
                case ControlType.Forward:
                    Value = PacketReader.ReadInt(c);
                    break;

            }
        }

        public override void WritePacket()
        {
            throw new IOException("Is a read-only packet"); //For now
        }

    }

    public enum ControlType
    {

        FullScreen,

        Pause,

        Play,

        Rewind, //Has value 1-10

        Forward, //Has value 1-10

        SkipForward,

        SkipBackwards


    }
}
