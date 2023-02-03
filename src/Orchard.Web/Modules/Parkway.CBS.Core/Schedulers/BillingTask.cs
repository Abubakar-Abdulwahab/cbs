using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using Parkway.CBS.Core.Schedulers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Schedulers
{
    public class BillingTask //: IScheduledTaskHandler
    {
        private readonly IScheduledTaskManager _taskManager;
        //private readonly IBillingScheduler _taskHandler;
        public ILogger Logger { get; set; }

        public BillingTask(IScheduledTaskManager taskManager/*, IBillingScheduler taskHandler*/)
        {
            //_taskHandler = taskHandler;
            _taskManager = taskManager;
            Logger = NullLogger.Instance;
            ScheduleNextTask(DateTime.UtcNow.AddMinutes(1));
        }

        public void Process(ScheduledTaskContext context)
        {
            //var er = context.Task.TaskType;
            DateTime now = DateTime.Now.ToLocalTime();
            var dateTime = RoundUp(now, TimeSpan.FromMinutes(30));
            //if (_taskHandler.AnyScheduledTask(dateTime))
            //{
            //    _taskHandler.RunBillingSchedule(dateTime);
            //}
            //Logger.Information("Task is running info");
            //Logger.Error("Task is running");
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

        private void ScheduleNextTask(DateTime date)
        {
            //if (date > DateTime.Now.ToLocalTime())
            {
                var tasks = this._taskManager.GetTasks(date.ToString());
                //if (tasks == null || tasks.Count() == 0)
                this._taskManager.CreateTask(date.ToString(), date, null);
            }
        }
    }
}