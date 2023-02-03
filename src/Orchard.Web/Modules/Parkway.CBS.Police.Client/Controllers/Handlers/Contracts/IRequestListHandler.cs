using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IRequestListHandler : IDependency
    {
        /// <summary>
        /// Get redirect model for request details using file number
        /// </summary>
        /// <param name="fileRefNumber">file Ref Number</param>
        /// <returns></returns>
        RequestDetailsVM GetRequestBranchViewDetails(string fileRefNumber);

        /// <summary>
        /// Gets request list vm for subusers
        /// </summary>
        /// <param name="searchParams">search filter info</param>
        /// <returns>RequestListVM</returns>
        RequestListVM GetRequestBranchListVM(RequestsReportSearchParams searchParams);

        /// <summary>
        /// Gets request list vm
        /// </summary>
        /// <param name="searchParams">search filter info</param>
        /// <returns>RequestListVM</returns>
        RequestListVM GetRequestListVM(RequestsReportSearchParams searchParams);

        /// <summary>
        /// Get redirect model for request details
        /// </summary>
        /// <param name="fileRefNumber">file Ref Number</param>
        /// <param name="taxEntityId">taxEntityId</param>
        /// <returns></returns>
        RequestDetailsVM GetRequestViewDetails(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Get invoices for request with specified id
        /// </summary>
        /// <param name="fileNumber">fileNumber</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>PSSRequestInvoiceVM</returns>
        PSSRequestInvoiceVM GetInvoicesForRequest(string fileNumber);

        /// <summary>
        /// Generate service specific document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateServiceDocumentByteFile(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Generate rejection character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateRejectionCharacterCertificateByteFile(string fileRefNumber, long taxEntityId);
    }
}