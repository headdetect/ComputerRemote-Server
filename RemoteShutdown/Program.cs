using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RemoteShutdown
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var service = new ReceiverService();

            if (Environment.UserInteractive)
            {
                service.OnStartDelegate(args);
                Console.WriteLine("Press dem keys to stop the program.");
                Console.Read();
                service.OnStopDelegate();
            }
            else
                ServiceBase.Run(service);
        }
    }
}
