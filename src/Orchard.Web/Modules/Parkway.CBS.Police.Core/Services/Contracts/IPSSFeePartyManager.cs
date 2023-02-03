using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSFeePartyManager<PSSFeeParty> : IDependency, IBaseManager<PSSFeeParty>
    {
        /// <summary>
        /// Gets all active parties
        /// </summary>
        /// <returns></returns>
        List<PSSFeePartyVM> GetAllActiveFeeParties();

        /// <summary>
        /// Checks if the fee party exists using the <paramref name="feePartyId"/>
        /// </summary>
        /// <param name="feePartyId"></param>
        /// <returns></returns>
        bool CheckIfFeePartyExistById(int feePartyId);
    }
}
