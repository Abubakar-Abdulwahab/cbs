using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletPaymentAJAXHandler : IAccountWalletPaymentAJAXHandler
    {
        private readonly IAccountNumberValidation _accountNumberValidation;
        private readonly IBankManager<Bank> _bankManager;
        private readonly ICoreReadyCashService _coreReadyCashService;
        private readonly IAccountWalletConfigurationManager<AccountWalletConfiguration> _walletConfigurationManager;
        public AccountWalletPaymentAJAXHandler(IAccountNumberValidation accountNumberValidation, IBankManager<Bank> bankManager, ICoreReadyCashService coreReadyCashService, IAccountWalletConfigurationManager<AccountWalletConfiguration> walletConfigurationManager)
        {
            _accountNumberValidation = accountNumberValidation;
            _bankManager = bankManager;
            _coreReadyCashService = coreReadyCashService;
            _walletConfigurationManager = walletConfigurationManager;
        }

        /// <summary>
        /// Validates account number and returns the account name
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public APIResponse ValidateAccountNumber(string accountNumber,int bankId)
        {
            BankViewModel bankVM = _bankManager.GetActiveBankByBankId(bankId);

            if (bankVM == null)
            {
                return new APIResponse { Error = true, ResponseObject = "Invalid bank selected" };
            }

            return new APIResponse { ResponseObject = _accountNumberValidation.ValidateAccountNumber(accountNumber, bankVM.Code) };
        }

        /// <summary>
        /// Tries to retrieve account balance
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public APIResponse GetAccountBalance(int walletId)
        {
            string accountIdentifier = _walletConfigurationManager.GetWalletAccountNumber(walletId);

            if (string.IsNullOrEmpty(accountIdentifier?.Trim()))
            {
                return new APIResponse { Error = true, ResponseObject = "No account number found" };
            }
            decimal balance = _coreReadyCashService.GetCustomerAccountBalance(accountIdentifier);

            return new APIResponse { ResponseObject = $"{balance}" };
        }
    }
}