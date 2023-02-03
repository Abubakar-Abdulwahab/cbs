using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IBankManager<Bank> : IDependency, IBaseManager<Bank>
    {
        /// <summary>
        /// Gets all active banks
        /// </summary>
        /// <returns></returns>
        List<BankViewModel> GetAllActiveBanks();

        /// <summary>
        /// Gets active bank by <paramref name="bankCode"/>
        /// </summary>
        /// <returns></returns>
        BankViewModel GetActiveBankByBankCode(string bankCode);

        /// <summary>
        /// Gets active bank by <paramref name="bankId"/>
        /// </summary>
        /// <returns></returns>
        BankViewModel GetActiveBankByBankId(int bankId);

        /// <summary>
        /// Checks if a bank exists
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>CommandVM</returns>
        bool CheckIfBankExist(int bankId);
    }
}
