using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace RemoteLib.Utils
{

    /// <summary>
    /// Cast a message to any listening device on the network.
    /// Useful for sending the name and address of the computer without
    /// searching for it on the device.
    /// </summary>
    public class Multicast
    {
        /// <summary>
        /// Gets or sets the message to cast.
        /// </summary>
        /// <value>
        /// The message to cast.
        /// </value>
        public string MessageToCast { get; set; }

        /// <summary>
        /// Gets or sets the IP to bind to.
        /// </summary>
        /// <value>
        /// The IP to bind to.
        /// </value>
        public string BoundCastingIP { get; private set; }

        /// <summary>
        /// Gets or sets the IP the server is bound to.
        /// </summary>
        /// <value>
        /// The bound server IP.
        /// </value>
        public string BoundServerIP { get; set; }


        private Socket _castSocket;

        private bool _run;

        private readonly IPAddress _castAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Multicast"/> class.
        /// </summary>
        public Multicast()
            : this("224.0.2.60")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Multicast"/> class.
        /// </summary>
        /// <param name="ipToBindTo">The ip to bind to.</param>
        public Multicast(string ipToBindTo)
        {
            BoundCastingIP = ipToBindTo;
            _castAddress = IPAddress.Parse(BoundCastingIP);

            _castSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _castSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(_castAddress));

            _run = true;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void BeginCast()
        {
            if (_castSocket == null) return;
            IPEndPoint endPoint = new IPEndPoint(_castAddress, 5000);
            _castSocket.Connect(endPoint);

            new Thread(RunReply).Start();
        }

        void RunReply()
        {

            /*
             * Sends a message to any listening diagram socket every 2 seconds
             */


            while (_run && _castSocket != null)
            {
                try
                {

                    if (_castSocket == null)
                        break;

                    byte[] data = Encoding.UTF8.GetBytes(MessageToCast ?? Environment.MachineName);
                    _castSocket.Send(data);

                    Thread.Sleep(2000);
                }
                catch
                {
                    return;
                }
            }

        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {

            _run = false;
            try
            {
                if (_castSocket == null) return;
                _castSocket.Close(1);
                _castSocket.Dispose();
                _castSocket = null;
            }
            catch
            {
            }
        }

    }
}
