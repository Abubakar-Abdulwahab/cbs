using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Net.Http;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IShortCodeSMSHandler : IDependency
    {
        /// <summary>
        /// Process shortcode SMS content update request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessRequest(ShortCodeSMSRequestModel model);
    }
}
