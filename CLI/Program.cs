﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerRemote.CLI.Utils;
using CLI.Packets;
using ComputerRemote.Networking;
using RemoteLib.Networking;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace ComputerRemote.CLI {
    class Program {

        private static ConsoleColor _regularForColor;
        private static ConsoleColor _regularBackColor;

        private static Server _server;

        static void Main ( string[] args ) {
            //Register packets

            Packet.RegisterPacket( typeof( PacketMessage ), 0x04 );
            Packet.RegisterPacket( typeof( PacketCommand ), 0x05 );

            //End register

            _regularBackColor = Console.BackgroundColor;
            _regularForColor = Console.ForegroundColor;


            for ( int i = 0; i < args.Length; i++ ) {
                if ( args[ i ].IndexOf("--debug", StringComparison.Ordinal) != -1 ) {
                    Paramaters.DebugEnabled = true;
                }
                else if ( args[ i ].IndexOf("--nocast", StringComparison.Ordinal) != -1 ) {
                    Paramaters.Multicating = false;
                }
            }

            Logger.Init();
            Logger.OnRecieveLog += OnLog;
            Logger.OnRecieveErrorLog += OnError;
            _server = new Server();
            Packet.PacketRecieved += Packet_PacketRecieved;
            MultiCast cast = null;

            _server.Start();
            Logger.Log( "Server Started (" + _server.LocalIP.ToString() + ")" );


            Client.ClientJoined += Client_ClientJoined;
            Client.ClientLeft += Client_ClientLeft;

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
                    for ( int i = 0; i < _server.Clients.Count; i++ ) {
                        Client c = _server.Clients[ i ];
                        c.PacketQueue.Enqueue( new PacketMessage( read ) );
                    }

                    Console.SetCursorPosition( Console.CursorLeft, Console.CursorTop - 1 );
                    Logger.Log( read );
                }
            }

            if ( cast != null ) {
                cast.Stop();
            }

            _server.Stop();
            Console.Clear();
            Console.WriteLine( "Press any key to close" );
            Console.Read();
        }

        static void Client_ClientLeft ( object sender, Client.ClientConnectionEventArgs e ) {
            Logger.Log( "Client disconnected (" + ( (IPEndPoint) e.Client.ClientSocket.Client.RemoteEndPoint ).Address.ToString() + ")" );
        }

        static void Client_ClientJoined ( object sender, Client.ClientConnectionEventArgs e ) {
            Logger.Log( "Client connected (" + ( (IPEndPoint) e.Client.ClientSocket.Client.RemoteEndPoint ).Address + ")" );
        }

        static void Packet_PacketRecieved ( object sender, Packet.PacketEventArgs e ) {
            if ( e.Packet is PacketMessage ) {
                PacketMessage msg = e.Packet as PacketMessage;

                Logger.Log( "(Client) " + msg.Message );
            }
            else if ( e.Packet is PacketCommand ) {
                PacketCommand cmd = e.Packet as PacketCommand;

                Logger.Log( "Running command (" + cmd.Command + ")" );

                try {
                    Thread objThread = new Thread( RunCommand ) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
                    objThread.Start( cmd.Command );
                }
                catch ( Exception ex ) {
                    Logger.LogError( ex );
                }
            }
        }

        static void OnLog ( object sender, LogEventArgs e ) {
            Console.ForegroundColor = e.TextColor;
            Console.BackgroundColor = e.BackgroundColor;

            Console.WriteLine( "[ " + DateTime.Now.ToString( "T" ) + " ] - " + e.Message );

            Console.ForegroundColor = _regularForColor;
            Console.BackgroundColor = _regularBackColor;
        }
        static void OnError ( object sender, ErrorLogEventArgs e ) {
            Console.ForegroundColor = e.TextColor;
            Console.BackgroundColor = e.BackgroundColor;

            Console.Error.WriteLine( "[Error] " + e.Message );

            Console.ForegroundColor = _regularForColor;
            Console.BackgroundColor = _regularBackColor;
        }

        static void RunCommand ( object cmd ) {

            try {

                ProcessStartInfo procStartInfo = new ProcessStartInfo( "cmd", "/c " + cmd ) {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine( result );

                for ( int i = 0; i < _server.Clients.Count; i++ ) {
                    Client client = _server.Clients[ i ];
                    client.PacketQueue.Enqueue( new PacketCommand( result ) );
                }
            }
            catch ( Exception ex ) {
                Logger.LogError( ex );
            }
        }
    }
}
