using RemoteLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerRemote.CLI.Utils
{
    public class Paramaters
    {

        private static Configuration config = new Configuration("options.json");

        static Paramaters()
        {
            if (!config.KeyExists("Debug"))
                config["Debug"] = new ConfigBlob("Debug", true);

            if (!config.KeyExists("Multicast"))
                config["Multicast"] = new ConfigBlob("Multicast", true);

            config.Save();
        }


        /// <summary>
        /// Gets or sets a value indicating whether debug OUTPUT is enabled
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug OUTPUT is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugEnabled
        {
            get
            {
                return (bool)config["Debug"];
            }
            set
            {
                config["Debug"] = value;
                config.Save();
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether server multicasting is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if server multicasting is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool Multicating       
        {
            get
            {
                return (bool)config["Multicast"];
            }
            set
            {
                config["Multicast"] = value;
                config.Save();
            }
        }
    }
}
