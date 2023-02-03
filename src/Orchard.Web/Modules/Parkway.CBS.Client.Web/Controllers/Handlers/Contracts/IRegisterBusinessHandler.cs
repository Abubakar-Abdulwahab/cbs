using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IRegisterBusinessHandler : IDependency, ICommonBaseHandler
    {
        /// <summary>
        /// Try register new business as a corporate entity and create CBS user
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        void TryRegisterBusiness(BaseController callback, RegisterBusinessObj model);
    }
}
