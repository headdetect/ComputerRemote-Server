using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ComputerRemote.IO;

namespace RemoteLib.Networking {

    public class MultiCast : IDisposable {

        private Socket castSocket;


        private bool run;

        private static readonly IPAddress castAddress = IPAddress.Parse( "224.0.2.60" );

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiCast"/> class.
        /// </summary>
        public MultiCast () {
            castSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
            castSocket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption( castAddress ) );
            castSocket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2 );

            run = true;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start () {
            if ( castSocket != null ) {
                IPEndPoint endPoint = new IPEndPoint( castAddress, 5000 );
                castSocket.Connect( endPoint );

                new Thread( RunReply ).Start();
            }
        }

        void RunReply () { 

            /*
             * Sends a message to any listening diagram socket every 2 seconds
             */ 

            while ( run ) {

                if ( castSocket == null )
                    break;

                byte[] data = Encoding.UTF8.GetBytes(Environment.MachineName);
                castSocket.Send( data );

                Thread.Sleep( 2000 );
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop () {
            if ( castSocket != null ) {
                castSocket.Close();
            }

            run = false;
        }


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose () {
            if ( castSocket != null ) {
                castSocket.Dispose();
            }
        }

        #endregion
    }
}
