using System;
using Gtk;

namespace ComputerRemote
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			ServerWindow win = new ServerWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
