using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EIRSPaymentSettlementTask
{
    public class TaskEntry : IJob
    {
        private ILog Logger;
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                SchedulerHelper taskUtil = new SchedulerHelper(context);
                Logger = taskUtil.Logger;
                Logger.Debug("About to execute the task");
                var taskImpl = new TaskImpl(taskUtil);
                //taskImpl.TestProcess();
                taskImpl.Process();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                if (Logger == null)
                    Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
    }
}
