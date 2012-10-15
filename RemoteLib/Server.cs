using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using ComputerRemote.IO;

namespace ComputerRemote {
    public class Server {
        private TcpListener mListener;
        private bool _shuttingDown;

        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public List<Client> Clients { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        public Server () {
            Clients = new List<Client>( 24 );
            IPEndPoint iep = new IPEndPoint( IPAddress.Any, 45903 );
            mListener = new TcpListener( iep );

        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop () {
            _shuttingDown = true;
            ObjectOutput.Write( "Stopping Server" );
            mListener.Stop();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start () {
            mListener.Start();
            mListener.BeginAcceptTcpClient( CallBack, null );

            string ips = "";

            //Get localIP
            foreach ( var ip in Dns.GetHostEntry( Dns.GetHostName() ).AddressList ) {
                if ( ip.AddressFamily == AddressFamily.InterNetwork ) {
                    ips = ip.ToString();
                }
            }
            //End get localIP

            ObjectOutput.Write( "Server Started (" + ips + ")" );
        }

        void CallBack ( IAsyncResult result ) {

            try {

                Client client = new Client( mListener.EndAcceptTcpClient( result ) ); //Thread stuck until client connects
                Clients.Add( client );
                client.StartClient();
                ObjectOutput.Write( "Client connected (" + ((IPEndPoint)client.ClientSocket.Client.RemoteEndPoint).Address.ToString() + ")" );
            }
            catch ( Exception e ) {
                ObjectOutput.Write( e );
            }

            if ( !_shuttingDown )
                mListener.BeginAcceptTcpClient( CallBack, null );


        }


    }
}

