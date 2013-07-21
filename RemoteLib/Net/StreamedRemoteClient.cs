using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemoteLib.Net
{
    public abstract class StreamedRemoteClient : RemoteClient
    {
        private readonly Stream _stream;

        protected StreamedRemoteClient(Stream stream)
        {
            _stream = stream;
        }


        public override void WriteByte(byte i)
        {
            byte[] data = new[] { i };
            _stream.Write(data, 0, data.Length);
        }

        public override void WriteShort(short i)
        {
            byte[] data = IOOperations.GetShort(i);
            _stream.Write(data, 0, data.Length);
        }


        public override void WriteInt(int i)
        {
            byte[] data = IOOperations.GetInt(i);
            _stream.Write(data, 0, data.Length);
        }


        public override void WriteLong(long i)
        {
            byte[] data = IOOperations.GetLong(i);
            _stream.Write(data, 0, data.Length);
        }


        public override void WriteFloat(float i)
        {
            byte[] data = IOOperations.GetFloat(i);
            _stream.Write(data, 0, data.Length);
        }

        public override void WriteDouble(double i)
        {
            byte[] data = IOOperations.GetDouble(i);
            _stream.Write(data, 0, data.Length);
        }

        public override void WriteString(string s)
        {
            byte[] data = IOOperations.GetString(s);
            _stream.Write(data, 0, data.Length);
        }

        public override void WriteBoolean(bool b)
        {
            byte[] data = IOOperations.GetBoolean(b);
            _stream.Write(data, 0, data.Length);
        }



        public override byte ReadByte()
        {
            return IOOperations.ReadByte(_stream);
        }


        public override short ReadShort()
        {
            return IOOperations.ReadShort(_stream);
        }


        public override int ReadInt()
        {
            return IOOperations.ReadInt(_stream);
        }


        public override long ReadLong()
        {
            return IOOperations.ReadLong(_stream);
        }


        public override float ReadFloat()
        {
            return IOOperations.ReadFloat(_stream);
        }

        public override double ReadDouble()
        {
            return IOOperations.ReadDouble(_stream);
        }

        public override string ReadString()
        {
            return IOOperations.ReadString(_stream);
        }

        public override bool ReadBoolean()
        {
            return IOOperations.ReadBoolean(_stream);
        }

    }
}
