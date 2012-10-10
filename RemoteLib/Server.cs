using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using ComputerRemote.IO;

namespace ComputerRemote {
    public class Server {
        private TcpListener mListener;
        private TcpListener mPingListener;
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
        public Server ( ) {
            Clients = new List<Client>( 24 );
            IPEndPoint iep = new IPEndPoint( IPAddress.Any, 45903 );
            mListener = new TcpListener( iep );

        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop ( ) {
            _shuttingDown = true;
            ObjectOutput.Write( "Stopping Server" );
            mListener.Stop();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start ( ) {
            checkPort();
            mListener.Start();
            mListener.BeginAcceptTcpClient( CallBack, null );

            ObjectOutput.Write( "Server Started" );
        }

        void CallBack ( IAsyncResult result ) {

            try {
                ObjectOutput.Write( "Client connected" );
                Client client = new Client( mListener.EndAcceptTcpClient( result ) );
                Clients.Add( client );
                client.StartClient();

            }
            catch ( Exception e ) {
                ObjectOutput.Write( e );
            }

            if ( !_shuttingDown ) 
                mListener.BeginAcceptTcpClient( CallBack, null );
            

        }

        /// <summary>
        /// Checks to see if port 7 is open (the global ping port), if it is not, it creates a listener on it
        /// </summary>
        void checkPort ( ) {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach ( TcpConnectionInformation tcpi in tcpConnInfoArray )
                if ( tcpi.LocalEndPoint.Port == 7 )
                    return;

            mPingListener = new TcpListener( IPAddress.Any, 7 );
            mPingListener.Start();
            ObjectOutput.Write( "Ping server started" );
            mPingListener.BeginAcceptTcpClient( onPinged, null );

        }

        void onPinged ( IAsyncResult rec ) {
            try {
                ObjectOutput.Write( "Pinged" );
                TcpClient c = mPingListener.EndAcceptTcpClient( rec );
                c.Close();
            }
            catch { }
            if ( !_shuttingDown )
                mPingListener.BeginAcceptTcpClient( onPinged, null );
        }
    }
}

