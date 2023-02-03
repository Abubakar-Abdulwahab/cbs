using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;


namespace Parkway.CBS.Core.Seeds
{
    public class ACL : IACL
    {
        private readonly ICBSRoleManager<CBSRole> _roleRepo;
        private readonly ICBSPermissionManager<CBSPermission> _permRepo;
        private readonly ICBSUserRoleManager<CBSUserRole> _userRoleRepo;
        private readonly ICBSRolePermissionManager<CBSRolePermission> _rolePermRepo;

        public ACL(ICBSRoleManager<CBSRole> roleRepo, ICBSPermissionManager<CBSPermission> permRepo, ICBSUserRoleManager<CBSUserRole> userRoleRepo, ICBSRolePermissionManager<CBSRolePermission> rolePermRepo)
        {
            _roleRepo = roleRepo;
            _permRepo = permRepo;
            _userRoleRepo = userRoleRepo;
            _rolePermRepo = rolePermRepo;
        }


        public void CreateRoles()
        {
            List<CBSRole> roles = new List<CBSRole>
            {
                { new CBSRole { LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = 2 }, Name = "User",  Description = "User role" } }
            };

            if (!_roleRepo.SaveBundle(roles)) { throw new Exception("Could not save record"); }
        }


        public void CreatePermissions()
        {
            List<CBSPermission> permissions = new List<CBSPermission>
            {
                { new CBSPermission { Description = "Can edit user", Name = "CAN_EDIT_USER", LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = 2 } } }
            };
            if (!_permRepo.SaveBundle(permissions)) { throw new Exception("Could not save permissions"); }
        }


        public void CreateUserRole()
        {
            List<CBSUserRole> userRoles = new List<CBSUserRole>
            {
                { new CBSUserRole { Role = new CBSRole { Id = 1 }, User = new Orchard.Users.Models.UserPartRecord { Id = 2 } } }
            };

            if (!_userRoleRepo.SaveBundle(userRoles)) { throw new Exception("Could not save user roles"); }
        }


        public void CreateRolePermission()
        {
            List<CBSRolePermission> rolePermissions = new List<CBSRolePermission>
            {
                { new CBSRolePermission { Role = new CBSRole { Id = 1 }, Permission = new CBSPermission { Id = 1 } } },
            };

            if (!_rolePermRepo.SaveBundle(rolePermissions)) { throw new Exception("Could not save role permissions"); }

        }

    }
}