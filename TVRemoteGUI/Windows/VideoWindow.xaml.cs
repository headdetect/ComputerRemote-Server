using System;
using System.Windows;
using System.Windows.Controls;
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
        }

    }

 
}
