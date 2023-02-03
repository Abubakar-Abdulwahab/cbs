using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IInvestigationReportDetailsManager<InvestigationReportDetails> : IDependency, IBaseManager<InvestigationReportDetails>
    {
        /// <summary>
        /// Get investigation report request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<ExtractDetailsVM></returns>
        IEnumerable<ExtractDetailsVM> GetInvestigationReportRequestViewDetails(string fileRefNumber, long taxEntityId);
    }
}
