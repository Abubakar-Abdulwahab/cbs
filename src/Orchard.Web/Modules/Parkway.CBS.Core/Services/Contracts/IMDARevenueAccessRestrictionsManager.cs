using System;
using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IMDARevenueAccessRestrictionsManager<MDARevenueAccessRestrictions> : IDependency, IBaseManager<MDARevenueAccessRestrictions>
    {
        /// <summary>
        /// Synchronize MDARevenueAccessRestrictions table with changes in the MDARevenueAccessRestrictionsStaging table
        /// </summary>
        /// <param name="hashCodeOfImplementingClass">hash code of the operation class</param>
        /// <param name="operationTypeIdentifierId"></param>
        /// <param name="MDARevenueHeadEntryStagingId"></param>
        void UpdateRestrictionsRecords(int hashCodeOfImplementingClass, long operationTypeIdentifierId, Int64 MDARevenueHeadEntryStagingId);

    }
}
