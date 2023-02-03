using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.CentralBillingSystem
{
    class VaribaleBillingJob : IJob
    {
        static int count = 0;
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() => { });
        }
    }
}