using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ComputerRemote
{
	public class Server
	{
		private Socket mListener;
		
		/// <summary>
		/// Gets the clients.
		/// </summary>
		/// <value>
		/// The clients.
		/// </value>
		public List<Client> Clients { get; private set; }
		
		public Server ()
		{
			Clients = new List<Client>(24);
			mListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
		
		public void Stop(){
			
			
		}
		
		public void Start(){
			IPEndPoint iep = new IPEndPoint(IPAddress.Any, 69101);
			mListener.Bind(iep);
			mListener.Listen(23);
			mListener.BeginAccept(CallBack, null);
		}
		
		void CallBack(IAsyncResult result){
			
		}
	}
}

