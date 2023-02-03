using ParkwayScheduler.Configuration;
using Quartz;

namespace EIRSPaymentSettlementTask
{
    public class SchedulerHelper : JobDataProvider
    {
        public SchedulerHelper(IJobExecutionContext context) : base(context)
        {
        }
    }
}