using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.HangfireScheduler.Configuration.Contracts
{
    public interface IHangFireScheduler
    {
        /// <summary>
        /// Get the list of scheduler item
        /// </summary>
        /// <returns>List{HangFireSchedulerItem}</returns>
        List<HangFireSchedulerItem> GetHangFireSchedulerItems();

        /// <summary>
        /// Start the HangFire Dashboard for each of the tenant
        /// </summary>
        /// <param name="tenantName">name of the tenant</param>
        /// <param name="connectionString"></param>
        /// <param name="DashboardUrl"></param>
        /// <returns></returns>
        bool StartHangFireDashboard(string tenantName, string connectionString, string DashboardUrl);

    }
}
