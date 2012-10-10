using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVRemote {
    public class Program {

        [STAThread]
        public static void Main () {
            Application.SetCompatibleTextRenderingDefault( true );
            Application.EnableVisualStyles();

            Application.Run( new VideoForm(@"C:\Users\Brayden\Downloads\Breaking Bad Season 3\Breaking.Bad.S03E02.DVDRip.XviD-aAF.avi") );
        }
    }
}
