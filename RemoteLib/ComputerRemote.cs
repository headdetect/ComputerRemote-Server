using System;

namespace ComputerRemote
{
	public static class ComputerRemote
	{
		public static Server Server { get; set; }
		
		private static bool _started = false;
		
		public static void Start(){
			if(_started) return;
			_started = true;
			
			if(Server == null)
				Server = new Server();
			
			Server.Start();
		}
		
		public static void Stop(){
			if(!_started || Server == null) return;
			_started = false;
			
			Server.Stop();
		}
		
	}
}

