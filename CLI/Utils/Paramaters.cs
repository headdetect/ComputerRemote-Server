using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerRemote.CLI.Utils {
    public class Paramaters {

        static Paramaters () {
            DebugEnabled = true;
            Multicating = true;
        }


        /// <summary>
        /// Gets or sets a value indicating whether debug OUTPUT is enabled
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug OUTPUT is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugEnabled { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether server multicasting is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if server multicasting is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool Multicating { get; set; }
    }
}
