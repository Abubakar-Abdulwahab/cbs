using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Parkway.CBS.HangfireScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BillingSchedulerService()
            };

            ServiceBase.Run(ServicesToRun);

        }
    }
}
