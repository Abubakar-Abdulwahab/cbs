using Parkway.CBS.POSSAP.Scheduler.Models;

namespace Parkway.CBS.POSSAP.Services.Adapters.Contracts
{
    public interface IPOSSAPSchedulerAdapter
    {

        /// <summary>
        /// Add log to call log
        /// </summary>
        /// <param name="callModel"></param>
        void SaveCallLog(CallLogForExternalSystem callModel);

    }
}
