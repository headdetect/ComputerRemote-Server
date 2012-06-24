using System;

namespace ComputerRemote
{
	/// <summary>
	/// Packet identifier.
	/// </summary>
	public enum PacketID
	{
		/// <summary>
		/// Constant login. <c>Client -> Server</c>
		/// </summary>
		Login,
		
		/// <summary>
		/// Constant hand shake. <c>Client <- Server</c>
		/// </summary>
		HandShake, 
		
		/// <summary>
		/// Constant message. <c>Client <-> Server</c>
		/// </summary>
		Message, 
		
		/// <summary>
		/// Constant command. <c>Client <-> Server</c>
		/// </summary>
		Command,
		
		/// <summary>
		/// Constant ping.
		/// </summary>
		Ping,
		
		/// <summary>
		/// Constant kick.
		/// </summary>
		Kick,
		
	}
}

