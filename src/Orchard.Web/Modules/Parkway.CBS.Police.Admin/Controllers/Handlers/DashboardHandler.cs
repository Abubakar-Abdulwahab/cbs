using Orchard;
using Orchard.Layouts.Framework.Elements;
using Orchard.Localization;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class DashboardHandler : IDashboardHandler
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IAdminRequestFilter _adminRequestFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;


        public DashboardHandler(IOrchardServices orchardServices, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, IAdminRequestFilter adminRequestFilter)
        {
            T = NullLocalizer.Instance;
            _orchardServices = orchardServices;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _adminRequestFilter = adminRequestFilter;
        }

        /// <summary>
        /// Get POSSAP dashboard view
        /// </summary>
        /// <returns>PSSDashboardViewModel</returns>
        public PSSDashboardViewModel GetDashboardView()
        {
            try
            {
                List<CategoryDescriptor> tabCats = new List<CategoryDescriptor>();
                var tab1 = new CategoryDescriptor("DashBoard", T("DashBoard"), T("Dashboard"), 0);
                tabCats.Add(tab1);

                int adminUserId = _orchardServices.WorkContext.CurrentUser.Id;
                int accessRoleUserId = _approvalAccesRoleManager.Value.GetAccessRoleUserId(adminUserId, AdminUserType.Approver);
                bool applyApprovalAccessRestrictions = accessRoleUserId > 0;
                bool applyAccessRestrictions = _approvalAccesRoleManager.Value.GetAccessRoleUserId(adminUserId) > 0;


                //Check if the user has only view access, if yes, don't display the pending approval statistics
                if (!applyApprovalAccessRestrictions && applyAccessRestrictions)
                {
                    return new PSSDashboardViewModel { Categories = tabCats.ToArray() };
                }

                IEnumerable<ReportStatsVM> totalApprovedRequestsForMonth = _adminRequestFilter.GetAdminRequestStatistics(PSSRequestStatus.Approved, adminUserId, accessRoleUserId, applyAccessRestrictions, applyApprovalAccessRestrictions,  true);
                IEnumerable<ReportStatsVM> totalRejectedRequestsForMonth = _adminRequestFilter.GetAdminRequestStatistics(PSSRequestStatus.Rejected, adminUserId, accessRoleUserId, applyAccessRestrictions, applyApprovalAccessRestrictions, true);
                IEnumerable<ReportStatsVM> totalPendingRequestsForMonth = _adminRequestFilter.GetAdminRequestStatistics(PSSRequestStatus.PendingApproval, adminUserId, accessRoleUserId, applyAccessRestrictions, applyApprovalAccessRestrictions, true);
                IEnumerable<ReportStatsVM> totalPendingRequests = _adminRequestFilter.GetAdminRequestStatistics(PSSRequestStatus.PendingApproval, adminUserId, accessRoleUserId, applyAccessRestrictions, applyApprovalAccessRestrictions, false);

                PSSDashboardViewModel model = new PSSDashboardViewModel();
                model.TotalApprovedRequests = totalApprovedRequestsForMonth.FirstOrDefault().TotalRecordCount;
                model.TotalRejectedRequests = totalRejectedRequestsForMonth.FirstOrDefault().TotalRecordCount;
                model.TotalPendingApprovalRequests = totalPendingRequestsForMonth.FirstOrDefault().TotalRecordCount;
                model.CurrentUserTotalPendingApprovalRequests = totalPendingRequests.FirstOrDefault().TotalRecordCount;
                model.Month = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
                model.Categories = tabCats.ToArray();

                return model;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}