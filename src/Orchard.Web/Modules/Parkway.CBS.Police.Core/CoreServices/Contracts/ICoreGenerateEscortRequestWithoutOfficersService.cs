using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreGenerateEscortRequestWithoutOfficersService : IDependency
    {
        /// <summary>
        /// Generates request for unknown officers for default branch if <paramref name="isDefaultBranch"/> is true else a branch
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="batchId"></param>
        /// <param name="isDefaultBranch"></param>
        /// <returns></returns>
        InvoiceGenerationResponse GenerateRequestForUnknownOfficers(EscortRequestVM requestVM, long batchId, bool isDefaultBranch = false);
    }
}
