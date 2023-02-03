using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSAdminUserReportVM
    {
        public IEnumerable<AdminUserVM> AdminUsers { get; set; }

        public dynamic Pager { get; set; }

        public List<UserRoleVM> RoleTypes { get; set; }

        public IEnumerable<CommandCategoryVM> CommandCategories { get; set; }

        public List<CommandVM> Commands { get; set; }

        public int CommandCategoryId { get; set; }

        public int Status { get; set; }

        public int CommandId { get; set; }

        public string Username { get; set; }

        public int RoleTypeId { get; set; }

        public int TotalAdminUserRecord { get; set; }

    }
}