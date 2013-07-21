using System;

namespace RemoteLib.Net
{
    public abstract class RemoteClient
    {
        /// <summary>
        /// Occurs when the client has SUCCESSFULLY joined the server and passed all verifications.
        /// </summary>
        public static event EventHandler<ClientConnectionEventArgs> ClientJoined;

        /// <summary>
        /// Occurs when a client leaves the server. No current streams will be closed at the time this is called.
        /// However, they may not be connected.
        /// </summary>
        public static event EventHandler<ClientConnectionEventArgs> ClientLeft;

        /// <summary>
        /// Gets the packet reader.
        /// </summary>
        /// <value>
        /// The packet reader.
        /// </value>
        public PacketReader PacketReader { get; protected set; }

        /// <summary>
        /// Gets the packet writer.
        /// </summary>
        /// <value>
        /// The packet writer.
        /// </value>
        public PacketWriter PacketWriter { get; protected set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteClient"/> class.
        /// </summary>
        protected RemoteClient()
        {
            PacketReader = new PacketReader(this);
            PacketWriter = new PacketWriter(this);
        }


        /// <summary>
        /// Writes a byte to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteByte(byte i);

        /// <summary>
        /// Writes a short to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteShort(short i);

        /// <summary>
        /// Writes an int to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteInt(int i);

        /// <summary>
        /// Writes a long to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteLong(long i);

        /// <summary>
        /// Writes a float to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteFloat(float i);

        /// <summary>
        /// Writes a double to the connected device
        /// </summary>
        /// <param name="i">The i.</param>
        public abstract void WriteDouble(double i);

        /// <summary>
        /// Writes a string to the connected device
        /// </summary>
        /// <param name="s">The s.</param>
        public abstract void WriteString(string s);

        /// <summary>
        /// Writes the boolean to the connected device
        /// </summary>
        /// <param name="b">the bool to write.</param>
        public abstract void WriteBoolean(bool b);


        /// <summary>
        /// Reads a byte from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract byte ReadByte();


        /// <summary>
        /// Reads a short from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract short ReadShort();


        /// <summary>
        /// Reads a int from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract int ReadInt();


        /// <summary>
        /// Reads a long from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract long ReadLong();


        /// <summary>
        /// Reads a float from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract float ReadFloat();

        /// <summary>
        /// Reads a double from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract double ReadDouble();

        /// <summary>
        /// Reads a string from the connected device.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadString();

        public abstract bool ReadBoolean();

        public abstract void Disconnect();

        internal void OnClientJoined()
        {
            if (ClientJoined != null)
                ClientJoined(this, new ClientConnectionEventArgs(this));
        }

        internal void OnClientLeft()
        {
            if (ClientLeft != null)
                ClientLeft(this, new ClientConnectionEventArgs(this));
        }
    }

    public class ClientConnectionEventArgs : EventArgs
    {

        /// <summary>
        /// Gets the client.
        /// </summary>
        public RemoteClient RemoteClient { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public ClientConnectionEventArgs(RemoteClient client)
        {
            RemoteClient = client;
        }
    }
}
