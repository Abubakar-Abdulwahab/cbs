using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging> : IDependency, IBaseManager<MDARevenueHeadEntryStaging>
    {

        /// <summary>
        /// Get the Id of the reference
        /// </summary>
        /// <param name="mdaRevenueHeadEntryStagingReference"></param>
        /// <param name="implementingClassHashCode"></param>
        /// <param name="opsIdentifier">Operation identifier</param>
        /// <returns>long</returns>
        long GetReferenceId(string mdaRevenueHeadEntryStagingReference, int implementingClassHashCode, int opsIdentifier);

    }
}
