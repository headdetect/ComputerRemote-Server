/*
Copyright 2012 MCForge
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using System.IO;
using System.Net;
using System;

namespace ComputerRemote.CLI.Utils {


    public class FileUtils {

        public const string PropertiesPath = "properties/";
        public const string DllsPath = "dlls/";
        public const string ExtrasPath = "extras/";
        public const string LogsPath = "logs/";



        /// <summary>
        /// Downloads  a file from the specifed website
        /// </summary>
        /// <param name="url">File address</param>
        /// <param name="saveLocation">Location to save the file</param>
        public static void CreateFileFromWeb ( string url, string saveLocation ) {
            using ( var client = new WebClient() )
                client.DownloadFile( url, saveLocation );
        }

        /// <summary>
        /// Creates the file from byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="saveLocation">The save location.</param>
        public static void CreateFileFromBytes ( byte[] bytes, string saveLocation ) {
            using ( var stuff = File.Create( saveLocation ) )
                stuff.Write( bytes, 0, bytes.Length );
        }


        /// <summary>
        /// Creates a directory if it doesn't exist, will log results
        /// </summary>
        public static void CreateDirIfNotExist ( string directory ) {
            if ( Directory.Exists( directory ) )
                return;

            Directory.CreateDirectory( directory );
            Logger.Log( string.Format( "[Directory] Created \"{0}\"", directory ) );
        }

        /// <summary>
        /// Creats a file if it doesnt already exist, logs results.
        /// </summary>
        /// <param name="fileLoc"></param>
        public static void CreateFileIfNotExist ( string fileLoc, string contents = null ) {
            if ( File.Exists( fileLoc ) )
                return;

            if ( contents == null )
                File.Create( fileLoc ).Close();
            else
                using ( var filer = File.CreateText( fileLoc ) )
                    filer.Write( contents );

            Logger.Log( string.Format( "[File] \"{0}\" was created", fileLoc ) );
        }

    }
}