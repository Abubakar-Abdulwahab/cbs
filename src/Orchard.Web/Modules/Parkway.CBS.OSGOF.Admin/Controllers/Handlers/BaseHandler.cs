using Orchard;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrchardPermission = Orchard.Security.Permissions.Permission;


namespace Parkway.CBS.OSGOF.Admin.Controllers.Handlers
{
    public abstract class BaseHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;

        protected BaseHandler(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
        }
        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>IBaseHandler</returns>
        protected T IsAuthorized<T>(OrchardPermission permission) where T : BaseHandler
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
            return (T)this;
        }

    }
}