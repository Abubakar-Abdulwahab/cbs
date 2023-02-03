using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentAJAXHandler : IDependency
    {
        /// <summary>
        /// Validates account number and returns the account name
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        APIResponse ValidateAccountNumber(string accountNumber, int bankId);

        /// <summary>
        /// Tries to retrieve account balance
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        APIResponse GetAccountBalance(int walletId);
    }
}
