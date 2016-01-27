using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CLI.Packets;
using CLI.Utils;
using ComputerRemote.CLI.Utils;
using RemoteLib.Net;
using RemoteLib.Net.TCP;
using RemoteLib.Utils;

namespace CLI
{
    class Program
    {

        private static TcpRemoteServer _server;
        private static DeviceDiscovery _cast;
        private static WirelessMouse _mouse;

        public static void Main(string[] args)
        {

            //Register packets

            Packet.RegisterPacket(typeof(PacketMessage), 0x04);
            Packet.RegisterPacket(typeof(PacketCommand), 0x05);
            Packet.RegisterPacket(typeof(PacketBeep), 0x06);

            //End register


            Logger.Init();
            Logger.OnRecieveLog += OnLog;
            Logger.OnRecieveErrorLog += OnError;
            _server = new TcpRemoteServer();
            Packet.PacketRecieved += Packet_PacketRecieved;


            _server.Start();
            Logger.Log("Server Started (" + _server.LocalIP + ")");


            RemoteClient.ClientJoined += Client_ClientJoined;
            RemoteClient.ClientLeft += Client_ClientLeft;

            _mouse = new WirelessMouse();
            _mouse.Start();

            // if (!Paramaters.Multicating) return;

            //_cast = new DeviceDiscovery();
            //_cast.BeginCast();

            //Logger.Log("Casting server started");

            while (ReadLine())
            {
                // Block //
            }

        }


        static void Exit()
        {
            Logger.DeInit();

            _cast?.EndCast();

            _server?.Stop();

            _mouse?.Stop();


            _cast = null;
            _server = null;
        }

        static void Client_ClientLeft(object sender, ClientConnectionEventArgs e)
        {
            //Logger.Log("Client disconnected (" + ((IPEndPoint)e.RemoteClient..Client.RemoteEndPoint).Address + ")");
        }

        static void Client_ClientJoined(object sender, ClientConnectionEventArgs e)
        {
            //Logger.Log("Client connected (" + ((IPEndPoint)e.RemoteClient.TcpClient.Client.RemoteEndPoint).Address + ")");
        }

        static void Packet_PacketRecieved(object sender, Packet.PacketEventArgs e)
        {
            if (e.Packet is PacketMessage)
            {
                var msg = (PacketMessage) e.Packet;

                Logger.Log("(Client) " + msg.Message);
            }
            else if (e.Packet is PacketCommand)
            {
                var cmd = (PacketCommand) e.Packet;

                Logger.Log("Running command (" + cmd.Command + ")");

                try
                {
                    var objThread = new Thread(RunCommand) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
                    objThread.Start(cmd.Command);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
            else if (e.Packet is PacketBeep)
            {
                Console.Beep(new Random().Next(2000, 3000), 500);
            }
        }

        static void OnLog(object sender, LogEventArgs e)
        {
            Console.BackgroundColor = e.BackgroundColor.ToConsoleColor();
            Console.ForegroundColor = e.TextColor.ToConsoleColor();
            Console.WriteLine("[ {0} ] - {1}", DateTime.Now.ToString("T"), e.Message);
        }

        static void OnError(object sender, ErrorLogEventArgs e)
        {
            OnLog(sender, new LogEventArgs("[ERROR]" + e.Message, LogType.Error, Color.Red, Color.Black));
        }

        static void RunCommand(object cmd)
        {
            try
            {
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + cmd)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                var result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);

                foreach (var client in _server.Clients)
                {
                    client.PacketWriter.EnqueuePacket(new PacketCommand(result));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        static bool ReadLine()
        {
            var read = Console.ReadLine();

            if (read == "/stop")
            {
                Exit();
                return false;
            }

            foreach (var c in _server.Clients)
            {
                c.PacketWriter.EnqueuePacket(new PacketMessage(read));
            }

            Logger.Log(read);

            return true;
        }
    }
}
