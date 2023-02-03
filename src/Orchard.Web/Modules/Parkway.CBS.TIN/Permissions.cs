using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN
{
    public class Permissions : IPermissionProvider
    {
        //if you can create you can view
        //if u have the create permission you can view, if u only have the view permission u cannot create
        public static readonly Permission ViewTINReport = new Permission { Description = "Create View TIN Applicant Report", Name = "ViewTINReport" };
        

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewTINReport } }
               
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewTINReport,
            };
        }
    }
}