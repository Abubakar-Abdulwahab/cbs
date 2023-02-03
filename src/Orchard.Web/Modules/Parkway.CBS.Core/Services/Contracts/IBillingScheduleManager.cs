using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IBillingScheduleManager<BillingSchedule> : IDependency, IBaseManager<BillingSchedule>
    {
        IEnumerable<Models.BillingSchedule> GetScheduleForTaxPayers(BillingModel billing, IEnumerable<TaxEntity> taxPayers);
    }
}
