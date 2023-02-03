using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Parkway.CBS.ReferenceData.Admin
{
    public class Permissions : IPermissionProvider
    {
        //if you can create you can view
        //if u have the create permission you can view, if u only have the view permission u cannot create
        public static readonly Permission EnumerationData = new Permission { Description = "Access to do enumreation data operations", Name = "EnumerationData" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[] 
                { EnumerationData } },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EnumerationData,
            };
        }
    }
}