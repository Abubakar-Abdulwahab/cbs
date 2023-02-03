using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateRequestApproverManager<TaxClearanceCertificateRequestApprover> : IDependency, IBaseManager<TaxClearanceCertificateRequestApprover>
    {
        /// <summary>
        /// Get user approver details using the userAdminId
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns>WorkflowApproverDetailVM</returns>
        WorkflowApproverDetailVM GetApproverDetails(int adminUserId);

    }
}
