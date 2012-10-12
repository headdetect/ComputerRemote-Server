using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TVRemote {
    internal class Program {

        [STAThread]
        public static void Main () {
            new Application ().Run ( new VideoWindow () );
        }
    }
}
