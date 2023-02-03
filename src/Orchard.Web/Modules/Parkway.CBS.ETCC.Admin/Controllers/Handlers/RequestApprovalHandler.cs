using Orchard;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.StateConfig;
using OrchardPermission = Orchard.Security.Permissions.Permission;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Orchard.Logging;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class RequestApprovalHandler : IRequestApprovalHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> _tccRequestManager;
        private readonly IEnumerable<IApprovalComposition> _approvalCompositionImpl;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        public ILogger Logger { get; set; }

        public RequestApprovalHandler(IOrchardServices orchardServices, Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> tccRequestManager, IEnumerable<Lazy<ISMSProvider>> smsProvider, IEnumerable<IApprovalComposition> approvalCompositionImpl)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _tccRequestManager = tccRequestManager;
            _smsProvider = smsProvider;
            _approvalCompositionImpl = approvalCompositionImpl;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanApproveTCCRequests"></param>
        public void CheckForPermission(Permission CanApproveTCCRequests)
        {
            IsAuthorized(CanApproveTCCRequests);
        }
        
        /// <summary>
        /// Save request approval details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <returns>bool</returns>
        public bool ProcessRequestApproval(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors)
        {
            foreach (var impl in _approvalCompositionImpl)
            {
                if (impl.GetApprovalLevelDefinition == (TCCApprovalLevel)requestDetailVM.ApprovalStatusLevelId)
                {
                    return impl.ProcessRequestApproval(requestDetailVM, ref errors);
                }
            }

            throw new NoBillingTypeSpecifiedException("Could not find approval level implementation. Approval level Id" + requestDetailVM.ApprovalStatusLevelId);
        }

        /// <summary>
        /// Check if we can send sms notification for a specified tenant
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private bool CheckSendSMSNotification(string phoneNumber)
        {
            bool canSendNotification = false;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                bool isSMSEnabled = false;
                bool.TryParse(node.Value, out isSMSEnabled);
                if (isSMSEnabled && !string.IsNullOrEmpty(phoneNumber))
                {
                    canSendNotification = true;
                }
            }

            return canSendNotification;
        }


        /// <summary>
        /// Save request rejection details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <returns>bool</returns>
        public bool ProcessRequestRejection(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors)
        {
            foreach (var impl in _approvalCompositionImpl)
            {
                if (impl.GetApprovalLevelDefinition == (TCCApprovalLevel)requestDetailVM.ApprovalStatusLevelId)
                {
                    return impl.ProcessRequestRejection(requestDetailVM, ref errors);
                }
            }

            throw new NoBillingTypeSpecifiedException("Could not find approval level implementation. Approval level Id" + requestDetailVM.ApprovalStatusLevelId);
        }

        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void IsAuthorized(OrchardPermission permission)
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
        }
    }
}