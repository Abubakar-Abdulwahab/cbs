using Orchard;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreTINValidationService : IDependency
    {
        /// <summary>
        /// Validate FIRS Tax Identification Number
        /// </summary>
        /// <param name="tin"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        string ValidateTIN(string tin, out string errorMessage);
    }
}
