using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface IRequestApprovalLogHandler : IDependency
    {
        /// <summary>
        /// Get request approval log vm for request with specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>TCCRequestApprovalLogVM</returns>
        TCCRequestApprovalLogVM GetRequestApprovalLogVM(string applicationNumber);
    }
}
