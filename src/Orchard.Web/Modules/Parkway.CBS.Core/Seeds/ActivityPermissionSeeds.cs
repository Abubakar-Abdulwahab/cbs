using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class ActivityPermissionSeeds : IActivityPermissionSeeds
    {
        private readonly IActivityPermissionManager<ActivityPermission> _activityPermissionRepo;
        private readonly ICBSPermissionManager<CBSPermission> _permissionRepo;

        public ActivityPermissionSeeds(IActivityPermissionManager<ActivityPermission> activityPermissionRepo, ICBSPermissionManager<CBSPermission> permissionRepo)
        {
            _activityPermissionRepo = activityPermissionRepo;
            _permissionRepo = permissionRepo;
        }

        /// <summary>
        /// Seeds "Allow_Part_Payment" record to the CBSPermission table
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>void</returns>
        public void SeedCBSPermissions(int currentUser)
        {
            var permission = new CBSPermission { IsActive = true, Name = nameof(CBSPermissionName.Allow_Part_Payment), Description = "This permission is to indicate part payment preference", LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = currentUser } };
            if (!_permissionRepo.Save(permission)) { throw new CouldNotSaveRecord("Could not save permission record"); }
        }
    }
}