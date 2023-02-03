using Orchard;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSBranchSubUsersUploadBatchItemsStagingService : IDependency
    {
        /// <summary>
        /// Creates branch sub users
        /// </summary>
        /// <param name="batchId"></param>
        void CreateBranchSubUsers(long batchId);
    }
}
