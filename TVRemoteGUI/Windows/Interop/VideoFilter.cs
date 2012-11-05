
using System.IO;

namespace TVRemoteGUI.Windows.Interop {

    /// <summary>
    /// Sorts through a filter of options to pick the best video for viewing.
    /// </summary>
    public class VideoFilter {

        public static string[] Filters = new[] {
            "Rendered - %" //Adobe stuff throws a "Rendered" in front of the prerendered files
        };

        /// <summary>
        /// Searches through the filters to check if the video file is in it.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static bool IsInFilter ( string fileName ) {
            for ( int i = 0; i < Filters.Length; i++ ) {
                var name = Path.GetFileName ( fileName );
                if ( name != null ) {
                    if ( name.EndsWith ( "%", System.StringComparison.Ordinal ) ) {
                        if ( name.Substring ( 0, name.Length - 1 ).EndsWith ( Filters[ i ] ) ) {
                            return true;
                        }
                    } else if ( name.StartsWith ( "%", System.StringComparison.Ordinal ) ) {
                        if ( name.Substring ( 1, name.Length ).StartsWith ( Filters[ i ] ) ) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
