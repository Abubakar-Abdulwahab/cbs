using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreFourCoreBVNValidationService : IDependency
    {
        /// <summary>
        /// Validates Bank Verification Number
        /// </summary>
        /// <returns></returns>
        FourCoreBVNValidationResponse ValidateBVN(string bvn);
    }
}
