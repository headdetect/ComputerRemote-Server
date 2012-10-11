using System;

namespace ComputerRemote
{
	public static class ComputerRemote
	{
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
		public static Server Server { get; set; }
		
		private static bool _started = false;

        /// <summary>
        /// Starts this instance.
        /// </summary>
		public static void Start(){
			if(_started) return;
			_started = true;
			
			if(Server == null)
				Server = new Server();
			
			Server.Start();
		}

        /// <summary>
        /// Stops this instance.
        /// </summary>
		public static void Stop(){
			if(!_started || Server == null) return;
			_started = false;
			
			Server.Stop();
		}
		
	}
}

