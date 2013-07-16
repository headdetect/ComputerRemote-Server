using System;
using System.Windows.Forms;
using ComputerRemote.CLI.Utils;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string t in args)
            {
                if (t.IndexOf("--debug", StringComparison.Ordinal) != -1)
                {
                    Paramaters.DebugEnabled = true;
                }
                else if (t.IndexOf("--nocast", StringComparison.Ordinal) != -1)
                {
                    Paramaters.Multicating = false;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new MainForm());

        }
    }
}
