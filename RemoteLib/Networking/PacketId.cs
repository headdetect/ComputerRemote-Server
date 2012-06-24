using System;

namespace ComputerRemote
{
	/// <summary>
	/// Packet identifier.
	/// </summary>
	public enum PacketID
	{
        /// <summary>
        /// Constant login. <c>Client -&gt; Server</c>
        /// </summary>
		Login,

        /// <summary>
        /// Constant hand shake. <c>Server -&gt; Client</c>
        /// </summary>
		HandShake, 
		
		/// <summary>
		/// Constant message. <c>Client &lt;-&gt; Server</c>
		/// </summary>
		Message, 
		
		/// <summary>
		/// Constant command. <c>Client &lt;-&gt; Server</c>
		/// </summary>
		Command,
		
		/// <summary>
		/// Constant ping. <c>Client &lt;-&gt; Server</c>
		/// </summary>
		Ping,
		
		/// <summary>
		/// Constant kick. <c>Client -&gt; Server</c>
		/// </summary>
		Kick,


        /// <summary>
        /// 
        /// </summary>
        GetInfo,

        SendInfo
		
	}
}

