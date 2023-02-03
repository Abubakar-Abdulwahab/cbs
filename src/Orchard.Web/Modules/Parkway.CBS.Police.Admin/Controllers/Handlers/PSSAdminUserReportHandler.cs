using Orchard;
using Orchard.Logging;
using Orchard.Roles.Services;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.AdminUserReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSAdminUserReportHandler : IPSSAdminUserReportHandler
    {
        ILogger Logger { get; set; }
        private readonly ICoreCommand _coreCommand;
        private readonly Lazy<IAdminUserReportFilter> _adminUserReportFilter;
        private readonly IRoleService _roleService;
        private readonly ICommandCategoryManager<CommandCategory> _commandCategoryManager;
        private readonly IUserRolesPartRecordManager _userRolesPartManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;

        public PSSAdminUserReportHandler(Lazy<IAdminUserReportFilter> adminUserReportFilter, ICoreCommand coreCommand, IRoleService roleService, ICommandCategoryManager<CommandCategory> commandCategoryManager, IOrchardServices orchardServices, IUserRolesPartRecordManager userRolesPartManager, IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager)
        {
            _adminUserReportFilter = adminUserReportFilter;
            Logger = NullLogger.Instance;
            _coreCommand = coreCommand;
            _roleService = roleService;
            _commandCategoryManager = commandCategoryManager;
            _orchardServices = orchardServices;
            _userRolesPartManager = userRolesPartManager;
            _adminUsersManager = adminUsersManager;
        }

        /// <summary>
        /// Gets admin user view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSAdminUserReportVM GetVMForReports(AdminUserReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _adminUserReportFilter.Value.GetAdminUserReportViewModel(searchParams);
            IEnumerable<AdminUserVM> reports = (IEnumerable<AdminUserVM>)recordsAndAggregate.ReportRecords;

            return new PSSAdminUserReportVM
            {
                RoleTypes = _roleService.GetRoles().Select(x => new UserRoleVM { Id = x.Id, Name = x.Name }).ToList(),
                CommandCategories = _commandCategoryManager.GetCategories(),
                Commands = searchParams.CommandCategoryId != 0 ? _coreCommand.GetCommandsByCommandCategory(searchParams.CommandCategoryId) : null,
                CommandId = searchParams.CommandId,
                RoleTypeId = searchParams.RoleType,
                CommandCategoryId = searchParams.CommandCategoryId,
                Username = searchParams.Username,
                Status = searchParams.Status,
                AdminUsers = reports,
                TotalAdminUserRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfAdminUsersRecords).First().TotalRecordCount,
            };
        }

        /// <summary>
        /// Toggles <see cref="UserPart.RegistrationStatus"/> with the value from <paramref name="isActive"/>
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="isActive"></param>
        /// <returns>The user's username </returns>
        public string ToggleIsActiveAdminUser(int userPartRecordId, bool isActive)
        {
            try
            {

                string username = _userRolesPartManager.ToggleIsUserRegistrationStatus(userPartRecordId, isActive, lastUpdatedById: _orchardServices.WorkContext.CurrentUser.Id);
                _adminUsersManager.UpdateLastUpdatedBy(userPartRecordId, isActive, lastUpdatedById: _orchardServices.WorkContext.CurrentUser.Id);
                return username;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}