using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Mono.Zeroconf.Providers.Bonjour;
using System.IO;
using System.EnterpriseServices.Internal;

namespace RemoteLib.Utils
{
    /// <summary>
    /// Cast a message to any listening device on the network.
    /// Useful for sending the name and address of the computer without
    /// searching for it on the device.
    /// </summary>
    public class DeviceDiscovery
    {

        /// <summary>
        /// Gets or sets the message to cast.
        /// </summary>
        /// <value>
        /// The message to cast.
        /// </value>
        public string MessageToCast { get; private set; }

        /// <summary>
        /// Gets or sets the port to cast on.
        /// </summary>
        /// <value>
        /// The port to cast on.
        /// </value>
        public short Port { get; private set; }

        private RegisterService _service;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDiscovery"/> class.
        /// </summary>
        public DeviceDiscovery()
            : this(3689, Environment.MachineName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDiscovery"/> class.
        /// </summary>
        public DeviceDiscovery(short port)
            : this(port, Environment.MachineName)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDiscovery"/> class.
        /// </summary>
        public DeviceDiscovery(string messageToCast)
            : this(3689, messageToCast)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDiscovery"/> class.
        /// </summary>
        public DeviceDiscovery(short port, string messageToCast)
        {
            try
            {
                Port = port;
                MessageToCast = messageToCast;

                // Register all providers //
                foreach (var assembly in Directory.GetFiles(".", "*Mono.Zeroconf.Providers.*.dll", SearchOption.AllDirectories))
                    new Publish().GacInstall(Path.GetFullPath(assembly));


                _service = new RegisterService()
                {
                    Name = MessageToCast ?? Environment.MachineName,
                    RegType = "_test._tcp",
                    ReplyDomain = "local.",
                    Port = Port
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// Starts the service discovery process
        /// </summary>
        public void BeginCast()
        {
            try
            {
                _service.Register();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void EndCast()
        {
            //TODO: ?
        }


    }
}
