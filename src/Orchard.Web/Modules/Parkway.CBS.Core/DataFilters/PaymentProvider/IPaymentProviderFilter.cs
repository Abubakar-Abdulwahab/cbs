using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.PaymentProvider
{
    public interface IPaymentProviderFilter : IDependency
    {
        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReportStatsVM> GetAggregate(PaymentProviderSearchParams searchParams);

        /// <summary>
        /// Get payment providers
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ExternalPaymentProviderVM> GetProviders(PaymentProviderSearchParams searchParams);

        /// <summary>
        /// Get payment providers view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ProviderRecords, Aggregate }</returns>
        dynamic GetPaymentProvidersViewModel(PaymentProviderSearchParams searchParams);
    }
}
