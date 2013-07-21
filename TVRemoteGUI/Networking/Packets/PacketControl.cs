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

        public override byte PacketId
        {
            get { return 0x0a; }
        }

        public override void ReadPacket(RemoteClient c)
        {
            Control = (ControlType)c.ReadShort();
            Value = -1;
            switch (Control)
            {
                case ControlType.Play:
                    ValueString = c.ReadString();
                    break;
                case ControlType.Rewind:
                case ControlType.Forward:
                    Value = c.ReadInt();
                    break;

            }
        }

        public override void WritePacket(RemoteClient c)
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
