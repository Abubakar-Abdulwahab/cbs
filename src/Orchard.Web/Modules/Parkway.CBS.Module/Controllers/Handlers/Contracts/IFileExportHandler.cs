using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IFileExportHandler : IDependency
    {
        /// <summary>
        /// Get record details for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>CollectionReportViewModel</returns>
        CollectionReportViewModel GetCollectionReport(CollectionSearchParams searchData);


        /// <summary>
        /// Check for permissions
        /// </summary>
        /// <param name="viewInvoiceReport"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission viewInvoiceReport);


        /// <summary>
        /// Get recors for assessment report
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>MDAReportViewModel</returns>
        MDAReportViewModel GetAssessmentReport(InvoiceAssessmentSearchParams searchData);
    }
}
