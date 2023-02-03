using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IValidateDocumentHandler : IDependency
    {

        /// <summary>
        /// Validate request approval number format
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>approval number string if successful, null otherwise</returns>
        string ValidateDocumentApprovalNumber(string approvalNumber);



        /// <summary>
        /// Get request document info with specified approval number
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        ValidatedDocumentVM GetDocumentInfoWithApprovalNumber(string approvalNumber, long taxEntityId);

        /// <summary>
        /// Get request document info with specified approval number
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns>ValidatedDocumentVM</returns>
        ValidatedDocumentVM GetDocumentInfoWithApprovalNumber(string approvalNumber);

    }
}