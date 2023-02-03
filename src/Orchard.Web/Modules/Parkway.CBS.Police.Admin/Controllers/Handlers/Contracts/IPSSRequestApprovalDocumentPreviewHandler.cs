using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSRequestApprovalDocumentPreviewHandler : IDependency
    {
        /// <summary>
        /// Generate draft service document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDraftServiceDocument(string fileRefNumber);

        /// <summary>
        /// Confirms the admin user has viewed the service specific draft document during approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        APIResponse ConfirmAdminHasViewedDraftDocument(string fileRefNumber, int userId);
    }
}
