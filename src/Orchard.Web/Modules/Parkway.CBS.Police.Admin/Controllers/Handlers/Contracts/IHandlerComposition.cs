using Orchard;
using OrchardPermission = Orchard.Security.Permissions.Permission;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IHandlerComposition : IDependency
    {

        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void IsAuthorized(OrchardPermission permission);

    }
}
