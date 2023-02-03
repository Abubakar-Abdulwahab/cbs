using Orchard;

namespace Parkway.CBS.Police.Core.PSSIdentification.Contracts
{
    public interface IAccountNumberValidation : IDependency
    {
        /// <summary>
        /// Validates and verify the account number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankCode"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        string ValidateAccountNumber(string accountNumber, string bankCode);
    }
}
