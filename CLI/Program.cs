using System;
using System.Windows.Forms;
using ComputerRemote.CLI.Utils;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new MainForm());

        }
    }
}
