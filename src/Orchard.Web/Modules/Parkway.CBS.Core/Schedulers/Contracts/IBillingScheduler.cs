using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Schedulers.Contracts
{
    public interface IBillingScheduler : IDependency
    {
        
        bool AnyScheduledTask(DateTime dateTime);
        void RunBillingSchedule(DateTime dateTime);

        void SendAnInvoice(BillingSchedule schedule);



        /// <summary>
        /// Schedule a billing task
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="revenueHead"></param>
        /// <param name="helper"></param>
        void CreateAFixedBillingSchedule(ExpertSystemSettings tenant, RevenueHead revenueHead, ScheduleHelperModel helper, bool reshedule = false);
    }
}
