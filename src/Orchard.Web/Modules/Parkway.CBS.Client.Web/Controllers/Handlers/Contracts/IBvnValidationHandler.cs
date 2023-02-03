using Orchard;
using Parkway.CBS.Client.Web.ViewModels;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IBvnValidationHandler : IDependency, ICommonBaseHandler
    {
        /// <summary>
        /// Register new user and generate state tin (payerid)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        RegisterUserResponse TryRegisterCBSUser(BvnValidationController callback, ValidateBvnVM model);

        /// <summary>
        /// Check if a particular BVN has been registered
        /// </summary>
        /// <param name="bvn"></param>
        /// <returns></returns>
        APIResponse CheckIfBvnExists(string bvn);
    }
}
