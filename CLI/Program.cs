using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.CLI.Utils;
using CLI.Packets;
using ComputerRemote.Networking;

namespace ComputerRemote.CLI {
    class Program {

        private static ConsoleColor regularForColor;
        private static ConsoleColor regularBackColor;

        static void Main ( string[] args ) {
            //Register packets

            Packet.RegisterPacket( typeof( PacketMessage ), 0x04 );
            Packet.RegisterPacket( typeof( PacketInfoExchange ), 0x05 );

            //End register

            regularBackColor = Console.BackgroundColor;
            regularForColor = Console.ForegroundColor;


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
                string read = Console.ReadLine();
                if ( read == "stop" ) {
                    break;
                }
                else {
                    for ( int i = 0; i < server.Clients.Count; i++ ) {
                        Client c = server.Clients[ i ];
                        c.PacketQueue.Enqueue( new PacketMessage( read ) );
                    }

                    Console.SetCursorPosition( Console.CursorLeft, Console.CursorTop - 1 );
                    Logger.Log(  "[ " + DateTime.Now.ToString("T") + " ] - " + read);
                }
            }

            server.Stop();
            Console.Clear();
            Console.WriteLine("Press any key to close");
            Console.Read();
        }

        static void OnLog ( object sender, LogEventArgs e ) {
            Console.ForegroundColor = e.TextColor;
            Console.BackgroundColor = e.BackgroundColor;

            Console.WriteLine( e.Message );

            Console.ForegroundColor = regularForColor;
            Console.BackgroundColor = regularBackColor;
        }
        static void OnError ( object sender, ErrorLogEventArgs e ) {
            Console.ForegroundColor = e.TextColor;
            Console.BackgroundColor = e.BackgroundColor;

            Console.Error.WriteLine( "[Error] " + e.Message );

            Console.ForegroundColor = regularForColor;
            Console.BackgroundColor = regularBackColor;
        }
    }
}
