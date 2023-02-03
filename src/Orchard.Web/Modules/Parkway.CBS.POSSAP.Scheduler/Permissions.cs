using System.Collections.Generic;
using Orchard.Security.Permissions;
using Orchard.Environment.Extensions.Models;

namespace Parkway.CBS.POSSAP.Scheduler
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission CanViewOfficersSchedule = new Permission { Description = "Can view officers schedule reports", Name = "CanViewOfficersSchedule" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[]
                { CanViewOfficersSchedule } },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                CanViewOfficersSchedule
            };
        }
    }
}