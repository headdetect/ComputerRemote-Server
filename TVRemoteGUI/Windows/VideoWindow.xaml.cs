using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TVRemoteGUI.Windows.Interop;
using TVRemoteGUI.Windows.Utils;

namespace TVRemoteGUI.Windows {
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow {

        public VideoWindow () {
            
        }

        private void WindowLoaded ( object sender, RoutedEventArgs e ) {
            GlassArea = new Margins(-1);
            mPlayer.LoadedBehavior = MediaState.Manual;
            mPlayer.Source = new Uri( @"C:\Users\Brayden\Videos\Basketball Movie\BasketballSequence.avi" );
            mPlayer.Play();

            var ar = new VideoDiscoverer ();
            new Thread(ar.Discover).Start();
            ar.VideoDiscovered += AddVideos;
        }

        void AddVideos(object sender, VideoDiscoveredArgs args) {
            if(VideoFilter.IsInFilter(args.FileLocation)) {
                return;
            }


            //TODO: not make it do multiple stuffs.
            
            if(lstVideos.Items.Contains(args.FileLocation)) {
                return;
            }

            lstVideos.Dispatcher.Invoke ( new Action( () => lstVideos.Items.Add(args.FileLocation) ));
        }

        private void lstVideos_MouseDoubleClick ( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
           if(lstVideos.SelectedIndex != -1 && lstVideos.SelectedIndex < lstVideos.Items.Count) {
               string file = lstVideos.SelectedItem.ToString ();

               mPlayer.Source = new Uri( file, UriKind.RelativeOrAbsolute);
               mPlayer.Play();
           }
        }

    }

    
}
