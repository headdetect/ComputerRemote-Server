using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.IO;
using RemoteLib.Net.Packets;
using RemoteLib.Net.TCP;

namespace RemoteLib.Net
{

    /// <summary>
    /// A class containing many tools for packet manipulation. Packet inherentence begins here.
    /// </summary>
    public abstract class Packet
    {

        #region Packet Registration
        /// <summary>
        /// Array of all the registered packet types.
        /// 0-3 are System Level packets, and cannot be modified.
        /// </summary>
        public readonly static Type[] PacketTypes = new Type[0xFF];

        static Packet()
        {

            PacketTypes[2] = typeof(PacketPing);
            PacketTypes[1] = typeof(PacketDisconnect);
            PacketTypes[0] = typeof(PacketInit);
        }

        /// <summary>
        /// Registers the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public static void RegisterPacket(Packet packet)
        {
            RegisterPacket(packet.GetType(), packet.PacketId);
        }

        /// <summary>
        /// Registers the packet.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        public static void RegisterPacket(Type type, int id)
        {

            if (id > byte.MaxValue || id < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException("id", "Id must be in bounds of byte size (0-254)");
            }

            if (id >= 0 && id <= 2)
            {
                throw new ArgumentException("The specified ID is reserved for the system");
            }

            if (PacketTypes[id] != null)
            {
                throw new ArgumentException("PacketID is already taken, please use a different ID or unregister the current one");
            }

            PacketTypes[id] = type;

        }

        /// <summary>
        /// Unregisters the packet from the system.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void UnregisterPacket(Type type)
        {
            if (type == typeof(PacketPing) || type == typeof(PacketDisconnect) || type == typeof(PacketInit))
            {
                throw new ArgumentException("Cannot unregister system packet");
            }

            for (int i = 0; i < 0xFF; i++)
            {
                if (PacketTypes[i] == type)
                {
                    PacketTypes[i] = null;
                }
            }
        }

        /// <summary>
        /// Unregisters the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public static void UnregisterPacket(Packet packet)
        {
            UnregisterPacket(packet.GetType());
        }

        #endregion


        #region Inherited

        /// <summary>
        /// Gets the packet ID (must be 3 - 255).
        /// </summary>
        public abstract byte PacketId { get; }

        /// <summary>
        /// Reads the packet. The "Data" property MUST be set or problems will occur.
        /// </summary>
        /// <param name="c">The client stream to read from.</param>
        public abstract void ReadPacket(RemoteClient c);


        /// <summary>
        /// Writes the packet to the client stream.
        /// </summary>
        /// <remarks>This is where you would actually write data to the stream</remarks>
        public abstract void WritePacket(RemoteClient c);

        #endregion

        /// <summary>
        /// Gets an empty packet, usually a packet to be filled with data.
        /// </summary>
        /// <param name="pId">The p id.</param>
        /// <returns></returns>
        public static Packet GetPacket(byte pId)
        {
            Type t = PacketTypes[pId];
            if (t == null)
            {
                return null;
            }
            Packet p = (Packet)Activator.CreateInstance(PacketTypes[pId]);
            return p;
        }



        #region Event Handlers

        internal static void OnPacketRecieved(RemoteClient c, Packet p)
        {
            if (PacketRecieved != null)
            {
                PacketRecieved(null, new PacketEventArgs(c, p));
            }
        }

        internal static void OnPacketSent(RemoteClient c, Packet p)
        {
            if (PacketSent != null)
            {
                PacketSent(null, new PacketEventArgs(c, p));
            }
        }

        /// <summary>
        /// Occurs when a packet is recieved.
        /// </summary>
        public static event EventHandler<PacketEventArgs> PacketRecieved;

        /// <summary>
        /// Occurs when a packet is sent.
        /// </summary>
        public static event EventHandler<PacketEventArgs> PacketSent;



        public class PacketEventArgs : EventArgs
        {

            /// <summary>
            /// Gets or sets the packet of the event.
            /// </summary>
            /// <value>
            /// The packet.
            /// </value>
            public Packet Packet { get; set; }

            /// <summary>
            /// Gets or sets the remote client.
            /// </summary>
            /// <value>
            /// The remote client.
            /// </value>
            public RemoteClient RemoteClient { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketEventArgs"/> class.
            /// </summary>
            /// <param name="client">The client</param>
            /// <param name="packet">The packet.</param>
            public PacketEventArgs(RemoteClient client, Packet packet)
            {
                RemoteClient = client;
                Packet = packet;
            }
        }


        #endregion


    }
}

