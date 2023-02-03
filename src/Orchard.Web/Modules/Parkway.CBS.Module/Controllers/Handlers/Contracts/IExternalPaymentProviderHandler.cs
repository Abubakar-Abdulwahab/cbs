using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IExternalPaymentProviderHandler : IDependency
    {
        /// <summary>
        /// Creates an external payment provider
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="userInput"></param>
        void TryCreateExtPaymentProvider(ExternalPaymentProviderController callBack,ExternalPaymentProviderVM userInput);


        /// <summary>
        /// Get payment provider list vm
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PaymentProviderListVM GetPaymentProviderListVM(PaymentProviderSearchParams searchParams);

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);


        /// <summary>
        /// Get client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>APIResponse | null</returns>
        APIResponse GetClientSecret(string clientId);

    }
}
