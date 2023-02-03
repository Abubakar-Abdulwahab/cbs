using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ITCCValidationHandler : IDependency
    {
        /// <summary>
        /// Get request details for TCC with specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        TCCRequestDetailVM GetRequestDetail(string applicationNumber);

        /// <summary>
        /// Validate application number format
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        bool ValidateApplicationNumberFormat(string applicationNumber);
    }
}
