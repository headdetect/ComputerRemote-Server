using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using RemoteLib.Net;
using RemoteLib.Net.TCP;
using RemoteLib.Utils;
using TVRemoteGUI.Windows.Interop;
using TVRemoteGUI.Windows.Utils;
using TVRemoteGUI.Networking.Packets;
using System.Net;
using System.Windows.Media;

namespace TVRemoteGUI.Windows {
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow {

        private TcpRemoteServer _server;
        private DeviceDiscovery _castServer;
        private RemoteClient _client;

        public VideoWindow () {

        }

        private void WindowLoaded ( object sender, RoutedEventArgs e ) {
            new Thread ( HandleNetwork ).Start ();

            GlassArea = new Margins ( -1 );
            mPlayer.LoadedBehavior = MediaState.Manual;
            mPlayer.Source = new Uri ( @"C:\Users\Brayden\Videos\Basketball Movie\BasketballSequence.avi" );
            mPlayer.Play ();

        }

        void AddVideos ( object sender, VideoDiscoveredArgs args ) {
            if ( VideoFilter.IsInFilter ( args.Video.Location ) ) {
                return;
            }

            if ( _client != null ) {
                _client.PacketWriter.EnqueuePacket ( new PacketVideo ( args.Video ) );
            }


            lstVideos.Dispatcher.Invoke ( new Action ( () => lstVideos.Items.Add ( args.Video.Location ) ) );
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
            _server = new TcpRemoteServer ();
            Packet.PacketRecieved += PacketRecieved;

            _server.Start ();
            Logger.Log ( "Server Started (" + _server.LocalIP.ToString () + ")" );


            RemoteClient.ClientJoined += Client_ClientJoined;
            RemoteClient.ClientLeft += Client_ClientLeft;

            _castServer = new DeviceDiscovery ();
            _castServer.BeginCast();

            Logger.Log ( "Casting server started" );

        }

        private void AeroWindow_Closing ( object sender, System.ComponentModel.CancelEventArgs e ) {
            if ( _server != null ) {
                _server.Stop ();
            }

            if ( _castServer != null ) {
                _castServer.EndCast ();
            }

            Logger.DeInit ();
        }


        void OnLog ( object sender, LogEventArgs args ) {
            txtLog.Dispatcher.BeginInvoke (
                new Action ( () => txtLog.AppendText ( args.Message + Environment.NewLine )
             ) );
        }

        private object _defaultContent;

        void PacketRecieved ( object sender, Packet.PacketEventArgs args ) {
            if ( args.Packet is PacketControl ) {
                PacketControl ctrl = args.Packet as PacketControl;
                
                if ( mPlayer != null ) {
                    mPlayer.Dispatcher.BeginInvoke ( new Action ( () => {
                        switch ( ctrl.Control ) {
                            case ControlType.Play:
                                mPlayer.Source = new Uri ( ctrl.ValueString );
                                mPlayer.Play ();
                                break;
                            case ControlType.Pause:
                                if ( mPlayer.HasVideo && mPlayer.CanPause ) {
                                    mPlayer.Pause ();
                                }
                                break;
                            case ControlType.FullScreen:

                                if ( _defaultContent == null ) {
                                    _defaultContent = Content;
                                }

                                if ( mPlayer.Equals(Content) ) {
                                    this.Background = Brushes.Transparent;
                                    this.Content = _defaultContent;
                                } else {
                                    this.Background = Brushes.Black;
                                    this.Content = mPlayer;
                                }
                                break;
                        }
                    } ) );
                }
            } else if ( args.Packet is PacketVideo ) {
                PacketVideo pack = args.Packet as PacketVideo;
                Logger.Log ( "Recieved video request" );
                if ( pack.VideoID == -1 ) {

                    var ar = new VideoDiscoverer ( DiscoverType.Downloads );
                    new Thread ( ar.Discover ).Start ();
                    ar.VideoDiscovered += AddVideos;

                    lstVideos.Dispatcher.Invoke ( new Action ( () => lstVideos.Items.Clear () ) );
                }
            }
        }

        void Client_ClientLeft(object sender, ClientConnectionEventArgs e)
        {
            if ( e.RemoteClient == _client ) {
                _client = null;
            }

            Logger.Log ( "Client disconnected" );
        }

        void Client_ClientJoined(object sender, ClientConnectionEventArgs e)
        {
            if ( _client == null ) {
                _client = e.RemoteClient;
            } else {
                e.RemoteClient.Disconnect();
                return;
            }
            //TODO: fix
            //Logger.Log("Client connected (" + ((IPEndPoint)e.TcpRemoteClient.TcpClient.Client.RemoteEndPoint).Address + ")");
        }
    }


}
