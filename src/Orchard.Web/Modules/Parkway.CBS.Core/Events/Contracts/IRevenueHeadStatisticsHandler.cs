using Orchard.Events;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Events.Contracts
{
    public interface IRevenueHeadStatisticsEventHandler : IEventHandler
    {
        /// <summary>
        /// set stats for invoice
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Stats</returns>
        Stats InvoiceAdded(StatsContext context);

        /// <summary>
        /// Update the stats table when an invoice payment notification has been received
        /// </summary>
        /// <param name="context"></param>
        void InvoicePaymentNotification(StatsContextUpdate statsContext);
    }
}
