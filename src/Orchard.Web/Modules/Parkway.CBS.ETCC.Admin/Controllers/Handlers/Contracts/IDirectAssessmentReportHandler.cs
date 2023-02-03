using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface IDirectAssessmentReportHandler : IDependency
    {
        /// <summary>
        /// Get model for direct assessment report view
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>PAYEDirectAssessmentReportVM</returns>
        PAYEDirectAssessmentReportVM GetDirectAssessmentReport(DirectAssessmentSearchParams searchParams);
    }
}