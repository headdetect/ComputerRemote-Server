using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVRemote {
    public partial class VideoForm : Form {

        private string _videoUrl;

        /// <summary>
        /// Gets or sets the video file location. 
        /// Once the location is changed, the media player control will play that file automatically.
        /// </summary>
        /// <value>
        /// The video URL.
        /// </value>
        public string VideoURL {
            get {
                return _videoUrl;
            }

            set {
                _videoUrl = value;
                mPlayer.URL = value;
            }
        }

        /// <summary>
        /// Gets the media player for this form.
        /// </summary>
        public AxWMPLib.AxWindowsMediaPlayer MediaPlayer {
            get { return mPlayer; }
        }

        public VideoForm () {
            InitializeComponent();
        }

        public VideoForm ( string uri ) : this() {
            VideoURL = uri;
        }

        private void VideoForm_Load ( object sender, EventArgs e ) {

        }

        private void btnFullScreen_Click ( object sender, EventArgs e ) {
            mPlayer.fullScreen = true;
        }
    }
}
