using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSAdminUserService : ICorePSSAdminUserService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;
        private readonly IMembershipService _membershipService;
        private readonly IUserRolesPartRecordManager _userRolesPartManager;
        private readonly IRepository<UserRolesPartRecord> _userRolePartRecordRepo;
        public ILogger Logger { get; set; }

        public CorePSSAdminUserService(IOrchardServices orchardServices, IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager, IMembershipService membershipService, IRepository<UserRolesPartRecord> userRolePartRecordRepo, IUserRolesPartRecordManager userRolesPartManager)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _adminUsersManager = adminUsersManager;
            _membershipService = membershipService;
            _userRolePartRecordRepo = userRolePartRecordRepo;
            _userRolesPartManager = userRolesPartManager;
        }

        /// <summary>
        /// Create an admin user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>PSSAdminUsers</returns>
        public PSSAdminUsers TryCreateAdminUser(AdminUserCreationVM model, ref List<ErrorModel> errors)
        {
            try
            {
                //Check if the user with the email already exist
                if(_adminUsersManager.CheckUser(model.Email))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"User with the specified email or phone number already exist", FieldName = $"{nameof(model.Email)}"});
                    throw new DirtyFormDataException();
                }

                //Check if the username exist
                if (_membershipService.GetUser(model.Username.Trim()) != null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.usernamealreadyexists().ToString(), FieldName = nameof(model.Username) });
                    throw new DirtyFormDataException();
                }

                //Create the UserPartRecord
                var user = _membershipService.CreateUser(new CreateUserParams(model.Username.Trim(), model.PhoneNumber, model.Email, null, null, true));
                if(user == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.unabletocreateuser().ToString(), FieldName = "AdminUser" });
                    throw new DirtyFormDataException();
                }

                //Map the user to a role
                _userRolePartRecordRepo.Create(new UserRolesPartRecord { UserId = user.Id, Role = new RoleRecord { Id = model.RoleTypeId } });

                PSSAdminUsers adminUser = new PSSAdminUsers
                {
                    Fullname = model.Fullname,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    CommandCategory = new CommandCategory { Id = model.CommandCategoryId },
                    Command = new Command { Id = model.CommandId },
                    CreatedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    User = new Orchard.Users.Models.UserPartRecord { Id = user.Id },
                    RoleType = new RoleRecord { Id = model.RoleTypeId }
                };

                if (!_adminUsersManager.Save(adminUser))
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = $"AdminUser" });
                    Logger.Error("Unable to save admin user record");
                    throw new CouldNotSaveRecord();
                }
                return adminUser;
            }
            catch (Exception)
            {
                _adminUsersManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Edit an admin user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>AdminUserVM</returns>
        public AdminUserVM TryEditAdminUser(AdminUserCreationVM model, ref List<ErrorModel> errors)
        {
            try
            {
                //Get the UserPartRecord and Check if the username exist
                IUser user = _membershipService.GetUser(model.Username.Trim());
                if (user == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.usernotfound().ToString(), FieldName = nameof(model.Username) });
                    throw new DirtyFormDataException();
                }

                //Map the user to a role
                UserRolesPartRecord userRolesPartRecord = _userRolePartRecordRepo.Get(x => x.UserId == user.Id);

                if (userRolesPartRecord == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.usernotfound().ToString(), FieldName = nameof(model.RoleTypeId) });
                    throw new DirtyFormDataException();
                }

                // Update role Id
                _userRolesPartManager.UpdateRoleTypeId(model.RoleTypeId, userRolesPartRecord.Id);

                // Update admin user
                AdminUserVM adminUsers = new AdminUserVM { Fullname = model.Fullname, PhoneNumber = model.PhoneNumber, CommandId = model.CommandId, CommandCategoryId = model.CommandCategoryId, RoleTypeId = model.RoleTypeId, UserPartRecordId = user.Id, LastUpdatedById = _orchardServices.WorkContext.CurrentUser.Id };
                _adminUsersManager.UpdateAdminUser(adminUsers);

                return adminUsers;
            }
            catch (Exception)
            {
                _adminUsersManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Get command
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public CommandVM GetCommandForAdmin()
        {
            try
            {
                return _adminUsersManager.GetCommandForUser(_orchardServices.WorkContext.CurrentUser.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}