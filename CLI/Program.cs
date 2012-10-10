using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.CLI.Utils;

namespace ComputerRemote.CLI {
    class Program {
        static void Main ( string[] args ) {

            for ( int i = 0; i < args.Length; i++ ) {
                if ( args[ i ].IndexOf( "--debug" ) != -1) {
                    Paramaters.DebugEnabled = true;
                }
            }
            Logger.Init();
            Logger.OnRecieveLog += OnLog;
            Logger.OnRecieveErrorLog += OnError;
            var server = new Server();
            server.Start();

            while ( true ) {
                if ( Console.ReadLine() == "stop" )
                    break;
            }

            server.Stop();
            Console.Clear();
            Console.WriteLine("Press any key to close");
            Console.Read();
        }

        static void OnLog ( object sender, LogEventArgs e ) {
            Console.WriteLine( e.Message );
        }
        static void OnError ( object sender, LogEventArgs e ) {
            Console.Error.WriteLine( e.Message );
        }
    }
}
