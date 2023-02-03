using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchItemsManager<PAYEBatchItems> : IDependency, IBaseManager<PAYEBatchItems>
    {
        /// <summary>
        /// Get PAYE payment summary for specified year and taxEntityId
        /// </summary>
        /// <param name="year"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<TCCCertificateStatsVM></returns>
        IEnumerable<TCCCertificateStatsVM> GetPAYEPaymentSummary(int year, long taxEntityId);


        /// <summary>
        /// Get PAYE payments for specified year and taxEntityId
        /// </summary>
        /// <param name="year"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<TaxPaymentDetailVM></returns>
        IEnumerable<TaxPaymentDetailVM> GetPAYEPayments(int year, long taxEntityId);

    }
}
