using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TVRemoteGUI.Windows.Interop;
using TVRemoteGUI.Windows.Utils;
using ComputerRemote.Networking;
using TVRemoteGUI.Networking.Packets;
using ComputerRemote;
using RemoteLib.Networking;
using System.Net;

namespace TVRemoteGUI.Windows {
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow {

        private Server _server;
        private MultiCast _castServer;

        public VideoWindow () {

        }

        private void WindowLoaded ( object sender, RoutedEventArgs e ) {
            new Thread ( HandleNetwork ).Start ();

            GlassArea = new Margins ( -1 );
            mPlayer.LoadedBehavior = MediaState.Manual;
            mPlayer.Source = new Uri ( @"C:\Users\Brayden\Videos\Basketball Movie\BasketballSequence.avi" );
            mPlayer.Play ();

            var ar = new VideoDiscoverer ( DiscoverType.Downloads );
            new Thread ( ar.Discover ).Start ();
            ar.VideoDiscovered += AddVideos;
        }

        void AddVideos ( object sender, VideoDiscoveredArgs args ) {
            if ( VideoFilter.IsInFilter ( args.FileLocation ) ) {
                return;
            }


            //TODO: not make it do multiple stuffs.

            if ( lstVideos.Items.Contains ( args.FileLocation ) ) {
                // return;
            }

            lstVideos.Dispatcher.Invoke ( new Action ( () => lstVideos.Items.Add ( args.FileLocation ) ) );
        }

        private void lstVideos_MouseDoubleClick ( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
            if ( lstVideos.SelectedIndex != -1 && lstVideos.SelectedIndex < lstVideos.Items.Count ) {
                string file = lstVideos.SelectedItem.ToString ();

                mPlayer.Source = new Uri ( file, UriKind.RelativeOrAbsolute );
                mPlayer.Play ();
            }
        }


        void HandleNetwork () {

            //--- Packet Registers ---

            Packet.RegisterPacket ( typeof ( PacketControl ), 0x0a );
            Packet.RegisterPacket ( typeof ( PacketVideo ), 0x09 );

            //--- End register ---

            Logger.Init ();
            Logger.OnRecieveLog += OnLog;
            Logger.OnRecieveErrorLog += OnLog;
            _server = new Server ();
            Packet.PacketRecieved += PacketRecieved;

            _server.Start ();
            Logger.Log ( "Server Started (" + _server.LocalIP.ToString () + ")" );


            Client.ClientJoined += Client_ClientJoined;
            Client.ClientLeft += Client_ClientLeft;

            _castServer = new MultiCast ();
            _castServer.Start ();

            Logger.Log ( "Casting server started" );

        }

        private void AeroWindow_Closing ( object sender, System.ComponentModel.CancelEventArgs e ) {
            if ( _server != null ) {
                _server.Stop ();
            }

            if ( _castServer != null ) {
                _castServer.Stop ();
            }

            Logger.DeInit ();
        }


        void OnLog ( object sender, LogEventArgs args ) {
            txtLog.Dispatcher.BeginInvoke (
                new Action ( () => txtLog.AppendText ( args.Message + Environment.NewLine )
             ) );
        }

        void PacketRecieved ( object sender, Packet.PacketEventArgs args ) {
            if ( args.Packet is PacketControl ) {
                PacketControl ctrl = args.Packet as PacketControl;
            } else if ( args.Packet is PacketVideo ) {
                PacketVideo pack = args.Packet as PacketVideo;
            }
        }

        void Client_ClientLeft ( object sender, Client.ClientConnectionEventArgs e ) {
            Logger.Log ( "Client disconnected (" + ( (IPEndPoint) e.Client.ClientSocket.Client.RemoteEndPoint ).Address.ToString () + ")" );
        }

        void Client_ClientJoined ( object sender, Client.ClientConnectionEventArgs e ) {
            Logger.Log ( "Client connected (" + ( (IPEndPoint) e.Client.ClientSocket.Client.RemoteEndPoint ).Address + ")" );
        }
    }


}
