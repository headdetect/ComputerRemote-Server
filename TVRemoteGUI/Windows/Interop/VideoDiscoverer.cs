using System;
using System.IO;

namespace TVRemoteGUI.Windows.Interop {

    /// <summary>
    /// Uses async methods to search for videos on the hard drive. It is quite hacky and bruteforcy.
    /// </summary>
    public class VideoDiscoverer {

        private readonly string _parentFolder;
        private static readonly IntPtr _INVALID_HANDLE = new IntPtr ( -1 );
        private static readonly string[] _FILE_TYPES = new[] { "avi", "wmv", "mp4" };


        /// <summary>
        /// Occurs when a video has been discovered.
        /// </summary>
        public event EventHandler < VideoDiscoveredArgs > VideoDiscovered;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDiscoverer"/> class.
        /// </summary>
        /// <param name="type">Where to look for the files.</param>
        public VideoDiscoverer ( DiscoverType type = DiscoverType.Videos ) {

            switch ( type ) {
                case DiscoverType.Videos:
                    _parentFolder = Environment.GetFolderPath ( Environment.SpecialFolder.MyVideos );
                    break;
                case DiscoverType.Downloads:
                    _parentFolder = Environment.GetFolderPath ( Environment.SpecialFolder.UserProfile ) + "\\Downloads";
                    break;
                case DiscoverType.Library:
                    _parentFolder = Environment.GetFolderPath ( Environment.SpecialFolder.UserProfile );
                    break;
                case DiscoverType.Full:
                    _parentFolder = Path.GetPathRoot ( Environment.CurrentDirectory );
                    break;

            }

        }
        /// <summary>
        /// Discovers the videos from the specified point.
        /// </summary>
        public void Discover () {
            Start ( _parentFolder );
        }


        private void Start ( string path ) {

            //Sets the defaults
            IntPtr fHandle = _INVALID_HANDLE;
            NativeFileUtils.WIN32_FIND_DATAW data;

            for ( int i = 0; i < 3; i++ ) {
                string extention = _FILE_TYPES[ i ];
                try {
                    fHandle = NativeFileUtils.FindFirstFileW ( path + "\\*", out data );
                    if ( fHandle != _INVALID_HANDLE ) {
                        do {
                            string fullpath = path + ( path.EndsWith ( "\\" ) ? "" : "\\" ) + data.cFileName;

                            if ( data.cFileName == "." || data.cFileName == ".." ) {
                                //They just happen to be directories, not files yo.
                                continue;
                            }

                            if ( ( data.dwFileAttributes & FileAttributes.Directory ) != 0 ) {
                                Start ( fullpath );
                            }

                            if ( VideoDiscovered != null && fullpath.EndsWith ( extention, StringComparison.OrdinalIgnoreCase ) ) {
                                VideoDiscovered ( this, new VideoDiscoveredArgs ( fullpath ) );
                            }


                        } while ( NativeFileUtils.FindNextFile ( fHandle, out data ) );
                    }
                } finally {
                    if ( fHandle != _INVALID_HANDLE ) {
                        NativeFileUtils.FindClose ( fHandle );
                    }
                }
            }

        }

    }

    public class VideoDiscoveredArgs : EventArgs {

        /// <summary>
        /// Gets the file location.
        /// </summary>
        public Video Video { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDiscoveredArgs"/> class.
        /// </summary>
        /// <param name="location">The location of the file.</param>
        public VideoDiscoveredArgs ( string location ) {
            Video = new Video ( location );
        }

    }

    public enum DiscoverType {

        /// <summary>
        /// Look for videos on every crack of the hard drive, note this will use a ton of resources
        /// </summary>
        Full,

        /// <summary>
        /// Just look for videos in the user's library
        /// </summary>
        Library,

        /// <summary>
        /// Just look for videos in the user's download folder
        /// </summary>
        Downloads,

        /// <summary>
        /// Just look for videos in the user's videos folder
        /// </summary>
        Videos

    }
}
