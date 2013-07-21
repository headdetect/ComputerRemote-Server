using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
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
    public partial class MainForm : Form
    {
        private TcpRemoteServer _server;
        private Multicast _cast;
        private WirelessMouse _mouse;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
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

            if (!Paramaters.Multicating) return;

            _cast = new Multicast();
            _cast.BeginCast();

            Logger.Log("Casting server started");
        }

        void Exit()
        {
            Logger.DeInit();

            if (_cast != null)
                _cast.Stop();

            if (_server != null)
                _server.Stop();

            if (_mouse != null)
                _mouse.Stop();


            _cast = null;
            _server = null;
        }

        void Client_ClientLeft(object sender, ClientConnectionEventArgs e)
        {
            //Logger.Log("Client disconnected (" + ((IPEndPoint)e.RemoteClient..Client.RemoteEndPoint).Address + ")");
        }

        void Client_ClientJoined(object sender, ClientConnectionEventArgs e)
        {
            //Logger.Log("Client connected (" + ((IPEndPoint)e.RemoteClient.TcpClient.Client.RemoteEndPoint).Address + ")");
        }

        void Packet_PacketRecieved(object sender, Packet.PacketEventArgs e)
        {
            if (e.Packet is PacketMessage)
            {
                PacketMessage msg = e.Packet as PacketMessage;

                Logger.Log("(Client) " + msg.Message);
            }
            else if (e.Packet is PacketCommand)
            {
                PacketCommand cmd = e.Packet as PacketCommand;

                Logger.Log("Running command (" + cmd.Command + ")");

                try
                {
                    Thread objThread = new Thread(RunCommand) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
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

        void OnLog(object sender, LogEventArgs e)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke((MethodInvoker)(() => OnLog(sender, e)));
                }
                catch (ObjectDisposedException)
                { }

                return;
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = e.TextColor;
            txtLog.SelectionBackColor = e.BackgroundColor;
            txtLog.AppendText("[ " + DateTime.Now.ToString("T") + " ] - " + e.Message + "\n");
            txtLog.SelectionBackColor = BackColor;
            txtLog.SelectionColor = ForeColor;
        }
        void OnError(object sender, ErrorLogEventArgs e)
        {
            OnLog(sender, new LogEventArgs("[ERROR]" + e.Message, LogType.Error, Color.Red, Color.White));
        }

        void RunCommand(object cmd)
        {

            try
            {

                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + cmd)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);

                foreach (RemoteClient client in _server.Clients)
                {
                    client.PacketWriter.EnqueuePacket(new PacketCommand(result));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }

        private void txtCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                TextEntered();
            }
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            TextEntered();
        }

        void TextEntered()
        {
            string read = txtCommand.Text;
            txtCommand.Text = "";

            if (read == "/stop")
            {
                Exit();
                return;
            }

            foreach (TcpRemoteClient c in _server.Clients)
            {
                c.PacketWriter.EnqueuePacket(new PacketMessage(read));
            }

            Logger.Log(read);
        }

    }
}
