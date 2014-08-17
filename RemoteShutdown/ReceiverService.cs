using RemoteLib.Net;
using RemoteLib.Net.TCP;
using RemoteLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RemoteShutdown
{
    public partial class ReceiverService : ServiceBase
    {
        private TcpRemoteServer server;
        private DeviceDiscovery multicast;

        public ReceiverService()
        {
            InitializeComponent();
        }

        internal void OnStartDelegate(string[] args) { OnStart(args);  }
        protected override void OnStart(string[] args)
        {
            Packet.RegisterPacket(typeof(Packets.PacketLockDesktop));
            Packet.RegisterPacket(typeof(Packets.PacketSleep));
            Packet.RegisterPacket(typeof(Packets.PacketHibernate));
            Packet.RegisterPacket(typeof(Packets.PacketShutdown));
            Packet.RegisterPacket(typeof(Packets.PacketRestart));
            Packet.RegisterPacket(typeof(Packets.PacketMessageBox));

            Packet.PacketRecieved += Packet_PacketRecieved;

            server = new TcpRemoteServer();
            server.Start();

            multicast = new DeviceDiscovery();
            multicast.BeginCast();
        }

        void Packet_PacketRecieved(object sender, Packet.PacketEventArgs e)
        {
            var packet = e.Packet;
            if (packet is Packets.PacketLockDesktop)
            {
                RunCommand("rundll32.exe user32.dll, LockWorkStation");
            }
            else if (packet is Packets.PacketSleep)
            {
                RunCommand("rundll32.exe powrprof.dll,SetSuspendState 0,1,0");
            }
            else if (packet is Packets.PacketHibernate)
            {
                RunCommand("rundll32.exe PowrProf.dll,SetSuspendState");
            }
            else if (packet is Packets.PacketShutdown)
            {
                RunCommand("Shutdown.exe -s -t 00");
            }
            else if (packet is Packets.PacketRestart)
            {
                RunCommand("Shutdown.exe -r -t 00");
            }
            else if (packet is Packets.PacketMessageBox)
            {
                var messagePacket = packet as Packets.PacketMessageBox;
                RunCommand("Msg * \"" + messagePacket.Message + "\"");
            }
        }

        void RunCommand(object cmd)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + cmd)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                Process proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }
            catch (Exception)
            {
            }
        }

        internal void OnStopDelegate() { OnStop(); }
        protected override void OnStop()
        {
            if (multicast != null)
                multicast.EndCast();

            if (server != null)
                server.Stop();
        }
    }
}
