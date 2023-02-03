using Orchard;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using OrchardPermission = Orchard.Security.Permissions.Permission;


namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class HandlerComposition : IHandlerComposition
    {

        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;

        public HandlerComposition(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
        }


        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void IsAuthorized(OrchardPermission permission)
        {
            if (!_authorizer.Authorize(permission, PoliceErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
        }

    }
}