/*
	Copyright (c) 2012 Brayden (headdetect)

	Permission is hereby granted, free of charge, 
	to any person obtaining a copy of this software
	and associated documentation files (the "Software"),
	to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify,
	merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom
	the Software is furnished to do so, subject to the
	following conditions:

	The above copyright notice and this permission notice
	shall be included in all copies or substantial portions
	of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
	ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
	TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
	SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
	IN THE SOFTWARE.
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