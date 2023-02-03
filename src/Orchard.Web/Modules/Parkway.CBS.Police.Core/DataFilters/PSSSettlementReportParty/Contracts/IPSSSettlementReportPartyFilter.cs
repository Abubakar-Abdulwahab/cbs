using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportParty.Contracts
{
    public interface IPSSSettlementReportPartyFilter : IDependency
    {
        /// <summary>
        /// Get view model for settlement report parties
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new {  ReportRecords, TotalNumberOfFeeParties, TotalAmountSettled  }</returns>
        dynamic GetRequestReportViewModel(PSSSettlementReportPartySearchParams searchParams);
    }
}
