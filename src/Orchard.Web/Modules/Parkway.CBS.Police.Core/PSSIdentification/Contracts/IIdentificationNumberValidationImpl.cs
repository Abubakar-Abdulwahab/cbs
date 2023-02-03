using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.PSSIdentification.Contracts
{
    public interface IIdentificationNumberValidationImpl : IDependency
    {
        /// <summary>
        /// Implementation class name
        /// </summary>
        string ImplementingClassName { get; }

        /// <summary>
        /// Validates the specified identification number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="idType"></param>
        /// <param name="errorMessage"></param>
        /// <param name="isRevalidation"></param>
        /// <returns></returns>
        ValidateIdentificationNumberResponseModel Validate(string number, int idType, out string errorMessage, bool isRevalidation = false);

        /// <summary>
        /// Validates the specified identification number
        /// </summary>
        /// <param name="number"></param>
        /// <returns>ValidateIdentificationNumberResponseModel</returns>
        ValidateIdentificationNumberResponseModel Validate(string number);

        /// <summary>
        /// Revalidates user info by updating cbs user and tax entity for user with specified identification number using the most recent record available
        /// </summary>
        /// <param name="identificationNumber"></param>
        void Revalidate(string identificationNumber);

    }
}
