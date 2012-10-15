using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.CLI.Utils;
using CLI.Packets;
using ComputerRemote.Networking;
using RemoteLib.Networking;

namespace ComputerRemote.CLI {
    class Program {

        private static ConsoleColor regularForColor;
        private static ConsoleColor regularBackColor;

        static void Main ( string[] args ) {
            //Register packets

            Packet.RegisterPacket( typeof( PacketMessage ), 0x04 );

            //End register

            regularBackColor = Console.BackgroundColor;
            regularForColor = Console.ForegroundColor;


            for ( int i = 0; i < args.Length; i++ ) {
                if ( args[ i ].IndexOf( "--debug" ) != -1 ) {
                    Paramaters.DebugEnabled = true;
                }
                else if ( args[ i ].IndexOf( "--nocast" ) != -1 ) {
                    Paramaters.Multicating = false;
                }
            }

            Logger.Init();
            Logger.OnRecieveLog += OnLog;
            Logger.OnRecieveErrorLog += OnError;
            Server server = new Server();
            Packet.PacketRecieved += new EventHandler<Packet.PacketEventArgs>( Packet_PacketRecieved );
            MultiCast cast = null;

            server.Start();
            

            if ( Paramaters.Multicating ) {
                cast = new MultiCast();
                cast.Start();

                Logger.Log( "Casting server started" );
            }

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
                    Logger.Log( "[ " + DateTime.Now.ToString( "T" ) + " ] - " + read );
                }
            }

            if ( cast != null ) {
                cast.Stop();
            }

            server.Stop();
            Console.Clear();
            Console.WriteLine( "Press any key to close" );
            Console.Read();
        }

        static void Packet_PacketRecieved ( object sender, Packet.PacketEventArgs e ) {
            if ( e.Packet is PacketMessage ) {
                PacketMessage msg = e.Packet as PacketMessage;

                Logger.Log( msg.Message );
            }
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
