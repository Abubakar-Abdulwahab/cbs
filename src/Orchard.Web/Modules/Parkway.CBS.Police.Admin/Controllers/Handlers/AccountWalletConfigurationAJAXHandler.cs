using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletConfigurationAJAXHandler : IAccountWalletConfigurationAJAXHandler
    {
        private readonly ICoreCommand _coreCommand;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;
        private readonly ICommandManager<Command> _commandManager;

        public ILogger Logger { get; set; }

        public AccountWalletConfigurationAJAXHandler(IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager, ICoreCommand coreCommand, ICommandManager<Command> commandManager)
        {
            _adminUsersManager = adminUsersManager;
            Logger = NullLogger.Instance;
            _coreCommand = coreCommand;
            _commandManager = commandManager;
        }

        /// <summary>
        /// Gets admin user detail using the <paramref name="adminUsername"/>
        /// </summary>
        /// <param name="adminUsername"></param>
        /// <returns></returns>
        public APIResponse GetAdminUserDetail(string adminUsername)
        {
            AdminUserVM adminUserVM = _adminUsersManager.GetAdminUser(adminUsername);

            if (adminUserVM == null)
            {
                return new APIResponse
                {
                    Error = true,
                    ResponseObject = $"No user with username {adminUsername} was found."
                };
            }

            Core.HelperModels.CommandVM commandDetailVM = _coreCommand.GetCommandDetails(adminUserVM.CommandId);
            string[] commandCode = commandDetailVM.Code.Split('-');

            return new APIResponse
            {
                ResponseObject = new AdminUserDetailVM
                {
                    OfficerSubSection = commandCode.Length > 3 ? commandDetailVM.Name : string.Empty,
                    OfficerSection = commandCode.Length > 2 ? _commandManager.GetCommandWithCode(string.Join("-", commandCode, 0, 3)).Name : string.Empty,
                    OfficerDepartment = commandCode.Length > 1 ? _commandManager.GetCommandWithCode(string.Join("-", commandCode, 0, 2)).Name : string.Empty,
                    OfficerFormation = adminUserVM.CommandCategoryName,
                    CommandId = adminUserVM.CommandId,
                    Email = adminUserVM.Email,
                    PhoneNumber = adminUserVM.PhoneNumber,
                    Id = adminUserVM.Id,
                    Fullname = adminUserVM.Fullname,
                    Username = adminUserVM.Username
                }
            };
        }

    }
}