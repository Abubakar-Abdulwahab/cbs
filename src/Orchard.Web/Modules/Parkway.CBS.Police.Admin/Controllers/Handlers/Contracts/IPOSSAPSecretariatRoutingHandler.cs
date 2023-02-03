using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPOSSAPSecretariatRoutingHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);

        /// <summary>
        /// Routes the request to the escort stage for the selected escort process stage definition
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        /// <param name="errors"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        SecretariatRoutingApprovalResponse RouteToEscortStage(EscortRequestDetailsVM requestDetailsVM, ref List<ErrorModel> errors);


        /// <summary>
        /// Routes the request to the selected character certificate flow definition level
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        /// <param name="errors"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        SecretariatRoutingApprovalResponse RouteToCharacterCertificateStage(CharacterCertificateRequestDetailsVM requestDetailsVM, ref List<ErrorModel> errors);
    }
}
