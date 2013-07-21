using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteLib.Net;
using RemoteLib.Net.Packets;
using RemoteLib.Net.UDP;

namespace LibTests
{
    [TestClass]
    public class TestUdpServer
    {
        private static Socket connectToServerClient;

        private static EndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        [TestMethod]
        public void TestClientConnection()
        {
            SetupServer();

            connectToServerClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Assert.IsNotNull(connectToServerClient);

            try
            {
               // connectToServerClient.Connect(ipEnd);
            }
            catch (Exception e)
            {

                Assert.Fail(e.Message);
            }

            prepareTestNetwork();

            Assert.IsNotNull(connectToServerClient);

            //Creating Init packet//
            byte[] data = new byte[512];
            data[0] = 0x00;

            try
            {
                // We must send PacketInit to init the client with in the server //
                connectToServerClient.SendTo(data, ipEnd);

                // PacketPing //
                data[0] = 0x02;

                // We send again just for this test //
                connectToServerClient.SendTo(data, ipEnd);
            }
            catch (Exception e)
            {

                Assert.Fail(e.Message);
            }

            try
            {
                byte[] buffer = new byte[512];
                connectToServerClient.ReceiveFrom(buffer, ref ipEnd);
                if (buffer[0] != 0x02)
                {
                    Assert.Fail("Recieved invalid packet");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Method probably timed out\nMake sure a server is on to test the requests\n" + e.Message);
            }
        }
        
        private void prepareTestNetwork()
        {
            //ThreadPool.QueueUserWorkItem(a => Thread.Sleep(10000));
        }


        private UdpRemoteServer mServer;

        public void SetupServer()
        {
            mServer = new UdpRemoteServer(6000);
            mServer.Start();


            Packet.PacketRecieved += (sender, args) =>
            {
                var packet = args.Packet as PacketPing;
                if (packet == null) return;
                args.RemoteClient.PacketWriter.WritePacketNow(new PacketPing());
            };
        }

        
    }
}
