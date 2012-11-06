using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.Networking;
using System.IO;

namespace TVRemoteGUI.Networking.Packets {
    public class PacketControl : Packet {

        public ControlType Control { get; private set; }

        public int Value { get; private set; }

        public override byte PacketID {
            get { return 0x0a; }
        }


        public override byte[] DataWritten {
            get { return new byte[ 0 ]; }
        }

        public override void ReadPacket ( ComputerRemote.Client c ) {
            Control = (ControlType) Packet.ReadShort ( c.NStream );
            Value = -1;
            switch ( Control ) {
                case ControlType.Rewind:
                case ControlType.Forward:
                    Value = Packet.ReadInt ( c.NStream );
                    break;

            }
        }

        public override void WritePacket ( ComputerRemote.Client c ) {
            throw new IOException ( "Is a read-only packet" ); //For now
        }

    }

    public enum ControlType {

        FullScreen,

        Pause,

        Play,

        Rewind, //Has value 1-10

        Forward, //Has value 1-10

        SkipForward,

        SkipBackwards


    }
}
