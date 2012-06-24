using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ComputerRemote
{
	public class Client
	{
		
		public TcpClient ClientSocket { get; set; }

        public NetworkStream NStream { get; set; }

        public Queue<Packet> PacketQueue { get; set; }


		public Client (TcpClient client)
		{
			ClientSocket = client;
			NStream = client.GetStream();
			PacketQueue = new Queue<Packet>();
		}
		
		public void HandleLogin(PacketLogin packet){
		
		}
	}
}

