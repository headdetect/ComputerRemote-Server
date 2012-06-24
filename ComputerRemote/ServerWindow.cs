using System;

namespace ComputerRemote
{
	public partial class ServerWindow : Gtk.Window
	{
		public ServerWindow () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

