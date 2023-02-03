using System;
using System.Threading.Tasks;
using Quartz;
using Parkway.Scheduler.Interface.Loggers.Contracts;

namespace Parkway.Scheduler.Interface.Schedulers.Quartz
{
    internal class HelloJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                ISchedulerLogger logger = context.Scheduler.Context.Get("Logger") as ISchedulerLogger;
                logger.Error("Hello from hello job " + DateTime.Now.ToLocalTime());
            });
        }
    }
}