using Parkway.Scheduler.Interface.Loggers.Contracts;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.CentralBillingSystem
{
    internal class PenaltyJob : BaseJob, IJob
    {
        static int count = 0;
        public Task Execute(IJobExecutionContext context)
        {
            if (count % 2 != 0)
            {
                count++;
                return Task.Run(() => { });
            }
            return Task.Run(() => {
                IJobDetail jobDetail = context.JobDetail;
                ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();
                var desc = context.JobDetail.Description;
                logger.Information(string.Format("Running penalty job SN: {3} JN: {0} JG: {1} Date: {2}", jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime(), context.Scheduler.SchedulerName));
            });
        }
    }
}
