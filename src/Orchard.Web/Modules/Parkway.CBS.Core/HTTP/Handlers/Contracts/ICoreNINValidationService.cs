using Orchard;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreNINValidationService : IDependency
    {
        /// <summary>
        /// Validate National Identification Number
        /// </summary>
        /// <param name="nin"></param>
        /// <param name="errormessage"></param>
        /// <returns></returns>
        dynamic ValidateNIN(string nin, out string errormessage);
    }
}
